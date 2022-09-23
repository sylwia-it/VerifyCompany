using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using VerifyCompany.Common.Lib;
using VerifyWhiteListCompany.Lib;
using VerifyNIPActivePayer.Lib;
using VerifyActiveCompany.Lib;
using Microsoft.Office.Interop.Excel;
using System.Linq;
using ExcelDataManager.Lib.Import;
using Range = Microsoft.Office.Interop.Excel.Range;

namespace ExcelDataManager.Lib.Export
{

    public class SpreadSheetWriter
    {
        private string _exportFilePath;
        private List<ColumnConfig> _importColumnsConfig;
        private List<ColumnConfig> _exportColumnsConfig;
        private readonly string _prefixOfHeaders;
        private List<string> _sheetNamesToExclude; //name of sheet that shall be considered as source
        Dictionary<string, OverallResult> _overallVerificationResult = new Dictionary<string, OverallResult>();

        private const int AllAccountsColumnDelta = 1;
        private const int FullNameColumnDelta = 2;
        private const int FullResidenceAddressColumnDelta = 3;
        private const int FullWorkingAddressColumnDelta = 4;

        private const int AllVerificationStatusColumnDelta = 5;
        private const int WhiteListVerificationStatusColumnDelta = 6;
        private const int NIPVerficationStatusColumnDelta = 7;
        private const int REGONVerificationStatusColumnDelta = 8;
        private const int WhiteListVerificationForInvoiceDateStatusColumnDelta = 9;
        private const int WhiteListVerificationForInvoiceDateConfirmAndDateColumnDelta = 10;
        private const int InputErrorColumnDelta = 11;

        private const int DateColumnDelta = 12;
        private const int StringConfIDColumnDelta = 13;
        private const int AccountSeparteColumnStartDelta = 14;
        private int _numberOfColumnsToClean = 14 + 20;
        private const string _notChecked = "Firma NIE była sprawdzana w systemie";

        public SpreadSheetWriter(string exportFilePath)
        {
            _exportFilePath = exportFilePath;
            IConfiguration configuration = new ConfigurationBuilder()
               .AddJsonFile("importSettings.json", true, true)
               .Build();
            _importColumnsConfig = configuration.GetSection("InputColumnsConfig").Get<List<ColumnConfig>>();
            _exportColumnsConfig = configuration.GetSection("ExportColumnsConfig").Get<List<ColumnConfig>>();
            _sheetNamesToExclude = configuration.GetSection("InputSheetNameExclude").Get<List<string>>();
            _prefixOfHeaders = configuration.GetSection("ExportStartLettersInHeader").Get<string>();

            if (_importColumnsConfig.Count != Enum.GetNames(typeof(ImportColumnName)).Length ||
                _exportColumnsConfig.Count != Enum.GetNames(typeof(ExportColumnName)).Length)
                throw new SpreadSheetWriterExcpetion("W pliku konfiguracyjnym brakuje wszyskich komumn.");
     
        }

        
       public void WriteResultsToFile(List<InputCompany> companiesReadFromFile, List<InputCompany> erroredWhileReadingInputFileCompanies, Dictionary<string, VerifyNIPResult> verifiedNips, Dictionary<string, BiRVerifyResult> areCompaniesActive, Dictionary<string, WhiteListVerResult> verifiedCompanies, Dictionary<string, WhiteListVerResult> verifiedCompaniesForInvoiceDate, bool addAccountsToSeparateColumns)
        {
            Application app = null;
            Workbook theWorkbook = null;

            try
            {
                app = new Application();
                theWorkbook = app.Workbooks.Open(
                _exportFilePath, true, false,
                        Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                Sheets sheets = theWorkbook.Worksheets;
                int worksheetNumber = sheets.Count;

                //Do not Assume worksheet is the first one
                int i = 1;
                Worksheet worksheet = (Worksheet)sheets.get_Item(i);
                while (SpreadSheetHelper.SheetNameContainsNamesToExclude(worksheet.Name.ToLower(), _sheetNamesToExclude))
                {
                    i++;
                    if (i > sheets.Count)
                    {
                        throw new SpreadSheetReaderException("Błąd formatu arkusza. Nazwy arkuszy zawierają niedozwolone słowa. Sprawdź plik konfiguracjny aplikacji.");
                    }
                    worksheet = (Worksheet)sheets.get_Item(i);
                }

                int headerRow = SpreadSheetHelper.FindHeaderRow(worksheet, _importColumnsConfig);
                int lastColumn = GetLastColumnWithOriginalData(worksheet, headerRow);
                int nipColumn = GetNipColumn(worksheet, headerRow);
                int lpColumn = GetLpColumn(worksheet, headerRow);
                foreach (var company in companiesReadFromFile)
                {
                    _overallVerificationResult.Add(company.ID, OverallResult.OK);
                }

                ClearEarlierResultsIfPresent(worksheet, headerRow, lastColumn, companiesReadFromFile, erroredWhileReadingInputFileCompanies);

                AddCompanyDataAndVerConfirmation(worksheet, headerRow, lastColumn, nipColumn, lpColumn, companiesReadFromFile,  verifiedCompanies, addAccountsToSeparateColumns);
                AddNIPVerification(worksheet, headerRow, lastColumn, nipColumn, lpColumn, companiesReadFromFile, verifiedNips);
                AddREGONVerification(worksheet, headerRow, lastColumn, nipColumn, lpColumn, companiesReadFromFile, areCompaniesActive);
                AddWhiteListVerification(worksheet, headerRow, lastColumn, nipColumn, lpColumn, companiesReadFromFile, verifiedCompanies);
                
                    AddWhiteListVerificationForInvoiceDate(worksheet, headerRow, lastColumn, nipColumn, lpColumn, companiesReadFromFile, verifiedCompaniesForInvoiceDate);
                
                
                AddErroredWhileReadingInputCompanies(worksheet, headerRow, lastColumn, nipColumn, lpColumn, erroredWhileReadingInputFileCompanies);
                AddOverallResultsToFile(worksheet, headerRow, lastColumn, nipColumn, lpColumn, companiesReadFromFile, erroredWhileReadingInputFileCompanies);

              
                theWorkbook.Save();
                theWorkbook.Close(true, _exportFilePath, Type.Missing); //true = save changes
                app.Quit();
            }
            catch (Exception e)
            {
                if (theWorkbook != null)
                    theWorkbook.Close(Type.Missing, Type.Missing, Type.Missing);
                if (app != null)
                    app.Quit();
                if (e as SpreadSheetReaderException != null)
                {
                    throw new System.Exception($"Błąd podczas odczytywania danych - zly format danych! {e.Message}", e);
                }
                else
                {
                    throw new System.Exception($"Nieznany blad podczas importu! {e.Message}", e);
                }

            }
        }

        private const string VerOKMsg = "OK";
        private const string VerFailedMsg = "Błąd";
        private const string VerWarningMsg = "Ostrzeżenie";
        private void AddOverallResultsToFile(Worksheet worksheet, int headerRow, int lastColumn, int nipColumn, int lpColumn, List<InputCompany> companiesReadFromFile, List<InputCompany> erroredWhileReadingInputFileCompanies)
        {
            int overallVerColumn = lastColumn + AllVerificationStatusColumnDelta;
            ((Range)worksheet.Cells[headerRow, overallVerColumn]).Formula = _exportColumnsConfig.First(c=> c.ID == ExportColumnName.ALLVerificationStatusHeader.ToString()).HeaderText;
            var allCompaniesRead = companiesReadFromFile.Concat(erroredWhileReadingInputFileCompanies);
            foreach (var company in allCompaniesRead)
            {
                string nipFromCell = ((Range)worksheet.Cells[company.RowNumber, nipColumn]).Formula.ToString().Trim();
                string lpFromCell = ((Range)worksheet.Cells[company.RowNumber, lpColumn]).Formula.ToString().Trim();
                if (company.ID != InputCompany.GetID(company.RowNumber, lpFromCell, nipFromCell))
                {
                    throw new ArgumentException("Nip z odpowiedzi serwera różny od nipu z pliku. Błąd przy zapisie overall result do pliku.");
                }

                var result = _overallVerificationResult[company.ID];

                if (result == OverallResult.Error)
                {
                    ((Range)worksheet.Cells[company.RowNumber, overallVerColumn]).Formula = VerFailedMsg;
                    ((Range)worksheet.Cells[company.RowNumber, overallVerColumn]).Font.Color = XlRgbColor.rgbWhite;
                    ((Range)worksheet.Cells[company.RowNumber, overallVerColumn]).Interior.Color = XlRgbColor.rgbRed;
                } 
                else if (result == OverallResult.Warning)
                {
                    ((Range)worksheet.Cells[company.RowNumber, overallVerColumn]).Formula = VerWarningMsg;
                    ((Range)worksheet.Cells[company.RowNumber, overallVerColumn]).Font.Color = XlRgbColor.rgbWhite;
                    ((Range)worksheet.Cells[company.RowNumber, overallVerColumn]).Interior.Color = XlRgbColor.rgbOrange;
                }
                else
                {
                    ((Range)worksheet.Cells[company.RowNumber, overallVerColumn]).Formula = VerOKMsg;
                }

            }
        }

        private void AddWhiteListVerification(Worksheet worksheet, int headerRow, int lastColumn, int nipColumn, int lpColumn, List<InputCompany> companiesReadFromFile, Dictionary<string, WhiteListVerResult> verifiedCompanies)
        {
            int whiteListVerColumn = lastColumn + WhiteListVerificationStatusColumnDelta;
            ((Range)worksheet.Cells[headerRow, whiteListVerColumn]).Formula = _exportColumnsConfig.First(c=>c.ID == ExportColumnName.WhiteListVerificationStatusHeader.ToString()).HeaderText;

            foreach (var company in companiesReadFromFile)
            {
                string nipFromCell = ((Range)worksheet.Cells[company.RowNumber, nipColumn]).Formula.ToString().Trim();
                string lpFromCell = ((Range)worksheet.Cells[company.RowNumber, lpColumn]).Formula.ToString().Trim();
                if (company.ID != InputCompany.GetID(company.RowNumber, lpFromCell, nipFromCell))
                {
                    throw new ArgumentException($"Nip z odpowiedzi serwera różny od nipu z pliku. Błąd przy zapisie wyniku białej listy do pliku. Z serwera: NIP={company.NIP}, LP={company.LP}; z pliku: NIP={nipFromCell}, LP={lpFromCell}.");
                }

                if (verifiedCompanies != null)
                {
                    var verificationResult = verifiedCompanies.FirstOrDefault(vN => vN.Key == company.ID);

                    if (!verificationResult.Equals(default(KeyValuePair<string, WhiteListVerResult>)))
                    {
                        var result = verificationResult.Value.ToMessage();

                        ((Range)worksheet.Cells[company.RowNumber, whiteListVerColumn]).Formula = result;

                        if (_overallVerificationResult[verificationResult.Key] != OverallResult.Error && (verificationResult.Value.VerificationStatus == WhiteListVerResultStatus.ActiveVATPayerVerScuccessButGivenAccountNotVerified || 
                            verificationResult.Value.VerificationStatus == WhiteListVerResultStatus.ActiveVATPayerButGivenAccountNotOnWhiteList ||
                            verificationResult.Value.VerificationStatus == WhiteListVerResultStatus.ActiveVATPayerButHasNoAccounts))
                        {
                            _overallVerificationResult[verificationResult.Key] = OverallResult.Warning;
                        }
                        //else if {WhiteListVerResultStatus.

                        //}
                        else if (verificationResult.Value.VerificationStatus != WhiteListVerResultStatus.ActiveVATPayerVerSuccessfull && verificationResult.Value.VerificationStatus != WhiteListVerResultStatus.ActiveVATPayerAccountOKVerSuccessfull && verificationResult.Value.VerificationStatus != WhiteListVerResultStatus.ActiveVATPayerButGivenAccountWrong && verificationResult.Value.VerificationStatus != WhiteListVerResultStatus.ActiveVATPayerButHasNoAccounts)
                        {
                            _overallVerificationResult[verificationResult.Key] = OverallResult.Error;
                        }
                    }
                } else
                {
                    ((Range)worksheet.Cells[company.RowNumber, whiteListVerColumn]).Formula = _notChecked;
                }
            }
        }

        private void AddWhiteListVerificationForInvoiceDate(Worksheet worksheet, int headerRow, int lastColumn, int nipColumn, int lpColumn, List<InputCompany> companiesReadFromFile, Dictionary<string, WhiteListVerResult> verifiedCompaniesForInvoiceDate)
        {
            int whiteListVerColumnForInvoiceDate = lastColumn + WhiteListVerificationForInvoiceDateStatusColumnDelta;
            int whiteListVerColumnForInvoiceDateConfirmString = lastColumn + WhiteListVerificationForInvoiceDateConfirmAndDateColumnDelta;

            ((Range)worksheet.Cells[headerRow, whiteListVerColumnForInvoiceDate]).Formula = _exportColumnsConfig.First(c => c.ID == ExportColumnName.WhiteListVerificationForInvoiceDateStatusHeader.ToString()).HeaderText;
            ((Range)worksheet.Cells[headerRow, whiteListVerColumnForInvoiceDateConfirmString]).Formula = _exportColumnsConfig.First(c => c.ID == ExportColumnName.WhiteListVerificationForInvoiceDateConfirmAndDateHeader.ToString()).HeaderText;

            if (verifiedCompaniesForInvoiceDate != null && verifiedCompaniesForInvoiceDate.Count > 0)
            {
                foreach (var company in companiesReadFromFile)
                {
                    string nipFromCell = ((Range)worksheet.Cells[company.RowNumber, nipColumn]).Formula.ToString().Trim();
                    string lpFromCell = ((Range)worksheet.Cells[company.RowNumber, lpColumn]).Formula.ToString().Trim();
                    if (company.ID != InputCompany.GetID(company.RowNumber, lpFromCell, nipFromCell))
                    {
                        throw new ArgumentException($"Nip z odpowiedzi serwera różny od nipu z pliku. Błąd przy zapisie wyniku białej listy do pliku na dzień faktury. Z serwera: NIP={company.NIP}, LP={company.LP}; z pliku: NIP={nipFromCell}, LP={lpFromCell}.");
                    }

                    if (verifiedCompaniesForInvoiceDate != null)
                    {
                        var verificationResult = verifiedCompaniesForInvoiceDate.FirstOrDefault(vN => vN.Key == company.ID);

                        if (!verificationResult.Equals(default(KeyValuePair<string, WhiteListVerResult>)))
                        {
                            var result = verificationResult.Value.ToMessage();

                            ((Range)worksheet.Cells[company.RowNumber, whiteListVerColumnForInvoiceDate]).Formula = result;
                            ((Range)worksheet.Cells[company.RowNumber, whiteListVerColumnForInvoiceDateConfirmString]).Formula = $"{verificationResult.Value.VerificationDate} - {verificationResult.Value.ConfirmationResponseString}";

                            if (verificationResult.Value.VerificationStatus == WhiteListVerResultStatus.ActiveVATPayerVerScuccessButGivenAccountNotVerified &&
                                _overallVerificationResult[verificationResult.Key] != OverallResult.Error)
                            {
                                _overallVerificationResult[verificationResult.Key] = OverallResult.Warning;
                            }
                            else if (verificationResult.Value.VerificationStatus != WhiteListVerResultStatus.ActiveVATPayerVerSuccessfull && verificationResult.Value.VerificationStatus != WhiteListVerResultStatus.ActiveVATPayerAccountOKVerSuccessfull && verificationResult.Value.VerificationStatus != WhiteListVerResultStatus.ActiveVATPayerButGivenAccountWrong && verificationResult.Value.VerificationStatus != WhiteListVerResultStatus.ActiveVATPayerButHasNoAccounts)
                            {
                                _overallVerificationResult[verificationResult.Key] = OverallResult.Error;
                            }
                        }
                    }
                    else
                    {
                        ((Range)worksheet.Cells[company.RowNumber, whiteListVerColumnForInvoiceDate]).Formula = _notChecked;
                    }
                }
            }
        }
        private void AddREGONVerification(Worksheet worksheet, int headerRow, int lastColumn, int nipColumn, int lpColumn, List<InputCompany> companiesReadFromFile, Dictionary<string, BiRVerifyResult> areCompaniesActive)
        {
            int regonVerColumn = lastColumn + REGONVerificationStatusColumnDelta;
            ((Range)worksheet.Cells[headerRow, regonVerColumn]).Formula = _exportColumnsConfig.First(c => c.ID == ExportColumnName.REGONVerificationStatusHeader.ToString()).HeaderText;

            foreach (var company in companiesReadFromFile)
            {
                string nipFromCell = ((Range)worksheet.Cells[company.RowNumber, nipColumn]).Formula.ToString().Trim();
                string lpFromCell = ((Range)worksheet.Cells[company.RowNumber, lpColumn]).Formula.ToString().Trim();
                if (company.ID != InputCompany.GetID(company.RowNumber, lpFromCell, nipFromCell))
                {
                    throw new ArgumentException($"Nip z odpowiedzi serwera różny od nipu z pliku. Błąd przy zapisie wyniku regon (BiR) do pliku.  Z serwera: NIP={company.NIP}, LP={company.LP}; z pliku: NIP={nipFromCell}, LP={lpFromCell}.");
                }

                if (areCompaniesActive != null)
                {
                    var isCompanyActiveStatus = areCompaniesActive.FirstOrDefault(vN => vN.Key == company.ID);

                    if (!isCompanyActiveStatus.Equals(default(KeyValuePair<string, BiRVerifyStatus>)))
                    {
                        var result = isCompanyActiveStatus.Value.Message;

                        ((Range)worksheet.Cells[company.RowNumber, regonVerColumn]).Formula = result;

                        if (isCompanyActiveStatus.Value.BiRVerifyStatus != BiRVerifyStatus.IsActive)
                            _overallVerificationResult[isCompanyActiveStatus.Key] = OverallResult.Error;
                    }
                }
                else
                {
                    ((Range)worksheet.Cells[company.RowNumber, regonVerColumn]).Formula = _notChecked;
                }

            }
        }

        private void AddNIPVerification(Worksheet worksheet, int headerRow, int lastColumn, int nipColumn, int lpColumn, List<InputCompany> inputCompanies, Dictionary<string, VerifyNIPResult> verifiedNips )
        {
            int nipVerColumn = lastColumn + NIPVerficationStatusColumnDelta;
            ((Range)worksheet.Cells[headerRow, nipVerColumn]).Formula = _exportColumnsConfig.First(c => c.ID == ExportColumnName.NIPVerificationStatusHeader.ToString()).HeaderText;

            foreach (var company in inputCompanies)
            {

                string nipFromCell = ((Range)worksheet.Cells[company.RowNumber, nipColumn]).Formula.ToString().Trim();
                string lpFromCell = ((Range)worksheet.Cells[company.RowNumber, lpColumn]).Formula.ToString().Trim();
                if (company.ID != InputCompany.GetID(company.RowNumber, lpFromCell, nipFromCell))
                {
                    throw new SpreadSheetWriterExcpetion($"Nip z odpowiedzi serwera różny od nipu z pliku. Błąd przy zapisywaniu wyniku zapytania NIP verification do pliku xls. Z serwera: NIP={company.NIP}, LP={company.LP}; z pliku: NIP={nipFromCell}, LP={lpFromCell}.");
                }

                if (verifiedNips != null)
                {
                    var nipVerifyStatus = verifiedNips.FirstOrDefault(vN => vN.Key == company.ID);


                    if (!nipVerifyStatus.Equals(default(KeyValuePair<string, VerifyNIPResult>)))
                    {
                        var result = nipVerifyStatus.Value.ToMessage();
                        ((Range)worksheet.Cells[company.RowNumber, nipVerColumn]).Formula = result;

                        if (nipVerifyStatus.Value != VerifyNIPResult.IsActiveVATPayer)
                            _overallVerificationResult[nipVerifyStatus.Key] = OverallResult.Error;
                    }
                }
                else
                {
                    ((Range)worksheet.Cells[company.RowNumber, nipVerColumn]).Formula = _notChecked;
                }

            }
        }

        private void AddErroredWhileReadingInputCompanies(Worksheet worksheet, int headerRow, int lastColumn, int nipColumn, int lpColumn, List<InputCompany> verifiedCompanies)
        {
            int inputErrorColumn = lastColumn + InputErrorColumnDelta;
            ((Range)worksheet.Cells[headerRow, inputErrorColumn]).Formula = _exportColumnsConfig.First(c => c.ID == ExportColumnName.ImportFileError.ToString()).HeaderText;

            foreach (var company in verifiedCompanies)
            {
                string nipFromCell = ((Range)worksheet.Cells[company.RowNumber, nipColumn]).Formula.ToString().Trim();
                string lpFromCell = ((Range)worksheet.Cells[company.RowNumber, lpColumn]).Formula.ToString().Trim();
                if (company.ID != InputCompany.GetID(company.RowNumber, lpFromCell, nipFromCell))
                {
                    throw new ArgumentException($"Nip wczytany z pliku różny od nipu z pliku wyjściowego. Błąd przy zapisie wyniku importu danych z pliku. Z pliku wejściowego: NIP={company.NIP}, LP={company.LP}; z pliku wyjściowego: NIP={nipFromCell}, LP={lpFromCell}.");
                }

                if (company.FormatErrors != null && company.FormatErrors.Count > 0)
                {
                    StringBuilder sB = new StringBuilder();
                    foreach (var error in company.FormatErrors)
                    {
                        sB.Append($"{error.ToMessage()} ");
                    }

                    ((Range)worksheet.Cells[company.RowNumber, inputErrorColumn]).Formula = sB.ToString().Trim();
                    ((Range)worksheet.Cells[company.RowNumber, inputErrorColumn]).Font.Color = XlRgbColor.rgbWhite;
                    ((Range)worksheet.Cells[company.RowNumber, inputErrorColumn]).Interior.Color = XlRgbColor.rgbRed;

                    _overallVerificationResult[company.ID] = OverallResult.Error;
                }
            }
        }

        private void AddCompanyDataAndVerConfirmation(Worksheet worksheet, int headerRow, int lastColumn, int nipColumn, int lpColumn, List<InputCompany> companiesReadFromFile, Dictionary<string, WhiteListVerResult> verifiedCompanies, bool addAccountsToSeparateColumns)
        {
            int accountsColumn = lastColumn + AllAccountsColumnDelta;
            int fullNameColumn = lastColumn + FullNameColumnDelta;
            int dataColumn = lastColumn + DateColumnDelta;
            int stringConfIDColumn = lastColumn + StringConfIDColumnDelta;
            int accountSepColStartingColumn = lastColumn + AccountSeparteColumnStartDelta;
            int fullResidenceAddressColumn = lastColumn + FullResidenceAddressColumnDelta;
            int fullWorkingAddressColumn = lastColumn + FullWorkingAddressColumnDelta;

            ((Range)worksheet.Cells[headerRow, accountsColumn]).Formula = _exportColumnsConfig.First(c=> c.ID == ExportColumnName.AllAccountsHeader.ToString()).HeaderText;
            ((Range)worksheet.Cells[headerRow, fullNameColumn]).Formula = _exportColumnsConfig.First(c => c.ID == ExportColumnName.FullNameColumnHeader.ToString()).HeaderText;
            ((Range)worksheet.Cells[headerRow, dataColumn]).Formula = _exportColumnsConfig.First(c => c.ID == ExportColumnName.DateColumnHeader.ToString()).HeaderText;
            ((Range)worksheet.Cells[headerRow, stringConfIDColumn]).Formula = _exportColumnsConfig.First(c => c.ID == ExportColumnName.StringConfIDHeader.ToString()).HeaderText;
            ((Range)worksheet.Cells[headerRow, fullResidenceAddressColumn]).Formula = _exportColumnsConfig.First(c => c.ID == ExportColumnName.FullResidenceAddressHeader.ToString()).HeaderText;
            ((Range)worksheet.Cells[headerRow, fullWorkingAddressColumn]).Formula = _exportColumnsConfig.First(c => c.ID == ExportColumnName.FullWorkingAddressHeader.ToString()).HeaderText;

            int maxNumberOfAccounts = 0;
            if (addAccountsToSeparateColumns)
            {
                if (verifiedCompanies.Count(kp => kp.Value.AccountNumbers != null) > 0)
                    maxNumberOfAccounts = verifiedCompanies.Where(kp => kp.Value.AccountNumbers != null).Max(kp => kp.Value.AccountNumbers.Count);

                for (int i = 0; i < maxNumberOfAccounts; i++)
                {
                    ((Range)worksheet.Cells[headerRow, accountSepColStartingColumn + i]).Formula = string.Format("{0}{1}", _prefixOfHeaders, i);
                }
            }

            //If e.g., there was no check run in the white list company checker
            if (verifiedCompanies != null)
            {
                AddCompanyData(worksheet, nipColumn, lpColumn, companiesReadFromFile, verifiedCompanies, addAccountsToSeparateColumns, accountsColumn, fullNameColumn, dataColumn, stringConfIDColumn, accountSepColStartingColumn, fullResidenceAddressColumn, fullWorkingAddressColumn);
            }

        }

        private static void AddCompanyData(Worksheet worksheet, int nipColumn, int lpColumn, List<InputCompany> companiesReadFromFile, Dictionary<string, WhiteListVerResult> verifiedCompanies, bool addAccountsToSeparateColumns, int accountsColumn, int fullNameColumn, int dataColumn, int stringConfIDColumn, int accountSepColStartingColumn, int fullResidenceAddressColumn, int fullWorkingAddressColumn)
        {
            foreach (var company in companiesReadFromFile)
            {
                string nipFromCell = ((Range)worksheet.Cells[company.RowNumber, nipColumn]).Formula.ToString().Trim();
                string lPFromCell = ((Range)worksheet.Cells[company.RowNumber, lpColumn]).Formula.ToString().Trim();

                if (company.ID != InputCompany.GetID(company.RowNumber, lPFromCell, nipFromCell))
                {
                    throw new SpreadSheetWriterExcpetion($"Nip z odpowiedzi serwera różny od nipu z pliku. Błąd przy zapisie do pliku. Z serwera: NIP={company.NIP}, LP={company.LP}; z pliku: NIP={nipFromCell}, LP={lPFromCell}.");
                }

                var verifiedCompResult = verifiedCompanies[company.ID];
                if (verifiedCompResult != null)
                {
                    var accounts = verifiedCompResult.AccountNumbers;
                    ((Range)worksheet.Cells[company.RowNumber, accountsColumn]).Formula = DataFormatHelper.GetAccountsInString(accounts);

                    var fullName = verifiedCompResult.FullName;
                    ((Range)worksheet.Cells[company.RowNumber, fullNameColumn]).Formula = fullName;

                    ((Range)worksheet.Cells[company.RowNumber, dataColumn]).Formula = verifiedCompResult.VerificationDate;

                    ((Range)worksheet.Cells[company.RowNumber, stringConfIDColumn]).Formula = verifiedCompResult.ConfirmationResponseString;

                    ((Range)worksheet.Cells[company.RowNumber, fullResidenceAddressColumn]).Formula = verifiedCompResult.FullResidenceAddress;

                    ((Range)worksheet.Cells[company.RowNumber, fullWorkingAddressColumn]).Formula = verifiedCompResult.FullWorkingAddress;

                    if (addAccountsToSeparateColumns && verifiedCompResult.AccountNumbers != null)
                    {
                        for (int j = 0; j < verifiedCompResult.AccountNumbers.Count; j++)
                        {
                            ((Range)worksheet.Cells[company.RowNumber, accountSepColStartingColumn + j]).Formula = DataFormatHelper.GetAccountInString(verifiedCompResult.AccountNumbers[j]);
                        }
                    }

                }
            }
        }

        private void ClearEarlierResultsIfPresent(Worksheet worksheet, int headerRow, int lastColumn, List<InputCompany> verifiedCompanies, List<InputCompany> erroredWhileReadingInputFileCompanies)
        {
            foreach (var company in verifiedCompanies)
            {
                Range rangeToClean = worksheet.Range[SpreadSheetHelper.ConvertCellAddresFromNumsToLetterNum(company.RowNumber, lastColumn + 1), SpreadSheetHelper.ConvertCellAddresFromNumsToLetterNum(company.RowNumber, lastColumn + _numberOfColumnsToClean)];
                ClearRange(rangeToClean);
            }

            foreach (var errcompany in erroredWhileReadingInputFileCompanies)
            {
                Range rangeToClean = worksheet.Range[SpreadSheetHelper.ConvertCellAddresFromNumsToLetterNum(errcompany.RowNumber, lastColumn + 1), SpreadSheetHelper.ConvertCellAddresFromNumsToLetterNum(errcompany.RowNumber, lastColumn + _numberOfColumnsToClean)];
                ClearRange(rangeToClean);
            }

            Range headerRange = worksheet.Range[SpreadSheetHelper.ConvertCellAddresFromNumsToLetterNum(headerRow, lastColumn + 1), SpreadSheetHelper.ConvertCellAddresFromNumsToLetterNum(headerRow, lastColumn + _numberOfColumnsToClean)];
            headerRange.Font.Bold = true;
            headerRange.Interior.Color = XlRgbColor.rgbLightGray;
            //headerRange.Formula = string.Empty;
            headerRange.WrapText = true;

        }

        private static void ClearRange(Range rangeToClean)
        {
            Borders bordersToClean = rangeToClean.Borders;
            bordersToClean[XlBordersIndex.xlEdgeLeft].LineStyle = XlLineStyle.xlContinuous;
            bordersToClean[XlBordersIndex.xlEdgeRight].LineStyle = XlLineStyle.xlContinuous;
            bordersToClean[XlBordersIndex.xlEdgeTop].LineStyle = XlLineStyle.xlContinuous;
            bordersToClean[XlBordersIndex.xlEdgeBottom].LineStyle = XlLineStyle.xlContinuous;
            bordersToClean.Color = XlRgbColor.rgbBlack;
            rangeToClean.Formula = string.Empty;
            rangeToClean.Font.Color = XlRgbColor.rgbBlack;
            rangeToClean.Interior.Pattern = XlPattern.xlPatternNone;
        }

        private int GetLpColumn(Worksheet worksheet, int headerRow)
        {
            string lpHeaderCaption = _importColumnsConfig.FirstOrDefault(c => c.ID.ToLower().Contains(ImportColumnName.LP.ToString().ToLower())).HeaderText.ToLower();

            for (int column = 1; column <= 75; column++)
            {
                var tempCellContent = ((string)((Range)worksheet.Cells[headerRow, column]).Formula).Trim().ToLower();
                if (tempCellContent.StartsWith(lpHeaderCaption))
                {
                    return column;
                }
            }
            throw new SpreadSheetWriterExcpetion("Błąd formatu pliku. Brak kolumny LP.");
        }

        private int GetNipColumn(Worksheet worksheet, int headerRow)
        {
            string nipHeaderCaption = _importColumnsConfig.FirstOrDefault(c => c.ID.ToLower().Contains(ImportColumnName.NIP.ToString().ToLower())).HeaderText.ToLower();

            for (int column = 1; column <= 75; column++)
            {
                var tempCellContent = ((string)((Range)worksheet.Cells[headerRow, column]).Formula).Trim().ToLower();
                if (tempCellContent.StartsWith(nipHeaderCaption))
                {
                    return column;
                }
            }

            throw new SpreadSheetWriterExcpetion("Błąd formatu pliku. Brak kolumny NIPu.");
        }

        private int GetLastColumnWithOriginalData(Worksheet worksheet, int headerRow)
        {
            for (int column = 1; column <= 75; column++)
            {
                var tempCellContent = ((string)((Range)worksheet.Cells[headerRow, column]).Formula).Trim().ToLower();
                if (string.IsNullOrEmpty(tempCellContent))
                {
                    return column - 1;

                }
                else if (tempCellContent.StartsWith(_prefixOfHeaders.ToLower()))
                {
                    return column - 1;
                }
            }

            throw new SpreadSheetWriterExcpetion("Błąd formatu pliku. Brak ostatniej kolumny.");
        }
    }
}
