using ExpectedObjects;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using QAToolKit.Core.Models;
using QAToolKit.Source.Swagger.Test.Fixtures.BicycleApi.Get;
using QAToolKit.Source.Swagger.Test.Fixtures.BicycleApi.Get.Helpers;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace QAToolKit.Source.Swagger.Test.SwaggerTests.BicycleApi.Get
{
    public class SwaggerProcessorGetBicycleByIdTests
    {
        private readonly ILogger<SwaggerProcessorGetBicycleByIdTests> _logger;

        public SwaggerProcessorGetBicycleByIdTests(ITestOutputHelper testOutputHelper)
        {
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(new XunitLoggerProvider(testOutputHelper));
            _logger = loggerFactory.CreateLogger<SwaggerProcessorGetBicycleByIdTests>();
        }

        [Fact]
        public async Task PetsSwaggerGetPetByIdWithExampleValuesTest_Successfull()
        {
            var urlSource = new SwaggerUrlSource(options =>
            {
                options.AddBaseUrl(new Uri("https://qatoolkitapi.azurewebsites.net/"));
                options.AddRequestFilters(new RequestFilter()
                {
                    EndpointNameWhitelist = new string[] { "GetAllBikes" }
                });
                options.UseSwaggerExampleValues = true;
            });

            var requests = await urlSource.Load(new Uri[] {
                new Uri("https://qatoolkitapi.azurewebsites.net/swagger/v1/swagger.json")
            });

            _logger.LogInformation(JsonConvert.SerializeObject(requests, Formatting.Indented));

            Assert.NotNull(requests);
            Assert.Single(requests);
            Assert.Empty(requests.FirstOrDefault().AuthenticationTypes);
            Assert.Equal("https://qatoolkitapi.azurewebsites.net/", requests.FirstOrDefault().BasePath);
            Assert.Equal("Get all bikes", requests.FirstOrDefault().Description);
            Assert.Equal(HttpMethod.Get, requests.FirstOrDefault().Method);
            Assert.Equal("GetAllBikes", requests.FirstOrDefault().OperationId);
            Assert.Equal(3, requests.FirstOrDefault().Parameters.Count);
            Assert.Equal("/api/bicycles", requests.FirstOrDefault().Path);
            Assert.Empty(requests.FirstOrDefault().RequestBodies);
            Assert.Equal(2, requests.FirstOrDefault().Responses.Count);

            var getBikesParameters = BicyclesParameters.Get(true).ToExpectedObject();
            getBikesParameters.ShouldEqual(requests.FirstOrDefault().Parameters);

            var expectedPetsResponse = GetBicycleByIdResponse.Get().ToExpectedObject();
            expectedPetsResponse.ShouldEqual(requests.FirstOrDefault().Responses);

            Assert.Equal("Get all bikes by filter", requests.FirstOrDefault().Summary);
            Assert.Collection(requests.FirstOrDefault().Tags, item =>
            {
                item = "Public";
            });
            Assert.Empty(requests.FirstOrDefault().TestTypes);
        }

        [Fact]
        public async Task PetsSwaggerGetPetByIdWithExampleValuesAndDifferentBaseAddressTest_Successfull()
        {
            var urlSource = new SwaggerUrlSource(options =>
            {
                options.AddBaseUrl(new Uri("http://localhost/"));
                options.AddRequestFilters(new RequestFilter()
                {
                    EndpointNameWhitelist = new string[] { "GetAllBikes" }
                });
                options.UseSwaggerExampleValues = true;
            });

            var requests = await urlSource.Load(new Uri[] {
                new Uri("https://qatoolkitapi.azurewebsites.net/swagger/v1/swagger.json")
            });

            _logger.LogInformation(JsonConvert.SerializeObject(requests, Formatting.Indented));

            Assert.NotNull(requests);
            Assert.Single(requests);
            Assert.Empty(requests.FirstOrDefault().AuthenticationTypes);
            Assert.Equal("http://localhost/", requests.FirstOrDefault().BasePath);
            Assert.Equal("Get all bikes", requests.FirstOrDefault().Description);
            Assert.Equal(HttpMethod.Get, requests.FirstOrDefault().Method);
            Assert.Equal("GetAllBikes", requests.FirstOrDefault().OperationId);
            Assert.Equal(3, requests.FirstOrDefault().Parameters.Count);
            Assert.Equal("/api/bicycles", requests.FirstOrDefault().Path);
            Assert.Empty(requests.FirstOrDefault().RequestBodies);
            Assert.Equal(2, requests.FirstOrDefault().Responses.Count);

            var getBikesParameters = BicyclesParameters.Get(true).ToExpectedObject();
            getBikesParameters.ShouldEqual(requests.FirstOrDefault().Parameters);

            var expectedPetsResponse = GetBicycleByIdResponse.Get().ToExpectedObject();
            expectedPetsResponse.ShouldEqual(requests.FirstOrDefault().Responses);

            Assert.Equal("Get all bikes by filter", requests.FirstOrDefault().Summary);
            Assert.Collection(requests.FirstOrDefault().Tags, item =>
            {
                item = "Public";
            });
            Assert.Empty(requests.FirstOrDefault().TestTypes);
        }
    }
}
