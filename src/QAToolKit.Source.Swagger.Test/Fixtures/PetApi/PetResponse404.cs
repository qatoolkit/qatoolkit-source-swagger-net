using QAToolKit.Core.Models;
using System.Net;

namespace QAToolKit.Source.Swagger.Test.Fixtures.PetApi
{
    public static class PetResponse404
    {
        public static Response Get()
        {
            return new Response()
            {
                StatusCode = HttpStatusCode.NotFound,
                Type = ResponseType.Undefined,
                Properties = null
            };
        }
    }
}
