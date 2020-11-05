using QAToolKit.Core.Models;
using System.Collections.Generic;

namespace QAToolKit.Source.Swagger.Test.Fixtures.PetApi.Post.Helpers
{
    public class ApiResponseObject
    {
        public static List<Property> GetProperties()
        {
            return new List<Property>()
            {
                new Property()
                {
                    Name = "code",
                    Description = null,
                    Format = "int32",
                    Required = false,
                    Properties = null,
                    Type = "integer",
                    Value = null
                },
                new Property()
                {
                    Name = "type",
                    Description = null,
                    Format = null,
                    Required = false,
                    Properties = null,
                    Type = "string",
                    Value = null
                },
                new Property()
                {
                    Name = "message",
                    Description = null,
                    Format = null,
                    Required = false,
                    Properties = null,
                    Type = "string",
                    Value = null
                }
            };
        }
    }
}
