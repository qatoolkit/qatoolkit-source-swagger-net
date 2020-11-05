using QAToolKit.Core.Models;
using System.Collections.Generic;
using System.Net;

namespace QAToolKit.Source.Swagger.Test.Fixtures.BicycleApi
{
    public static class BicycleResponse400
    {
        public static Response Get()
        {
            return new Response()
            {
                StatusCode = HttpStatusCode.BadRequest,
                Type = ResponseType.Object,
                Properties = new List<Property>()
                {
                    new Property() {
                        Name = "statusCode",
                        Description = null,
                        Format = "int32",
                        Required = false,
                        Properties = null,
                        Type = "integer",
                        Value = null
                    },
                    new Property() {
                        Name = "statusDescription",
                        Description = null,
                        Format = null,
                        Required = false,
                        Properties = null,
                        Type = "string",
                        Value = null
                    },
                    new Property() {
                        Name = "message",
                        Description = null,
                        Format = null,
                        Required = false,
                        Properties = null,
                        Type = "string",
                        Value = null
                    },
                    new Property() {
                        Name = "errorCode",
                        Description = null,
                        Format = "int32",
                        Required = false,
                        Properties = null,
                        Type = "integer",
                        Value = null
                    }
                }
            };
        }
    }
}
