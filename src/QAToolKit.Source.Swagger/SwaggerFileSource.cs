using Microsoft.OpenApi.Readers;
using QAToolKit.Core.Interfaces;
using QAToolKit.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace QAToolKit.Source.Swagger
{
    public class SwaggerFileSource : ITestSource<IList<FileInfo>, IList<HttpTestRequest>>
    {
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
                using (var fileStream = File.OpenRead(filePath.FullName))
                {
                    var openApiDocument = new OpenApiStreamReader().Read(fileStream, out var diagnostic);

                    restRequests.AddRange(processor.MapFromOpenApiDocument(null, openApiDocument));
                }
            }

            return restRequests;
        }
    }
}
