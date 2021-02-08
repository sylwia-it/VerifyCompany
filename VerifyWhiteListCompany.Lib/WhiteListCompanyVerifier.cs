using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VerifyCompany.Common.Lib;
using VerifyWhiteListCompany.Lib.WebServiceModel;

namespace VerifyWhiteListCompany.Lib
{
    public class WhiteListCompanyVerifier
    {
        private const int _maxNumOfRequestsPerDay = 10;
        private const int _maxNumOfNipsPerOneRequest = 30;
        private int _numOfNipsAskedInTheAppRun = 0;
        private int _numOfRequestsInTheAppRun = 0;
        
        IWhiteListClient _whiteListClient;

        private readonly Regex _accountPattern = new Regex(@"[0-9]{26}",
         RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _nipPattern = new Regex(@"\(*\)",
          RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private const string _comma = ",";

        private static string _activeVatPayerConst = "Czynny";
        private string _activeVATPayerResponseMsg = "Pomiot jest CZYNNYM płatnikiem VAT.";
        private const string _emptyVerResponseMsg = "Serwis Białej Listy Firm nie zwrócił odpowiedzi.";
        private const string _nipErrorVerResponseMsg = "NIP nie jest poprawny.";
        private const string _nipEmptyVerResponseMsg = "Podmiot nie był sprawdzony w serwisie Biała Lista Przedzsiębiorstw.NIE podano NIPu w pliku źródłowym";
        private const string _ownNipEmptyVerResponseMsgFormat = "NIP nie jest poprawny. Kod błędu: {{0}} Wiadomość: {{1}}";
        private const string _activeButAccountNotGivenResponseMsg = "Podmiot jest CZYNNYM płatnikiem VAT. Konto bankowe NIE było sprawdzone, BRAK podania konta razem z NIPem w pliku wejściowym.";
        private const string _activeButHasNoAccountsOnWhiteListResponseMsg = "Podmiot NIE ma podanego żadnego KONTA bankowego w systemie Biała Lista Przedsiębiorstw. Podmiot jest CZYNNYM płatnikiem VAT";
        private const string _activeAndAccountOkResponseMsg = "KONTO jest poprawne. Podmiot jest CZYNNYM płatnikiem VAT. W systemie Biała Lista Przedsiębiorstw konto bankowe jest przypisane do podmiotu o podanym NIPie.";
        private const string _activeButGivenAccountNotOnWhiteListResponseMsg = "Brak KONTA bankowego na liście kont podmiotu w systemie Biała Lista Przesiębiorstw. Podmiot jest CZYNNYM płatnikiem VAT.";
        private const string _activeButGivenAccountHasWrongFormatResponsMsg = "Podmiot jest CZYNNYM płatnikiem VAT. Konto bankowe NIE było sprawdzone - ma ZŁY format danych w systemie Biała Lista Przedsiębiorstw.";
        private const string _notActiveVATPayerResponseMsg = "Podmiot NIE jest czynnym płatnikiem VAT w systemie Biała Lista Przedsiębiorstw.";
        private const string _verProcessFailedResponseMsg = "Proces weryfikacji zakończył się niepoprawnie.";
        private const string _companyNotInVATRegisterResponseMsg = "Podmiot NIE figuruje w rejestrze VAT.";

        public WhiteListCompanyVerifier()
        {
            _whiteListClient = WhiteListClientFactory.GetClient();
        }

        /// <summary>
        /// Verifies the given list of companies. The list of companies is divided into chunks are verified in chunks 
        /// to speed up the process.
        /// API restricts to have max. 10 requests of max. 30 nips
        /// see more: https://www.gov.pl/web/kas/api-wykazu-podatnikow-vat
        /// </summary>
        /// <param name="inputCompaniesToVerify"></param>
        /// <param name="riskyInputCompanies">Risky companies are to be verified separetly</param>
        /// <param name="verifyBankAccount">If the bank account shall be verified if exists on the list of allowed/correct bank accounts</param>
        /// <returns><code>null</code> when there are no companies to verify</returns>
        /// <exception cref="ArgumentNullException">When input companies are null.</exception>
        public Dictionary<string, WhiteListVerResult> VerifyCompanies(List<Company> inputCompaniesToVerify, List<Company> riskyInputCompanies, bool verifyBankAccount)
        {
            if (inputCompaniesToVerify == null)
            {
                throw new ArgumentNullException("inputCompaniesToVerify", "comapnies to verify are null.");
            }

            if (inputCompaniesToVerify.Count == 0)
            {
                return new Dictionary<string, WhiteListVerResult>();
            }

            Dictionary<string, WhiteListVerResult> result = new Dictionary<string, WhiteListVerResult>();

            List<Company> companiesToVerify = inputCompaniesToVerify.Where(c => !string.IsNullOrEmpty(c.NIP)).ToList();
            List<Company> nipEmptyCompanies = inputCompaniesToVerify.Where(iC => string.IsNullOrEmpty(iC.NIP)).ToList();

            if (riskyInputCompanies != null)
            {
                companiesToVerify = companiesToVerify.Where(iC => riskyInputCompanies.Count(rIC => rIC.ID == iC.ID) == 0).ToList();
            }


            try
            {   // Not risky companies
                for (int i = 0; i <= companiesToVerify.Count / _maxNumOfNipsPerOneRequest && i < _maxNumOfRequestsPerDay; i++)
                {
                    var chunkOfCompaies = companiesToVerify.Skip(i * _maxNumOfNipsPerOneRequest).Take(_maxNumOfNipsPerOneRequest).ToList();
                    string nipsInString = GetNIPsInOneString(chunkOfCompaies);
                    Dictionary<string, WhiteListVerResult> chunkVerification = VerifyChunkOfCompanies(chunkOfCompaies, nipsInString, verifyBankAccount);
                    foreach (var verResult in chunkVerification)
                    {
                        result.Add(verResult.Key, verResult.Value);
                        _numOfNipsAskedInTheAppRun++;
                    }
                    _numOfRequestsInTheAppRun++;
                }

                if (riskyInputCompanies != null)
                {
                    foreach (var riskyCompany in riskyInputCompanies)
                    {
                        if (!string.IsNullOrEmpty(riskyCompany.NIP))
                        {
                            var chunkOfCompaies = new List<Company>() { riskyCompany };
                            string nipsInString = GetNIPsInOneString(riskyCompany);

                            Dictionary<string, WhiteListVerResult> chunkVerification = VerifyChunkOfCompanies(chunkOfCompaies, nipsInString, verifyBankAccount);

                            foreach (var verResult in chunkVerification)
                            {
                                result.Add(verResult.Key, verResult.Value);
                                _numOfNipsAskedInTheAppRun++;
                            }
                            _numOfRequestsInTheAppRun++;
                        }
                    }
                }

                if (nipEmptyCompanies != null)
                {
                    foreach (var nipEmptyCompany in nipEmptyCompanies)
                    {
                        WhiteListVerResult resultC = new WhiteListVerResult();
                        resultC.ConfirmationResponseString = string.Empty;
                        resultC.FullName = string.Empty;
                        resultC.FullResidenceAddress = string.Empty;
                        resultC.FullWorkingAddress = string.Empty;
                        resultC.GivenAccountNumber = nipEmptyCompany.BankAccountNumber;
                        resultC.IsActiveVATPayer = false;
                        resultC.IsGivenAccountNumOnWhiteList = false;
                        resultC.Nip = string.Empty;
                        resultC.VerificationDate = DateTime.Now;
                        resultC.VerificationStatus = WhiteListVerResultStatus.ErrorNIPEmpty;
                        result.Add(nipEmptyCompany.ID, resultC);
                    }
                }
            }
            catch (System.Exception e)
            {
                throw e;
            }
            return result;
        }

        private  Dictionary<string, WhiteListVerResult> VerifyChunkOfCompanies(List<Company> chunkOfCompaies, string nipsInString, bool verifyBankAccount)
        {
            EntityListResponse content = null;
            Dictionary<string, WhiteListVerResult> result = new Dictionary<string, WhiteListVerResult>();
            WhiteListVerResult tempWhiteListVerResult = null;

            try
            {
                content = _whiteListClient.VerifyCompanies(nipsInString).GetAwaiter().GetResult();
            }
            catch (WhiteListClientException wSE)
            {
                content = new EntityListResponse();
                content.Exception = wSE.GetException();
            }

            foreach (var companyToVerify in chunkOfCompaies)
            {
                tempWhiteListVerResult = new WhiteListVerResult()
                {
                    Nip = companyToVerify.NIP,
                    GivenAccountNumber = companyToVerify.BankAccountNumber is null ? string.Empty : companyToVerify.BankAccountNumber,
                    VerificationDate = DateTime.Now
                };

                if (content == null || content.Exception != null)
                {
                    AddInformationAboutError(ref tempWhiteListVerResult, content);
                } 
                else
                {
                    tempWhiteListVerResult.ConfirmationResponseString = content.Result.RequestId;

                    var verifiedCompanies = content.Result.Subjects;

                    if (verifiedCompanies.Any(c => c.Nip == companyToVerify.NIP))
                    {
                        Entity verifiedCompany = verifiedCompanies.FirstOrDefault(c => c.Nip == companyToVerify.NIP);
                        tempWhiteListVerResult.IsActiveVATPayer = verifiedCompany.StatusVat == _activeVatPayerConst;
                        tempWhiteListVerResult.AccountNumbers = verifiedCompany.AccountNumbers;
                        tempWhiteListVerResult.IsGivenAccountNumOnWhiteList = IsAccountNumberOnWhiteList(tempWhiteListVerResult.AccountNumbers, tempWhiteListVerResult.GivenAccountNumber);
                        tempWhiteListVerResult.FullName = verifiedCompany.Name;
                        tempWhiteListVerResult.FullResidenceAddress = verifiedCompany.ResidenceAddress;
                        tempWhiteListVerResult.FullWorkingAddress = verifiedCompany.WorkingAddress;


                        if (verifyBankAccount && tempWhiteListVerResult.IsActiveVATPayer)
                        {
                            if (companyToVerify.BankAccountNumber is null)
                            {
                                tempWhiteListVerResult.VerificationStatus = WhiteListVerResultStatus.ActiveVATPayerVerSuccessfull;
                                tempWhiteListVerResult.SetVerStatusMessage(_activeButAccountNotGivenResponseMsg);

                            }
                            else if (tempWhiteListVerResult.AccountNumbers is null || tempWhiteListVerResult.AccountNumbers.Count == 0)
                            {
                                tempWhiteListVerResult.VerificationStatus = WhiteListVerResultStatus.ActiveVATPayerButHasNoAccounts;
                                tempWhiteListVerResult.SetVerStatusMessage(_activeButHasNoAccountsOnWhiteListResponseMsg);
                            }
                            else if (IsGivenAccountNumberGiven(tempWhiteListVerResult.GivenAccountNumber))
                            {
                                if (tempWhiteListVerResult.IsGivenAccountNumOnWhiteList)
                                {
                                    tempWhiteListVerResult.VerificationStatus = WhiteListVerResultStatus.ActiveVATPayerAccountOKVerSuccessfull;
                                    tempWhiteListVerResult.SetVerStatusMessage(_activeAndAccountOkResponseMsg);
                                }
                                else
                                {
                                    tempWhiteListVerResult.VerificationStatus = WhiteListVerResultStatus.ActiveVATPayerButGivenAccountNotOnWhiteList;
                                    tempWhiteListVerResult.SetVerStatusMessage(_activeButGivenAccountNotOnWhiteListResponseMsg);
                                }
                            }
                            else if (string.IsNullOrEmpty(tempWhiteListVerResult.GivenAccountNumber))
                            {
                                
                                tempWhiteListVerResult.VerificationStatus = WhiteListVerResultStatus.ActiveVATPayerVerSuccessfull;
                                tempWhiteListVerResult.SetVerStatusMessage(_activeButAccountNotGivenResponseMsg);
                            }
                            else
                            {
                                tempWhiteListVerResult.VerificationStatus = WhiteListVerResultStatus.ActiveVATPayerButGivenAccountWrong;
                                tempWhiteListVerResult.SetVerStatusMessage(_activeButGivenAccountHasWrongFormatResponsMsg);
                            }
                        }
                        else if (!verifyBankAccount && tempWhiteListVerResult.IsActiveVATPayer)
                        {
                            tempWhiteListVerResult.VerificationStatus = WhiteListVerResultStatus.ActiveVATPayerVerSuccessfull;
                            tempWhiteListVerResult.SetVerStatusMessage(_activeVATPayerResponseMsg);
                        }
                        else 
                        {
                            tempWhiteListVerResult.VerificationStatus = WhiteListVerResultStatus.NotActiveVATPayer;
                            tempWhiteListVerResult.SetVerStatusMessage(_notActiveVATPayerResponseMsg);

                        }
                    }
                    else if (verifiedCompanies.Count == 0)
                    {
                        tempWhiteListVerResult.VerificationStatus = WhiteListVerResultStatus.NotInVATRegisterCompany;
                        tempWhiteListVerResult.SetVerStatusMessage(_companyNotInVATRegisterResponseMsg);
                    }
                    else
                    {

                        tempWhiteListVerResult.VerificationStatus = WhiteListVerResultStatus.ErrorVerProcessFailed;
                        tempWhiteListVerResult.SetVerStatusMessage(_verProcessFailedResponseMsg);
                        
                    }
                }
                result.Add(companyToVerify.ID, tempWhiteListVerResult);
            }

            return result;
        }

        private bool IsAccountNumberOnWhiteList(List<string> accountNumbers, string givenAccountNumber)
        {
            if (accountNumbers == null || accountNumbers.Count == 0)
                return false;
            if (string.IsNullOrEmpty(givenAccountNumber))
                return false;
            return accountNumbers.Contains(givenAccountNumber);
        }

        private bool IsGivenAccountNumberGiven(string givenAccountNumber)
        {
            return _accountPattern.IsMatch(givenAccountNumber);
        }

        private string GetNIPsInOneString(List<Company> companies)
        {
            if (companies == null || companies.Count == 0)
            { throw new ArgumentOutOfRangeException("companies", "Companies null or empty."); }

            StringBuilder sB = new StringBuilder();
            sB.Append(companies[0].NIP);

            for (int i = 1; i < companies.Count; i++)
            {
                sB.Append(_comma);
                sB.Append(companies[i].NIP);
            }
            return sB.ToString();
        }

        private string GetNIPsInOneString(Company company)
        {
            if (company == null)
            { throw new ArgumentOutOfRangeException("company", "Company is null"); }

            return company.NIP;

        }

        private readonly List<string> _errorCodes = new List<string>() { "WL-112", "WL-113", "WL-114", "WL-115" };
        private void AddInformationAboutError(ref WhiteListVerResult tempWhiteListVerResult, EntityListResponse content)
        {
            if (content == null)
            {
                tempWhiteListVerResult.VerificationStatus = WhiteListVerResultStatus.ErrorEmptyResponse;
                tempWhiteListVerResult.SetVerStatusMessage(_emptyVerResponseMsg);
                tempWhiteListVerResult.ConfirmationResponseString = string.Empty;
            }
            else if (content.Exception != null)
            {
                if (_errorCodes.Contains(content.Exception.Code))
                {
                    if (content.Exception.Message.Contains(tempWhiteListVerResult.Nip))
                    {
                        tempWhiteListVerResult.VerificationStatus = WhiteListVerResultStatus.ErrorNIPError;
                        tempWhiteListVerResult.SetVerStatusMessage(string.Format(_ownNipEmptyVerResponseMsgFormat, content.Exception.Code, content.Exception.Message));
                        tempWhiteListVerResult.ConfirmationResponseString = string.Empty;

                    }
                    else
                    {
                        string errNip = _nipPattern.Match(content.Exception.Message).Value;
                        tempWhiteListVerResult.SetVerStatusMessage(string.Format("NIP nie był sprawdzony. Popraw błąd w innym NIPie: {2} i ponów wyszukiwanie. Kod błędu: {0} Wiadomość: {1}", content.Exception.Code, content.Exception.Message, errNip));
                        tempWhiteListVerResult.VerificationStatus = WhiteListVerResultStatus.ErrorOtherNIPError;
                        tempWhiteListVerResult.ConfirmationResponseString = string.Empty;
                    }

                }
                else
                {
                    tempWhiteListVerResult.SetVerStatusMessage(string.Format("Kod błędu: {0} Wiadomość: {1}", content.Exception.Code, content.Exception.Message));
                    tempWhiteListVerResult.VerificationStatus = WhiteListVerResultStatus.Error;
                    tempWhiteListVerResult.ConfirmationResponseString = string.Empty;
                }
            }
        }
    }
}
