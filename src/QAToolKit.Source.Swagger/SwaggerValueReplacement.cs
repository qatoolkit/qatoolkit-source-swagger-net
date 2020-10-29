using QAToolKit.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace QAToolKit.Source.Swagger
{
    /// <summary>
    /// Replace swagger request body, url path and parameters with values from ReplacementValue[]
    /// </summary>
    public class SwaggerValueReplacement
    {
        private readonly IList<HttpTestRequest> _requests;
        private readonly ReplacementValue[] _replacementValues;

        /// <summary>
        /// Swagger value replacement
        /// </summary>
        /// <param name="requests"></param>
        /// <param name="replacementValues"></param>
        public SwaggerValueReplacement(IList<HttpTestRequest> requests, ReplacementValue[] replacementValues)
        {
            _requests = requests;
            _replacementValues = replacementValues;
        }

        /// <summary>
        /// Replace request body, url path and parameters with values from ReplacementValue[]
        /// </summary>
        /// <returns></returns>
        public List<HttpTestRequest> ReplaceAll()
        {
            ReplaceRequestBodyModel();
            ReplaceUrlParameters();
            ReplacePathParameters();

            return _requests.ToList();
        }

        private void ReplaceRequestBodyModel()
        {
            foreach (var request in _requests)
            {
                foreach (var requestBody in request.RequestBodies)
                {
                    if (requestBody.Properties == null)
                    {
                        continue;
                    }

                    if (_replacementValues != null)
                    {
                        foreach (var replacementValue in _replacementValues)
                        {
                            var prop = requestBody.Properties.FirstOrDefault(p => p.Name == replacementValue.Key);

                            if (prop != null)
                            {
                                prop.Value = replacementValue.Value;
                            }
                        }
                    }
                }
            }
        }

        private void ReplaceUrlParameters()
        {
            if (_replacementValues != null)
            {
                foreach (var replacementValue in _replacementValues)
                {
                    foreach (var request in _requests)
                    {
                        foreach (var parameter in request.Parameters)
                        {
                            if (parameter.Name == replacementValue.Key)
                            {
                                parameter.Value = replacementValue.Value;
                            }
                        }
                    }
                }
            }
        }

        private void ReplacePathParameters()
        {
            if (_replacementValues != null)
            {
                foreach (var replacementValue in _replacementValues)
                {
                    foreach (var request in _requests)
                    {
                        request.Path = request.Path.Replace("{" + replacementValue.Key + "}", replacementValue.Value);
                    }
                }
            }
        }
    }
}
