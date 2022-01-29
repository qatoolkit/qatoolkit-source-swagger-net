using System;
using System.Runtime.Serialization;

namespace QAToolKit.Source.Swagger.Exceptions
{
    /// <summary>
    /// UnsupportedSwagger Exception
    /// </summary>
    [Serializable]
    public class UnsupportedSwaggerException : Exception
    {
        /// <summary>
        /// UnsupportedSwagger Exception
        /// </summary>
        public UnsupportedSwaggerException(string message) : base(message)
        {
        }

        /// <summary>
        /// UnsupportedSwagger Exception
        /// </summary>
        public UnsupportedSwaggerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// UnsupportedSwagger Exception
        /// </summary>
        protected UnsupportedSwaggerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}