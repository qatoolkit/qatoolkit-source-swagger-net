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
    public class SwaggerProcessorFindByTagsTests
    {
        private readonly ILogger<SwaggerProcessorFindByTagsTests> _logger;

        public SwaggerProcessorFindByTagsTests(ITestOutputHelper testOutputHelper)
        {
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(new XunitLoggerProvider(testOutputHelper));
            _logger = loggerFactory.CreateLogger<SwaggerProcessorFindByTagsTests>();
        }

        [Fact]
        public async Task PetsSwaggerGetByPetByIdWithExcampleValuesTest_Successfull()
        {
            var fileSource = new SwaggerFileSource(options =>
            {
                options.AddBaseUrl(new Uri("https://petstore3.swagger.io/"));
                options.AddRequestFilters(new RequestFilter()
                {
                    EndpointNameWhitelist = new string[] { "findPetsByTags" }
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
            Assert.Equal("Multiple tags can be provided with comma separated strings. Use tag1, tag2, tag3 for testing.", requests.FirstOrDefault().Description);
            Assert.Equal(HttpMethod.Get, requests.FirstOrDefault().Method);
            Assert.Equal("findPetsByTags", requests.FirstOrDefault().OperationId);
            Assert.Single(requests.FirstOrDefault().Parameters);
            Assert.False(requests.FirstOrDefault().Parameters.FirstOrDefault().Nullable);
            Assert.False(requests.FirstOrDefault().Parameters.FirstOrDefault().Required);
            Assert.Equal(Location.Query, requests.FirstOrDefault().Parameters.FirstOrDefault().Location);
            Assert.Equal("array", requests.FirstOrDefault().Parameters.FirstOrDefault().Type);
            Assert.Equal("tags", requests.FirstOrDefault().Parameters.FirstOrDefault().Name);
            Assert.Null(requests.FirstOrDefault().Parameters.FirstOrDefault().Value);
            Assert.Equal("/pet/findByTags", requests.FirstOrDefault().Path);
            Assert.Empty(requests.FirstOrDefault().RequestBodies);
            Assert.Equal(2, requests.FirstOrDefault().Responses.Count);

            var expectedPetsResponse = FindPetsByTagsResponses.Get(true).ToExpectedObject();
            expectedPetsResponse.ShouldEqual(requests.FirstOrDefault().Responses);

            Assert.Equal("Finds Pets by tags", requests.FirstOrDefault().Summary);
            Assert.Collection(requests.FirstOrDefault().Tags, item =>
            {
                item = "pet";
            });
            Assert.Empty(requests.FirstOrDefault().TestTypes);
        }

        [Fact]
        public async Task PetsSwaggerGetByPetByIdWithoutExcampleValuesTest_Successfull()
        {
            var fileSource = new SwaggerFileSource(options =>
            {
                options.AddBaseUrl(new Uri("https://petstore3.swagger.io/"));
                options.AddRequestFilters(new RequestFilter()
                {
                    EndpointNameWhitelist = new string[] { "findPetsByTags" }
                });
                options.UseSwaggerExampleValues = false;
            });

            var requests = await fileSource.Load(new List<FileInfo>() {
                new FileInfo("Assets/swagger-pets-test.json")
            });

            _logger.LogInformation(JsonConvert.SerializeObject(requests, Formatting.Indented));

            Assert.NotNull(requests);
            Assert.Equal(1, requests.Count);
            Assert.Empty(requests.FirstOrDefault().AuthenticationTypes);
            Assert.Equal("https://petstore3.swagger.io/api/v3", requests.FirstOrDefault().BasePath);
            Assert.Equal("Multiple tags can be provided with comma separated strings. Use tag1, tag2, tag3 for testing.", requests.FirstOrDefault().Description);
            Assert.Equal(HttpMethod.Get, requests.FirstOrDefault().Method);
            Assert.Equal("findPetsByTags", requests.FirstOrDefault().OperationId);
            Assert.Single(requests.FirstOrDefault().Parameters);
            Assert.False(requests.FirstOrDefault().Parameters.FirstOrDefault().Nullable);
            Assert.False(requests.FirstOrDefault().Parameters.FirstOrDefault().Required);
            Assert.Equal(Location.Query, requests.FirstOrDefault().Parameters.FirstOrDefault().Location);
            Assert.Equal("array", requests.FirstOrDefault().Parameters.FirstOrDefault().Type);
            Assert.Equal("tags", requests.FirstOrDefault().Parameters.FirstOrDefault().Name);
            Assert.Equal("/pet/findByTags", requests.FirstOrDefault().Path);
            Assert.Empty(requests.FirstOrDefault().RequestBodies);
            Assert.Equal(2, requests.FirstOrDefault().Responses.Count);

            var expectedPetsResponse = FindPetsByTagsResponses.Get(false).ToExpectedObject();
            expectedPetsResponse.ShouldEqual(requests.FirstOrDefault().Responses);

            Assert.Equal("Finds Pets by tags", requests.FirstOrDefault().Summary);
            Assert.Collection(requests.FirstOrDefault().Tags, item =>
            {
                item = "pet";
            });
            Assert.Empty(requests.FirstOrDefault().TestTypes);
        }
    }
}
