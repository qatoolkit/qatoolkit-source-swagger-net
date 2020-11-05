using QAToolKit.Core.Models;
using System.Collections.Generic;

namespace QAToolKit.Source.Swagger.Test.Fixtures.PetApi.Get.Helpers
{
    public static class PetObjectWithExampleValues
    {
        public static List<Property> GetProperties()
        {
            return new List<Property>()
                    {
                        new Property(){
                            Name = "id",
                            Description = null,
                            Format = "int64",
                            Required = false,
                            Properties = null,
                            Type = "integer",
                            Value = "10"
                        },
                        new Property(){
                            Name = "name",
                            Description = null,
                            Format = null,
                            Required = false,
                            Properties = null,
                            Type = "string",
                            Value = "doggie"
                        },
                        new Property(){
                            Name = "Category",
                            Description = null,
                            Format = null,
                            Required = false,
                            Properties = new List<Property>(){
                                new Property()
                                {
                                    Name = "id",
                                    Description = null,
                                    Format = "int64",
                                    Required = false,
                                    Properties = null,
                                    Type = "integer",
                                    Value = "1"
                                },
                                new Property()
                                {
                                    Name = "name",
                                    Description = null,
                                    Format = null,
                                    Required = false,
                                    Properties = null,
                                    Type = "string",
                                    Value = "Dogs"
                                }
                            },
                            Type = "object",
                            Value = null
                        },
                        new Property(){
                            Name = "photoUrls",
                            Description = null,
                            Format = null,
                            Required = false,
                            Properties = new List<Property>(){
                                new Property()
                                {
                                    Name = null,
                                    Description = null,
                                    Format = null,
                                    Required = false,
                                    Properties = null,
                                    Type = "string",
                                    Value = null
                                }
                            },
                            Type = "array",
                            Value = null
                        },
                        new Property(){
                            Name = "tags",
                            Description = null,
                            Format = null,
                            Required = false,
                            Properties = new List<Property>()
                            {
                                new Property()
                                {
                                    Name = "Tag",
                                    Description = null,
                                    Format = null,
                                    Required = false,
                                    Properties = new List<Property>(){
                                        new Property()
                                        {
                                            Name = "id",
                                            Description = null,
                                            Format = "int64",
                                            Required = false,
                                            Properties = null,
                                            Type = "integer",
                                            Value = null
                                        },
                                        new Property()
                                        {
                                            Name = "name",
                                            Description = null,
                                            Format = null,
                                            Required = false,
                                            Properties = null,
                                            Type = "string",
                                            Value = null
                                        }
                                    },
                                    Type = "object",
                                    Value = null
                                }
                            },
                            Type = "array",
                            Value = null
                        },
                        new Property(){
                            Name = "status",
                            Description = "pet status in the store",
                            Format = null,
                            Required = false,
                            Properties = new List<Property>(){
                                        new Property()
                                        {
                                            Name = null,
                                            Description = null,
                                            Format = null,
                                            Required = false,
                                            Properties = null,
                                            Type = "string",
                                            Value = "available"
                                        },
                                        new Property()
                                        {
                                            Name = null,
                                            Description = null,
                                            Format = null,
                                            Required = false,
                                            Properties = null,
                                            Type = "string",
                                            Value = "pending"
                                        },
                                        new Property()
                                        {
                                            Name = null,
                                            Description = null,
                                            Format = null,
                                            Required = false,
                                            Properties = null,
                                            Type = "string",
                                            Value = "sold"
                                        }
                                    },
                            Type = "enum",
                            Value = null
                        }
                    };
        }
    }
}
