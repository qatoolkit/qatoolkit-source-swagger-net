using QAToolKit.Core.Models;
using QAToolKit.Source.Swagger.Test.Fixtures.BicycleApi.Get.Helpers;
using System.Collections.Generic;

namespace QAToolKit.Source.Swagger.Test.Fixtures.BicycleApi.Get
{
    public static class GetBicycleByIdResponse
    {
        public static List<Response> Get()
        {
            return new List<Response>()
            {
                BicyclesResponse200.Get(),
                BicycleResponse400.Get()
            };
        }
    }
}
