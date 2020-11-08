﻿using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Writers;
using QAToolKit.Core.Interfaces;
using QAToolKit.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace QAToolKit.Source.Swagger
{
    /// <summary>
    /// Swagger Url source
    /// </summary>
    public class SwaggerUrlSource : ITestSource<Uri[], IEnumerable<HttpRequest>>
    {
        private readonly SwaggerOptions _swaggerOptions;

        /// <summary>
        /// Swagger Url source constructor
        /// </summary>
        /// <param name="options"></param>
        public SwaggerUrlSource(Action<SwaggerOptions> options = null)
        {
            _swaggerOptions = new SwaggerOptions();
            options?.Invoke(_swaggerOptions);
        }

        /// <summary>
        /// Load swagger files from the URL and apply loader settings if necessary
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public Task<IEnumerable<HttpRequest>> Load(Uri[] source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return LoadInternal(source);
        }

        private async Task<IEnumerable<HttpRequest>> LoadInternal(Uri[] source)
        {
            var restRequests = new List<HttpRequest>();
            var processor = new SwaggerProcessor(_swaggerOptions);

            foreach (var uri in source)
            {
                using (var httpClient = new HttpClient())
                {
                    if (_swaggerOptions.UseBasicAuth)
                    {
                        var authenticationString = $"{_swaggerOptions.UserName}:{_swaggerOptions.Password}";
                        var base64EncodedAuthenticationString = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(authenticationString));
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
                    }

                    var stream = await httpClient.GetStreamAsync(uri);

                    var openApiDocument = new OpenApiStreamReader().Read(stream, out var diagnostic);
                    var textWritter = new OpenApiJsonWriter(new StringWriter());
                    openApiDocument.SerializeAsV3(textWritter);

                    var requests = processor.MapFromOpenApiDocument(new Uri($"{uri.Scheme}://{uri.Host}"), openApiDocument);

                    restRequests.AddRange(requests);
                }
            }

            return restRequests;
        }
    }
}
