using QAToolKit.Core.Models;
using System.Collections.Generic;

namespace QAToolKit.Source.Swagger.Test.Fixtures.PetApi.Post
{
    public static class UploadPetImageParameters
    {
        public static List<Parameter> Get(bool exampleValues)
        {
            if (exampleValues)
            {
                return new List<Parameter>()
                {
                    new Parameter() {
                        Name = "petId",
                        Location = Location.Path,
                        Nullable = false,
                        Type = "integer",
                        Required = true,
                        Value = null
                    },
                    new Parameter() {
                        Name = "additionalMetadata",
                        Location = Location.Query,
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
                        Name = "petId",
                        Location = Location.Path,
                        Nullable = false,
                        Type = "integer",
                        Required = true,
                        Value = null
                    },
                    new Parameter() {
                        Name = "additionalMetadata",
                        Location = Location.Query,
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
