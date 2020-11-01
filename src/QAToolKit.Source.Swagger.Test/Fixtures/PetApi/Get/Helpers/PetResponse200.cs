using QAToolKit.Core.Models;
using System.Net;

namespace QAToolKit.Source.Swagger.Test.Fixtures.PetApi.Get.Helpers
{
    public static class PetResponse200
    {
        public static Response Get(bool exampleValues)
        {
            if (exampleValues)
            {
                return new Response()
                {
                    StatusCode = HttpStatusCode.OK,
                    Type = ResponseType.Object,
                    Properties = PetObjectWithExampleValues.GetProperties()
                };
            }
            else
            {
                return new Response()
                {
                    StatusCode = HttpStatusCode.OK,
                    Type = ResponseType.Object,
                    Properties = PetObjectWithoutExampleValues.GetProperties()
                };
            }
        }
    }
}
