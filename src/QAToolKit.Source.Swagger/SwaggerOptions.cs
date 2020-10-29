using QAToolKit.Core.Models;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("QAToolKit.Source.Swagger.Test")]
namespace QAToolKit.Source.Swagger
{
    /// <summary>
    /// Swagger options
    /// </summary>
    public class SwaggerOptions
    {
        /// <summary>
        /// Request filter for filtering out the swagger endpoints
        /// </summary>
        internal RequestFilter RequestFilter { get; private set; }
        /// <summary>
        /// Key/value pairs of replacement values
        /// </summary>
        internal ReplacementValue[] ReplacementValues { get; private set; }
        /// <summary>
        /// Is Swagger protected with Basic authentication?
        /// </summary>
        internal bool UseBasicAuth { get; private set; } = false;
        /// <summary>
        /// Use request filters?
        /// </summary>
        internal bool UseRequestFilter { get; private set; } = false;
        /// <summary>
        /// Swagger basic authentication user name
        /// </summary>
        internal string UserName { get; private set; }
        /// <summary>
        /// Swagger basic authentication password
        /// </summary>
        internal string Password { get; private set; }
        /// <summary>
        /// Set custom base API URL
        /// </summary>
        internal Uri BaseUrl { get; private set; }
        /// <summary>
        /// Should data be automatically generated
        /// </summary>
        internal bool UseDataGeneration { get; private set; } = false;

        /// <summary>
        /// Add basic authentication
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public SwaggerOptions AddBasicAuthentication(string userName, string password)
        {
            UseBasicAuth = true;
            UserName = userName;
            Password = password;
            return this;
        }

        /// <summary>
        /// Use request filters
        /// </summary>
        /// <param name="requestFilter"></param>
        /// <returns></returns>
        public SwaggerOptions AddRequestFilters(RequestFilter requestFilter)
        {
            UseRequestFilter = true;
            RequestFilter = requestFilter;
            return this;
        }

        /// <summary>
        /// Use replacement values
        /// </summary>
        /// <param name="replacementValues"></param>
        /// <returns></returns>
        public SwaggerOptions AddReplacementValues(ReplacementValue[] replacementValues)
        {
            ReplacementValues = replacementValues;
            return this;
        }

        /// <summary>
        /// Add base url
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <returns></returns>
        public SwaggerOptions AddBaseUrl(Uri baseUrl)
        {
            BaseUrl = baseUrl;
            return this;
        }

        /// <summary>
        /// Add data generation to the Swagger processor
        /// </summary>
        /// <returns></returns>
        public SwaggerOptions AddDataGeneration()
        {
            UseDataGeneration = true;
            return this;
        }
    }
}
