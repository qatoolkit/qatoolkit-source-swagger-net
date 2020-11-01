using QAToolKit.Core.Models;
using QAToolKit.Source.Swagger.Test.Fixtures.PetApi.Get.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace QAToolKit.Source.Swagger.Test.Fixtures.PetApi.Post
{
    public static class AddNewPetBody
    {
        public static List<RequestBody> Get(bool exampleValues)
        {
            if (exampleValues)
            {
                return new List<RequestBody>()
                {
                    new RequestBody() {
                        ContentType = ContentType.ToEnum(ContentType.Json),
                        Name = "Pet",
                        Required = true,
                        Properties = PetObjectWithExampleValues.GetProperties()
                    }
                };
            }
            else
            {
                return new List<RequestBody>()
                {
                    new RequestBody() {
                        ContentType = ContentType.ToEnum(ContentType.Json),
                        Name = "Pet",
                        Required = true,
                        Properties = PetObjectWithoutExampleValues.GetProperties()
                    }
                };
            }
        }
    }
}
