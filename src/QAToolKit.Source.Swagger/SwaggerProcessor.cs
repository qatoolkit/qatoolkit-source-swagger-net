using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using QAToolKit.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace QAToolKit.Source.Swagger
{
    /// <summary>
    /// Swagger processor
    /// </summary>
    public class SwaggerProcessor
    {
        private readonly SwaggerOptions _swaggerOptions;

        /// <summary>
        /// Create new SwaggerProcessor instance
        /// </summary>
        /// <param name="swaggerOptions"></param>
        public SwaggerProcessor(SwaggerOptions swaggerOptions)
        {
            _swaggerOptions = swaggerOptions;
        }

        /// <summary>
        /// Map Swagger documents to a list of objects
        /// </summary>
        /// <param name="baseUri"></param>
        /// <param name="openApiDocument"></param>
        /// <returns></returns>
        public IList<HttpRequest> MapFromOpenApiDocument(Uri baseUri, OpenApiDocument openApiDocument)
        {
            var restRequests = new List<HttpRequest>();

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
                    if (baseUri == null)
                    {
                        throw new Exception("Swagger from file source needs BaseUrl defined. Inject baseUrl with AddBaseUrl in your SwaggerSource instantiation.");
                    }

                    baseUri = new Uri(baseUri, tempUri);
                }
            }

            foreach (var path in openApiDocument.Paths)
            {
                restRequests.AddRange(GetRestRequestsForPath(baseUri, path));
            }

            if (_swaggerOptions.UseRequestFilter)
            {
                var filters = new SwaggerRequestFilter(restRequests);
                restRequests = filters.FilterRequests(_swaggerOptions.RequestFilter).ToList();
            }

            return restRequests;
        }

        private IList<HttpRequest> GetRestRequestsForPath(Uri baseUri, KeyValuePair<string, OpenApiPathItem> path)
        {
            var requests = new List<HttpRequest>();

            foreach (var operation in path.Value.Operations)
            {
                requests.Add(new HttpRequest()
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

        private HttpStatusCode? GetHttpStatus(string statusCode)
        {
            switch (statusCode)
            {
                case "200":
                    return HttpStatusCode.OK;
                case "202":
                    return HttpStatusCode.Accepted;
                case "204":
                    return HttpStatusCode.NoContent;
                case "400":
                    return HttpStatusCode.BadRequest;
                case "404":
                    return HttpStatusCode.NotFound;
                case "405":
                    return HttpStatusCode.MethodNotAllowed;
                case "409":
                    return HttpStatusCode.Conflict;
                case "500":
                    return HttpStatusCode.InternalServerError;
                case "default":
                    return null;
                default:
                    throw new Exception("HttpStatusCode not found.");
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
                    Nullable = parameter.Schema.Nullable,
                    Required = parameter.Required,
                    Location = GetPlacement(parameter.In.Value)
                });
            }

            return parameters;
        }

        private Location GetPlacement(ParameterLocation In)
        {
            if (In == ParameterLocation.Query)
            {
                return Location.Query;
            }
            else if (In == ParameterLocation.Path)
            {
                return Location.Path;
            }
            else if (In == ParameterLocation.Header)
            {
                return Location.Header;
            }
            else if (In == ParameterLocation.Cookie)
            {
                return Location.Cookie;
            }
            else
            {
                return Location.Undefined;
            }
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

                //TODO: support other content types
                foreach (var contentType in openApiOperation.Value.RequestBody.Content)
                {
                    var requestBody = new RequestBody
                    {
                        Name = contentType.Value.Schema.Reference != null ? contentType.Value.Schema.Reference.Id : null,
                        ContentType = ContentType.ToEnum(contentType.Key),
                        Properties = null,
                        Required = openApiOperation.Value.RequestBody.Required
                    };

                    foreach (KeyValuePair<string, OpenApiSchema> property in contentType.Value.Schema.Properties)
                    {
                        var temp = GetPropertiesRecursively(property);
                        if (temp != null)
                        {
                            if (requestBody.Properties == null)
                                requestBody.Properties = new List<Property>();

                            requestBody.Properties.AddRange(temp);
                        }
                    }

                    requests.Add(requestBody);
                }

                return requests;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return new List<RequestBody>();
            }
        }


        private List<Property> GetPropertiesRecursively(KeyValuePair<string, OpenApiSchema> source)
        {
            var properties = new List<Property>();

            #region Items
            Property itemsProperty = null;

            if (source.Value.Items != null)
            {
                itemsProperty = new Property
                {
                    Description = source.Value.Items.Description,
                    Format = source.Value.Items.Description,
                    Type = source.Value.Items.Type,
                    Properties = null,
                    Name = source.Value.Items.Reference != null ? source.Value.Items.Reference.Id : null
                };
                itemsProperty.Required = source.Value.Items.Required.Contains(itemsProperty.Name);

                if (_swaggerOptions.UseSwaggerExampleValues)
                    itemsProperty.Value = SetValue(source.Value.Items.Example);

                foreach (var property in source.Value.Items.Properties)
                {
                    var recursiveProperties = GetPropertiesRecursively(property);

                    if (recursiveProperties != null)
                    {
                        if (itemsProperty.Properties == null)
                        {
                            itemsProperty.Properties = new List<Property>();
                        }
                        itemsProperty.Properties.AddRange(recursiveProperties);
                    }
                }
            }
            #endregion

            #region enums
            Property enumProperty = null;
            if (source.Value.Enum != null && source.Value.Enum.Count > 0)
            {
                enumProperty = new Property
                {
                    Description = null,
                    Format = null,
                    Type = "enum",
                    Properties = new List<Property>(),
                    Name = source.Value.Reference != null ? source.Value.Reference.Id : null,
                };
                enumProperty.Required = source.Value.Required.Contains(enumProperty.Name);

                foreach (var enumerable in source.Value.Enum)
                {
                    if (enumerable != null)
                    {
                        enumProperty.Properties.Add(new Property()
                        {
                            Description = null,
                            Format = null,
                            Name = null,
                            Properties = null,
                            Required = false,
                            Type = "string",
                            Value = SetValue(enumerable)
                        });
                    }
                }
            }
            #endregion

            var prop = new Property
            {
                Name = source.Value.Reference != null ? source.Value.Reference.Id : source.Key,
                Description = source.Value.Description,
                Type = source.Value.Type,
                Format = source.Value.Format,
                Properties = null
            };

            if (itemsProperty != null)
            {
                if (prop.Properties == null)
                    prop.Properties = new List<Property>();

                prop.Properties.Add(itemsProperty);
            }

            if (enumProperty != null)
            {
                if (prop.Properties == null)
                    prop.Properties = new List<Property>();

                prop.Type = enumProperty.Type;
                prop.Properties = enumProperty.Properties;
            }

            prop.Required = source.Value.Required.Contains(prop.Name);

            if (_swaggerOptions.UseSwaggerExampleValues)
                prop.Value = SetValue(source.Value.Example);

            foreach (var property in source.Value.Properties)
            {
                var propsTem = GetPropertiesRecursively(property);

                if (propsTem != null)
                    if (prop.Properties == null)
                    {
                        prop.Properties = new List<Property>();
                    }
                prop.Properties.AddRange(propsTem);
            }

            properties.Add(prop);

            return properties;
        }

        private object SetValue(IOpenApiAny value)
        {
            if (value == null)
            {
                return null;
            }

            using (var outputString = new StringWriter())
            {
                var writer = new OpenApiJsonWriter(outputString);
                value.Write(writer, Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0);

                string exampleString = outputString.ToString().Replace("\"", "");

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
                var responseTemp = new Response()
                {
                    Properties = GetResponseProperties(response.Value),
                    StatusCode = GetHttpStatus(response.Key)
                };

                responseTemp.Type = GetResponseType(response.Value);

                responses.Add(responseTemp);
            }

            return responses;
        }

        private ResponseType GetResponseType(OpenApiResponse value)
        {
            foreach (var contentType in value.Content.Where(contentType =>
                  ContentType.From(contentType.Key) == ContentType.Json))
            {
                if (contentType.Value.Schema.Items != null &&
                    (contentType.Value.Schema.Items.Type == "array" || contentType.Value.Schema.Items.Type != "object") &&
                    contentType.Value.Schema.Type == "array")
                {
                    return ResponseType.Array;
                }
                else if (contentType.Value.Schema.Items != null &&
                    (contentType.Value.Schema.Items.Type == "array" || contentType.Value.Schema.Items.Type == "object") &&
                    contentType.Value.Schema.Type == "array")
                {
                    return ResponseType.Objects;
                }
                else if (contentType.Value.Schema.Items == null && contentType.Value.Schema.Type == "object")
                {
                    return ResponseType.Object;
                }
                else if (contentType.Value.Schema.Items == null &&
                        (contentType.Value.Schema.Type != "object" || contentType.Value.Schema.Type != "array"))
                {
                    return ResponseType.Primitive;
                }
                else
                {
                    return ResponseType.Undefined;
                }
            }

            return ResponseType.Empty;
        }

        private List<Property> GetResponseProperties(OpenApiResponse openApiResponse)
        {
            var properties = new List<Property>();

            if (openApiResponse.Content == null)
            {
                return null;
            }

            if (openApiResponse.Content.Count == 0)
            {
                return null;
            }

            //TODO: support other content types
            foreach (var contentType in openApiResponse.Content.Where(contentType =>
            ContentType.From(contentType.Key) == ContentType.Json))
            {
                if (contentType.Value.Schema.Properties != null && contentType.Value.Schema.Properties.Count > 0)
                {
                    foreach (KeyValuePair<string, OpenApiSchema> property in contentType.Value.Schema.Properties)
                    {
                        properties.AddRange(GetPropertiesRecursively(property));
                    }
                }

                if (contentType.Value.Schema.Items != null)
                {
                    foreach (KeyValuePair<string, OpenApiSchema> property in contentType.Value.Schema.Items.Properties)
                    {
                        properties.AddRange(GetPropertiesRecursively(property));
                    }
                }
            }

            return properties;
        }
    }
}
