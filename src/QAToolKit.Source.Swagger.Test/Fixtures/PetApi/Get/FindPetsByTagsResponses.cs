using QAToolKit.Core.Models;
using QAToolKit.Source.Swagger.Test.Fixtures.PetApi.Get.Helpers;
using System.Collections.Generic;

namespace QAToolKit.Source.Swagger.Test.Fixtures.PetApi.Get
{
    public static class FindPetsByTagsResponses
    {
        public static List<Response> Get(bool exampleValues)
        {
            return new List<Response>()
            {
               PetsResponse200.Get(exampleValues),
               PetResponse400.Get()
            };
        }
    }
}
