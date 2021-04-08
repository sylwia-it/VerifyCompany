using System;
using System.Runtime.Serialization;

namespace ExcelDataManager.Lib.Export
{
    [Serializable]
    internal class SpreadSheetWriterExcpetion : Exception
    {
        public SpreadSheetWriterExcpetion()
        {
        }

        public SpreadSheetWriterExcpetion(string message) : base(message)
        {
        }

        public SpreadSheetWriterExcpetion(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SpreadSheetWriterExcpetion(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}