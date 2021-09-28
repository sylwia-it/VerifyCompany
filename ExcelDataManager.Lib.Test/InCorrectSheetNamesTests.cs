using ExcelDataManager.Lib.Import;
using NUnit.Framework;
using ExcelDataManager.Lib;
using System.IO;

namespace ExcelDataManager.Lib.Test
{
    public class InCorrectSheetNamesTests
    {
        private string _configFilePath = @"..\..\..\..\ExcelDataManager.Lib.Test\InputData\InCorrectSheetNames-importSettings.json";
        private string _filePath = @"..\..\..\..\ExcelDataManager.Lib.Test\InputData\InCorrectSheetNames.xlsx";
        private SpreadSheetReader _ssr;
        [SetUp]
        public void Setup()
        {

            FileInfo fI = new FileInfo("importSettings.json");
            fI.CopyTo("importSettingsCopy.json", true);

            fI = new FileInfo(_configFilePath);
            if (fI.Exists)
            {
                fI.CopyTo("importSettings.json", true);
            }
            else
            {
                throw new FileLoadException("cannot load the input config");
            }
            fI = new FileInfo(_filePath);
            if (fI.Exists)
                _ssr = new SpreadSheetReader(fI.FullName, true, true);
            else
                throw new FileLoadException("cannot load the input file");


        }

        [Test]
        public void ReadWorkbookWithNoCorrectSheetNameTest()
        {
           Assert.Throws<SpreadSheetReaderException>(()=>_ssr.ReadDataFromFile());
        }

        [TearDown]
        public void CleanUp()
        {
            FileInfo fI = new FileInfo("importSettingsCopy.json");
            fI.CopyTo("importSettings.json", true);
        }
    }
}