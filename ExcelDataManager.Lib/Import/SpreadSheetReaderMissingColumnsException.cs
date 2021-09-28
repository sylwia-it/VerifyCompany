using System;
using System.Runtime.Serialization;

namespace ExcelDataManager.Lib.Import
{
    [Serializable]
    public class SpreadSheetReaderMissingColumnsException : SpreadSheetReaderException
    {
        

        public SpreadSheetReaderMissingColumnsException(string message) : base(message)
        {
        }

        public SpreadSheetReaderMissingColumnsException(string message, Exception innerException) : base(message, innerException)
        {
        }

    }
}