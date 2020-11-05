using QAToolKit.Core.Models;
using QAToolKit.Source.Swagger.Test.Fixtures.PetApi.Post.Helpers;
using System.Collections.Generic;

namespace QAToolKit.Source.Swagger.Test.Fixtures.PetApi.Post
{
    public static class UploadPetImageResponse
    {
        public static List<Response> Get()
        {
            return new List<Response>()
            {
                ApiResponse200.Get()
            };
        }
    }
}
