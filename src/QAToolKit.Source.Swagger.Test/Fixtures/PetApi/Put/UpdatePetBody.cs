using QAToolKit.Core.Models;
using QAToolKit.Source.Swagger.Test.Fixtures.PetApi.Get.Helpers;
using System.Collections.Generic;

namespace QAToolKit.Source.Swagger.Test.Fixtures.PetApi.Post
{
    public static class UpdatePetBody
    {
        public static List<RequestBody> Get(bool exampleValues)
        {
            if (exampleValues)
            {
                return new List<RequestBody>()
                {
                    new RequestBody() {
                        ContentType = ContentType.Json.Value(),
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
                        ContentType = ContentType.Json.Value(),
                        Name = "Pet",
                        Required = true,
                        Properties = PetObjectWithoutExampleValues.GetProperties()
                    }
                };
            }
        }
    }
}
