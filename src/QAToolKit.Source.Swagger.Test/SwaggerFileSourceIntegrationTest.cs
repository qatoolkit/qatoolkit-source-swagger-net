using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using QAToolKit.Core.Models;
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

        [Fact]
        public async void SwaggerFileSourceYamlWithoutWhitelistTest_Fails()
        {
            var fileSource = new SwaggerFileSource(options =>
            {
                options.AddBaseUrl(new Uri("https://petstore3.swagger.io/"));
            });

            var requests = await fileSource.Load(new List<FileInfo>() {
                new FileInfo("Assets/support-tickets.yaml")
            });

            _logger.LogInformation(JsonConvert.SerializeObject(requests));

            File.WriteAllText("d:\\support.json", JsonConvert.SerializeObject(requests, Formatting.Indented));

            Assert.Equal(7, requests.Count());
        }

        [Fact]
        public async void SwaggerFileSourceYamlWithWhitelistTest_Fails()
        {
            var fileSource = new SwaggerFileSource(options =>
            {
                options.AddBaseUrl(new Uri("https://petstore3.swagger.io/"));
                options.AddRequestFilters(new RequestFilter()
                {
                    EndpointNameWhitelist = new string[] { "getSupportTicketCasesList" }
                });
            });

            var requests = await fileSource.Load(new List<FileInfo>() {
                new FileInfo("Assets/support-tickets.yaml")
            });

            _logger.LogInformation(JsonConvert.SerializeObject(requests));

            Assert.Single(requests);
        }
    }
}
