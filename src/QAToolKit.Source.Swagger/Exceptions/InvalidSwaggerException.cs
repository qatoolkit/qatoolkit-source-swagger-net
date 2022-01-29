using System;
using System.Runtime.Serialization;

namespace QAToolKit.Source.Swagger.Exceptions
{
    /// <summary>
    /// InvalidSwagger exception
    /// </summary>
    [Serializable]
    public class InvalidSwaggerException : Exception
    {
        /// <summary>
        /// InvalidSwagger exception
        /// </summary>
        public InvalidSwaggerException(string message) : base(message)
        {
        }

        /// <summary>
        /// InvalidSwagger exception
        /// </summary>
        public InvalidSwaggerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// InvalidSwagger exception
        /// </summary>
        protected InvalidSwaggerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}