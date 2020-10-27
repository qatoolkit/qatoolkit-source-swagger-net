using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Writers;
using QAToolKit.Core.Interfaces;
using QAToolKit.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace QAToolKit.Source.Swagger
{
    /// <summary>
    /// Swagger file source
    /// </summary>
    public class SwaggerFileSource : ITestSource<IList<FileInfo>, IList<HttpTestRequest>>
    {
        private readonly SwaggerOptions _swaggerOptions;

        /// <summary>
        /// New instance of swagger file source
        /// </summary>
        /// <param name="options"></param>
        public SwaggerFileSource(Action<SwaggerOptions> options = null)
        {
            _swaggerOptions = new SwaggerOptions();
            options?.Invoke(_swaggerOptions);
        }

        /// <summary>
        /// Load swagger file sources from storage
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public async Task<IList<HttpTestRequest>> Load(IList<FileInfo> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var restRequests = new List<HttpTestRequest>();
            var processor = new SwaggerProcessor();

            foreach (var filePath in source)
            {
                using (FileStream SourceStream = File.OpenRead(filePath.FullName))
                {
                    var openApiDocument = new OpenApiStreamReader().Read(SourceStream, out var diagnostic);

                    var textWritter = new OpenApiJsonWriter(new StringWriter());
                    openApiDocument.SerializeAsV3(textWritter);

                    restRequests.AddRange(processor.MapFromOpenApiDocument(_swaggerOptions.BaseUrl, openApiDocument, _swaggerOptions.ReplacementValues));
                }
            }

            return restRequests;
        }

        internal Task Load(FileInfo fileInfo)
        {
            throw new NotImplementedException();
        }
    }
}
