using ExcelDataManager.Lib.Import;
using NUnit.Framework;
using ExcelDataManager.Lib;
using System.IO;
using System;
using VerifyCompany.Common.Lib;

namespace ExcelDataManager.Lib.Test
{
    public class DataReadColumnTests
    {
        
        private string _filePathCorrect = @"..\..\..\..\ExcelDataManager.Lib.Test\InputData\CorrectDataSheet.xlsx";
        private string _filePathBankAccountTooShort = @"..\..\..\..\ExcelDataManager.Lib.Test\InputData\BankAccountTooShortDataSheet.xlsx";
        private string _filePathBankAccountWithLetters = @"..\..\..\..\ExcelDataManager.Lib.Test\InputData\BankAccountWithLettersDataSheet.xlsx";
        private string _filePathNipTooShort = @"..\..\..\..\ExcelDataManager.Lib.Test\InputData\NipTooShortDataSheet.xlsx";
        private string _filePathNipInCorrect = @"..\..\..\..\ExcelDataManager.Lib.Test\InputData\NipInCorrectDataSheet.xlsx";
        private string _filePathNipWithLetter = @"..\..\..\..\ExcelDataManager.Lib.Test\InputData\NipWithLetterDataSheet.xlsx";
        private SpreadSheetReader _ssr;

       

        [Test]
        public void CorrectWorkbookGenertateNoteTest()
        {
            FileInfo fI = new FileInfo(_filePathCorrect);
            if (fI.Exists)
                _ssr = new SpreadSheetReader(fI.FullName, true);
            else
                throw new FileLoadException("cannot load the input file");

            var companies = _ssr.ReadDataFromFile();

            Assert.IsTrue(_ssr.ColumnMapping.ContainsKey(ImportColumnName.PaymentDate));
            Assert.IsTrue(_ssr.ColumnMapping.ContainsKey(ImportColumnName.NoteTitle));
            Assert.IsTrue(_ssr.ColumnMapping.ContainsKey(ImportColumnName.NoteID));
            Assert.IsTrue(_ssr.ColumnMapping.ContainsKey(ImportColumnName.NoteDate));
            Assert.IsTrue(_ssr.ColumnMapping.ContainsKey(ImportColumnName.NoteAmount));
            Assert.IsTrue(_ssr.ColumnMapping.ContainsKey(ImportColumnName.NIP));
            Assert.IsTrue(_ssr.ColumnMapping.ContainsKey(ImportColumnName.LP));
            Assert.IsTrue(_ssr.ColumnMapping.ContainsKey(ImportColumnName.AccountNumber));
            Assert.IsTrue(_ssr.ColumnMapping.Count == 8);

            Assert.IsTrue(companies.Count == 5);
            InputCompany comp = companies[0];
            Assert.AreEqual("46124017501111001068892448", comp.BankAccountNumber);
            Assert.AreEqual("1-7792369887", comp.ID);
            Assert.AreEqual("1", comp.LP);
            Assert.AreEqual("7792369887", comp.NIP);
            Assert.AreEqual("12.01.2019r.", comp.NoteDate);
            Assert.AreEqual("12", comp.NoteID);
            Assert.AreEqual("1000", comp.NoteNettoAmount);
            Assert.AreEqual("Q1", comp.NoteTitle);
            Assert.AreEqual("15.02.2019r.", comp.PaymentDate);
            Assert.AreEqual(5, comp.RowNumber);
        }

        [Test]
        public void CorrectWorkbookDoNotGenertateNoteTest()
        {
            FileInfo fI = new FileInfo(_filePathCorrect);
            if (fI.Exists)
                _ssr = new SpreadSheetReader(fI.FullName, false);
            else
                throw new FileLoadException("cannot load the input file");

            var companies = _ssr.ReadDataFromFile();

            Assert.IsTrue(_ssr.ColumnMapping.ContainsKey(ImportColumnName.PaymentDate));
            Assert.IsFalse(_ssr.ColumnMapping.ContainsKey(ImportColumnName.NoteTitle));
            Assert.IsFalse(_ssr.ColumnMapping.ContainsKey(ImportColumnName.NoteID));
            Assert.IsFalse(_ssr.ColumnMapping.ContainsKey(ImportColumnName.NoteDate));
            Assert.IsFalse(_ssr.ColumnMapping.ContainsKey(ImportColumnName.NoteAmount));
            Assert.IsTrue(_ssr.ColumnMapping.ContainsKey(ImportColumnName.NIP));
            Assert.IsTrue(_ssr.ColumnMapping.ContainsKey(ImportColumnName.LP));
            Assert.IsTrue(_ssr.ColumnMapping.ContainsKey(ImportColumnName.AccountNumber));
            Assert.IsTrue(_ssr.ColumnMapping.Count == 4);

            Assert.IsTrue(companies.Count == 5);
            InputCompany comp = companies[3];
            Assert.AreEqual("03124061751111001095041381", comp.BankAccountNumber);
            Assert.AreEqual("4-7811767696", comp.ID);
            Assert.AreEqual("4", comp.LP);
            Assert.AreEqual("7811767696", comp.NIP);
            Assert.IsNull(comp.NoteDate);
            Assert.IsNull(comp.NoteID);
            Assert.IsNull(comp.NoteNettoAmount);
            Assert.IsNull(comp.NoteTitle);
            Assert.AreEqual("15.02.2019r.", comp.PaymentDate);
            Assert.AreEqual(8, comp.RowNumber);
        }

        [Test]
        public void BankAccountTooShortTest()
        {
            FileInfo fI = new FileInfo(_filePathBankAccountTooShort);
            if (fI.Exists)
                _ssr = new SpreadSheetReader(fI.FullName, true);
            else
                throw new FileLoadException("cannot load the input file");

            SpreadSheetReaderException e = Assert.Throws<SpreadSheetReaderException>(() => _ssr.ReadDataFromFile());
            Assert.IsTrue(e.InnerException.Message.Contains("nie spe³nia wymogów konta"));
        }

        [Test]
        public void BankAccountWithLettersTest()
        {
            FileInfo fI = new FileInfo(_filePathBankAccountWithLetters);
            if (fI.Exists)
                _ssr = new SpreadSheetReader(fI.FullName, true);
            else
                throw new FileLoadException("cannot load the input file");

            SpreadSheetReaderException e = Assert.Throws<SpreadSheetReaderException>(() => _ssr.ReadDataFromFile());
            Assert.IsTrue(e.InnerException.Message.Contains("nie spe³nia wymogów konta"));
        }

        [Test]
        public void NipTooShortTest()
        {
            FileInfo fI = new FileInfo(_filePathNipTooShort);
            if (fI.Exists)
                _ssr = new SpreadSheetReader(fI.FullName, true);
            else
                throw new FileLoadException("cannot load the input file");

            SpreadSheetReaderException e = Assert.Throws<SpreadSheetReaderException>(() => _ssr.ReadDataFromFile());
            Assert.IsTrue(e.InnerException.Message.Contains("nie jest poprawny"));
        }

        [Test]
        public void NipWithLettersTest()
        {
            FileInfo fI = new FileInfo(_filePathNipWithLetter);
            if (fI.Exists)
                _ssr = new SpreadSheetReader(fI.FullName, true);
            else
                throw new FileLoadException("cannot load the input file");

            SpreadSheetReaderException e = Assert.Throws<SpreadSheetReaderException>(() => _ssr.ReadDataFromFile());
            Assert.IsTrue(e.InnerException.Message.Contains("nie jest poprawny"));
        }

        [Test]
        public void NipInCorrectTest()
        {
            FileInfo fI = new FileInfo(_filePathNipInCorrect);
            if (fI.Exists)
                _ssr = new SpreadSheetReader(fI.FullName, true);
            else
                throw new FileLoadException("cannot load the input file");

            SpreadSheetReaderException e = Assert.Throws<SpreadSheetReaderException>(() => _ssr.ReadDataFromFile());
            Assert.IsTrue(e.InnerException.Message.Contains("nie jest poprawny"));
        }




    }
}