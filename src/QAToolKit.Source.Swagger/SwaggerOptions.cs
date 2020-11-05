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
        /// Use Swagger example values that come with Swagger file
        /// </summary>
        public bool UseSwaggerExampleValues { get; set; } = false;

        /// <summary>
        /// Add basic authentication
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public SwaggerOptions AddBasicAuthentication(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentException(nameof(userName));
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException(nameof(password));

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
            RequestFilter = requestFilter ?? throw new ArgumentException(nameof(requestFilter));
            return this;
        }

        /// <summary>
        /// Add base url
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <returns></returns>
        public SwaggerOptions AddBaseUrl(Uri baseUrl)
        {
            BaseUrl = baseUrl ?? throw new ArgumentException(nameof(baseUrl));
            return this;
        }
    }
}
