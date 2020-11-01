using QAToolKit.Core.Models;
using System.Collections.Generic;

namespace QAToolKit.Source.Swagger.Test.Fixtures.PetApi.Post
{
    public static class UploadPetImageBody
    {
        public static List<RequestBody> Get()
        {
            return new List<RequestBody>()
                {
                    new RequestBody() {
                        ContentType = ContentType.ToEnum(ContentType.OctetStream),
                        Name = null,
                        Required = false,
                        Properties = null
                    }
                };
        }
    }
}
