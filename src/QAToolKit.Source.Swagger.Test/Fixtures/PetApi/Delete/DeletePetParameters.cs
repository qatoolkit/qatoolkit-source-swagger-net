using QAToolKit.Core.Models;
using System.Collections.Generic;

namespace QAToolKit.Source.Swagger.Test.Fixtures.PetApi.Post
{
    public static class DeletePetParameters
    {
        public static List<Parameter> Get(bool exampleValues)
        {
            if (exampleValues)
            {
                return new List<Parameter>()
                {
                    new Parameter() {
                        Name = "api_key",
                        Location = Location.Header,
                        Nullable = false,
                        Type = "string",
                        Required = false,
                        Value = null
                    },
                    new Parameter() {
                        Name = "petId",
                        Location = Location.Path,
                        Nullable = false,
                        Type = "integer",
                        Required = true,
                        Value = null
                    }
                };
            }
            else
            {
                return new List<Parameter>()
                {
                    new Parameter() {
                        Name = "api_key",
                        Location = Location.Header,
                        Nullable = false,
                        Type = "string",
                        Required = false,
                        Value = null
                    },
                    new Parameter() {
                        Name = "petId",
                        Location = Location.Path,
                        Nullable = false,
                        Type = "integer",
                        Required = true,
                        Value = null
                    }
                };
            }
        }
    }
}
