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
    public class SwaggerProcessorGetPetByIdTests
    {
        private readonly ILogger<SwaggerProcessorGetPetByIdTests> _logger;

        public SwaggerProcessorGetPetByIdTests(ITestOutputHelper testOutputHelper)
        {
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(new XunitLoggerProvider(testOutputHelper));
            _logger = loggerFactory.CreateLogger<SwaggerProcessorGetPetByIdTests>();
        }

        [Fact]
        public async Task PetsSwaggerGetPetByIdWithExampleValuesTest_Successfull()
        {
            var fileSource = new SwaggerFileSource(options =>
            {
                options.AddBaseUrl(new Uri("https://petstore3.swagger.io/"));
                options.AddRequestFilters(new RequestFilter()
                {
                    EndpointNameWhitelist = new string[] { "getPetById" }
                });
                options.UseSwaggerExampleValues = true;
            });

            var requests = await fileSource.Load(new List<FileInfo>() {
                new FileInfo("Assets/swagger-pets-test.json")
            });

            _logger.LogInformation(JsonConvert.SerializeObject(requests, Formatting.Indented));

            Assert.NotNull(requests);
            Assert.Equal(1, requests.Count);
            Assert.Empty(requests.FirstOrDefault().AuthenticationTypes);
            Assert.Equal("https://petstore3.swagger.io/api/v3", requests.FirstOrDefault().BasePath);
            Assert.Equal("Returns a single pet", requests.FirstOrDefault().Description);
            Assert.Equal(HttpMethod.Get, requests.FirstOrDefault().Method);
            Assert.Equal("getPetById", requests.FirstOrDefault().OperationId);
            Assert.Single(requests.FirstOrDefault().Parameters);
            Assert.Equal("petId", requests.FirstOrDefault().Parameters.FirstOrDefault().Name);
            Assert.False(requests.FirstOrDefault().Parameters.FirstOrDefault().Nullable);
            Assert.True(requests.FirstOrDefault().Parameters.FirstOrDefault().Required);
            Assert.Equal(Location.Path, requests.FirstOrDefault().Parameters.FirstOrDefault().Location);
            Assert.Equal("integer", requests.FirstOrDefault().Parameters.FirstOrDefault().Type);
            Assert.Null(requests.FirstOrDefault().Parameters.FirstOrDefault().Value);
            Assert.Equal("/pet/{petId}", requests.FirstOrDefault().Path);
            Assert.Empty(requests.FirstOrDefault().RequestBodies);
            Assert.Equal(3, requests.FirstOrDefault().Responses.Count);

            var expectedPetsResponse = GetPetByIdResponse.Get(true).ToExpectedObject();
            expectedPetsResponse.ShouldEqual(requests.FirstOrDefault().Responses);

            Assert.Equal("Find pet by ID", requests.FirstOrDefault().Summary);
            Assert.Collection(requests.FirstOrDefault().Tags, item =>
            {
                item = "pet";
            });
            Assert.Empty(requests.FirstOrDefault().TestTypes);
        }

        [Fact]
        public async Task PetsSwaggerGetPetByIdWithoutExampleValuesTest_Successfull()
        {
            var fileSource = new SwaggerFileSource(options =>
            {
                options.AddBaseUrl(new Uri("https://petstore3.swagger.io/"));
                options.AddRequestFilters(new RequestFilter()
                {
                    EndpointNameWhitelist = new string[] { "getPetById" }
                });
            });

            var requests = await fileSource.Load(new List<FileInfo>() {
                new FileInfo("Assets/swagger-pets-test.json")
            });

            _logger.LogInformation(JsonConvert.SerializeObject(requests, Formatting.Indented));

            Assert.NotNull(requests);
            Assert.Equal(1, requests.Count);
            Assert.Empty(requests.FirstOrDefault().AuthenticationTypes);
            Assert.Equal("https://petstore3.swagger.io/api/v3", requests.FirstOrDefault().BasePath);
            Assert.Equal("Returns a single pet", requests.FirstOrDefault().Description);
            Assert.Equal(HttpMethod.Get, requests.FirstOrDefault().Method);
            Assert.Equal("getPetById", requests.FirstOrDefault().OperationId);
            Assert.Single(requests.FirstOrDefault().Parameters);
            Assert.Equal("petId", requests.FirstOrDefault().Parameters.FirstOrDefault().Name);
            Assert.False(requests.FirstOrDefault().Parameters.FirstOrDefault().Nullable);
            Assert.True(requests.FirstOrDefault().Parameters.FirstOrDefault().Required);
            Assert.Equal(Location.Path, requests.FirstOrDefault().Parameters.FirstOrDefault().Location);
            Assert.Equal("integer", requests.FirstOrDefault().Parameters.FirstOrDefault().Type);
            Assert.Equal("/pet/{petId}", requests.FirstOrDefault().Path);
            Assert.Empty(requests.FirstOrDefault().RequestBodies);
            Assert.Equal(3, requests.FirstOrDefault().Responses.Count);

            var expectedPetsResponse = GetPetByIdResponse.Get(false).ToExpectedObject();
            expectedPetsResponse.ShouldEqual(requests.FirstOrDefault().Responses);

            Assert.Equal("Find pet by ID", requests.FirstOrDefault().Summary);
            Assert.Collection(requests.FirstOrDefault().Tags, item =>
            {
                item = "pet";
            });
            Assert.Empty(requests.FirstOrDefault().TestTypes);
        }
    }
}
