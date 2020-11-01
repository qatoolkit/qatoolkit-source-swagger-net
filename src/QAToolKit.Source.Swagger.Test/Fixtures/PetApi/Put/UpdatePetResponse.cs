using QAToolKit.Core.Models;
using QAToolKit.Source.Swagger.Test.Fixtures.PetApi.Get.Helpers;
using System.Collections.Generic;

namespace QAToolKit.Source.Swagger.Test.Fixtures.PetApi.Post
{
    public static class UpdatePetResponse
    {
        public static List<Response> Get(bool exampleValues)
        {
            return new List<Response>()
            {
                PetResponse200.Get(exampleValues),
                PetResponse400.Get(),
                PetResponse404.Get(),
                PetResponse405.Get()
            };
        }
    }
}
