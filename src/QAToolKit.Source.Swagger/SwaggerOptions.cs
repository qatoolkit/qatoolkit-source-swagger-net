using QAToolKit.Core.Models;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("QAToolKit.Source.Swagger.Test")]
namespace QAToolKit.Source.Swagger
{
    /// <summary>
    /// Swagger processor options
    /// </summary>
    public class SwaggerOptions
    {
        /// <summary>
        /// Request filter for filtering out the swagger endpoints
        /// </summary>
        internal RequestFilter RequestFilter { get; private set; }
        /// <summary>
        /// Is Swagger protected with Basic authentication
        /// </summary>
        internal bool UseBasicAuth { get; private set; } = false;
        /// <summary>
        /// Is Swagger protected with NTLM authentication
        /// </summary>
        internal bool UseNTLMAuth { get; private set; } = false;
        /// <summary>
        /// Use request filters, to process only selected endpoints.
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
        /// Set custom base API URL to the resulting HttpRequest list.
        /// </summary>
        internal Uri BaseUrl { get; private set; }
        /// <summary>
        /// Use Swagger example values that come with Swagger file. Default us 'false'.
        /// </summary>
        public bool UseSwaggerExampleValues { get; set; } = false;
        /// <summary>
        /// Disable SSL validation when accessing swagger.json file. Default is 'false'.
        /// </summary>
        public bool DisableSSLValidation { get; set; } = false;

        /// <summary>
        /// Add basic authentication to access swagger.json file.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public SwaggerOptions AddBasicAuthentication(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException($"{nameof(userName)} is null.");
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException($"{nameof(password)} is null.");

            UseBasicAuth = true;
            UseNTLMAuth = false;
            UserName = userName;
            Password = password;
            return this;
        }

        /// <summary>
        /// Add NTLM authentication to access swagger.json file. Normally used when swagger is hosted in IIS.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public SwaggerOptions AddNTLMAuthentication(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException($"{nameof(userName)} is null.");
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException($"{nameof(password)} is null.");

            UseBasicAuth = false;
            UseNTLMAuth = true;
            UserName = userName;
            Password = password;
            return this;
        }

        /// <summary>
        /// Add Default NTLM authentication which represents the authentication credentials for the current security context in which the application is running.
        /// </summary>
        /// <returns></returns>
        public SwaggerOptions AddNTLMAuthentication()
        {
            UseBasicAuth = false;
            UseNTLMAuth = true;
            UserName = null;
            Password = null;
            return this;
        }

        /// <summary>
        /// Use request filters, to process only selected endpoints.
        /// </summary>
        /// <param name="requestFilter"></param>
        /// <returns></returns>
        public SwaggerOptions AddRequestFilters(RequestFilter requestFilter)
        {
            UseRequestFilter = true;
            RequestFilter = requestFilter ?? throw new ArgumentNullException($"{nameof(requestFilter)} is null.");
            return this;
        }

        /// <summary>
        /// Add base url, which will override swagger.json URL in the 'HttpRequest' list
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <returns></returns>
        public SwaggerOptions AddBaseUrl(Uri baseUrl)
        {
            BaseUrl = baseUrl ?? throw new ArgumentNullException($"{nameof(baseUrl)} is null.");
            return this;
        }
    }
}
