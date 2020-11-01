using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace QAToolKit.Source.Swagger.Test.SwaggerTests.PetApi.Get
{
    public class SwaggerProcessorPetApiTests
    {
        private readonly ILogger<SwaggerProcessorPetApiTests> _logger;

        public SwaggerProcessorPetApiTests(ITestOutputHelper testOutputHelper)
        {
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(new XunitLoggerProvider(testOutputHelper));
            _logger = loggerFactory.CreateLogger<SwaggerProcessorPetApiTests>();
        }

        [Fact]
        public async Task AllEndpointsSuccessfull()
        {
            var fileSource = new SwaggerFileSource(options =>
            {
                options.AddBaseUrl(new Uri("https://petstore3.swagger.io/"));
            });

            var requests = await fileSource.Load(new List<FileInfo>() {
                new FileInfo("Assets/swagger-pets-test.json")
            });

            _logger.LogInformation(JsonConvert.SerializeObject(requests, Formatting.Indented));

            Assert.NotNull(requests);
            Assert.Equal(19, requests.Count);
        }
    }
}
