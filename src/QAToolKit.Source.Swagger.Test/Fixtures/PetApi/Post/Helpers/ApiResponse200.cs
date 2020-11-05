using QAToolKit.Core.Models;
using System.Net;

namespace QAToolKit.Source.Swagger.Test.Fixtures.PetApi.Post.Helpers
{
    public static class ApiResponse200
    {
        public static Response Get()
        {
            return new Response()
            {
                StatusCode = HttpStatusCode.OK,
                Type = ResponseType.Object,
                Properties = ApiResponseObject.GetProperties()
            };
        }
    }
}
