using ExpectedObjects;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using QAToolKit.Core.Models;
using QAToolKit.Source.Swagger.Test.Fixtures.PetApi.Get;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace QAToolKit.Source.Swagger.Test.SwaggerTests.PetApi.Get
{
    public class SwaggerProcessorGetPetsByStatus
    {
        private readonly ILogger<SwaggerProcessorGetPetsByStatus> _logger;

        public SwaggerProcessorGetPetsByStatus(ITestOutputHelper testOutputHelper)
        {
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(new XunitLoggerProvider(testOutputHelper));
            _logger = loggerFactory.CreateLogger<SwaggerProcessorGetPetsByStatus>();
        }

        [Fact]
        public async Task PetsSwaggerGetPetsByStatuesWithExampleValuesTest_Successfull()
        {
            var fileSource = new SwaggerFileSource(options =>
            {
                options.AddBaseUrl(new Uri("https://petstore3.swagger.io/"));
                options.AddRequestFilters(new RequestFilter()
                {
                    EndpointNameWhitelist = new string[] { "findPetsByStatus" }
                });
                options.UseSwaggerExampleValues = true;
            });

            var requests = await fileSource.Load(new List<FileInfo>() {
                new FileInfo("Assets/swagger-pets-test.json")
            });

            _logger.LogInformation(JsonConvert.SerializeObject(requests, Formatting.Indented));

            Assert.NotNull(requests);
            Assert.Single(requests);
            Assert.Empty(requests.FirstOrDefault().AuthenticationTypes);
            Assert.Equal("https://petstore3.swagger.io/api/v3", requests.FirstOrDefault().BasePath);
            Assert.Equal("Multiple status values can be provided with comma separated strings", requests.FirstOrDefault().Description);
            Assert.Equal(HttpMethod.Get, requests.FirstOrDefault().Method);
            Assert.Equal("findPetsByStatus", requests.FirstOrDefault().OperationId);
            Assert.Single(requests.FirstOrDefault().Parameters);
            Assert.Equal("status", requests.FirstOrDefault().Parameters.FirstOrDefault().Name);
            Assert.False(requests.FirstOrDefault().Parameters.FirstOrDefault().Nullable);
            Assert.False(requests.FirstOrDefault().Parameters.FirstOrDefault().Required);
            Assert.Equal(Location.Query, requests.FirstOrDefault().Parameters.FirstOrDefault().Location);
            Assert.Equal("string", requests.FirstOrDefault().Parameters.FirstOrDefault().Type);
            Assert.Null(requests.FirstOrDefault().Parameters.FirstOrDefault().Value);
            Assert.Equal("/pet/findByStatus", requests.FirstOrDefault().Path);
            Assert.Empty(requests.FirstOrDefault().RequestBodies);
            Assert.Equal(2, requests.FirstOrDefault().Responses.Count);

            var expectedPetsResponse = FindPetsByStatusResponses.Get(true).ToExpectedObject();
            expectedPetsResponse.ShouldEqual(requests.FirstOrDefault().Responses);

            Assert.Equal("Finds Pets by status", requests.FirstOrDefault().Summary);
            Assert.Collection(requests.FirstOrDefault().Tags, item =>
            {
                item = "pet";
            });
            Assert.Empty(requests.FirstOrDefault().TestTypes);
        }

        [Fact]
        public async Task PetsSwaggerGetPetsByStatuesWithoutExampleValuesTest_Successfull()
        {
            var fileSource = new SwaggerFileSource(options =>
            {
                options.AddBaseUrl(new Uri("https://petstore3.swagger.io/"));
                options.AddRequestFilters(new RequestFilter()
                {
                    EndpointNameWhitelist = new string[] { "findPetsByStatus" }
                });
            });

            var requests = await fileSource.Load(new List<FileInfo>() {
                new FileInfo("Assets/swagger-pets-test.json")
            });

            _logger.LogInformation(JsonConvert.SerializeObject(requests, Formatting.Indented));

            Assert.NotNull(requests);
            Assert.Single(requests);
            Assert.Empty(requests.FirstOrDefault().AuthenticationTypes);
            Assert.Equal("https://petstore3.swagger.io/api/v3", requests.FirstOrDefault().BasePath);
            Assert.Equal("Multiple status values can be provided with comma separated strings", requests.FirstOrDefault().Description);
            Assert.Equal(HttpMethod.Get, requests.FirstOrDefault().Method);
            Assert.Equal("findPetsByStatus", requests.FirstOrDefault().OperationId);
            Assert.Single(requests.FirstOrDefault().Parameters);
            Assert.Equal("status", requests.FirstOrDefault().Parameters.FirstOrDefault().Name);
            Assert.False(requests.FirstOrDefault().Parameters.FirstOrDefault().Nullable);
            Assert.False(requests.FirstOrDefault().Parameters.FirstOrDefault().Required);
            Assert.Equal(Location.Query, requests.FirstOrDefault().Parameters.FirstOrDefault().Location);
            Assert.Equal("string", requests.FirstOrDefault().Parameters.FirstOrDefault().Type);
            Assert.Null(requests.FirstOrDefault().Parameters.FirstOrDefault().Value);
            Assert.Equal("/pet/findByStatus", requests.FirstOrDefault().Path);
            Assert.Empty(requests.FirstOrDefault().RequestBodies);
            Assert.Equal(2, requests.FirstOrDefault().Responses.Count);

            var expectedPetsResponse = FindPetsByStatusResponses.Get(false).ToExpectedObject();
            expectedPetsResponse.ShouldEqual(requests.FirstOrDefault().Responses);

            Assert.Equal("Finds Pets by status", requests.FirstOrDefault().Summary);
            Assert.Collection(requests.FirstOrDefault().Tags, item =>
            {
                item = "pet";
            });
            Assert.Empty(requests.FirstOrDefault().TestTypes);
        }
    }
}
