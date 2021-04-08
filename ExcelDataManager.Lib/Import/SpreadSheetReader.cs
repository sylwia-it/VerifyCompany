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
        private string _filePath;
        private List<ColumnConfig> _columnsConfig; //mapping of columns to specific header text in spreadsheet
        private List<string>_sheetNamesToExclude; //name of sheet that shall be considered as source

        public Dictionary<ImportColumnName, int> ColumnMapping { get; private set; }
        private int HeaderRow { get; set; }
        private List<InputCompany> CompaniesReadFromFile { get; set; }
        public bool IsDataToGenerateNotesRead { get; private set; }

        public SpreadSheetReader(string filePath, bool generateNotes)
        {
            _generateNotes = generateNotes;
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
                if (e as SpreadSheetReaderException != null)
                {
                    throw e;
                }
                else
                {
                    throw new Exception("Nieznany błąd podczas importu!" + e.Message, e);
                }

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
                    throw new SpreadSheetReaderException(string.Format("Błąd formatu aruksza. W arkuszu: {0} nie znaleziono nagłówka danych. Sprawdź czy któryś nagłówek zawiera słowo 'nip'", worksheet.Name));
                }

                DetermineColumns(worksheet);
               
                if (!MinimumColumnsArePresent())
                { 
                    throw new SpreadSheetReaderException("Nie wszystkie kolumny są obecne w arkuszu.");
                }
                               
                IsDataToGenerateNotesRead = CheckIfDataToGenerateNotesIsRead();
                if (_generateNotes && !IsDataToGenerateNotesRead)
                    throw new SpreadSheetReaderException("Nie wszystkie kolumny dotyczące not są obecne w arkuszu.");
                lastProcessedItem = "Wczytano nagłówek";


                InputCompany tempCompany;

                for (int i = HeaderRow + 1; !IsEndOfTable(i, 1, worksheet); i++)
                {
                    tempCompany = new InputCompany();
                    tempCompany.RowNumber = i;

                    lastProcessedItem = $"Przed wczytaniem NIPu. Aktualna pozycja: wiersz {i}";
                    lastReadColumn = ColumnMapping[ImportColumnName.NIP];
                    tempStr = GetNip(((Range)worksheet.Cells[i, ColumnMapping[ImportColumnName.NIP]]).Formula.ToString());
                    tempCompany.NIP = tempStr;
                    tempStr = string.Empty;
                    lastProcessedItem = $"Wczytano NIP. Aktualna pozycja: wiersz {i}";

                    lastProcessedItem = $"Przed wczytaniem konta bankowego. Aktualna pozycja: wiersz {i}";
                    lastReadColumn = ColumnMapping[ImportColumnName.AccountNumber];
                    tempStr = GetBankAccountNumber(((Range)worksheet.Cells[i, ColumnMapping[ImportColumnName.AccountNumber]]).Formula.ToString());
                    tempCompany.BankAccountNumber = tempStr;
                    tempStr = string.Empty;
                    lastProcessedItem = $"Wczytano Numer Konta. Aktualna pozycja: wiersz {i}";

                    lastProcessedItem = $"Przed wczytaniem LP. Aktualna pozycja: wiersz {i}";
                    lastReadColumn = ColumnMapping[ImportColumnName.LP];
                    tempStr = GetLP(((Range)worksheet.Cells[i, ColumnMapping[ImportColumnName.LP]]).Formula.ToString());
                    tempCompany.LP = tempStr;
                    tempStr = string.Empty;
                    if (CompaniesReadFromFile.Any(c => c.ID == tempCompany.ID))
                    {
                        throw new SpreadSheetReaderException($"Dane zawierają już wpis o takiej samej pozycji i nipie. Aktualna wiersz: {i}, Nip: {tempCompany.NIP}, LP: {tempCompany.LP}");
                    }
                    lastProcessedItem = $"Wczytano LP.Aktualna pozycja: wiersz {i}";


                    lastProcessedItem = $"Przed wczytaniem daty zapłaty. Aktualna pozycja: wiersz {i}";
                    lastReadColumn = ColumnMapping[ImportColumnName.PaymentDate];
                    tempStr = GetDate(((Range)worksheet.Cells[i, ColumnMapping[ImportColumnName.PaymentDate]]));
                    tempCompany.PaymentDate = tempStr;
                    tempStr = string.Empty;
                    lastProcessedItem = $"Wczytano datę zapłaty. Aktualna pozycja: wiersz {i}";

                    if (IsDataToGenerateNotesRead)
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
                        tempStr = GetDate((Range)worksheet.Cells[i, ColumnMapping[ImportColumnName.NoteDate]]);
                        tempCompany.NoteDate = tempStr;
                        tempStr = string.Empty;
                        lastProcessedItem = $"Wczytano datę noty. Aktualna pozycja: wiersz {i}";
                    }
                    CompaniesReadFromFile.Add(tempCompany);
                 }
            }
            catch (Exception e)
            {
                string errorMsg = string.Format("\nZłapano błąd w rzędzie gdy procedura była na kroku: {0}, w kolumnie: {2}, odczytała wartość: {1}.\n\n", lastProcessedItem, tempStr, lastReadColumn);
                throw new SpreadSheetReaderException(errorMsg, e);
            }

       
        }

        private const string _paymentDateFormat = "dd.MM.yyyyr.";
        private const string _yearDateSuffix = "r.";
        private string GetDate(Range cell)
        {
            string tempStr = cell.Formula.ToString().Trim();
            if (string.IsNullOrEmpty(tempStr))
            {
                throw new SpreadSheetReaderException("Data jest pusta.");
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

            if (!string.IsNullOrEmpty(tempString) && !_bankAccountRegEx.IsMatch(tempString))
                throw new SpreadSheetReaderException($"Konto bankowe ({contentOfCell}) nie spełnia wymogów konta bankowego.");

            return tempString;
        }

        private string GetNip(string contentOfCell)
        {
            string tempStr = contentOfCell.Trim();
            tempStr = tempStr.Replace(" ", string.Empty);
            tempStr = tempStr.Replace("-", string.Empty);
            if (!DataFormatHelper.IsNipValid(tempStr))
            {
                throw new SpreadSheetReaderException($"Nip {contentOfCell} nie jest poprawny.");
            }
            return tempStr;
        }

        private bool MinimumColumnsArePresent()
        {
            if (ColumnMapping.ContainsKey(ImportColumnName.LP) && ColumnMapping.ContainsKey(ImportColumnName.AccountNumber) && ColumnMapping.ContainsKey(ImportColumnName.NIP) && ColumnMapping.ContainsKey(ImportColumnName.PaymentDate))
                return true;
            return false;
        }

        private bool CheckIfDataToGenerateNotesIsRead()
        {
            if (ColumnMapping.ContainsKey(ImportColumnName.NoteAmount) && ColumnMapping.ContainsKey(ImportColumnName.NoteDate) && ColumnMapping.ContainsKey(ImportColumnName.NoteID) && ColumnMapping.ContainsKey(ImportColumnName.NoteTitle))
                return true;
            return false;
        }

        private void DetermineColumns(Worksheet worksheet)
        {
            string accountHeaderCaption = null, paymentDateCaption = null, idCaption = null, nipCaption = null;
            try
            {
                nipCaption = _columnsConfig.FirstOrDefault(c => string.Compare(c.ID, ImportColumnName.NIP.ToString(), true)==0).HeaderText;
                accountHeaderCaption = _columnsConfig.FirstOrDefault(c => c.ID == ImportColumnName.AccountNumber.ToString()).HeaderText;
                paymentDateCaption = _columnsConfig.FirstOrDefault(c => c.ID == ImportColumnName.PaymentDate.ToString()).HeaderText;
                idCaption = _columnsConfig.FirstOrDefault(c => c.ID == ImportColumnName.LP.ToString()).HeaderText;
            } 
            catch (Exception e)
            {
                throw new SpreadSheetReaderException("Brak podstawowych kolumn w pliku konfiguracyjnym tj. konto bankowe, data zapłaty, lp", e);
            }
            int numOfColumnToRead = Enum.GetNames(typeof(ImportColumnName)).Length; // nip is read

            string noteIDsCaption = null, noteAmountCaption = null, noteTitleCaption = null, noteDateCaption = null;

            if (_generateNotes)
            {
                noteIDsCaption = _columnsConfig.FirstOrDefault(c => c.ID == ImportColumnName.NoteID.ToString()).HeaderText;
                noteAmountCaption = _columnsConfig.FirstOrDefault(c => c.ID == ImportColumnName.NoteAmount.ToString()).HeaderText;
                noteTitleCaption = _columnsConfig.FirstOrDefault(c => c.ID == ImportColumnName.NoteTitle.ToString()).HeaderText;
                noteDateCaption = _columnsConfig.FirstOrDefault(c => c.ID == ImportColumnName.NoteDate.ToString()).HeaderText;
                
            } else
            {
                numOfColumnToRead -= 4;
            }
            
            for (int column = 1; column <= 75 && ColumnMapping.Count < numOfColumnToRead; column++)
            {
                var tempCellContent = ((string)((Range)worksheet.Cells[HeaderRow, column]).Formula).ToLower().Trim();

                if (tempCellContent.Contains(nipCaption, StringComparison.OrdinalIgnoreCase))
                {
                    ColumnMapping.Add(ImportColumnName.NIP, column);
                }
                else if (tempCellContent.Contains(idCaption, StringComparison.OrdinalIgnoreCase))
                {
                    ColumnMapping.Add(ImportColumnName.LP, column);
                }
                else if (tempCellContent.Contains(accountHeaderCaption, StringComparison.OrdinalIgnoreCase))
                {
                    ColumnMapping.Add(ImportColumnName.AccountNumber, column);
                }
                else if (tempCellContent.Contains(paymentDateCaption, StringComparison.OrdinalIgnoreCase))
                {
                    ColumnMapping.Add(ImportColumnName.PaymentDate, column);
                }
                else if (_generateNotes)
                {
                    if (noteIDsCaption != null && tempCellContent.Equals(noteIDsCaption,  StringComparison.OrdinalIgnoreCase))
                    {
                        ColumnMapping.Add(ImportColumnName.NoteID, column);
                    }
                    else if (noteAmountCaption != null && tempCellContent.Equals(noteAmountCaption, StringComparison.OrdinalIgnoreCase))
                    {
                        ColumnMapping.Add(ImportColumnName.NoteAmount, column);
                    }
                    else if (noteTitleCaption != null && tempCellContent.Equals(noteTitleCaption, StringComparison.OrdinalIgnoreCase))
                    {
                        ColumnMapping.Add(ImportColumnName.NoteTitle, column);
                    }
                    else if (noteDateCaption != null && tempCellContent.Equals(noteDateCaption, StringComparison.OrdinalIgnoreCase))
                    {
                        ColumnMapping.Add(ImportColumnName.NoteDate, column);
                    }
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
