using QAToolKit.Core.Models;
using System.Collections.Generic;

namespace QAToolKit.Source.Swagger.Test.Fixtures.BicycleApi.Get.Helpers
{
    public static class BicycleObjectWithoutExampleValues
    {
        public static List<Property> GetProperties()
        {
            return new List<Property>()
                    {
                        new Property(){
                            Name = "id",
                            Description = "Bike Id",
                            Format = "int32",
                            Required = false,
                            Properties = null,
                            Type = "integer",
                            Value = "1"
                        },
                        new Property(){
                            Name = "name",
                            Description = "Bike name",
                            Format = null,
                            Required = false,
                            Properties = null,
                            Type = "string",
                            Value = "Foil"
                        },
                        new Property(){
                            Name = "brand",
                            Description = "Bicycle brand",
                            Format = null,
                            Required = false,
                            Properties = null,
                            Type = "string",
                            Value = "Cannondale"
                        },
                        new Property(){
                            Name = "BicycleType",
                            Description = null,
                            Format = "int32",
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
                                            Value = "0"
                                        },
                                        new Property()
                                        {
                                            Name = null,
                                            Description = null,
                                            Format = null,
                                            Required = false,
                                            Properties = null,
                                            Type = "string",
                                            Value = "1"
                                        },
                                        new Property()
                                        {
                                            Name = null,
                                            Description = null,
                                            Format = null,
                                            Required = false,
                                            Properties = null,
                                            Type = "string",
                                            Value = "2"
                                        },
                                        new Property()
                                        {
                                            Name = null,
                                            Description = null,
                                            Format = null,
                                            Required = false,
                                            Properties = null,
                                            Type = "string",
                                            Value = "3"
                                        }
                                    },
                            Type = "enum",
                            Value = null
                        }
                    };
        }
    }
}
