using QAToolKit.Core.Models;
using System.Collections.Generic;

namespace QAToolKit.Source.Swagger.Test.Fixtures.BicycleApi.Get.Helpers
{
    public static class BicyclesParameters
    {
        public static List<Parameter> Get(bool exampleValues)
        {
            if (exampleValues)
            {
                return new List<Parameter>()
                {
                    new Parameter() {
                        Name = "bicycleType",
                        Location = Location.Query,
                        Nullable = false,
                        Type = "integer",
                        Format = "int32",
                        Required = false,
                        Value = null
                    },
                    new Parameter() {
                        Name = "api-version",
                        Location = Location.Query,
                        Nullable = false,
                        Type = "string",
                        Format = null,
                        Required = false,
                        Value = "1"
                    },
                    new Parameter() {
                        Name = "X-version",
                        Location = Location.Header,
                        Nullable = false,
                        Type = "string",
                        Required = false,
                        Value = null
                    }
                };
            }
            else
            {
                return new List<Parameter>()
                {
                   new Parameter() {
                        Name = "bicycleType",
                        Location = Location.Query,
                        Nullable = false,
                        Type = "integer",
                        Required = false,
                        Value = null
                    },
                    new Parameter() {
                        Name = "api-version",
                        Location = Location.Query,
                        Nullable = false,
                        Type = "string",
                        Required = false,
                        Value = null
                    },
                    new Parameter() {
                        Name = "X-version",
                        Location = Location.Header,
                        Nullable = false,
                        Type = "string",
                        Required = false,
                        Value = null
                    }
                };
            }
        }
    }
}
