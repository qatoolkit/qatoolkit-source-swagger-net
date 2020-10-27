﻿using QAToolKit.Core.Models;
using System;
using System.Threading.Tasks;
using Xunit;

namespace QAToolKit.Source.Swagger.Test
{
    public class SwaggerUrlSourceIntegrationTests
    {
        [Fact]
        public async Task SwaggerUrlSourceWithoutOptionsV3Test_Successfull()
        {
            var urlSource = new SwaggerUrlSource();
            var requests = await urlSource.Load(
                new Uri[]
                {
                      new Uri("https://petstore3.swagger.io/api/v3/openapi.json")
                });

            Assert.NotNull(requests);
        }

        [Fact]
        public async Task SwaggerUrlSourceWithOptionsV3Test_Successfull()
        {
            var swaggerSource = new SwaggerUrlSource(
              options =>
              {
                  options.AddRequestFilters(new RequestFilter()
                  {
                      EndpointNameWhitelist = new string[] { "getPetById" }
                  });
                  options.AddReplacementValues(new ReplacementValue[] {
              new ReplacementValue()
                  {
                      Key = "petId",
                      Value = "1"
                  }
                  });
              });

            var requests = await swaggerSource.Load(new Uri[] {
                   new Uri("https://petstore3.swagger.io/api/v3/openapi.json")
              });

            Assert.NotNull(requests);
        }

        [Fact]
        public async Task SwaggerUrlSourceWithoutUrlV3Test_Fails()
        {
            var urlSource = new SwaggerUrlSource();
            await Assert.ThrowsAsync<UriFormatException>(async () => await urlSource.Load(
                new Uri[]
                {
                      new Uri("")
                }));
        }

        /* [Fact]
         public async Task SwaggerUrlSourceWithoutOptionsV2Test_Successfull()
         {
             var urlSource = new SwaggerUrlSource();
             var requests = await urlSource.Load(
                 new Uri[]
                 {
                       new Uri("https://petstore.swagger.io/v2/swagger.json"),
                 });

             Assert.NotNull(requests);
         }*/
    }
}
