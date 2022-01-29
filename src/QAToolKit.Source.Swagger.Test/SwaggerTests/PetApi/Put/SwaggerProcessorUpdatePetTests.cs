﻿using ExpectedObjects;
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

namespace QAToolKit.Source.Swagger.Test.SwaggerTests.PetApi.Put
{
    public class SwaggerProcessorUpdatePetTests
    {
        private readonly ILogger<SwaggerProcessorUpdatePetTests> _logger;

        public SwaggerProcessorUpdatePetTests(ITestOutputHelper testOutputHelper)
        {
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(new XunitLoggerProvider(testOutputHelper));
            _logger = loggerFactory.CreateLogger<SwaggerProcessorUpdatePetTests>();
        }

        [Fact]
        public async Task UpdatePetWithoutExampleValuesTest_Successfull()
        {
            var fileSource = new SwaggerFileSource(options =>
            {
                options.AddBaseUrl(new Uri("https://petstore3.swagger.io/"));
                options.AddRequestFilters(new RequestFilter()
                {
                    EndpointNameWhitelist = new string[] { "updatePet" }
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
            Assert.Equal("Update an existing pet by Id", requests.FirstOrDefault().Description);
            Assert.Equal(HttpMethod.Put, requests.FirstOrDefault().Method);
            Assert.Equal("updatePet", requests.FirstOrDefault().OperationId);
            Assert.Empty(requests.FirstOrDefault().Parameters);
            Assert.Equal("/pet", requests.FirstOrDefault().Path);
            Assert.Equal(4, requests.FirstOrDefault().Responses.Count);
            
            var expectedPetsBody = JsonConvert.SerializeObject(UpdatePetBody.Get(false).FirstOrDefault(), Formatting.None);
            Assert.Equal(expectedPetsBody.ToLower(), JsonConvert.SerializeObject(requests.FirstOrDefault().RequestBodies
                .SingleOrDefault(c => c.ContentType == ContentType.Json.Value()), Formatting.None).ToLower());
            
            var expectedPetsResponse = JsonConvert.SerializeObject( UpdatePetResponse.Get(false), Formatting.None);
            Assert.Equal(expectedPetsResponse.ToLower(), JsonConvert.SerializeObject(requests.FirstOrDefault().Responses, Formatting.None).ToLower());
            
            Assert.Equal("Update an existing pet", requests.FirstOrDefault().Summary);
            Assert.Collection(requests.FirstOrDefault().Tags, item =>
            {
                item = "pet";
            });
            Assert.Empty(requests.FirstOrDefault().TestTypes);
        }

        [Fact]
        public async Task UpdatePetWithExampleValuesTest_Successfull()
        {
            var fileSource = new SwaggerFileSource(options =>
            {
                options.AddBaseUrl(new Uri("https://petstore3.swagger.io/"));
                options.AddRequestFilters(new RequestFilter()
                {
                    EndpointNameWhitelist = new string[] { "updatePet" }
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
            Assert.Equal("Update an existing pet by Id", requests.FirstOrDefault().Description);
            Assert.Equal(HttpMethod.Put, requests.FirstOrDefault().Method);
            Assert.Equal("updatePet", requests.FirstOrDefault().OperationId);
            Assert.Empty(requests.FirstOrDefault().Parameters);
            Assert.Equal("/pet", requests.FirstOrDefault().Path);
            Assert.Equal(4, requests.FirstOrDefault().Responses.Count);

            var expectedPetsBody = JsonConvert.SerializeObject(UpdatePetBody.Get(true).FirstOrDefault(), Formatting.None);
            Assert.Equal(expectedPetsBody.ToLower(), JsonConvert.SerializeObject(requests.FirstOrDefault().RequestBodies
                .SingleOrDefault(c => c.ContentType == ContentType.Json.Value()), Formatting.None).ToLower());
            
            var expectedPetsResponse = JsonConvert.SerializeObject( UpdatePetResponse.Get(true), Formatting.None);
            Assert.Equal(expectedPetsResponse.ToLower(), JsonConvert.SerializeObject(requests.FirstOrDefault().Responses, Formatting.None).ToLower());
            

            Assert.Equal("Update an existing pet", requests.FirstOrDefault().Summary);
            Assert.Collection(requests.FirstOrDefault().Tags, item =>
            {
                item = "pet";
            });
            Assert.Empty(requests.FirstOrDefault().TestTypes);
        }
    }
}
