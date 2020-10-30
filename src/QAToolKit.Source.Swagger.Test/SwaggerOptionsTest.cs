using Microsoft.Extensions.Logging;
using QAToolKit.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace QAToolKit.Source.Swagger.Test
{
    public class SwaggerOptionsTest
    {
        private readonly ILogger<SwaggerOptionsTest> _logger;

        public SwaggerOptionsTest(ITestOutputHelper testOutputHelper)
        {
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(new XunitLoggerProvider(testOutputHelper));
            _logger = loggerFactory.CreateLogger<SwaggerOptionsTest>();
        }

        [Fact]
        public void SwaggerBasicAuthTest_Successful()
        {
            var options = new SwaggerOptions();
            options.AddBasicAuthentication("user", "password");

            Assert.Equal("user", options.UserName);
            Assert.Equal("password", options.Password);
            Assert.True(options.UseBasicAuth);
        }

        [Fact]
        public void SwaggerAddReplacementValuesTest_Successful()
        {
            var options = new SwaggerOptions();
            options.AddReplacementValues(new ReplacementValue[] {
                new ReplacementValue() {
                    Key = "userId",
                    Value = "1"
                },
                new ReplacementValue() {
                    Key = "roleId",
                    Value = "100"
                }
            });

            Assert.Equal(2, options.ReplacementValues.Count());
            Assert.Equal("userId", options.ReplacementValues[0].Key);
            Assert.Equal("1", options.ReplacementValues[0].Value);
            Assert.Equal("roleId", options.ReplacementValues[1].Key);
            Assert.Equal("100", options.ReplacementValues[1].Value);
        }

        [Fact]
        public void SwaggerAuthenticationTypeRequestFiltersTest_Successful()
        {
            var options = new SwaggerOptions();
            options.AddRequestFilters(new RequestFilter()
            {
                AuthenticationTypes = new List<AuthenticationType>() { AuthenticationType.Administrator, AuthenticationType.Basic }
            });

            Assert.NotNull(options.RequestFilter.AuthenticationTypes.FirstOrDefault(i => i == AuthenticationType.Administrator));
            Assert.Null(options.RequestFilter.AuthenticationTypes.FirstOrDefault(i => i == AuthenticationType.Customer));
            Assert.Null(options.RequestFilter.AuthenticationTypes.FirstOrDefault(i => i == AuthenticationType.ApiKey));
            Assert.NotNull(options.RequestFilter.AuthenticationTypes.FirstOrDefault(i => i == AuthenticationType.Basic));
            Assert.True(options.UseRequestFilter);
        }

        [Fact]
        public void SwaggerEndpointNameWhitelistRequestFiltersTest_Successful()
        {
            var options = new SwaggerOptions();
            options.AddRequestFilters(new RequestFilter()
            {
                EndpointNameWhitelist = new[] { "GetUsers" }
            });

            Assert.NotNull(options.RequestFilter.EndpointNameWhitelist.FirstOrDefault(i => i == "GetUsers"));
            Assert.True(options.UseRequestFilter);
        }

        [Fact]
        public void SwaggerTestTypeRequestFiltersTest_Successful()
        {
            var options = new SwaggerOptions();
            options.AddRequestFilters(new RequestFilter()
            {
                TestTypes = new List<TestType>() { TestType.LoadTest }
            });

            Assert.NotNull(options.RequestFilter.TestTypes.FirstOrDefault(i => i == TestType.LoadTest));
            Assert.Null(options.RequestFilter.TestTypes.FirstOrDefault(i => i == TestType.IntegrationTest));
            Assert.Null(options.RequestFilter.TestTypes.FirstOrDefault(i => i == TestType.SecurityTest));
            Assert.Null(options.RequestFilter.TestTypes.FirstOrDefault(i => i == TestType.SqlTest));
            Assert.True(options.UseRequestFilter);
        }

        [Fact]
        public void SwaggerAddBaseUrlOnRequestFiltersTest_Successful()
        {
            var options = new SwaggerOptions();
            options.AddBaseUrl(new Uri("https://petstore3.swagger.io/"));

            Assert.Equal("https://petstore3.swagger.io/", options.BaseUrl.ToString());
        }

        [Fact]
        public void SwaggerAddBaseUrlOffRequestFiltersTest_Successful()
        {
            var options = new SwaggerOptions();

            Assert.Null(options.BaseUrl);
        }

        [Fact]
        public void SwaggerAddDataGenerationOnRequestFiltersTest_Successful()
        {
            var options = new SwaggerOptions();
            options.AddDataGeneration();

            Assert.True(options.UseDataGeneration);
        }

        [Fact]
        public void SwaggerAddDataGenerationOffRequestFiltersTest_Successful()
        {
            var options = new SwaggerOptions();

            Assert.False(options.UseDataGeneration);
        }
    }
}
