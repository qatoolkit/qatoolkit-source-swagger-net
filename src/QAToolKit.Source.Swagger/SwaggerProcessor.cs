using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using QAToolKit.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
        /// <returns></returns>
        public IList<HttpTestRequest> MapFromOpenApiDocument(Uri baseUri, OpenApiDocument openApiDocument)
        {
            var requests = new List<HttpTestRequest>();

            var server = openApiDocument.Servers.FirstOrDefault();
            if (server != null)
            {
                var tempUri = new Uri(server.Url, UriKind.RelativeOrAbsolute);
                if (tempUri.IsAbsoluteUri)
                {
                    baseUri = tempUri;
                }
                else
                {
                    baseUri = new Uri(baseUri, tempUri);
                }
            }

            foreach (var path in openApiDocument.Paths)
            {
                requests.AddRange(GetRestRequestsForPath(baseUri, path));
            }

            return requests;
        }

        private IList<HttpTestRequest> GetRestRequestsForPath(Uri baseUri, KeyValuePair<string, OpenApiPathItem> path)
        {
            var requests = new List<HttpTestRequest>();

            foreach (var operation in path.Value.Operations)
            {
                requests.Add(new HttpTestRequest()
                {
                    BasePath = baseUri.ToString(),
                    Path = GetPath(path.Key),
                    Method = GetHttpMethod(operation),
                    Summary = GetSummary(operation),
                    Description = GetDescription(operation),
                    OperationId = GetOperationId(operation),
                    Parameters = GetParameters(operation).ToList().ToList(),
                    RequestBodies = GetRequestBodies(operation),
                    Responses = GetResponses(operation),
                    Tags = GetTags(operation),
                    AuthenticationTypes = GetAuthenticationTypes(operation),
                    TestTypes = GetTestTypes(operation)
                });
            }

            return requests;
        }

        private List<TestType> GetTestTypes(KeyValuePair<OperationType, OpenApiOperation> operation)
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

        private List<AuthenticationType> GetAuthenticationTypes(KeyValuePair<OperationType, OpenApiOperation> operation)
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

        private List<RequestBody> GetRequestBodies(KeyValuePair<OperationType, OpenApiOperation> openApiOperation)
        {
            try
            {
                var requests = new List<RequestBody>();

                if (openApiOperation.Value.RequestBody == null)
                {
                    return new List<RequestBody>();
                }

                if (openApiOperation.Value.RequestBody.Content.Count == 0)
                {
                    return new List<RequestBody>();
                }

                foreach (var contentType in openApiOperation.Value.RequestBody.Content)
                {
                    var RequestBody = new RequestBody
                    {
                        Name = contentType.Value.Schema.Reference != null ? contentType.Value.Schema.Reference.Id : null,
                        ContentType = ContentType.FromString(contentType.Key),
                        Properties = new List<Property>()
                    };

                    foreach (KeyValuePair<string, OpenApiSchema> property in contentType.Value.Schema.Properties)
                    {
                        RequestBody.Properties.AddRange(GetPropertiesRecursively(property));
                    }

                    requests.Add(RequestBody);
                }

                return requests;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return new List<RequestBody>();
            }
        }


        private static List<Property> GetPropertiesRecursively(KeyValuePair<string, OpenApiSchema> source)
        {
            var properties = new List<Property>();
            Property itemsProperty = null;

            if (source.Value.Items != null)
            {
                itemsProperty = new Property
                {
                    Description = source.Value.Items.Description,
                    Format = source.Value.Items.Description,
                    Type = source.Value.Items.Type,
                    Properties = new List<Property>(),
                    Name = source.Value.Items.Reference != null ? source.Value.Items.Reference.Id : null
                };
                itemsProperty.Value = SetValue(source.Value.Items.Example);

                foreach (var property in source.Value.Items.Properties)
                {
                    var recursiveProperties = GetPropertiesRecursively(property);

                    if (recursiveProperties != null)
                        itemsProperty.Properties.AddRange(recursiveProperties);
                }
            }

            var prop = new Property
            {
                Name = source.Value.Reference != null ? source.Value.Reference.Id : source.Key,
                Description = source.Value.Description,
                Type = source.Value.Type,
                Format = source.Value.Format,
                Properties = new List<Property>(),
                Items = itemsProperty
            };
            prop.Value = SetValue(source.Value.Example);

            foreach (var property in source.Value.Properties)
            {
                var propsTem = GetPropertiesRecursively(property);

                if (propsTem != null)
                    prop.Properties.AddRange(propsTem);
            }

            properties.Add(prop);

            return properties;
        }

        private static string SetValue(IOpenApiAny value)
        {
            if (value == null)
            {
                return null;
            }

            using (var outputString = new StringWriter())
            {
                var writer = new OpenApiJsonWriter(outputString);
                value.Write(writer, Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0);

                string exampleString = outputString.ToString();

                if (exampleString != null)
                {
                    return exampleString;
                }
            }

            return null;
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
