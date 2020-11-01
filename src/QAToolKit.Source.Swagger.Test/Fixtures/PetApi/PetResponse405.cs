using QAToolKit.Core.Models;
using System.Net;

namespace QAToolKit.Source.Swagger.Test.Fixtures.PetApi
{
    public static class PetResponse405
    {
        public static Response Get()
        {
            return new Response()
            {
                StatusCode = HttpStatusCode.MethodNotAllowed,
                Type = ResponseType.Empty,
                Properties = null
            };
        }
    }
}
