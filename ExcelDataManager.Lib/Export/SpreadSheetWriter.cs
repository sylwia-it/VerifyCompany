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
        private List<ColumnConfig> _columnsConfig;
        private string _prefixOfHeaders;
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

        private const int DateColumnDelta = 9;
        private const int StringConfIDColumnDelta = 10;
        private const int AccountSeparteColumnStartDelta = 11;
        private int _numberOfColumnsToClean = 12 + 20;
        public SpreadSheetWriter(string exportFilePath)
        {
            _exportFilePath = exportFilePath;
            IConfiguration configuration = new ConfigurationBuilder()
               .AddJsonFile("importSettings.json", true, true)
               .Build();
            _columnsConfig = configuration.GetSection("ExportColumnsConfig").Get<List<ColumnConfig>>();
            _sheetNamesToExclude = configuration.GetSection("InputSheetNameExclude").Get<List<string>>();
            _prefixOfHeaders = configuration.GetSection("ExportStartLettersInHeader").Get<string>();

            if (_columnsConfig.Count != Enum.GetNames(typeof(ExportColumnName)).Length)
                throw new SpreadSheetWriterExcpetion("W pliku konfiguracyjnym brakuje wszyskich komumn.");
     
        }

        
       public void WriteResultsToFile(List<InputCompany> companiesReadFromFile, Dictionary<string, VerifyNIPResult> verifiedNips, Dictionary<string, BiRVerifyStatus> areCompaniesActive, Dictionary<string, WhiteListVerResult> verifiedCompanies, bool addAccountsToSeparateColumns)
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

                int headerRow = SpreadSheetHelper.FindHeaderRow(worksheet, _columnsConfig);
                int lastColumn = GetLastColumnWithOriginalData(worksheet, headerRow);
                int nipColumn = GetNipColumn(worksheet, headerRow);
                int lpColumn = GetLpColumn(worksheet, headerRow);
                foreach (var verifiedNip in verifiedNips)
                {
                    _overallVerificationResult.Add(verifiedNip.Key, OverallResult.OK);
                }

                ClearEarlierResultsIfPresent(worksheet, headerRow, lastColumn, companiesReadFromFile);

                AddCompanyDataAndVerConfirmation(worksheet, headerRow, lastColumn, nipColumn, lpColumn, companiesReadFromFile,  verifiedCompanies, addAccountsToSeparateColumns);
                AddNIPVerification(worksheet, headerRow, lastColumn, nipColumn, lpColumn, companiesReadFromFile, verifiedNips);
                AddREGONVerification(worksheet, headerRow, lastColumn, nipColumn, lpColumn, companiesReadFromFile, areCompaniesActive);
                AddWhiteListVerification(worksheet, headerRow, lastColumn, nipColumn, lpColumn, companiesReadFromFile, verifiedCompanies);
                AddOverallResultsToFile(worksheet, headerRow, lastColumn, nipColumn, lpColumn, companiesReadFromFile);

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
        private void AddOverallResultsToFile(Worksheet worksheet, int headerRow, int lastColumn, int nipColumn, int lpColumn, List<InputCompany> companiesReadFromFile)
        {
            int overallVerColumn = lastColumn + AllVerificationStatusColumnDelta;
            ((Range)worksheet.Cells[headerRow, overallVerColumn]).Formula = _columnsConfig.First(c=> c.ID == ExportColumnName.ALLVerificationStatusHeader.ToString());
            foreach (var company in companiesReadFromFile)
            {
                string nipFromCell = ((Range)worksheet.Cells[company.RowNumber, nipColumn]).Formula.ToString().Trim();
                string lpFromCell = ((Range)worksheet.Cells[company.RowNumber, lpColumn]).Formula.ToString().Trim();
                if (company.ID != InputCompany.GetID(lpFromCell, nipFromCell))
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
            ((Range)worksheet.Cells[headerRow, whiteListVerColumn]).Formula = _columnsConfig.First(c=>c.ID == ExportColumnName.WhiteListVerificationStatusHeader.ToString());

            foreach (var company in companiesReadFromFile)
            {
                string nipFromCell = ((Range)worksheet.Cells[company.RowNumber, nipColumn]).Formula.ToString().Trim();
                string lpFromCell = ((Range)worksheet.Cells[company.RowNumber, lpColumn]).Formula.ToString().Trim();
                if (company.ID != InputCompany.GetID(lpFromCell, nipFromCell))
                {
                    throw new ArgumentException($"Nip z odpowiedzi serwera różny od nipu z pliku. Błąd przy zapisie wyniku białej listy do pliku. Z serwera: NIP={company.NIP}, LP={company.LP}; z pliku: NIP={nipFromCell}, LP={lpFromCell}.");
                }
                var verificationResult = verifiedCompanies.FirstOrDefault(vN => vN.Key == company.ID);


                if (!verificationResult.Equals(default(KeyValuePair<string, WhiteListVerResult>)))
                {
                    var result = verificationResult.Value.ToMessage();

                    ((Range)worksheet.Cells[company.RowNumber, whiteListVerColumn]).Formula = result;

                    if (verificationResult.Value.VerificationStatus == WhiteListVerResultStatus.ActiveVATPayerVerScuccessButGivenAccountNotVerified &&
                        _overallVerificationResult[verificationResult.Key] != OverallResult.Error)
                    {
                        _overallVerificationResult[verificationResult.Key] = OverallResult.Warning;
                    }
                    else if (verificationResult.Value.VerificationStatus != WhiteListVerResultStatus.ActiveVATPayerVerSuccessfull || verificationResult.Value.VerificationStatus != WhiteListVerResultStatus.ActiveVATPayerAccountOKVerSuccessfull || verificationResult.Value.VerificationStatus != WhiteListVerResultStatus.ActiveVATPayerButGivenAccountWrong || verificationResult.Value.VerificationStatus != WhiteListVerResultStatus.ActiveVATPayerButHasNoAccounts)
                    {
                        _overallVerificationResult[verificationResult.Key] = OverallResult.Error;
                    } 
                }
            }
        }

        private void AddREGONVerification(Worksheet worksheet, int headerRow, int lastColumn, int nipColumn, int lpColumn, List<InputCompany> companiesReadFromFile, Dictionary<string, BiRVerifyStatus> areCompaniesActive)
        {
            int regonVerColumn = lastColumn + REGONVerificationStatusColumnDelta;
            ((Range)worksheet.Cells[headerRow, regonVerColumn]).Formula = _columnsConfig.First(c => c.ID == ExportColumnName.REGONVerificationStatusHeader.ToString());

            foreach (var company in companiesReadFromFile)
            {
                string nipFromCell = ((Range)worksheet.Cells[company.RowNumber, nipColumn]).Formula.ToString().Trim();
                string lpFromCell = ((Range)worksheet.Cells[company.RowNumber, lpColumn]).Formula.ToString().Trim();
                if (company.ID != InputCompany.GetID(lpFromCell, nipFromCell))
                {
                    throw new ArgumentException($"Nip z odpowiedzi serwera różny od nipu z pliku. Błąd przy zapisie wyniku regon (BiR) do pliku.  Z serwera: NIP={company.NIP}, LP={company.LP}; z pliku: NIP={nipFromCell}, LP={lpFromCell}.");
                }

                var isCompanyActiveStatus = areCompaniesActive.FirstOrDefault(vN => vN.Key == company.ID);

                if (!isCompanyActiveStatus.Equals(default(KeyValuePair<string, BiRVerifyStatus>)))
                {
                    var result = isCompanyActiveStatus.Value.ToMessage();

                    ((Range)worksheet.Cells[company.RowNumber, regonVerColumn]).Formula = result;

                    if (isCompanyActiveStatus.Value != BiRVerifyStatus.IsActive)
                        _overallVerificationResult[isCompanyActiveStatus.Key] = OverallResult.Error;
                }

            }
        }

        private void AddNIPVerification(Worksheet worksheet, int headerRow, int lastColumn, int nipColumn, int lpColumn, List<InputCompany> inputCompanies, Dictionary<string, VerifyNIPResult> verifiedNips )
        {
            int nipVerColumn = lastColumn + NIPVerficationStatusColumnDelta;
            ((Range)worksheet.Cells[headerRow, nipVerColumn]).Formula = _columnsConfig.First(c => c.ID == ExportColumnName.NIPVerificationStatusHeader.ToString());

            foreach (var company in inputCompanies)
            {

                string nipFromCell = ((Range)worksheet.Cells[company.RowNumber, nipColumn]).Formula.ToString().Trim();
                string lpFromCell = ((Range)worksheet.Cells[company.RowNumber, lpColumn]).Formula.ToString().Trim();
                if (company.ID != InputCompany.GetID(lpFromCell, nipFromCell))
                {
                    throw new SpreadSheetWriterExcpetion($"Nip z odpowiedzi serwera różny od nipu z pliku. Błąd przy zapisywaniu wyniku zapytania NIP verification do pliku xls. Z serwera: NIP={company.NIP}, LP={company.LP}; z pliku: NIP={nipFromCell}, LP={lpFromCell}.");
                }

                var nipVerifyStatus = verifiedNips.FirstOrDefault(vN => vN.Key == company.ID);


                if (!nipVerifyStatus.Equals(default(KeyValuePair<string, VerifyNIPResult>)))
                {
                    var result = nipVerifyStatus.Value.ToMessage();
                    ((Range)worksheet.Cells[company.RowNumber, nipVerColumn]).Formula = result;

                    if (nipVerifyStatus.Value != VerifyNIPResult.IsActiveVATPayer)
                        _overallVerificationResult[nipVerifyStatus.Key] = OverallResult.Error;
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

            ((Range)worksheet.Cells[headerRow, accountsColumn]).Formula = _columnsConfig.First(c=> c.ID == ExportColumnName.AllAccountsHeader.ToString());
            ((Range)worksheet.Cells[headerRow, fullNameColumn]).Formula = _columnsConfig.First(c => c.ID == ExportColumnName.FullNameColumnHeader.ToString());
            ((Range)worksheet.Cells[headerRow, dataColumn]).Formula = _columnsConfig.First(c => c.ID == ExportColumnName.DateColumnHeader.ToString());
            ((Range)worksheet.Cells[headerRow, stringConfIDColumn]).Formula = _columnsConfig.First(c => c.ID == ExportColumnName.StringConfIDHeader.ToString());
            ((Range)worksheet.Cells[headerRow, fullResidenceAddressColumn]).Formula = _columnsConfig.First(c => c.ID == ExportColumnName.FullResidenceAddressHeader.ToString());
            ((Range)worksheet.Cells[headerRow, fullWorkingAddressColumn]).Formula = _columnsConfig.First(c => c.ID == ExportColumnName.FullWorkingAddressHeader.ToString());

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



            foreach (var company in companiesReadFromFile)
            {
                string nipFromCell = ((Range)worksheet.Cells[company.RowNumber, nipColumn]).Formula.ToString().Trim();
                string lPFromCell = ((Range)worksheet.Cells[company.RowNumber, lpColumn]).Formula.ToString().Trim();

                if (company.ID != InputCompany.GetID(lPFromCell, nipFromCell))
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



        private void ClearEarlierResultsIfPresent(Worksheet worksheet, int headerRow, int lastColumn, List<InputCompany> verifiedCompanies)
        {
            foreach (var company in verifiedCompanies)
            {
                Range rangeToClean = worksheet.Range[SpreadSheetHelper.ConvertCellAddresFromNumsToLetterNum(company.RowNumber, lastColumn + 1), SpreadSheetHelper.ConvertCellAddresFromNumsToLetterNum(company.RowNumber, lastColumn + _numberOfColumnsToClean)];
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
            Range headerRange = worksheet.Range[SpreadSheetHelper.ConvertCellAddresFromNumsToLetterNum(headerRow, lastColumn + 1), SpreadSheetHelper.ConvertCellAddresFromNumsToLetterNum(headerRow, lastColumn + _numberOfColumnsToClean)];
            headerRange.Font.Bold = true;
            headerRange.Interior.Color = XlRgbColor.rgbLightGray;
            headerRange.Formula = string.Empty;
            headerRange.WrapText = true;

        }

        private int GetLpColumn(Worksheet worksheet, int headerRow)
        {
            string lpHeaderCaption = _columnsConfig.FirstOrDefault(c => c.ID.Contains(ImportColumnName.LP.ToString(), StringComparison.OrdinalIgnoreCase)).HeaderText;

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
            string nipHeaderCaption = _columnsConfig.FirstOrDefault(c => c.ID.Contains(ImportColumnName.NIP.ToString(), StringComparison.OrdinalIgnoreCase)).HeaderText;

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
                var tempCellContent = ((string)((Range)worksheet.Cells[headerRow, column]).Formula).Trim();
                if (string.IsNullOrEmpty(tempCellContent))
                {
                    return column - 1;

                }
                else if (tempCellContent.StartsWith(_prefixOfHeaders))
                {
                    return column - 1;
                }
            }

            throw new SpreadSheetWriterExcpetion("Błąd formatu pliku. Brak ostatniej kolumny.");
        }
    }
}
