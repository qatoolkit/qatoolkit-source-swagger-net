using QAToolKit.Core.Models;
using System.Net;

namespace QAToolKit.Source.Swagger.Test.Fixtures.PetApi
{
    public static class PetResponse400
    {
        public static Response Get()
        {
            return new Response()
            {
                StatusCode = HttpStatusCode.BadRequest,
                Type = ResponseType.Undefined,
                Properties = null
            };
        }
    }
}
