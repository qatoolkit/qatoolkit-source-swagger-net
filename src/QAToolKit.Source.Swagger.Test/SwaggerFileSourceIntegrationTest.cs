using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using QAToolKit.Core.Models;
using QAToolKit.Source.Swagger.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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

            var requests = await fileSource.Load(new List<FileInfo>()
            {
                new FileInfo("Assets/swagger-pets-test.json")
            });

            Assert.NotNull(requests);
            Assert.Equal(19, requests.Count());
        }

        [Fact]
        public async void SwaggerFileSourceWithoutOptionsTest_Fails()
        {
            var fileSource = new SwaggerFileSource();

            var exception = await Assert.ThrowsAsync<QAToolKitSwaggerException>(async () =>
                await fileSource.Load(new List<FileInfo>()
                {
                    new FileInfo("Assets/swagger-pets-test.json")
                }));

            _logger.LogInformation(exception.Message);
        }

        [Fact]
        public async void SwaggerFileSourceYamlWithoutWhitelistTest()
        {
            var fileSource = new SwaggerFileSource(options =>
            {
                options.AddBaseUrl(new Uri("https://petstore3.swagger.io/"));
            });

            var requests = await fileSource.Load(new List<FileInfo>()
            {
                new FileInfo("Assets/support-tickets-ok.yaml")
            });

            _logger.LogInformation(JsonConvert.SerializeObject(requests));

            File.WriteAllText("d:\\support.json", JsonConvert.SerializeObject(requests, Formatting.Indented));

            Assert.Equal(7, requests.Count());
        }

        [Fact]
        public async void SwaggerFileSourceYamlWithWhitelistTest()
        {
            var fileSource = new SwaggerFileSource(options =>
            {
                options.AddBaseUrl(new Uri("https://petstore3.swagger.io/"));
                options.AddRequestFilters(new RequestFilter()
                {
                    EndpointNameWhitelist = new string[] { "getSupportTicketCasesList" }
                });
            });

            var requests = await fileSource.Load(new List<FileInfo>()
            {
                new FileInfo("Assets/support-tickets-ok.yaml")
            });

            _logger.LogInformation(JsonConvert.SerializeObject(requests));

            Assert.Equal("https://petstore3.swagger.io/api/support/v2", requests.FirstOrDefault().BasePath);
            Assert.Equal(HttpMethod.Get, requests.FirstOrDefault().Method);
            Assert.Equal("getSupportTicketCasesList", requests.FirstOrDefault().OperationId);
            Assert.Empty(requests.FirstOrDefault().RequestBodies);
            Assert.Equal(4, requests.FirstOrDefault().Responses.Count);
            Assert.Equal(2, requests.FirstOrDefault().Parameters.Count);
            Assert.Null(requests.FirstOrDefault().Parameters.FirstOrDefault(p => p.Name == "Area").Value);
            Assert.Single(requests);
        }

        [Fact]
        public async void SwaggerFileSourceYamlWithWhitelistAndExampleTest()
        {
            var fileSource = new SwaggerFileSource(options =>
            {
                options.AddBaseUrl(new Uri("http://localhost/"));
                options.AddRequestFilters(new RequestFilter()
                {
                    EndpointNameWhitelist = new string[] { "getSupportTicketsList" }
                });
                options.UseSwaggerExampleValues = true;
                options.UseStrictParsing = true;
            });

            var requests = await fileSource.Load(new List<FileInfo>()
            {
                new FileInfo("Assets/support-tickets-ok.yaml")
            });

            _logger.LogInformation(JsonConvert.SerializeObject(requests));

            Assert.Equal("http://localhost/api/support/v2", requests.FirstOrDefault().BasePath);
            Assert.Equal(HttpMethod.Get, requests.FirstOrDefault().Method);
            Assert.Equal("getSupportTicketsList", requests.FirstOrDefault().OperationId);
            Assert.Empty(requests.FirstOrDefault().RequestBodies);
            Assert.Equal(4, requests.FirstOrDefault().Responses.Count);
            Assert.Equal(2, requests.FirstOrDefault().Parameters.Count);
            Assert.Equal("Company", requests.FirstOrDefault().Parameters.FirstOrDefault(p => p.Name == "Area").Value);
            Assert.Single(requests);
        }

        [Fact]
        public async void SwaggerFileSourceYamlWithoutWhitelistTest_Fails()
        {
            var fileSource = new SwaggerFileSource(options =>
            {
                options.AddBaseUrl(new Uri("https://petstore3.swagger.io/"));
            });

            Assert.ThrowsAsync<InvalidSwaggerException>(async () => await fileSource.Load(new List<FileInfo>()
            {
                new FileInfo("Assets/support-tickets.yaml")
            }));
        }
        
        [Fact]
        public async void InvalidSwaggerFile_Fails()
        {
            var fileSource = new SwaggerFileSource();

            Assert.ThrowsAsync<InvalidSwaggerException>(async () => await fileSource.Load(new List<FileInfo>()
            {
                new FileInfo("Assets/invalid.swagger.json")
            }));
        }
        
        [Fact]
        public async void UnsupportedSwaggerFilev1_Fails()
        {
            var fileSource = new SwaggerFileSource();

            Assert.ThrowsAsync<UnsupportedSwaggerException>(async () => await fileSource.Load(new List<FileInfo>()
            {
                new FileInfo("Assets/swagger-pets-test-unsuppoted-v1.0.json")
            }));
        }
        
        [Fact]
        public async void UnsupportedSwaggerFilev3_1_Fails()
        {
            var fileSource = new SwaggerFileSource();

            Assert.ThrowsAsync<UnsupportedSwaggerException>(async () => await fileSource.Load(new List<FileInfo>()
            {
                new FileInfo("Assets/swagger-pets-test-unsuppoted-v3.1.json")
            }));
        }
    }
}