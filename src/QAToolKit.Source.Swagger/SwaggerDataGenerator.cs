using QAToolKit.Core.Models;
using System.Collections.Generic;

namespace QAToolKit.Source.Swagger
{
    /// <summary>
    /// Data generator for Swagger that will generate the data for the models or URLs. It is applied after the user defined replaced values.
    /// </summary>
    public class SwaggerDataGenerator
    {
        private readonly IList<HttpTestRequest> _requests;

        /// <summary>
        /// Swagger data generator
        /// </summary>
        /// <param name="requests"></param>
        public SwaggerDataGenerator(IList<HttpTestRequest> requests)
        {
            _requests = requests;
        }

        /// <summary>
        /// Generate HTTP request model data
        /// </summary>
        /// <returns></returns>
        public IList<HttpTestRequest> GenerateModelValues()
        {
            foreach (var request in _requests)
            {
                foreach (var body in request.RequestBodies)
                {
                    if (body.ContentType == ContentType.Json)
                    {
                        var propsTemp = new List<Property>();
                        foreach (var property in body.Properties)
                        {
                            propsTemp.Add(GeneratePropertyValue(property));
                        }

                        body.Properties = propsTemp;
                    }
                }
            }

            return _requests;
        }

        private Property GeneratePropertyValue(Property property)
        {
            switch (property.Type)
            {
                case "integer":
                    if (string.IsNullOrEmpty(property.Value))
                    {
                        property.Value = Faker.RandomNumber.Next(0, 1).ToString();
                    }

                    break;
                case "string":
                    property.Value = Faker.Lorem.Sentence(1);
                    break;
                case "object":
                    property.Value = Faker.Lorem.Sentence(1);
                    break;
                case "array":
                    foreach (var prop in property.Properties)
                    {
                        prop.Value = Faker.Lorem.Sentence(1);
                    }
                    break;
                default:
                    break;
            }

            return property;
        }

        /*   public IList<HttpTestRequestV2> GenerateUrlValues()
           {

           }*/
    }
}
