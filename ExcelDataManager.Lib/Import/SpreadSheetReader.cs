using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ExcelDataManager.Lib;
using Microsoft.Extensions.Configuration;

using Microsoft.Extensions.Configuration.Json;
using Microsoft.Office.Interop.Excel;
using VerifyCompany.Common.Lib;
using Range = Microsoft.Office.Interop.Excel.Range;

namespace ExcelDataManager.Lib.Import
{
    public class SpreadSheetReader
    {
        private bool _generateNotes;
        private bool _readInvoiceDate;
        private string _filePath;
        private List<ColumnConfig> _columnsConfig; //mapping of columns to specific header text in spreadsheet
        private List<string>_sheetNamesToExclude; //name of sheet that shall be considered as source
        private string _columnPrefixToIgnore;

        public Dictionary<ImportColumnName, int> ColumnMapping { get; private set; }
        private int HeaderRow { get; set; }
        private List<InputCompany> CompaniesReadFromFile { get; set; }
        
        public SpreadSheetReader(string filePath, bool generateNotes, bool readInvoiceDate)
        {
            _generateNotes = generateNotes;
            _readInvoiceDate = readInvoiceDate;
            _filePath = filePath;
            ColumnMapping = new Dictionary<ImportColumnName, int>();
            LoadSettings();
        }

        private void LoadSettings()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("importSettings.json", true, true)
                .Build();
            _columnsConfig = configuration.GetSection("InputColumnsConfig").Get<List<ColumnConfig>>();
            _columnPrefixToIgnore = configuration.GetSection("ExportStartLettersInHeader").Get<string>().ToLower();
            _sheetNamesToExclude = configuration.GetSection("InputSheetNameExclude").Get<List<string>>();
        }
            
        

        public List<InputCompany> ReadDataFromFile()
        {

            Application app = null;
            Workbook theWorkbook = null;
            
            try
            {
                app = new Application();
                theWorkbook = app.Workbooks.Open(
                _filePath, true, false,
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

                AnalyzeWorksheet(worksheet);


                theWorkbook.Close(false, Type.Missing, Type.Missing);
                app.Quit();

                return CompaniesReadFromFile;

            }
            catch (Exception e)
            {
                if (theWorkbook != null)
                    theWorkbook.Close(Type.Missing, Type.Missing, Type.Missing);
                if (app != null)
                    app.Quit();
                if (e as SpreadSheetReaderException == null)
                {
                    throw new Exception("Nieznany błąd podczas importu!" + e.Message, e);
                }

                throw;
            }
        }

       
        private void AnalyzeWorksheet(Worksheet worksheet)
        {
            string lastProcessedItem = "Jeszcze nie zaczelismy przetwarzania";
            int lastReadColumn = -1;
            string tempStr = string.Empty;

            CompaniesReadFromFile = new List<InputCompany>();

            try
            {
                lastProcessedItem = "Przed wczytywaniem nagłówka";
                HeaderRow = -1;

                HeaderRow = SpreadSheetHelper.FindHeaderRow(worksheet, _columnsConfig);

                if (HeaderRow == -1)
                {
                    throw new SpreadSheetReaderHeaderException(string.Format("Błąd formatu aruksza. W arkuszu: {0} nie znaleziono nagłówka danych. Sprawdź czy któryś nagłówek zawiera słowo 'nip' albo czy pierwszy arkusz w pliku to arkusz z danymi.", worksheet.Name));
                }

                DetermineColumns(worksheet);
                ValidateColumns();

                lastProcessedItem = "Wczytano nagłówek";


                InputCompany tempCompany;
                int orderOfCompany = 1;
                for (int i = HeaderRow + 1; !IsEndOfTable(i, 1, worksheet); i++)
                {
                    tempCompany = new InputCompany(orderOfCompany);
                    orderOfCompany++;
                    tempCompany.RowNumber = i;

                    lastProcessedItem = $"Przed wczytaniem NIPu. Aktualna pozycja: wiersz {i}";
                    lastReadColumn = ColumnMapping[ImportColumnName.NIP];
                    tempStr = ((Range)worksheet.Cells[i, ColumnMapping[ImportColumnName.NIP]]).Formula.ToString();
                    tempCompany.NIP = GetNip(tempStr);
                    tempStr = string.Empty;
                    lastProcessedItem = $"Wczytano NIP. Aktualna pozycja: wiersz {i}";

                    lastProcessedItem = $"Przed wczytaniem konta bankowego. Aktualna pozycja: wiersz {i}";
                    lastReadColumn = ColumnMapping[ImportColumnName.AccountNumber];
                    tempStr = ((Range)worksheet.Cells[i, ColumnMapping[ImportColumnName.AccountNumber]]).Formula.ToString();
                    tempCompany.BankAccountNumber = GetBankAccountNumber(tempStr);
                    tempStr = string.Empty;
                    lastProcessedItem = $"Wczytano Numer Konta. Aktualna pozycja: wiersz {i}";

                    lastProcessedItem = $"Przed wczytaniem LP. Aktualna pozycja: wiersz {i}";
                    lastReadColumn = ColumnMapping[ImportColumnName.LP];
                    tempStr = ((Range)worksheet.Cells[i, ColumnMapping[ImportColumnName.LP]]).Formula.ToString();
                    tempCompany.LP = GetLP(tempStr);
                    tempStr = string.Empty;
                    if (CompaniesReadFromFile.Any(c => c.ID == tempCompany.ID))
                    {
                        throw new SpreadSheetReaderException($"Dane zawierają już wpis o takiej samej pozycji i nipie. Aktualna wiersz: {i}, Nip: {tempCompany.NIP}, LP: {tempCompany.LP}");
                    }
                    lastProcessedItem = $"Wczytano LP.Aktualna pozycja: wiersz {i}";


                    lastProcessedItem = $"Przed wczytaniem daty zapłaty. Aktualna pozycja: wiersz {i}";
                    lastReadColumn = ColumnMapping[ImportColumnName.PaymentDate];
                    var dateRange = ((Range)worksheet.Cells[i, ColumnMapping[ImportColumnName.PaymentDate]]);
                    tempStr = dateRange.Formula.ToString();
                    tempCompany.PaymentDate = GetDate(dateRange);
                    tempStr = string.Empty;
                    lastProcessedItem = $"Wczytano datę zapłaty. Aktualna pozycja: wiersz {i}";

                    if (ColumnMapping.ContainsKey(ImportColumnName.InvoiceDate))
                    {
                        lastProcessedItem = $"Przed wczytaniem daty faktury. Aktualna pozycja: wiersz {i}";
                        lastReadColumn = ColumnMapping[ImportColumnName.InvoiceDate];
                        var invoiceDateRange = ((Range)worksheet.Cells[i, ColumnMapping[ImportColumnName.InvoiceDate]]);
                        tempStr = invoiceDateRange.Formula.ToString();

                        double tempDateInDouble;
                        if (double.TryParse(tempStr, out tempDateInDouble))
                        {
                            try
                            {
                                tempCompany.InvoiceDate = DateTime.FromOADate(tempDateInDouble);
                            }
                            catch (ArgumentException)
                            { tempCompany.FormatErrors.Add(InputCompanyFormatError.InvoiceDateError); }
                        }
                        else
                        {
                            tempCompany.FormatErrors.Add(InputCompanyFormatError.InvoiceDateError);
                        }
                        tempStr = string.Empty;
                        lastProcessedItem = $"Wczytano datę faktury. Aktualna pozycja: wiersz {i}";
                    }


                    if ((_generateNotes && AreColumnsToGenerateNotesPresent()))
                    {
                        lastProcessedItem = $"Przed wczytaniem ID noty. Aktualna pozycja: wiersz {i}";
                        lastReadColumn = ColumnMapping[ImportColumnName.NoteID];
                        tempStr = ((Range)worksheet.Cells[i, ColumnMapping[ImportColumnName.NoteID]]).Formula.ToString().Trim();
                        tempCompany.NoteID = tempStr;
                        tempStr = string.Empty;
                        lastProcessedItem = $"Wczytano ID Noty. Aktualna pozycja: wiersz {i}";

                        lastProcessedItem = $"Przed wczytaniem tytułu Noty. Aktualna pozycja: wiersz {i}";
                        lastReadColumn = ColumnMapping[ImportColumnName.NoteTitle];
                        tempStr = ((Range)worksheet.Cells[i, ColumnMapping[ImportColumnName.NoteTitle]]).Formula.ToString().Trim();
                        tempCompany.NoteTitle = tempStr;
                        tempStr = string.Empty;
                        lastProcessedItem = $"Wczytano tytuł Noty. Aktualna pozycja: wiersz {i}";

                        lastProcessedItem = $"Przed wczytaniem netto noty. Aktualna pozycja: wiersz {i}";
                        lastReadColumn = ColumnMapping[ImportColumnName.NoteAmount];
                        tempStr = ((Range)worksheet.Cells[i, ColumnMapping[ImportColumnName.NoteAmount]]).Formula.ToString().Trim();
                        tempCompany.NoteNettoAmount = tempStr;
                        tempStr = string.Empty;
                        lastProcessedItem = $"Wczytano netto noty. Aktualna pozycja: wiersz {i}";

                        lastProcessedItem = $"Przed wczytaniem daty noty. Aktualna pozycja: wiersz {i}";
                        lastReadColumn = ColumnMapping[ImportColumnName.NoteDate];
                        var dataRange = (Range)worksheet.Cells[i, ColumnMapping[ImportColumnName.NoteDate]];
                        tempStr = dataRange.Formula.ToString();
                        tempCompany.NoteDate = GetDate(dataRange);
                        tempStr = string.Empty;
                        lastProcessedItem = $"Wczytano datę noty. Aktualna pozycja: wiersz {i}";
                    }
                    tempCompany.FormatErrors.AddRange(ValidateCompany(tempCompany));

                    CompaniesReadFromFile.Add(tempCompany);
                }
            }
            catch (Exception e)
            {
                if (e is SpreadSheetReaderHeaderException || e is SpreadSheetReaderMissingColumnsException)
                {
                    throw;
                }

                string errorMsg = string.Format("\nZłapano błąd w rzędzie gdy procedura była na kroku: {0}, w kolumnie: {2}, odczytała wartość: {1}.\n\n", lastProcessedItem, tempStr, lastReadColumn);
                throw new SpreadSheetReaderException(errorMsg, e);
            }

       
        }

        private void ValidateColumns()
        {
            bool areColumnsForNotesPresent = AreColumnsToGenerateNotesPresent();
            bool isAtLeastOneColumnForNotesPresent = IsAtLeastOneColumnForNotesPresent();

            if (_readInvoiceDate && !ColumnMapping.ContainsKey(ImportColumnName.InvoiceDate) && isAtLeastOneColumnForNotesPresent)
            {
                throw new SpreadSheetReaderMissingColumnsException($"Chcesz sprawdzić firmy na datę faktury, ale podajesz plik, który ma NOTY.");
            }

            if (!MinimumColumnsArePresent() || (_generateNotes && !areColumnsForNotesPresent) || (_readInvoiceDate && !ColumnMapping.ContainsKey(ImportColumnName.InvoiceDate)))
            {
                throw new SpreadSheetReaderMissingColumnsException($"Nie wszystkie kolumny są obecne w arkuszu. Brakuje: {GetMissingColumnsNames()}");
            }
        }

        private bool IsAtLeastOneColumnForNotesPresent()
        {
            if (ColumnMapping.ContainsKey(ImportColumnName.NoteAmount) || ColumnMapping.ContainsKey(ImportColumnName.NoteDate) || ColumnMapping.ContainsKey(ImportColumnName.NoteID) || ColumnMapping.ContainsKey(ImportColumnName.NoteTitle))
                return true;
            return false;
        }

        private List<InputCompanyFormatError> ValidateCompany(InputCompany tempCompany)
        {
            List<InputCompanyFormatError> errors = new List<InputCompanyFormatError>();
           

            if (!string.IsNullOrEmpty(tempCompany.BankAccountNumber) &&  !_bankAccountRegEx.IsMatch(tempCompany.BankAccountNumber))
            {
                errors.Add(InputCompanyFormatError.BankAccountFormatError);
            }

            if (string.IsNullOrEmpty(tempCompany.NIP) || !DataFormatHelper.IsNipValid(tempCompany.NIP))
            {
                errors.Add(InputCompanyFormatError.NIPFormatError);
            }

            if (string.IsNullOrEmpty(tempCompany.LP))
            {
                errors.Add(InputCompanyFormatError.LPFormatError);
            }

            return errors;
        }

        private const string _paymentDateFormat = "dd.MM.yyyyr.";
        private const string _yearDateSuffix = "r.";
        private string GetDate(Range cell)
        {
            string tempStr = cell.Formula.ToString().Trim();
            if (string.IsNullOrEmpty(tempStr))
            {
                return string.Empty;
            }
            try
            {
                double d = double.Parse(tempStr);
                DateTime conv = DateTime.FromOADate(d);
                tempStr = conv.ToString(_paymentDateFormat);
            }
            catch
            {
                //Value przechowuje datę z godziną
                tempStr = cell.Value.ToString().Trim();
                tempStr = tempStr.Substring(0, Math.Min(tempStr.Length, 10)); //10 znakow to data 01-01-2010
                tempStr = tempStr.Replace("-", ".");
                tempStr = string.Concat(tempStr, _yearDateSuffix);
            }
            return tempStr;
        }

        private string GetLP(string contentOfCell)
        {
            string tempStr = contentOfCell.Trim();
            tempStr = tempStr.Replace(".", string.Empty);
            return tempStr;
        }

        private readonly Regex _bankAccountRegEx = new Regex("[0-9]{26}", RegexOptions.Compiled);
        private string GetBankAccountNumber(string contentOfCell)
        {
            string tempString = contentOfCell.Trim().Replace(" ", string.Empty);

            return tempString;
        }

        private string GetNip(string contentOfCell)
        {
            string tempStr = contentOfCell.Trim();
            tempStr = tempStr.Replace(" ", string.Empty);
            tempStr = tempStr.Replace("-", string.Empty);
         
            return tempStr;
        }

        private bool MinimumColumnsArePresent()
        {
            if (ColumnMapping.ContainsKey(ImportColumnName.LP) && ColumnMapping.ContainsKey(ImportColumnName.AccountNumber) && ColumnMapping.ContainsKey(ImportColumnName.NIP) && ColumnMapping.ContainsKey(ImportColumnName.PaymentDate))
            {
                return true;
            }
            return false;
        }

        private string GetMissingColumnsNames()
        {
            StringBuilder sB = new StringBuilder();
            if (!ColumnMapping.ContainsKey(ImportColumnName.LP))
            {
                sB.Append("LP, ");
            }
            if (!ColumnMapping.ContainsKey(ImportColumnName.AccountNumber))
            {
                sB.Append("Konto bankowe, ");
            }
            if (!ColumnMapping.ContainsKey(ImportColumnName.NIP))
            {
                sB.Append("NIP, ");
            }
            if (!ColumnMapping.ContainsKey(ImportColumnName.PaymentDate))
            {
                sB.Append("Data zapłaty, ");
            }
            if (_readInvoiceDate && !ColumnMapping.ContainsKey(ImportColumnName.InvoiceDate))
            {
                sB.Append("Data faktury, ");
            }
            if (_generateNotes)
            {
                sB.Append("Danych do generowania not tj.: ");
                if (!ColumnMapping.ContainsKey(ImportColumnName.NoteAmount))
                {
                    sB.Append("Kwota noty, ");
                }
                if (!ColumnMapping.ContainsKey(ImportColumnName.NoteDate))
                {
                    sB.Append("Data noty, ");
                }
                if (!ColumnMapping.ContainsKey(ImportColumnName.NoteID))
                {
                    sB.Append("ID noty, ");
                }
                if (!ColumnMapping.ContainsKey(ImportColumnName.NoteTitle))
                {
                    sB.Append("Tytuł noty, ");
                }
            }

            string result = sB.ToString().Trim();
            result = result.Substring(0, result.LastIndexOf(","));
            return result;
            
        }

        private bool AreColumnsToGenerateNotesPresent()
        {
            if (ColumnMapping.ContainsKey(ImportColumnName.NoteAmount) && ColumnMapping.ContainsKey(ImportColumnName.NoteDate) && ColumnMapping.ContainsKey(ImportColumnName.NoteID) && ColumnMapping.ContainsKey(ImportColumnName.NoteTitle))
                return true;
            return false;
        }

     

        private void DetermineColumns(Worksheet worksheet)
        {
            string accountHeaderCaption = null, paymentDateCaption = null, idCaption = null, nipCaption = null, invoiceDateCaption = null;
            try
            {
                nipCaption = DataFormatHelper.RemovePolishLetters(_columnsConfig.FirstOrDefault(c => string.Compare(c.ID, ImportColumnName.NIP.ToString(), true)==0).HeaderText.ToLower());
                accountHeaderCaption = DataFormatHelper.RemovePolishLetters(_columnsConfig.FirstOrDefault(c => c.ID == ImportColumnName.AccountNumber.ToString()).HeaderText.ToLower());
                paymentDateCaption = DataFormatHelper.RemovePolishLetters(_columnsConfig.FirstOrDefault(c => c.ID == ImportColumnName.PaymentDate.ToString()).HeaderText.ToLower());
                idCaption = DataFormatHelper.RemovePolishLetters(_columnsConfig.FirstOrDefault(c => c.ID == ImportColumnName.LP.ToString()).HeaderText.ToLower());
                invoiceDateCaption = DataFormatHelper.RemovePolishLetters(_columnsConfig.FirstOrDefault(c => c.ID == ImportColumnName.InvoiceDate.ToString()).HeaderText.ToLower());
            } 
            catch (Exception e)
            {
                throw new SpreadSheetReaderException("Brak podstawowych kolumn w pliku konfiguracyjnym tj. konto bankowe, data zapłaty, lp, data faktury", e);
            }
            int maxNumOfColumnToRead = Enum.GetNames(typeof(ImportColumnName)).Length; // nip is read

            string noteIDsCaption = null, noteAmountCaption = null, noteTitleCaption = null, noteDateCaption = null;

            
            noteIDsCaption = DataFormatHelper.RemovePolishLetters(_columnsConfig.FirstOrDefault(c => c.ID == ImportColumnName.NoteID.ToString()).HeaderText.ToLower());
            noteAmountCaption = DataFormatHelper.RemovePolishLetters(_columnsConfig.FirstOrDefault(c => c.ID == ImportColumnName.NoteAmount.ToString()).HeaderText.ToLower());
            noteTitleCaption = DataFormatHelper.RemovePolishLetters(_columnsConfig.FirstOrDefault(c => c.ID == ImportColumnName.NoteTitle.ToString()).HeaderText.ToLower());
            noteDateCaption = DataFormatHelper.RemovePolishLetters(_columnsConfig.FirstOrDefault(c => c.ID == ImportColumnName.NoteDate.ToString()).HeaderText.ToLower());

                       
            for (int column = 1; column <= 75 && ColumnMapping.Count < maxNumOfColumnToRead; column++)
            {
                var tempCellContent = ((string)((Range)worksheet.Cells[HeaderRow, column]).Formula).ToLower().Trim();
                tempCellContent = DataFormatHelper.RemovePolishLetters(tempCellContent);

                if (tempCellContent.Contains(_columnPrefixToIgnore) || string.IsNullOrEmpty(tempCellContent))
                {
                    continue;
                }

                if (tempCellContent.Contains(nipCaption) && !ColumnMapping.ContainsKey(ImportColumnName.NIP))
                {
                    ColumnMapping.Add(ImportColumnName.NIP, column);
                }
                else if (tempCellContent.Contains(idCaption) && !ColumnMapping.ContainsKey(ImportColumnName.LP))
                {
                    ColumnMapping.Add(ImportColumnName.LP, column);
                }
                else if (tempCellContent.Contains(accountHeaderCaption) && !ColumnMapping.ContainsKey(ImportColumnName.AccountNumber))
                {
                    ColumnMapping.Add(ImportColumnName.AccountNumber, column);
                }
                else if (tempCellContent.Contains(paymentDateCaption) && !ColumnMapping.ContainsKey(ImportColumnName.PaymentDate))
                {
                    ColumnMapping.Add(ImportColumnName.PaymentDate, column);
                }
                else if (_readInvoiceDate && tempCellContent.Contains(invoiceDateCaption) && !ColumnMapping.ContainsKey(ImportColumnName.InvoiceDate))
                {
                    ColumnMapping.Add(ImportColumnName.InvoiceDate, column);

                }
                else if (noteIDsCaption != null && tempCellContent.Equals(noteIDsCaption, StringComparison.InvariantCultureIgnoreCase) && !ColumnMapping.ContainsKey(ImportColumnName.NoteID))
                {
                    ColumnMapping.Add(ImportColumnName.NoteID, column);
                }
                else if (noteAmountCaption != null && tempCellContent.Equals(noteAmountCaption, StringComparison.InvariantCultureIgnoreCase) && !ColumnMapping.ContainsKey(ImportColumnName.NoteAmount))
                {
                    ColumnMapping.Add(ImportColumnName.NoteAmount, column);
                }
                else if (noteTitleCaption != null && tempCellContent.Equals(noteTitleCaption, StringComparison.InvariantCultureIgnoreCase) && !ColumnMapping.ContainsKey(ImportColumnName.NoteTitle))
                {
                    ColumnMapping.Add(ImportColumnName.NoteTitle, column);
                }
                else if (noteDateCaption != null && tempCellContent.Equals(noteDateCaption, StringComparison.InvariantCultureIgnoreCase) && !ColumnMapping.ContainsKey(ImportColumnName.NoteDate))
                {
                    ColumnMapping.Add(ImportColumnName.NoteDate, column);
                }
            }
        }

 

        protected bool IsEndOfTable(int currentRow, int currentColumn, Worksheet worksheet)
        {
            if (((string)((Range)worksheet.Cells[currentRow, currentColumn]).Formula).Equals(string.Empty)
                && ((string)((Range)worksheet.Cells[currentRow + 1, currentColumn]).Formula).Equals(string.Empty))
                return true;
            return false;
        }
    }
}
