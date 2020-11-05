using QAToolKit.Core.Models;
using QAToolKit.Source.Swagger.Test.Fixtures.PetApi.Get.Helpers;
using System.Collections.Generic;

namespace QAToolKit.Source.Swagger.Test.Fixtures.PetApi.Post
{
    public static class AddNewPetResponse
    {
        public static List<Response> Get(bool exampleValues)
        {
            return new List<Response>()
            {
                PetResponse200.Get(exampleValues),
                PetResponse405.Get()
            };
        }
    }
}
