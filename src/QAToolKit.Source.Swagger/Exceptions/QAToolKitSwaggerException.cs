using System;
using System.Runtime.Serialization;

namespace QAToolKit.Source.Swagger.Exceptions
{
    internal class QAToolKitSwaggerException : Exception
    {
        public QAToolKitSwaggerException(string message) : base(message)
        {
        }

        public QAToolKitSwaggerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected QAToolKitSwaggerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
