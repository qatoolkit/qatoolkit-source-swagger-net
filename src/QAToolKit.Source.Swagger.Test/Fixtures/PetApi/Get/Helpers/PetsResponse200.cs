using QAToolKit.Core.Models;
using System.Net;

namespace QAToolKit.Source.Swagger.Test.Fixtures.PetApi.Get.Helpers
{
    public static class PetsResponse200
    {
        public static Response Get(bool exampleValues)
        {
            if (exampleValues)
            {
                return new Response()
                {
                    StatusCode = HttpStatusCode.OK,
                    Type = ResponseType.Objects,
                    Properties = PetObjectWithExampleValues.GetProperties()
                };
            }
            else
            {
                return new Response()
                {
                    StatusCode = HttpStatusCode.OK,
                    Type = ResponseType.Objects,
                    Properties = PetObjectWithoutExampleValues.GetProperties()
                };
            }
        }
    }
}
