﻿using QAToolKit.Core.Models;
using QAToolKit.Source.Swagger.Test.Fixtures.PetApi.Get.Helpers;
using System.Collections.Generic;

namespace QAToolKit.Source.Swagger.Test.Fixtures.PetApi.Post
{
    public static class DeletePetResponse
    {
        public static List<Response> Get()
        {
            return new List<Response>()
            {
                PetResponse400.Get()
            };
        }
    }
}
