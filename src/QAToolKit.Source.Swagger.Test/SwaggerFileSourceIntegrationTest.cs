using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace QAToolKit.Source.Swagger.Test
{
    public class SwaggerFileSourceIntegrationTest
    {
        [Fact]
        public async void SwaggerFileSourceWithOptionsTest_Successfull()
        {
            var fileSource = new SwaggerFileSource(options =>
            {
                options.AddBaseUrl(new Uri("https://petstore3.swagger.io/"));
            });

            var requests = await fileSource.Load(new List<FileInfo>() {
                new FileInfo("Assets/swagger-test.json")
            });

            Assert.NotNull(requests);
            Assert.Equal(19, requests.Count());
        }

        [Fact]
        public async void SwaggerFileSourceWithoutOptionsTest_Fails()
        {
            var fileSource = new SwaggerFileSource();

            await Assert.ThrowsAsync<Exception>(async () => await fileSource.Load(new List<FileInfo>() {
                new FileInfo("Assets/swagger-test.json")
            }));
        }
    }
}
