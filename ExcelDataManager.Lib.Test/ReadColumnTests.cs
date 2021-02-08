using ExcelDataManager.Lib.Import;
using NUnit.Framework;
using ExcelDataManager.Lib;
using System.IO;
using System;

namespace ExcelDataManager.Lib.Test
{
    public class ReadColumnTests
    {
        
        private string _filePathCorrect = @"..\..\..\..\ExcelDataManager.Lib.Test\InputData\CorrectHeaderSheet.xlsx";
        private string _filePathMinMissing = @"..\..\..\..\ExcelDataManager.Lib.Test\InputData\MissingOneMinimalFieldHeaderSheet.xlsx";
        private string _filePathNoteissing = @"..\..\..\..\ExcelDataManager.Lib.Test\InputData\MissingOneNoteHeaderSheet.xlsx";
        private SpreadSheetReader _ssr;

       

        [Test]
        public void CorrectWorkbookGenertateNoteTest()
        {
            FileInfo fI = new FileInfo(_filePathCorrect);
            if (fI.Exists)
                _ssr = new SpreadSheetReader(fI.FullName, true);
            else
                throw new FileLoadException("cannot load the input file");

            _ssr.ReadDataFromFile();

            Assert.IsTrue(_ssr.ColumnMapping.ContainsKey(ImportColumnName.PaymentDate));
            Assert.IsTrue(_ssr.ColumnMapping.ContainsKey(ImportColumnName.NoteTitle));
            Assert.IsTrue(_ssr.ColumnMapping.ContainsKey(ImportColumnName.NoteID));
            Assert.IsTrue(_ssr.ColumnMapping.ContainsKey(ImportColumnName.NoteDate));
            Assert.IsTrue(_ssr.ColumnMapping.ContainsKey(ImportColumnName.NoteAmount));
            Assert.IsTrue(_ssr.ColumnMapping.ContainsKey(ImportColumnName.NIP));
            Assert.IsTrue(_ssr.ColumnMapping.ContainsKey(ImportColumnName.LP));
            Assert.IsTrue(_ssr.ColumnMapping.ContainsKey(ImportColumnName.AccountNumber));
            Assert.IsTrue(_ssr.ColumnMapping.Count == 8);
        }

        [Test]
        public void CorrectWorkbookDoNotGenertateNoteTest()
        {
            FileInfo fI = new FileInfo(_filePathCorrect);
            if (fI.Exists)
                _ssr = new SpreadSheetReader(fI.FullName, false);
            else
                throw new FileLoadException("cannot load the input file");

            _ssr.ReadDataFromFile();

            Assert.IsTrue(_ssr.ColumnMapping.ContainsKey(ImportColumnName.PaymentDate));
            Assert.IsFalse(_ssr.ColumnMapping.ContainsKey(ImportColumnName.NoteTitle));
            Assert.IsFalse(_ssr.ColumnMapping.ContainsKey(ImportColumnName.NoteID));
            Assert.IsFalse(_ssr.ColumnMapping.ContainsKey(ImportColumnName.NoteDate));
            Assert.IsFalse(_ssr.ColumnMapping.ContainsKey(ImportColumnName.NoteAmount));
            Assert.IsTrue(_ssr.ColumnMapping.ContainsKey(ImportColumnName.NIP));
            Assert.IsTrue(_ssr.ColumnMapping.ContainsKey(ImportColumnName.LP));
            Assert.IsTrue(_ssr.ColumnMapping.ContainsKey(ImportColumnName.AccountNumber));
            Assert.IsTrue(_ssr.ColumnMapping.Count == 4);
        }

        [Test]
        public void MissingOneMinimalFieldHeaderSheet()
        {
            FileInfo fI = new FileInfo(_filePathMinMissing);
            if (fI.Exists)
                _ssr = new SpreadSheetReader(fI.FullName, true);
            else
                throw new FileLoadException("cannot load the input file");

            SpreadSheetReaderException e = Assert.Throws<SpreadSheetReaderException>(() => _ssr.ReadDataFromFile());
            Assert.IsTrue(e.InnerException.Message.Contains("Nie wszystkie kolumny s¹ obecne"));
        }

        [Test]
        public void MissingOneNoteHeaderSheet()
        {
            FileInfo fI = new FileInfo(_filePathNoteissing);
            if (fI.Exists)
                _ssr = new SpreadSheetReader(fI.FullName, true);
            else
                throw new FileLoadException("cannot load the input file");

            SpreadSheetReaderException e = Assert.Throws<SpreadSheetReaderException>(() => _ssr.ReadDataFromFile());
            Assert.IsTrue(e.InnerException.Message.Contains("dotycz¹ce not"));
        }




    }
}