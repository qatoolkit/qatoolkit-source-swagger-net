using Microsoft.Extensions.Logging;
using QAToolKit.Source.Swagger.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace QAToolKit.Source.Swagger.Test
{
    public class SwaggerFileSourceIntegrationTest
    {
        private readonly ILogger<SwaggerFileSourceIntegrationTest> _logger;

        public SwaggerFileSourceIntegrationTest(ITestOutputHelper testOutputHelper)
        {
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(new XunitLoggerProvider(testOutputHelper));
            _logger = loggerFactory.CreateLogger<SwaggerFileSourceIntegrationTest>();
        }

        [Fact]
        public async void SwaggerFileSourceWithOptionsTest_Successfull()
        {
            var fileSource = new SwaggerFileSource(options =>
            {
                options.AddBaseUrl(new Uri("https://petstore3.swagger.io/"));
            });

            var requests = await fileSource.Load(new List<FileInfo>() {
                new FileInfo("Assets/swagger-pets-test.json")
            });

            Assert.NotNull(requests);
            Assert.Equal(19, requests.Count());
        }

        [Fact]
        public async void SwaggerFileSourceWithoutOptionsTest_Fails()
        {
            var fileSource = new SwaggerFileSource();

            var exception = await Assert.ThrowsAsync<QAToolKitSwaggerException>(async () => await fileSource.Load(new List<FileInfo>() {
                new FileInfo("Assets/swagger-pets-test.json")
            }));

            _logger.LogInformation(exception.Message);
        }
    }
}
