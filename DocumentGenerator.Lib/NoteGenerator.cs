using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DocumentGenerator.Lib.Helpers;
using Microsoft.Office.Interop.Excel;
using VerifyCompany.Common.Lib;
using VerifyWhiteListCompany.Lib;
using Range = Microsoft.Office.Interop.Excel.Range;

namespace DocumentGenerator.Lib
{
    public class NoteGenerator
    {
        private DirectoryInfo _outputDirectoryInfo;
        //private Worksheet _worksheet;
        private Workbook _theWorkbook;
        Application _app = null;


        public void GenerateNotes(string outputPath, List<InputCompany> inputCompanies, Dictionary<string, WhiteListVerResult> verifiedCompanies)
        {

            try
            {
                if (Directory.Exists(outputPath))
                {
                    _outputDirectoryInfo = new DirectoryInfo(string.Format("{0}\\Noty-{1}{2}{3}-{4}{5}", outputPath, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute));
                    _outputDirectoryInfo.Create();
                }

                _app = new Application();

                foreach (var verifiedCompany in verifiedCompanies)
                {
                    if (CanGenerateNotes(verifiedCompany.Value.VerificationStatus))
                    {
                        var inputCompany = inputCompanies.FirstOrDefault(c => c.ID == verifiedCompany.Key);
                        CreateNote(inputCompany, verifiedCompany.Value, _outputDirectoryInfo.FullName);
                    }
                }

                _app.Quit();

            } catch (Exception e)
            {
                if (_theWorkbook != null)
                    _theWorkbook.Close(false, Type.Missing, Type.Missing);
                if (_app != null)
                    _app.Quit();
                throw new DocumentGeneratorException(e);
            }
        }
        private Regex _noteIDPattern = new Regex("[0-9]{1,3}", RegexOptions.Compiled);
        private const string _space = " ";
        private const string _underscore = "_";
        private const string _dot = ".";
        private Regex _fileNamePattern = new Regex("[a-zA-Z0-9_]{3,15}", RegexOptions.Compiled);
        private const int _lengthOfFullNameInFileNameWithNote = 15;
        private void CreateNote(InputCompany inputCompany, WhiteListVerResult whiteListVerResult, string outputPath)
        {
            string fileName = string.Empty;
            string templPath = string.Format("{0}\\{1}", Environment.CurrentDirectory, "templates\\NotaTemplate.xltx");
            _theWorkbook = _app.Workbooks.Add(templPath);

            Sheets sheets = _theWorkbook.Worksheets;
            Worksheet _worksheet = (Worksheet)sheets.get_Item(1);

            FillInWorksheetWithData(ref _worksheet, inputCompany, whiteListVerResult);

            fileName = whiteListVerResult.FullName.Replace(_space, _underscore);
            fileName = fileName.Replace(_dot, string.Empty);
            fileName = _fileNamePattern.IsMatch(fileName) ? _fileNamePattern.Match(fileName).Value : fileName;
            fileName = fileName.Length > _lengthOfFullNameInFileNameWithNote ? fileName.Substring(0, _lengthOfFullNameInFileNameWithNote) : fileName;

            string noteID = _noteIDPattern.IsMatch(inputCompany.NoteID) ? _noteIDPattern.Match(inputCompany.NoteID).Value : string.Empty;
            fileName = string.Format("{0}\\{1}-{2}.xlsx", outputPath, noteID, fileName);

            _theWorkbook.SaveAs(fileName, XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing, Type.Missing, Type.Missing, XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            _theWorkbook.Close(true, Type.Missing, Type.Missing);
        }

        private int _maxLengthOfLineInFile = 30;
        private const string _colon = ",";
        private Regex _postalCodePattern = new Regex("[0-9]{2}-[0-9]{3}", RegexOptions.Compiled);
        private void FillInWorksheetWithData(ref Worksheet worksheet, InputCompany inputCompany, WhiteListVerResult whiteListVerResult)
        {
            if (inputCompany.NoteID.Length < 4) // means that only number is given
            {
                ((Range)worksheet.Cells[3, 1]).Formula = string.Format($"nr {inputCompany.NoteID}/{DateTime.Now.Month}/{DateTime.Now.Year}");
            }
            else //means that the full number was given in the input file
            {
                ((Range)worksheet.Cells[3, 1]).Formula = string.Format($"nr {inputCompany.NoteID}");
            }

            if (string.IsNullOrEmpty(inputCompany.NoteDate))
            {
                ((Range)worksheet.Cells[3, 9]).Formula = DateTime.Now.ToString("dd.MM.yyyyr.");
            }
            else
            {
                ((Range)worksheet.Cells[3, 9]).Formula = string.Format($"{inputCompany.NoteDate}");
            }

            ((Range)worksheet.Cells[4, 9]).Formula = GetPaymentPeriod(inputCompany.NoteTitle);

            string fullNameWithAbbr = FormatHelper.AbbreviateFullNameOfCompany(whiteListVerResult.FullName);
            int numOfLinesForFullNames = 1 + (fullNameWithAbbr.Length / _maxLengthOfLineInFile);

            for (int j = 0; j < numOfLinesForFullNames; j++)
            {
                string temp = fullNameWithAbbr.Substring(j * _maxLengthOfLineInFile);
                temp = temp.Substring(0, Math.Min(_maxLengthOfLineInFile, temp.Length)).Trim();
                ((Range)worksheet.Cells[8 + j, 8]).Formula = temp;
            }

            string addressFull = string.IsNullOrEmpty(whiteListVerResult.FullResidenceAddress) ?
                    whiteListVerResult.FullWorkingAddress : whiteListVerResult.FullResidenceAddress;
            addressFull = string.Concat(AddressHelper.GetStreetPrefix(addressFull), addressFull);
            addressFull = addressFull.Replace(_colon, string.Empty);
            

            int addressDivIndex = _postalCodePattern.Match(addressFull).Index;
            ((Range)worksheet.Cells[8 + numOfLinesForFullNames, 8]).Formula = addressFull.Substring(0, addressDivIndex).Trim();
            ((Range)worksheet.Cells[9 + numOfLinesForFullNames, 8]).Formula = addressFull.Substring(addressDivIndex).Trim();

            ((Range)worksheet.Cells[14, 9]).Formula = whiteListVerResult.Nip.Insert(3, _space).Insert(6, _space).Insert(9, _space);

            ((Range)worksheet.Cells[17, 9]).Formula = FormatHelper.GetAccountNumberInString(whiteListVerResult.GivenAccountNumber);

            ((Range)worksheet.Cells[21, 9]).Formula = inputCompany.NoteNettoAmount;

            ((Range)worksheet.Cells[24, 3]).Formula = inputCompany.NoteTitle;

            string nettoAmountStr = inputCompany.NoteNettoAmount.Replace(_dot, _colon);
            double nettoAmount = double.Parse(nettoAmountStr);
            double vatAmout = Math.Round(0.08 * nettoAmount, 2);
            ((Range)worksheet.Cells[36, 3]).Formula = NumberToWordsConverter.ConvertNumberToAmountPln((nettoAmount + vatAmout).ToString());
        }

        
        

        private const string q1Format = "01-01-{0} - 31-03-{0}";
        private const string q2Format = "01-04-{0} - 30-06-{0}";
        private const string q3Format = "01-07-{0} - 30-09-{0}";
        private const string q4Format = "01-10-{0} - 31-12-{0}";
        private Regex _qPattern = new Regex("Q[0-4]", RegexOptions.Compiled);
        private Regex _yearPattern = new Regex("20[0-9][0-9]", RegexOptions.Compiled);
        private string GetPaymentPeriod(string noteTile)
        {

            if (_qPattern.IsMatch(noteTile))
            {
                string year = _yearPattern.IsMatch(noteTile) ? _yearPattern.Match(noteTile).Value : DateTime.Now.Year.ToString();
                switch (_qPattern.Match(noteTile).Value)
                {
                    case "Q1":
                        return string.Format(q1Format, year);
                    case "Q2":
                        return string.Format(q2Format, year);
                    case "Q3":
                        return string.Format(q3Format, year);
                    case "Q4":
                        return string.Format(q4Format, year);
                    default:
                        return string.Empty;
                }

            }
            return string.Empty;
        }

        private bool CanGenerateNotes(WhiteListVerResultStatus status)
        {
            if (status == WhiteListVerResultStatus.ActiveVATPayerAccountOKVerSuccessfull || status == WhiteListVerResultStatus.ActiveVATPayerButGivenAccountNotOnWhiteList || status == WhiteListVerResultStatus.ActiveVATPayerButHasNoAccounts || status == WhiteListVerResultStatus.ActiveVATPayerVerSuccessfull)
                return true;
            return false;
        }
    }
}
