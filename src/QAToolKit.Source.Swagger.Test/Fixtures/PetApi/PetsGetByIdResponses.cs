using QAToolKit.Core.Models;
using System.Collections.Generic;

namespace QAToolKit.Source.Swagger.Test.Fixtures.PetApi
{
    public static class PetsGetByIdResponseFixtures
    {
        public static List<Response> Get()
        {
            return new List<Response>()
            {
                PetResponse200.Get(),
                PetResponse400.Get(),
                PetResponse404.Get()
            };
        }
    }
}
