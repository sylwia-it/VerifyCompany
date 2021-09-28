using System;
using System.Runtime.Serialization;

namespace ExcelDataManager.Lib.Import
{
    [Serializable]
    public class SpreadSheetReaderHeaderException : SpreadSheetReaderException
    {
       

        public SpreadSheetReaderHeaderException(string message) : base(message)
        {
        }

        public SpreadSheetReaderHeaderException(string message, Exception innerException) : base(message, innerException)
        {
        }

       
    }
}