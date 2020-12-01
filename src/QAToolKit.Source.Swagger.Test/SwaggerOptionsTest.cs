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
            Assert.False(options.UseNTLMAuth);
        }

        [Fact]
        public void SwaggerNTLMAuthTest_Successful()
        {
            var options = new SwaggerOptions();
            options.AddNTLMAuthentication("user", "password");

            Assert.Equal("user", options.UserName);
            Assert.Equal("password", options.Password);
            Assert.False(options.UseBasicAuth);
            Assert.True(options.UseNTLMAuth);
        }

        [Theory]
        [InlineData("","")]
        [InlineData(null, null)]
        [InlineData(null, "test")]
        [InlineData("test", null)]
        public void SwaggerNTLMAuthTest_Fails(string userName, string password)
        {
            var options = new SwaggerOptions();
            Assert.Throws<ArgumentNullException>(() => options.AddNTLMAuthentication(userName, password));
        }

        [Fact]
        public void SwaggerNTLMAuthWithDefaultUserTest_Successful()
        {
            var options = new SwaggerOptions();
            options.AddNTLMAuthentication();

            Assert.Null(options.UserName);
            Assert.Null( options.Password);
            Assert.False(options.UseBasicAuth);
            Assert.True(options.UseNTLMAuth);
        }

        [Fact]
        public void SwaggerAuthenticationTypeRequestFiltersTest_Successful()
        {
            var options = new SwaggerOptions();
            options.AddRequestFilters(new RequestFilter()
            {
                AuthenticationTypes = new List<AuthenticationType.Enumeration>() { AuthenticationType.Enumeration.Administrator, AuthenticationType.Enumeration.Basic }
            });

            Assert.Contains(AuthenticationType.Enumeration.Administrator, options.RequestFilter.AuthenticationTypes);
            Assert.DoesNotContain(AuthenticationType.Enumeration.Customer, options.RequestFilter.AuthenticationTypes);
            Assert.DoesNotContain(AuthenticationType.Enumeration.ApiKey, options.RequestFilter.AuthenticationTypes);
            Assert.Contains(AuthenticationType.Enumeration.Basic, options.RequestFilter.AuthenticationTypes);
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
                TestTypes = new List<TestType.Enumeration>() { TestType.Enumeration.LoadTest }
            });

            Assert.Contains(TestType.Enumeration.LoadTest, options.RequestFilter.TestTypes);
            Assert.DoesNotContain(TestType.Enumeration.IntegrationTest, options.RequestFilter.TestTypes);
            Assert.DoesNotContain(TestType.Enumeration.SecurityTest, options.RequestFilter.TestTypes);
            Assert.DoesNotContain(TestType.Enumeration.SqlTest, options.RequestFilter.TestTypes);
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
    }
}
