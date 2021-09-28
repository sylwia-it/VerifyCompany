using System;

namespace ExcelDataManager.Lib.Import
{
    public class SpreadSheetReaderException : Exception
    {
        
        public SpreadSheetReaderException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public SpreadSheetReaderException(string message) : base(message)
        {

        }
    }
}