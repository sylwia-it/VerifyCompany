using System;
using System.Runtime.Serialization;

namespace VerifyActiveCompany.Lib
{
    [Serializable]
    internal class BiRClientSetUpException : Exception
    {
        public BiRClientSetUpException()
        {
        }

        public BiRClientSetUpException(string message) : base(message)
        {
        }

        public BiRClientSetUpException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BiRClientSetUpException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}