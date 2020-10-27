using Newtonsoft.Json;
using QAToolKit.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace QAToolKit.Source.Swagger.Test
{
    public class SwaggerFileSourceIntegrationTest
    {
        [Fact]
        public async void SwaggerFileSourceWithoutOptionsTest_Successfull()
        {
            var fileSource = new SwaggerFileSource(options =>
            {
                options.AddBaseUrl(new Uri("https://petstore3.swagger.io/"));
            });

            var requests = await fileSource.Load(new List<FileInfo>() {
                new FileInfo("Assets/swagger-test.json")
            });

            Assert.NotNull(requests);
        }
    }
}
