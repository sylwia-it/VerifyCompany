using System;
using System.Runtime.Serialization;

namespace DocumentGenerator.Lib
{
    [Serializable]
    internal class DocumentGeneratorException : Exception
    {
        private Exception e;

        public DocumentGeneratorException()
        {
        }

        public DocumentGeneratorException(Exception e)
        {
            this.e = e;
        }

        public DocumentGeneratorException(string message) : base(message)
        {
        }

        public DocumentGeneratorException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DocumentGeneratorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}