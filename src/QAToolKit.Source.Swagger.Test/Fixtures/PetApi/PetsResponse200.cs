using QAToolKit.Core.Models;
using System.Net;

namespace QAToolKit.Source.Swagger.Test.Fixtures.PetApi
{
    public static class PetsResponse200
    {
        public static Response Get()
        {
            return new Response()
            {
                StatusCode = HttpStatusCode.OK,
                Type = ResponseType.Objects,
                Properties = PetObjectResponse.GetProperties()
            };
        }
    }
}
