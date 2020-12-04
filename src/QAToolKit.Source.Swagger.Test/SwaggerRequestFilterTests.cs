using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using QAToolKit.Core.Models;
using QAToolKit.Source.Swagger.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace QAToolKit.Source.Swagger.Test
{
    public class SwaggerRequestFilterTests
    {
        [Fact]
        public async Task SwaggerRequestFilterOnlyOperationIdTest_Success()
        {
            var urlSource = new SwaggerUrlSource(options =>
            {
                options.AddBaseUrl(new Uri("https://qatoolkitapi.azurewebsites.net/"));
                options.AddRequestFilters(new RequestFilter()
                {
                    EndpointNameWhitelist = new string[] { "GetAllBikes", "UpdateBike" }
                });
                options.UseSwaggerExampleValues = true;
            });

            var requests = await urlSource.Load(new Uri[] {
                new Uri("https://qatoolkitapi.azurewebsites.net/swagger/v1/swagger.json")
            });

            Assert.Equal(2, requests.Count());
        }

        [Fact]
        public async Task SwaggerRequestFilterOnlyAuthTypeTest_Success()
        {
            var urlSource = new SwaggerUrlSource(options =>
            {
                options.AddBaseUrl(new Uri("https://qatoolkitapi.azurewebsites.net/"));
                options.AddRequestFilters(new RequestFilter()
                {
                    AuthenticationTypes = new List<AuthenticationType.Enumeration> { AuthenticationType.Enumeration.ApiKey }
                });
                options.UseSwaggerExampleValues = true;
            });

            var requests = await urlSource.Load(new Uri[] {
                new Uri("https://qatoolkitapi.azurewebsites.net/swagger/v2/swagger.json")
            });

            Assert.Equal(3, requests.Count());
        }

        [Fact]
        public async Task SwaggerRequestFilterOnlyTestTypeTest_Success()
        {
            var urlSource = new SwaggerUrlSource(options =>
            {
                options.AddBaseUrl(new Uri("https://qatoolkitapi.azurewebsites.net/"));
                options.AddRequestFilters(new RequestFilter()
                {
                    TestTypes = new List<TestType.Enumeration> { TestType.Enumeration.LoadTest }
                });
                options.UseSwaggerExampleValues = true;
            });

            var requests = await urlSource.Load(new Uri[] {
                new Uri("https://qatoolkitapi.azurewebsites.net/swagger/v2/swagger.json")
            });

            Assert.Equal(5, requests.Count());
        }

        [Fact]
        public async Task SwaggerRequestFilterTestAndAuthTypeTest_Success()
        {
            var urlSource = new SwaggerUrlSource(options =>
            {
                options.AddBaseUrl(new Uri("https://qatoolkitapi.azurewebsites.net/"));
                options.AddRequestFilters(new RequestFilter()
                {
                    TestTypes = new List<TestType.Enumeration> { TestType.Enumeration.LoadTest },
                    AuthenticationTypes = new List<AuthenticationType.Enumeration> { AuthenticationType.Enumeration.ApiKey }
                });
                options.UseSwaggerExampleValues = true;
            });

            var requests = await urlSource.Load(new Uri[] {
                new Uri("https://qatoolkitapi.azurewebsites.net/swagger/v2/swagger.json")
            });

            Assert.Equal(3, requests.Count());
        }

        [Fact]
        public async Task SwaggerRequestFilterOperationIdAndAuthTypeTest_Success()
        {
            var urlSource = new SwaggerUrlSource(options =>
            {
                options.AddBaseUrl(new Uri("https://qatoolkitapi.azurewebsites.net/"));
                options.AddRequestFilters(new RequestFilter()
                {
                    TestTypes = new List<TestType.Enumeration> { TestType.Enumeration.LoadTest },
                    EndpointNameWhitelist = new string[] { "UpdateBike" }
                });
                options.UseSwaggerExampleValues = true;
            });

            var requests = await urlSource.Load(new Uri[] {
                new Uri("https://qatoolkitapi.azurewebsites.net/swagger/v2/swagger.json")
            });

            Assert.Single(requests);
        }

        [Fact]
        public async Task SwaggerRequestFilterOperationIdAndTestTypeTest_Success()
        {
            var urlSource = new SwaggerUrlSource(options =>
            {
                options.AddBaseUrl(new Uri("https://qatoolkitapi.azurewebsites.net/"));
                options.AddRequestFilters(new RequestFilter()
                {
                    TestTypes = new List<TestType.Enumeration> { TestType.Enumeration.LoadTest },
                    EndpointNameWhitelist = new string[] { "GetAllBikes", "UpdateBike" }
                });
                options.UseSwaggerExampleValues = true;
            });

            var requests = await urlSource.Load(new Uri[] {
                new Uri("https://qatoolkitapi.azurewebsites.net/swagger/v2/swagger.json")
            });

            Assert.Equal(2, requests.Count());
        }

        [Fact]
        public async Task SwaggerRequestFilterOperationIdAndAuthTypeAndTestTypeTest_Success()
        {
            var urlSource = new SwaggerUrlSource(options =>
            {
                options.AddBaseUrl(new Uri("https://qatoolkitapi.azurewebsites.net/"));
                options.AddRequestFilters(new RequestFilter()
                {
                    TestTypes = new List<TestType.Enumeration> { TestType.Enumeration.LoadTest },
                    EndpointNameWhitelist = new string[] { "GetAllBikes", "UpdateBike" },
                    AuthenticationTypes = new List<AuthenticationType.Enumeration> { AuthenticationType.Enumeration.ApiKey }
                });
                options.UseSwaggerExampleValues = true;
            });

            var requests = await urlSource.Load(new Uri[] {
                new Uri("https://qatoolkitapi.azurewebsites.net/swagger/v2/swagger.json")
            });

            Assert.Single(requests);
        }

        [Fact]
        public async Task SwaggerRequestFilterOperationIdAndAuthTypeAndTestTypeAlternativeTest_Success()
        {
            var urlSource = new SwaggerUrlSource(options =>
            {
                options.AddBaseUrl(new Uri("https://qatoolkitapi.azurewebsites.net/"));
                options.AddRequestFilters(new RequestFilter()
                {
                    TestTypes = new List<TestType.Enumeration> { TestType.Enumeration.LoadTest },
                    EndpointNameWhitelist = new string[] { "DeleteBike", "UpdateBike" },
                    AuthenticationTypes = new List<AuthenticationType.Enumeration> { AuthenticationType.Enumeration.ApiKey }
                });
                options.UseSwaggerExampleValues = true;
            });

            var requests = await urlSource.Load(new Uri[] {
                new Uri("https://qatoolkitapi.azurewebsites.net/swagger/v2/swagger.json")
            });

            Assert.Equal(2, requests.Count());
        }

        [Fact]
        public async Task SwaggerRequestFilterOperationIdAndAuthTypeAndTestTypeAlternativeWithMethodTest_Success()
        {
            var urlSource = new SwaggerUrlSource(options =>
            {
                options.AddBaseUrl(new Uri("https://qatoolkitapi.azurewebsites.net/"));
                options.AddRequestFilters(new RequestFilter()
                {
                    TestTypes = new List<TestType.Enumeration> { TestType.Enumeration.LoadTest },
                    EndpointNameWhitelist = new string[] { "DeleteBike", "UpdateBike" },
                    AuthenticationTypes = new List<AuthenticationType.Enumeration> { AuthenticationType.Enumeration.ApiKey },
                    HttpMethodsWhitelist = new List<HttpMethod>() { HttpMethod.Put, HttpMethod.Delete }
                });
                options.UseSwaggerExampleValues = true;
            });

            var requests = await urlSource.Load(new Uri[] {
                new Uri("https://qatoolkitapi.azurewebsites.net/swagger/v2/swagger.json")
            });

            Assert.Equal(2, requests.Count());
        }

        [Fact]
        public async Task SwaggerRequestFilterTestTypeAlternativeWithMethodTestV2_Success()
        {
            var urlSource = new SwaggerUrlSource(options =>
            {
                options.AddBaseUrl(new Uri("https://qatoolkitapi.azurewebsites.net/"));
                options.AddRequestFilters(new RequestFilter()
                {
                    TestTypes = new List<TestType.Enumeration> { TestType.Enumeration.LoadTest },
                    HttpMethodsWhitelist = new List<HttpMethod>() { HttpMethod.Put, HttpMethod.Post, HttpMethod.Get }
                });
                options.UseSwaggerExampleValues = true;
            });

            var requests = await urlSource.Load(new Uri[] {
                new Uri("https://qatoolkitapi.azurewebsites.net/swagger/v2/swagger.json")
            });

            Assert.Equal(4, requests.Count());
        }

        [Fact]
        public async Task SwaggerRequestFilterTestTypeAlternativeWithGeneralContainsTest_Success()
        {
            var urlSource = new SwaggerUrlSource(options =>
            {
                options.AddBaseUrl(new Uri("https://qatoolkitapi.azurewebsites.net/"));
                options.AddRequestFilters(new RequestFilter()
                {
                    TestTypes = new List<TestType.Enumeration> { TestType.Enumeration.LoadTest },
                    HttpMethodsWhitelist = new List<HttpMethod>() { HttpMethod.Put, HttpMethod.Post, HttpMethod.Get },
                    GeneralContains = new string[] { "new bicycle" }
                });
                options.UseSwaggerExampleValues = true;
            });

            var requests = await urlSource.Load(new Uri[] {
                new Uri("https://qatoolkitapi.azurewebsites.net/swagger/v2/swagger.json")
            });

            Assert.Single(requests);
        }

        [Fact]
        public async Task SwaggerRequestFilterAuthTypeAndTestTypeAlternativeWithGeneralContainsTestV2_Success()
        {
            var urlSource = new SwaggerUrlSource(options =>
            {
                options.AddBaseUrl(new Uri("https://qatoolkitapi.azurewebsites.net/"));
                options.AddRequestFilters(new RequestFilter()
                {
                    TestTypes = new List<TestType.Enumeration> { TestType.Enumeration.LoadTest },
                    HttpMethodsWhitelist = new List<HttpMethod>() { HttpMethod.Put, HttpMethod.Post, HttpMethod.Get },
                    GeneralContains = new string[] { "bicycle" }
                });
                options.UseSwaggerExampleValues = true;
            });

            var requests = await urlSource.Load(new Uri[] {
                new Uri("https://qatoolkitapi.azurewebsites.net/swagger/v2/swagger.json")
            });

            Assert.Equal(3, requests.Count());
        }

        [Fact]
        public async Task SwaggerRequestFilterTestTypeAlternativeWithGeneralContainsTagTest_Success()
        {
            var urlSource = new SwaggerUrlSource(options =>
            {
                options.AddBaseUrl(new Uri("https://qatoolkitapi.azurewebsites.net/"));
                options.AddRequestFilters(new RequestFilter()
                {
                    TestTypes = new List<TestType.Enumeration> { TestType.Enumeration.LoadTest },
                    GeneralContains = new string[] { "PUBLIC" }
                });
                options.UseSwaggerExampleValues = true;
            });

            var requests = await urlSource.Load(new Uri[] {
                new Uri("https://qatoolkitapi.azurewebsites.net/swagger/v2/swagger.json")
            });

            Assert.Equal(5, requests.Count());
        }

        [Fact]
        public async Task SwaggerRequestFilterGeneralContainsTagTest_Success()
        {
            var urlSource = new SwaggerUrlSource(options =>
            {
                options.AddBaseUrl(new Uri("https://qatoolkitapi.azurewebsites.net/"));
                options.AddRequestFilters(new RequestFilter()
                {
                    GeneralContains = new string[] { "PUBLIC" }
                });
                options.UseSwaggerExampleValues = true;
            });

            var requests = await urlSource.Load(new Uri[] {
                new Uri("https://qatoolkitapi.azurewebsites.net/swagger/v1/swagger.json")
            });

            Assert.Equal(5, requests.Count());
        }
    }
}
