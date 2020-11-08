using System;
using System.Runtime.Serialization;

namespace QAToolKit.Source.Swagger.Exceptions
{
    /// <summary>
    /// QA Toolkit swagger exception
    /// </summary>
    [Serializable]
    public class QAToolKitSwaggerException : Exception
    {
        /// <summary>
        /// QA Toolkit swagger exception
        /// </summary>
        public QAToolKitSwaggerException(string message) : base(message)
        {
        }

        /// <summary>
        /// QA Toolkit swagger exception
        /// </summary>
        public QAToolKitSwaggerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// QA Toolkit swagger exception
        /// </summary>
        protected QAToolKitSwaggerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
