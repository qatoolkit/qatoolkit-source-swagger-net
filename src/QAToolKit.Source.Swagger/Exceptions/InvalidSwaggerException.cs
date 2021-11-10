using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.OpenApi.Models;

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

        /// <summary>s
        /// InvalidSwagger exception
        /// </summary>
        public InvalidSwaggerException(string message, IList<OpenApiError> errors) : base(message += string.Join(",",
            errors.Select(s => new KeyValuePair<string, string>(s.Message, s.Pointer)).ToList()))
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