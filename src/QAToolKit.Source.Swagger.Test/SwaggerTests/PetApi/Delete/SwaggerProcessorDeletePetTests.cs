using ExpectedObjects;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using QAToolKit.Core.Models;
using QAToolKit.Source.Swagger.Test.Fixtures.PetApi.Post;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace QAToolKit.Source.Swagger.Test.SwaggerTests.PetApi.Delete
{
    public class SwaggerProcessorDeletePetTests
    {
        private readonly ILogger<SwaggerProcessorDeletePetTests> _logger;

        public SwaggerProcessorDeletePetTests(ITestOutputHelper testOutputHelper)
        {
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(new XunitLoggerProvider(testOutputHelper));
            _logger = loggerFactory.CreateLogger<SwaggerProcessorDeletePetTests>();
        }

        [Fact]
        public async Task DeletePetWithoutExampleValuesTest_Successfull()
        {
            var fileSource = new SwaggerFileSource(options =>
            {
                options.AddBaseUrl(new Uri("https://petstore3.swagger.io/"));
                options.AddRequestFilters(new RequestFilter()
                {
                    EndpointNameWhitelist = new string[] { "deletePet" }
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
            Assert.Equal("", requests.FirstOrDefault().Description);
            Assert.Equal(HttpMethod.Delete, requests.FirstOrDefault().Method);
            Assert.Equal("deletePet", requests.FirstOrDefault().OperationId);
            Assert.Equal(2, requests.FirstOrDefault().Parameters.Count);

            var expectedPetDeleteResponse = DeletePetProperties.Get(false).ToExpectedObject();
            expectedPetDeleteResponse.ShouldEqual(requests.FirstOrDefault().Parameters);

            Assert.Equal("/pet/{petId}", requests.FirstOrDefault().Path);
            Assert.Single(requests.FirstOrDefault().Responses);

            var expectedPetsResponse = DeletePetResponse.Get().ToExpectedObject();
            expectedPetsResponse.ShouldEqual(requests.FirstOrDefault().Responses);

            Assert.Equal("Deletes a pet", requests.FirstOrDefault().Summary);
            Assert.Collection(requests.FirstOrDefault().Tags, item =>
            {
                item = "pet";
            });
            Assert.Empty(requests.FirstOrDefault().TestTypes);
        }

        [Fact]
        public async Task DeletePetWithExampleValuesTest_Successfull()
        {
            var fileSource = new SwaggerFileSource(options =>
            {
                options.AddBaseUrl(new Uri("https://petstore3.swagger.io/"));
                options.AddRequestFilters(new RequestFilter()
                {
                    EndpointNameWhitelist = new string[] { "deletePet" }
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
            Assert.Equal("", requests.FirstOrDefault().Description);
            Assert.Equal(HttpMethod.Delete, requests.FirstOrDefault().Method);
            Assert.Equal("deletePet", requests.FirstOrDefault().OperationId);
            Assert.Equal(2, requests.FirstOrDefault().Parameters.Count);

            var expectedPetDeleteResponse = DeletePetProperties.Get(true).ToExpectedObject();
            expectedPetDeleteResponse.ShouldEqual(requests.FirstOrDefault().Parameters);

            Assert.Equal("/pet/{petId}", requests.FirstOrDefault().Path);
            Assert.Single(requests.FirstOrDefault().Responses);

            var expectedPetsResponse = DeletePetResponse.Get().ToExpectedObject();
            expectedPetsResponse.ShouldEqual(requests.FirstOrDefault().Responses);

            Assert.Equal("Deletes a pet", requests.FirstOrDefault().Summary);
            Assert.Collection(requests.FirstOrDefault().Tags, item =>
            {
                item = "pet";
            });
            Assert.Empty(requests.FirstOrDefault().TestTypes);
        }
    }
}
