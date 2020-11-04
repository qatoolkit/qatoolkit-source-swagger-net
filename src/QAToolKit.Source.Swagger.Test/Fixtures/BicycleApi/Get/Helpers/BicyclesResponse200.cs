using QAToolKit.Core.Models;
using System.Net;

namespace QAToolKit.Source.Swagger.Test.Fixtures.BicycleApi.Get.Helpers
{
    public static class BicyclesResponse200
    {
        public static Response Get()
        {
            return new Response()
            {
                StatusCode = HttpStatusCode.OK,
                Type = ResponseType.Objects,
                Properties = BicycleObjectWithoutExampleValues.GetProperties()
            };
        }
    }
}
