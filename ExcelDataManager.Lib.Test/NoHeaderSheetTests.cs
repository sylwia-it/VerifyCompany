using ExcelDataManager.Lib.Import;
using NUnit.Framework;
using ExcelDataManager.Lib;
using System.IO;
using System;

namespace ExcelDataManager.Lib.Test
{
    public class NoHeaderSheetTests
    {
        
        private string _filePath = @"..\..\..\..\ExcelDataManager.Lib.Test\InputData\NoHeaderSheet.xlsx";
        private SpreadSheetReader _ssr;

        [SetUp]
        public void Setup()
        {
            FileInfo fI = new FileInfo(_filePath);
            if (fI.Exists)
                _ssr = new SpreadSheetReader(fI.FullName, true, true);
            else
                throw new FileLoadException("cannot load the input file");
        }

        [Test]
        public void ReadWorkbookWithNoHeaderTest()
        {
            SpreadSheetReaderHeaderException e = Assert.Throws<SpreadSheetReaderHeaderException>(() => _ssr.ReadDataFromFile());
            Assert.IsTrue(e.Message.Contains("nag³ówka")); 
        }

        
    }
}