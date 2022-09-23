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
        private const string _multipleEntriesError = "Serwis Białej Listy Firm zwrócił wiele podmiotów dla podanego NIPu";
        private const string _activeVATPayerResponseMsg = "Pomiot jest CZYNNYM płatnikiem VAT.";
        private const string _notInVATRegiestrCompany = "Serwis Białej Listy Firm nie zwrócił odpowiedzi dla tego podmiotu.";
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
        /// <param name="verifyBankAccount">If the bank account shall be verified if exists on the list of allowed/correct bank accounts</param>
        /// <returns><code>null</code> when there are no companies to verify</returns>
        /// <exception cref="ArgumentNullException">When input companies are null.</exception>
        public Dictionary<string, WhiteListVerResult> VerifyCompanies(List<InputCompany> inputCompaniesToVerify, bool verifyBankAccount, bool verifyForInvoiceDate)
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

            List<InputCompany> companiesToVerify = inputCompaniesToVerify.Where(c => !string.IsNullOrEmpty(c.NIP)).ToList();
            List<InputCompany> nipEmptyCompanies = inputCompaniesToVerify.Where(iC => string.IsNullOrEmpty(iC.NIP)).ToList();

            try
            {
                if (!verifyForInvoiceDate)
                {
                    for (int i = 0; i <= companiesToVerify.Count / _maxNumOfNipsPerOneRequest && i < _maxNumOfRequestsPerDay; i++)
                    {
                        var chunkOfCompaies = companiesToVerify.Skip(i * _maxNumOfNipsPerOneRequest).Take(_maxNumOfNipsPerOneRequest).ToList();
                        if (chunkOfCompaies.Count == 0)
                            continue;
                        string nipsInString = GetNIPsInOneString(chunkOfCompaies);
                        Dictionary<string, WhiteListVerResult> chunkVerification = VerifyChunkOfCompanies(chunkOfCompaies, nipsInString, DateTime.Now, verifyBankAccount);
                        foreach (var verResult in chunkVerification)
                        {
                            result.Add(verResult.Key, verResult.Value);
                            _numOfNipsAskedInTheAppRun++;
                        }
                        _numOfRequestsInTheAppRun++;
                    }
                }
                else
                {
                    var groupsOfComapnies = companiesToVerify.GroupBy(c => c.InvoiceDate, c => c);
                    foreach (var groupOfComanies in groupsOfComapnies)
                    {
                        for (int i = 0; i <= groupOfComanies.Count() / _maxNumOfNipsPerOneRequest; i++)
                        {
                            var chunkOfCompaies = groupOfComanies.Skip(i * _maxNumOfNipsPerOneRequest).Take(_maxNumOfNipsPerOneRequest).ToList();
                            if (chunkOfCompaies.Count == 0)
                            { continue; }
                            string nipsInString = GetNIPsInOneString(chunkOfCompaies);
                            Dictionary<string, WhiteListVerResult> chunkVerification = VerifyChunkOfCompanies(chunkOfCompaies, nipsInString, chunkOfCompaies[0].InvoiceDate, verifyBankAccount);
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
                        resultC.VerificationDate = DateTime.Now.ToString();
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

        private  Dictionary<string, WhiteListVerResult> VerifyChunkOfCompanies(List<InputCompany> chunkOfCompaies, string nipsInString, DateTime dateOfRequest, bool verifyBankAccount)
        {
            EntryListResponse content = null;
            Dictionary<string, WhiteListVerResult> result = new Dictionary<string, WhiteListVerResult>();
            WhiteListVerResult tempWhiteListVerResult = null;

            content = _whiteListClient.VerifyCompanies(nipsInString, dateOfRequest).GetAwaiter().GetResult();
            
            foreach (var companyToVerify in chunkOfCompaies)
            {
                tempWhiteListVerResult = ExtractResultForCompanyFromResponse(verifyBankAccount, content, companyToVerify);
                result.Add(companyToVerify.ID, tempWhiteListVerResult);
            }

            return result;
        }

        private WhiteListVerResult ExtractResultForCompanyFromResponse(bool verifyBankAccount, EntryListResponse content, InputCompany companyToVerify)
        {
            WhiteListVerResult tempWhiteListVerResult = new WhiteListVerResult()
            {
                Nip = companyToVerify.NIP,
                VerificationDate = DateTime.Now.ToString()
            };
            if (companyToVerify.BankAccountNumber is null || companyToVerify.FormatErrors.Contains(InputCompanyFormatError.BankAccountFormatError))
            {
                tempWhiteListVerResult.GivenAccountNumber = string.Empty;
            }
            else { tempWhiteListVerResult.GivenAccountNumber = companyToVerify.BankAccountNumber; }
    
            
            if (content == null || content.Result == null || content.Result.Entries == null)
            {
                AddInformationAboutGeneralError(ref tempWhiteListVerResult, content);
            }
            else
            {
                tempWhiteListVerResult.ConfirmationResponseString = content.Result.RequestId;
                tempWhiteListVerResult.VerificationDate = content.Result.RequestDateTime;

                var verifiedCompanies = content.Result.Entries;

                if (verifiedCompanies.Any(c => c.Identifier == companyToVerify.NIP))
                {
                    Entry verifiedCompanyEntry = verifiedCompanies.FirstOrDefault(c => c.Identifier == companyToVerify.NIP);
                    if (verifiedCompanyEntry.Subjects == null || verifiedCompanyEntry.Error != null || verifiedCompanyEntry.Subjects.Count != 1)
                    {
                        AddInformationAboutError(ref tempWhiteListVerResult, verifiedCompanyEntry);
                    }
                    else
                    {
                        AddInformationAboutVerifiedCompany(ref tempWhiteListVerResult, companyToVerify, verifiedCompanyEntry.Subjects[0], verifyBankAccount);
                    }
                }
                else
                {
                    tempWhiteListVerResult.VerificationStatus = WhiteListVerResultStatus.NotInVATRegisterCompany;
                    tempWhiteListVerResult.SetVerStatusMessage(_notInVATRegiestrCompany);
                }
            }

            return tempWhiteListVerResult;
        }

        private void AddInformationAboutVerifiedCompany(ref WhiteListVerResult tempWhiteListVerResult, InputCompany companyToVerify, Entity verifiedCompany, bool verifyBankAccount)
        {

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
                    tempWhiteListVerResult.VerificationStatus = WhiteListVerResultStatus.ActiveVATPayerVerScuccessButGivenAccountNotVerified;
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

                    tempWhiteListVerResult.VerificationStatus = WhiteListVerResultStatus.ActiveVATPayerVerScuccessButGivenAccountNotVerified;
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

        private string GetNIPsInOneString(List<InputCompany> companies)
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
        private void AddInformationAboutGeneralError(ref WhiteListVerResult tempWhiteListVerResult, EntryListResponse content)
        {
            

            if (content == null || content.Result == null)
            {
                tempWhiteListVerResult.VerificationStatus = WhiteListVerResultStatus.ErrorEmptyResponse;
                tempWhiteListVerResult.SetVerStatusMessage(_emptyVerResponseMsg);
                tempWhiteListVerResult.ConfirmationResponseString = string.Empty;
            }
            else if (content.Result.Entries == null)
            {
                tempWhiteListVerResult.VerificationStatus = WhiteListVerResultStatus.ErrorEmptyResponse;
                tempWhiteListVerResult.ConfirmationResponseString = content.Result.RequestId;
                tempWhiteListVerResult.VerificationDate = content.Result.RequestDateTime;
            }
        }

        private void AddInformationAboutError(ref WhiteListVerResult tempWhiteListVerResult, Entry verifiedCompanyEntry)
        {
            //if (verifiedCompany.Subjects == null || verifiedCompany.Error != null || verifiedCompany.Subjects.Count != 1)
            if (verifiedCompanyEntry.Error != null)
            {
                if (_errorCodes.Contains(verifiedCompanyEntry.Error.Code))
                {
                  
                        tempWhiteListVerResult.VerificationStatus = WhiteListVerResultStatus.ErrorNIPError;
                        tempWhiteListVerResult.SetVerStatusMessage(string.Format(_ownNipEmptyVerResponseMsgFormat, verifiedCompanyEntry.Error.Code, verifiedCompanyEntry.Error.Message));
                        tempWhiteListVerResult.ConfirmationResponseString = string.Empty;

                }
                else
                {
                    tempWhiteListVerResult.SetVerStatusMessage(string.Format("Kod błędu: {0} Wiadomość: {1}", verifiedCompanyEntry.Error.Code, verifiedCompanyEntry.Error.Message));
                    tempWhiteListVerResult.VerificationStatus = WhiteListVerResultStatus.Error;
                    tempWhiteListVerResult.ConfirmationResponseString = string.Empty;
                }
            }
            else if (verifiedCompanyEntry.Subjects == null)
            {
                tempWhiteListVerResult.VerificationStatus = WhiteListVerResultStatus.NotInVATRegisterCompany;
                tempWhiteListVerResult.SetVerStatusMessage(_notInVATRegiestrCompany);

            }
            else if (verifiedCompanyEntry.Subjects.Count != 1)
            {
                tempWhiteListVerResult.VerificationStatus = WhiteListVerResultStatus.MultipleEntriesError;
                tempWhiteListVerResult.SetVerStatusMessage(_multipleEntriesError);
            }
        }
    }
}
