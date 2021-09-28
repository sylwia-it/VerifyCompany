using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using VerifyActiveCompany.Lib.Service;
using VerifyCompany.Common.Lib;

namespace VerifyActiveCompany.Lib
{
    public class BiRClient : IBiRClient
    {
        private const string _codeOfMessageFromService = "KomunikatKod";
        private const string _prawnaCompanyType = "P";
        private const string _errCodeForNoSession = "7";
        private const string _errCodeForEverythingOK = "0";
        private const string _osFizycznaCompanyType = "F";
        private const string _dzialnoscWpisanaDoCedigSilosType = "1";
        private static UslugaBIRzewnPublClient _client = null;
        private string _sid;
        private string _url;
        private string _sidLogin;

        public BiRClient(string url, string sidLogin)
        {
            this._url = url;
            this._sidLogin = sidLogin;
        }

        public void Init()
        {
            try
            {
                WSHttpBinding binding = new WSHttpBinding();
                binding.Security.Mode = SecurityMode.Transport;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
                binding.MessageEncoding = WSMessageEncoding.Mtom;
                binding.AllowCookies = true;


                var endPoint = new EndpointAddress(_url);


                _client = new UslugaBIRzewnPublClient(binding, endPoint);
                
                _client.Open();
            }
            catch (Exception e)
            {
                throw new BiRClientSetUpException("Błąd podczas otwierania klienta usluga BiR", e);
            }

            LogIn(true);

            SetMessageProperties();

        }

        private void SetMessageProperties()
        {
            try
            {
                OperationContextScope scope = new OperationContextScope(_client.InnerChannel);
                HttpRequestMessageProperty reqProps = new HttpRequestMessageProperty();
                reqProps.Headers.Add("sid", _sid);
                reqProps.Headers.Add("user-agent", "CompanyVerifierbot/1.2 ()");
                reqProps.Headers.Add(System.Net.HttpRequestHeader.Cookie, "security=true");

                OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = reqProps;
            }
            catch (Exception e)
            {
                throw new BiRClientSetUpException("Błąd podczas ustawienia parametrów z usługą BiR", e);
            }

        }

        public bool Close()
        { try
            {
                _client.Wyloguj(_sid);
                if (_client != null)
                {
                    _client.Close();
                    return true;
                }
            }
            catch (Exception e)
            {
                throw new BiRClientSetUpException("Błąd podczas zamykania połączenia z usługą BiR", e);
            }
            return false;
        }

        private void LogIn(bool isFirstTime)
        {
            try
            {
                _sid = _client.Zaloguj(_sidLogin);

            }
            catch (Exception e)
            {
                throw new BiRClientSetUpException("Błąd podczas logowania do usługi BiR", e);
            }

            if (string.IsNullOrEmpty(_sid) && !isFirstTime)
            {
                throw new CommunicationException("SID po zalogowowaniu jest Login pusty");
            }
        }
        private BiRVerifyStatus _lastVerifyStatus = BiRVerifyStatus.NoSearchYet;
        public BiRVerifyStatus GetLastErrorStatus()
        {
            return _lastVerifyStatus;
        }


        private const string _raportOsPrawnaName = "BIR11OsPrawna";
        private const string _raportOsFizCedigName = "BIR11OsFizycznaDzialalnoscCeidg";

        private int couter = 0;
        public BiRCompany GetCompany(string nip)
        {
            try
            {
                _lastVerifyStatus = BiRVerifyStatus.NoSearchYet;

                var paramse = new ParametryWyszukiwania();
                paramse.Nip = nip;
                

                string errorCode;
                if (couter == 24)
                {
                    Init();
                    couter = 0;
                }
                string result = _client.DaneSzukajPodmioty(paramse);
                couter++;


                if (BiRResponseXMLParser.IsResponseEmpty(result) || BiRResponseXMLParser.ContainsError(result))
                {
                    errorCode = _client.GetValue(_codeOfMessageFromService);
                    Console.WriteLine(errorCode);
                    if (string.IsNullOrEmpty(errorCode) || errorCode.Equals(_errCodeForNoSession))
                    {
                        LogIn(false);
                        result = _client.DaneSzukajPodmioty(paramse);
                        if (BiRResponseXMLParser.IsResponseEmpty(result) || BiRResponseXMLParser.ContainsError(result))
                        {
                            errorCode = _client.GetValue(_codeOfMessageFromService);
                        }
                    }

                    if (!errorCode.Equals(_errCodeForEverythingOK))
                    {
                        _lastVerifyStatus = GetStatusFromErrorCode(errorCode, result);
                        return null;
                    }
                }

                BiRCompany company = BiRResponseXMLParser.GetCompanyFromDaneSzukajPodmiotyResponse(result);

                // "P" = Typ podmiotu  rejestru REGON:  jednostka prawna(= osoba  prawna lub  jednostka organizacyjna 
                // nieposiadająca osobowości prawnej, np.spółka cywilna) 
                if (company.Type == _prawnaCompanyType)
                {
                    company.CompanyType = BiRCompanyType.Prawna;
                    result = _client.DanePobierzPelnyRaport(company.Regon, _raportOsPrawnaName);
                    if (BiRResponseXMLParser.IsResponseEmpty(result) || BiRResponseXMLParser.ContainsError(result))
                    {
                        errorCode = _client.GetValue(_codeOfMessageFromService);
                        if (string.IsNullOrEmpty(errorCode) || errorCode.Equals(_errCodeForNoSession))
                        {
                            LogIn(false);
                            result = _client.DanePobierzPelnyRaport(company.Regon, _raportOsPrawnaName);
                            if (BiRResponseXMLParser.IsResponseEmpty(result) || BiRResponseXMLParser.ContainsError(result))
                            {
                                errorCode = _client.GetValue(_codeOfMessageFromService);
                            }
                        }
                        if (!errorCode.Equals(_errCodeForEverythingOK))
                        {
                            _lastVerifyStatus = GetStatusFromErrorCode(errorCode, result);
                            return null;
                        }
                    }

                    BiRResponseXMLParser.AddDanePrawna(ref company, result);
                }
                // Typ podmiotu rejestru REGON: jedn. fizyczna (= os. fizyczna prowadząca działalność gospodarczą) 
                else if (company.Type == _osFizycznaCompanyType && company.SilosID == _dzialnoscWpisanaDoCedigSilosType)
                {
                    company.CompanyType = BiRCompanyType.FizycznaProwadzacaDzialalnoscGosp;
                    result = _client.DanePobierzPelnyRaport(company.Regon, _raportOsFizCedigName);
                    if (BiRResponseXMLParser.IsResponseEmpty(result) || BiRResponseXMLParser.ContainsError(result))
                    {
                        errorCode = _client.GetValue(_codeOfMessageFromService);
                        if (string.IsNullOrEmpty(errorCode) || errorCode.Equals(_errCodeForNoSession))
                        {
                            LogIn(false);
                            result = _client.DanePobierzPelnyRaport(company.Regon, _raportOsFizCedigName);
                            if (BiRResponseXMLParser.IsResponseEmpty(result) || BiRResponseXMLParser.ContainsError(result))
                            {
                                errorCode = _client.GetValue(_codeOfMessageFromService);
                            }
                        }
                        if (!errorCode.Equals(_errCodeForEverythingOK))
                        {
                            _lastVerifyStatus = GetStatusFromErrorCode(errorCode, result);
                            return null;
                        }
                    }
                    BiRResponseXMLParser.AddDaneFizycznaCedig(ref company, result);
                }
                else
                {
                    // inaczej może być jeszcze LP i LF, czyli jednostek lokalnych
                    throw new ArgumentOutOfRangeException(result, "Zapytanie zwróciło nieobsługiwany typ podmiotu tj. typ = " + company.Type + " , silos = " + company.SilosID);
                }
                return company;

            } catch (Exception e)
            {

            }

            return null;
           
        }




        private BiRVerifyStatus GetStatusFromErrorCode(string errorCode, string result)
        {
            if (string.IsNullOrEmpty(errorCode) || errorCode.Equals("7") || BiRResponseXMLParser.IsResponseEmpty(result))
            {

                return BiRVerifyStatus.NoSession;
            }

            if (errorCode.Equals("2"))
            {
                return BiRVerifyStatus.TooManyIds;
            }

            if (errorCode.Equals("4"))
            {
                return BiRVerifyStatus.NotFound;
            }
            if (errorCode.Equals("5"))
            {
                return BiRVerifyStatus.ErroneusOrEmptyReportName;
            }

            return BiRVerifyStatus.Error;

        }
    }
}

     

     

