using QAToolKit.Core.Models;
using System.Collections.Generic;

namespace QAToolKit.Source.Swagger.Test.Fixtures.PetApi
{
    public static class PetsFindByTagsResponses
    {
        public static List<Response> Get()
        {
            return new List<Response>()
            {
               PetsResponse200.Get(),
               PetResponse400.Get()
            };
        }
    }
}
