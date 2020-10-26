using Microsoft.OpenApi.Models;
using QAToolKit.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace QAToolKit.Source.Swagger
{
    /// <summary>
    /// Swagger processor
    /// </summary>
    public class SwaggerProcessor
    {
        /// <summary>
        /// Map Swagger documents to a list of objects
        /// </summary>
        /// <param name="baseUri"></param>
        /// <param name="openApiDocument"></param>
        /// <param name="replacementValues"></param>
        /// <returns></returns>
        public IList<HttpTestRequest> MapFromOpenApiDocument(Uri baseUri, OpenApiDocument openApiDocument, ReplacementValue[] replacementValues)
        {
            var requests = new List<HttpTestRequest>();

            foreach (var path in openApiDocument.Paths)
            {
                requests.AddRange(GetRestRequestsForPath(baseUri, path, replacementValues));
            }

            return requests;
        }

        private IList<HttpTestRequest> GetRestRequestsForPath(Uri baseUri, KeyValuePair<string, OpenApiPathItem> path, ReplacementValue[] replacementValues)
        {
            var requests = new List<HttpTestRequest>();

            foreach (var operation in path.Value.Operations)
            {
                requests.Add(new HttpTestRequest()
                {
                    BasePath = baseUri.ToString(),
                    Path = ReplacePathParameters(GetPath(path.Key), replacementValues),
                    Method = GetHttpMethod(operation),
                    Summary = GetSummary(operation),
                    Description = GetDescription(operation),
                    OperationId = GetOperationId(operation),
                    Parameters = ReplaceUrlParameters(GetParameters(operation).ToList(), replacementValues).ToList(),
                    RequestBody = ReplaceRequestBodyModel(GetRequestBody(operation), replacementValues),
                    Responses = GetResponses(operation),
                    Tags = GetTags(operation),
                    AuthenticationTypes = GetAuthenticationTypes(operation),
                    TestTypes = GetTestTypes(operation)
                });
            }

            return requests;
        }

        private IEnumerable<TestType> GetTestTypes(KeyValuePair<OperationType, OpenApiOperation> operation)
        {
            var testType = new List<TestType>();

            if (operation.Value.Description.Contains(TestType.IntegrationTest.Value()))
            {
                testType.Add(TestType.IntegrationTest);
            }

            if (operation.Value.Description.Contains(TestType.LoadTest.Value()))
            {
                testType.Add(TestType.LoadTest);
            }

            if (operation.Value.Description.Contains(TestType.SecurityTest.Value()))
            {
                testType.Add(TestType.SecurityTest);
            }

            if (operation.Value.Description.Contains(TestType.SqlTest.Value()))
            {
                testType.Add(TestType.SqlTest);
            }

            return testType;
        }

        private IEnumerable<AuthenticationType> GetAuthenticationTypes(KeyValuePair<OperationType, OpenApiOperation> operation)
        {
            var authenticationTypes = new List<AuthenticationType>();

            if (operation.Value.Description.Contains(AuthenticationType.Administrator.Value()))
            {
                authenticationTypes.Add(AuthenticationType.Administrator);
            }

            if (operation.Value.Description.Contains(AuthenticationType.Customer.Value()))
            {
                authenticationTypes.Add(AuthenticationType.Customer);
            }

            if (operation.Value.Description.Contains(AuthenticationType.ApiKey.Value()))
            {
                authenticationTypes.Add(AuthenticationType.ApiKey);
            }

            if (operation.Value.Description.Contains(AuthenticationType.Oauth2.Value()))
            {
                authenticationTypes.Add(AuthenticationType.Oauth2);
            }

            return authenticationTypes;
        }

        private string GetDescription(KeyValuePair<OperationType, OpenApiOperation> operation)
        {
            return operation.Value.Description;
        }

        private string[] GetTags(KeyValuePair<OperationType, OpenApiOperation> operation)
        {
            return operation.Value.Tags.Select(t => t.Name).ToArray();
        }

        private string GetPath(string path)
        {
            return path;
        }

        private string ReplacePathParameters(string path, ReplacementValue[] replacementValues)
        {
            foreach (var replacementValue in replacementValues)
            {
                path = path.Replace("{" + replacementValue.Key + "}", replacementValue.Value);
            }

            return path;
        }

        private HttpMethod GetHttpMethod(KeyValuePair<OperationType, OpenApiOperation> openApiOperation)
        {
            var httpMethodString = openApiOperation.Key.ToString().ToLower();
            switch (httpMethodString)
            {
                case "get":
                    return HttpMethod.Get;
                case "post":
                    return HttpMethod.Post;
                case "put":
                    return HttpMethod.Put;
                case "delete":
                    return HttpMethod.Delete;
                case "options":
                    return HttpMethod.Options;
                case "head":
                    return HttpMethod.Head;
                case "trace":
                    return HttpMethod.Trace;
                case "patch":
#if NETSTANDARD2_0
                    return new HttpMethod("Patch");
#elif NETSTANDARD2_1
                    return HttpMethod.Patch;
#endif
                default:
                    throw new Exception("HttpMethod invalid.");
            }
        }

        private string GetSummary(KeyValuePair<OperationType, OpenApiOperation> openApiOperation)
        {
            return openApiOperation.Value.Summary;
        }

        private string GetOperationId(KeyValuePair<OperationType, OpenApiOperation> openApiOperation)
        {
            return openApiOperation.Value.OperationId;
        }

        private IList<Parameter> GetParameters(KeyValuePair<OperationType, OpenApiOperation> openApiOperation)
        {
            var parameters = new List<Parameter>();

            foreach (var parameter in openApiOperation.Value.Parameters)
            {
                parameters.Add(new Parameter()
                {
                    Name = parameter.Name,
                    Type = parameter.Schema.Type,
                    Nullable = parameter.Schema.Nullable
                });
            }

            return parameters;
        }

        private IList<Parameter> ReplaceUrlParameters(List<Parameter> urlParameters, ReplacementValue[] replacementValues)
        {
            foreach (var replacementValue in replacementValues)
            {
                foreach (var parameter in urlParameters)
                {
                    if (parameter.Name == replacementValue.Key)
                    {
                        parameter.Value = replacementValue.Value;
                    }
                }
            }

            return urlParameters;
        }

        private RequestBody GetRequestBody(KeyValuePair<OperationType, OpenApiOperation> openApiOperation)
        {
            try
            {
                if (openApiOperation.Value.RequestBody == null)
                {
                    return new RequestBody();
                }

                if (openApiOperation.Value.RequestBody.Content.Count == 0)
                {
                    return new RequestBody();
                }

                if (openApiOperation.Value.RequestBody.Content.FirstOrDefault().Value.Schema.Properties.Count == 0)
                {
                    return new RequestBody();
                }

                var requestBody = new RequestBody
                {
                    Name = openApiOperation.Value.RequestBody.Content.FirstOrDefault().Value.Schema.Reference.Id,
                    Properties = new List<Property>()
                };

                foreach (var property in openApiOperation.Value.RequestBody.Content.FirstOrDefault().Value.Schema.Properties)
                {
                    requestBody.Properties.Add(new Property()
                    {
                        Name = property.Key,
                        Description = property.Value.Description,
                        Type = property.Value.Type,
                    });
                }

                return requestBody;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return new RequestBody();
            }
        }

        private RequestBody ReplaceRequestBodyModel(RequestBody requestBody, ReplacementValue[] replacementValues)
        {
            if (requestBody.Properties == null)
            {
                return requestBody;
            }

            var requestBodyResult = new RequestBody
            {
                Name = requestBody.Name
            };

            foreach (var replacementValue in replacementValues)
            {
                var prop = requestBody.Properties.FirstOrDefault(p => p.Name == replacementValue.Key);

                if (prop != null)
                {
                    requestBodyResult.Properties.Add(new Property()
                    {
                        Description = prop.Description,
                        Name = prop.Name,
                        Type = prop.Type,
                        Value = replacementValue.Value
                    });
                }
            }

            return requestBodyResult;
        }

        private List<Response> GetResponses(KeyValuePair<OperationType, OpenApiOperation> openApiOperation)
        {
            var responses = new List<Response>();

            foreach (var response in openApiOperation.Value.Responses)
            {
                responses.Add(new Response()
                {
                    Properties = GetResponseProperties(response.Value),
                    StatusCode = response.Key
                });
            }

            return responses;
        }

        private List<Property> GetResponseProperties(OpenApiResponse openApiResponse)
        {
            if (openApiResponse.Content != null && openApiResponse.Content.Count > 0)
            {
                var properties = new List<Property>();

                foreach (var property in openApiResponse.Content.FirstOrDefault().Value.Schema.Properties)
                {
                    properties.Add(new Property()
                    {
                        Name = property.Key,
                        Description = property.Value.Description,
                        Type = property.Value.Type,
                    });
                }
                return properties;
            }
            else
            {
                return new List<Property>();
            }
        }
    }
}
