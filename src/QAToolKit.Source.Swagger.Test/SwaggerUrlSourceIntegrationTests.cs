using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using QAToolKit.Core.Models;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace QAToolKit.Source.Swagger.Test
{
    public class SwaggerUrlSourceIntegrationTests
    {
        private readonly ILogger<SwaggerUrlSourceIntegrationTests> _logger;

        public SwaggerUrlSourceIntegrationTests(ITestOutputHelper testOutputHelper)
        {
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(new XunitLoggerProvider(testOutputHelper));
            _logger = loggerFactory.CreateLogger<SwaggerUrlSourceIntegrationTests>();
        }

        [Fact]
        public async Task SwaggerUrlSourceWithoutOptionsV3Test_Successfull()
        {
            var urlSource = new SwaggerUrlSource();
            var requests = await urlSource.Load(
                new Uri[]
                {
                      new Uri("https://petstore3.swagger.io/api/v3/openapi.json")
                });

            Assert.NotNull(requests);
        }

        [Fact]
        public async Task SwaggerUrlSourceWithOptionsV3Test_Successfull()
        {
            var swaggerSource = new SwaggerUrlSource(
              options =>
              {
                  options.AddRequestFilters(new RequestFilter()
                  {
                      EndpointNameWhitelist = new string[] { "getPetById", "addPet" }
                  });
              });

            var requests = await swaggerSource.Load(new Uri[] {
                   new Uri("https://petstore3.swagger.io/api/v3/openapi.json")
              });

            _logger.LogInformation(JsonConvert.SerializeObject(requests, Formatting.Indented));

            Assert.NotNull(requests);
        }

        [Fact]
        public async Task SwaggerUrlSourceWithoutUrlV3Test_Fails()
        {
            var urlSource = new SwaggerUrlSource();
            await Assert.ThrowsAsync<UriFormatException>(async () => await urlSource.Load(
                new Uri[]
                {
                      new Uri("")
                }));
        }
    }
}
