using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using QAToolKit.Core.Models;
using QAToolKit.Source.Swagger.Exceptions;
using System;
using System.Collections.Generic;
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
        public IEnumerable<HttpRequest> MapFromOpenApiDocument(Uri baseUri, OpenApiDocument openApiDocument)
        {
            if (openApiDocument == null || openApiDocument.Paths == null)
            {
                throw new ArgumentNullException(nameof(openApiDocument));
            }

            var restRequests = new List<HttpRequest>();

            baseUri = ParseBaseUri(baseUri, openApiDocument.Servers.FirstOrDefault());

            foreach (var path in openApiDocument.Paths)
            {
                restRequests.AddRange(GetRestRequestsForPath(baseUri, path, openApiDocument.Components?.RequestBodies));
            }

            if (_swaggerOptions.UseRequestFilter)
            {
                var filters = new SwaggerRequestFilter(restRequests);
                restRequests = filters.FilterRequests(_swaggerOptions.RequestFilter).ToList();
            }

            return restRequests;
        }

        private static Uri ParseBaseUri(Uri baseUri, OpenApiServer openApiServer)
        {
            if (openApiServer != null)
            {
                Uri tempUri;
                try
                {
                    tempUri = new Uri(openApiServer.Url, UriKind.RelativeOrAbsolute);
                }
                catch (Exception)
                {
                    tempUri = baseUri;
                }

                if (tempUri.IsAbsoluteUri)
                {
                    baseUri = tempUri;
                }
                else
                {
                    if (baseUri == null)
                    {
                        throw new QAToolKitSwaggerException(
                            "Swagger needs BaseUrl defined. Inject baseUrl with AddBaseUrl in your SwaggerSource instantiation.");
                    }

                    baseUri = new Uri(baseUri, tempUri);
                }
            }
            else
            {
                if (baseUri == null)
                {
                    throw new QAToolKitSwaggerException(
                        "Swagger needs BaseUrl defined. Inject baseUrl with AddBaseUrl in your SwaggerSource instantiation.");
                }
            }

            return baseUri;
        }

        private IEnumerable<HttpRequest> GetRestRequestsForPath(Uri baseUri, KeyValuePair<string, OpenApiPathItem> path,
            IDictionary<string, OpenApiRequestBody> schemas = null)
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
                    Parameters = GetParameters(operation).ToList(),
                    RequestBodies = GetRequestBodies(operation, schemas),
                    Responses = GetResponses(operation),
                    Tags = GetTags(operation),
                    AuthenticationTypes = GetAuthenticationTypes(operation),
                    TestTypes = GetTestTypes(operation)
                });
            }

            return requests;
        }

        private List<TestType.Enumeration> GetTestTypes(KeyValuePair<OperationType, OpenApiOperation> operation)
        {
            var testType = new List<TestType.Enumeration>();

            if (operation.Value == null)
            {
                return testType;
            }

            if (operation.Value.Description == null)
            {
                return testType;
            }

            if (operation.Value.Description.Contains(TestType.IntegrationTest.Value()))
            {
                testType.Add(TestType.Enumeration.IntegrationTest);
            }

            if (operation.Value.Description.Contains(TestType.LoadTest.Value()))
            {
                testType.Add(TestType.Enumeration.LoadTest);
            }

            if (operation.Value.Description.Contains(TestType.SecurityTest.Value()))
            {
                testType.Add(TestType.Enumeration.SecurityTest);
            }

            if (operation.Value.Description.Contains(TestType.SqlTest.Value()))
            {
                testType.Add(TestType.Enumeration.SqlTest);
            }

            return testType;
        }

        private List<AuthenticationType.Enumeration> GetAuthenticationTypes(
            KeyValuePair<OperationType, OpenApiOperation> operation)
        {
            var authenticationTypes = new List<AuthenticationType.Enumeration>();

            if (operation.Value == null)
            {
                return authenticationTypes;
            }

            if (operation.Value.Description == null)
            {
                return authenticationTypes;
            }

            if (operation.Value.Description.Contains(AuthenticationType.Administrator.Value()))
            {
                authenticationTypes.Add(AuthenticationType.Enumeration.Administrator);
            }

            if (operation.Value.Description.Contains(AuthenticationType.Customer.Value()))
            {
                authenticationTypes.Add(AuthenticationType.Enumeration.Customer);
            }

            if (operation.Value.Description.Contains(AuthenticationType.ApiKey.Value()))
            {
                authenticationTypes.Add(AuthenticationType.Enumeration.ApiKey);
            }

            if (operation.Value.Description.Contains(AuthenticationType.OAuth2.Value()))
            {
                authenticationTypes.Add(AuthenticationType.Enumeration.OAuth2);
            }

            return authenticationTypes;
        }

        private string GetDescription(KeyValuePair<OperationType, OpenApiOperation> operation)
        {
            if (operation.Value == null)
            {
                return String.Empty;
            }

            if (operation.Value.Description == null)
            {
                return String.Empty;
            }

            return operation.Value.Description;
        }

        private string[] GetTags(KeyValuePair<OperationType, OpenApiOperation> operation)
        {
            if (operation.Value == null)
            {
                return Array.Empty<string>();
            }

            if (operation.Value.Tags == null)
            {
                return Array.Empty<string>();
            }

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
#if NETSTANDARD2_0
                case "patch":
                    return new HttpMethod("Patch");
#elif NETSTANDARD2_1
                case "patch":
                    return HttpMethod.Patch;
#elif NETCOREAPP3_1
                case "patch":
                    return HttpMethod.Patch;
#elif NET6_0
                case "patch":
                    return HttpMethod.Patch;
#endif
                default:
                    throw new QAToolKitSwaggerException($"HttpMethod invalid '{httpMethodString}'.");
            }
        }

        private HttpStatusCode? GetHttpStatus(string statusCode)
        {
            return statusCode switch
            {
                "2XX" => HttpStatusCode.OK,
                "200" => HttpStatusCode.OK,
                "201" => HttpStatusCode.Created,
                "202" => HttpStatusCode.Accepted,
                "204" => HttpStatusCode.NoContent,
                "302" => HttpStatusCode.Found,
                "400" => HttpStatusCode.BadRequest,
                "401" => HttpStatusCode.Unauthorized,
                "403" => HttpStatusCode.Forbidden,
                "404" => HttpStatusCode.NotFound,
                "405" => HttpStatusCode.MethodNotAllowed,
                "406" => HttpStatusCode.NotAcceptable,
                "409" => HttpStatusCode.Conflict,
                "410" => (HttpStatusCode)410,
                "413" => HttpStatusCode.RequestEntityTooLarge,
                "415" => HttpStatusCode.UnsupportedMediaType,
                "429" => (HttpStatusCode)429,
                "5XX" => HttpStatusCode.InternalServerError,
                "500" => HttpStatusCode.InternalServerError,
                "501" => HttpStatusCode.NotImplemented,
                "502" => HttpStatusCode.BadGateway,
                "503" => HttpStatusCode.ServiceUnavailable,
                "504" => HttpStatusCode.GatewayTimeout,
                "505" => HttpStatusCode.HttpVersionNotSupported,
#if NETSTANDARD2_0
                "422" => throw new QAToolKitSwaggerException($"HttpStatusCode not supported '{statusCode}'."),
#elif NETSTANDARD2_1
                "422" => HttpStatusCode.UnprocessableEntity,
#elif NETCOREAPP3_1
                "422" => HttpStatusCode.UnprocessableEntity,
#elif NET6_0
                "422" => HttpStatusCode.UnprocessableEntity,
#endif
                "default" => null,
                _ => (HttpStatusCode)Convert.ToInt16(statusCode)//throw new QAToolKitSwaggerException($"HttpStatusCode not found '{statusCode}'."),
            };
        }

        private string GetSummary(KeyValuePair<OperationType, OpenApiOperation> openApiOperation)
        {
            return openApiOperation.Value.Summary;
        }

        private string GetOperationId(KeyValuePair<OperationType, OpenApiOperation> openApiOperation)
        {
            return openApiOperation.Value.OperationId;
        }

        private IEnumerable<Parameter> GetParameters(KeyValuePair<OperationType, OpenApiOperation> openApiOperation)
        {
            var parameters = new List<Parameter>();

            foreach (var parameter in openApiOperation.Value.Parameters)
            {
                var param = new Parameter()
                {
                    Name = parameter.Name,
                    Type = parameter.Schema.Type,
                    Format = parameter.Schema.Format,
                    Nullable = parameter.Schema.Nullable,
                    Required = parameter.Required,
                    Location = GetPlacement(parameter.In.Value)
                };

                if (_swaggerOptions.UseSwaggerExampleValues &&
                    parameter.Example != null)
                    param.Value = SetValue(parameter.Example).ToString();


                parameters.Add(param);
            }

            return parameters;
        }

        private Location GetPlacement(ParameterLocation In)
        {
            return In switch
            {
                ParameterLocation.Query => Location.Query,
                ParameterLocation.Path => Location.Path,
                ParameterLocation.Header => Location.Header,
                ParameterLocation.Cookie => Location.Cookie,
                _ => throw new QAToolKitSwaggerException($"Invalid parameter location '{In}'."),
            };
        }

        private List<RequestBody> GetRequestBodies(KeyValuePair<OperationType, OpenApiOperation> openApiOperation,
            IDictionary<string, OpenApiRequestBody> schemas = null)
        {
            try
            {
                var requests = new List<RequestBody>();
                var schema = new KeyValuePair<string, OpenApiRequestBody>();

                if (openApiOperation.Value.RequestBody == null)
                {
                    return new List<RequestBody>();
                }

                if (openApiOperation.Value.RequestBody.Content.Count == 0)
                {
                    if (openApiOperation.Value.RequestBody.Reference == null)
                    {
                        return new List<RequestBody>();
                    }

                    if (schemas != null)
                    {
                        schema = schemas.FirstOrDefault(x => x.Key == openApiOperation.Value.RequestBody.Reference.Id);

                        foreach (var contentType in schema.Value.Content)
                        {
                            var requestBody = new RequestBody
                            {
                                Name = contentType.Value.Schema.Reference?.Id,
                                ContentType = contentType.Key,
                                Properties = null,
                                Required = openApiOperation.Value.RequestBody.Required
                            };

                            ExtractOpenApiSchema(contentType.Value.Schema.AllOf, requestBody);
                            ExtractOpenApiSchema(contentType.Value.Schema.AnyOf, requestBody);
                            ExtractOpenApiSchema(contentType.Value.Schema.OneOf, requestBody);
                            ExtractProperties(contentType, requestBody);

                            requests.Add(requestBody);
                        }
                    }
                }
                else
                {
                    //TODO: support other content types
                    foreach (var contentType in openApiOperation.Value.RequestBody.Content)
                    {
                        var requestBody = new RequestBody
                        {
                            Name = contentType.Value.Schema.Reference?.Id,
                            ContentType = contentType.Key,
                            Properties = null,
                            Required = openApiOperation.Value.RequestBody.Required
                        };

                        ExtractProperties(contentType, requestBody);

                        requests.Add(requestBody);
                    }
                }

                return requests;
            }
            catch
            {
                return new List<RequestBody>();
            }
        }

        private void ExtractProperties(KeyValuePair<string, OpenApiMediaType> contentType, RequestBody requestBody)
        {
            foreach (var property in contentType.Value.Schema.Properties)
            {
                var temp = GetPropertiesRecursively(property);
                if (temp != null)
                {
                    if (requestBody.Properties == null)
                        requestBody.Properties = new List<Property>();

                    requestBody.Properties.AddRange(temp);
                }
            }
        }

        private void ExtractOpenApiSchema(IList<OpenApiSchema> schemas, RequestBody requestBody)
        {
            foreach (var schema in schemas)
            {
                foreach (var property in schema.Properties)
                {
                    var temp = GetPropertiesRecursively(property);
                    if (temp != null)
                    {
                        if (requestBody.Properties == null)
                            requestBody.Properties = new List<Property>();

                        requestBody.Properties.AddRange(temp);
                    }
                }
            }
        }

        private List<Property> GetPropertiesRecursively(KeyValuePair<string, OpenApiSchema> source, int counter = 0)
        {
            var properties = new List<Property>();

            #region Items

            Property itemsProperty = null;

            if (source.Value.Items != null)
            {
                itemsProperty = new Property
                {
                    Description = source.Value.Items.Description,
                    Format = source.Value.Items.Format,
                    Type = source.Value.Items.Type,
                    Properties = null,
                    Name = source.Value.Items.Reference?.Id
                };
                itemsProperty.Required = source.Value.Items.Required.Contains(itemsProperty.Name);

                if (_swaggerOptions.UseSwaggerExampleValues)
                    itemsProperty.Value = SetValue(source.Value.Items.Example);

                foreach (var property in source.Value.Items.Properties)
                {
                    if (counter < 5)
                    {
                        var recursiveProperties = GetPropertiesRecursively(property, ++counter);
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
            }

            #endregion

            #region enums

            Property enumProperty = null;
            if (source.Value.Enum is { Count: > 0 })
            {
                enumProperty = new Property
                {
                    Description = null,
                    Format = null,
                    Type = "enum",
                    Properties = new List<Property>(),
                    Name = source.Value.Reference?.Id,
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
                Format = itemsProperty != null ? itemsProperty.Type : source.Value.Format,
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
                if (counter < 5)
                {
                    var propsTem = GetPropertiesRecursively(property, ++counter);

                    if (propsTem != null && prop.Properties == null)
                    {
                        prop.Properties = new List<Property>();
                    }

                    prop.Properties.AddRange(propsTem);
                }
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
                contentType.Key == ContentType.Json.Value()))
            {
                if (contentType.Value.Schema != null 
                    && contentType.Value.Schema.Items != null 
                    && (contentType.Value.Schema.Items.Type == "array" ||
                        contentType.Value.Schema.Items.Type != "object") &&
                        contentType.Value.Schema.Type == "array")
                {  
                    return ResponseType.Array;
                }
                else if (contentType.Value.Schema != null 
                         && contentType.Value.Schema.Items != null 
                         && (contentType.Value.Schema.Items.Type == "array" ||
                             contentType.Value.Schema.Items.Type == "object") &&
                             contentType.Value.Schema.Type == "array")
                {
                    return ResponseType.Objects;
                }
                else if (contentType.Value.Schema != null 
                         && contentType.Value.Schema.Items == null 
                         && contentType.Value.Schema.Type == "object")
                {
                    return ResponseType.Object;
                }
                else if (contentType.Value.Schema != null 
                         && contentType.Value.Schema.Items == null 
                         && (contentType.Value.Schema.Type != "object" ||
                             contentType.Value.Schema.Type != "array"))
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
                contentType.Key == ContentType.Json.Value()))
            {
                if (contentType.Value.Schema != null && contentType.Value.Schema.Properties != null &&
                    contentType.Value.Schema.Properties.Count > 0)
                {
                    foreach (KeyValuePair<string, OpenApiSchema> property in contentType.Value.Schema.Properties)
                    {
                        properties.AddRange(GetPropertiesRecursively(property));
                    }
                }

                if (contentType.Value.Schema != null && contentType.Value.Schema.AllOf != null &&
                    contentType.Value.Schema.AllOf.Count > 0)
                {
                    foreach (var schema in contentType.Value.Schema.AllOf)
                    {
                        foreach (var property in schema.Properties)
                        {
                            properties.AddRange(GetPropertiesRecursively(property));
                        }
                    }
                }

                if (contentType.Value.Schema != null && contentType.Value.Schema.AnyOf != null &&
                    contentType.Value.Schema.AnyOf.Count > 0)
                {
                    foreach (var schema in contentType.Value.Schema.AnyOf)
                    {
                        foreach (var property in schema.Properties)
                        {
                            properties.AddRange(GetPropertiesRecursively(property));
                        }
                    }
                }

                if (contentType.Value.Schema != null && contentType.Value.Schema.OneOf != null &&
                    contentType.Value.Schema.OneOf.Count > 0)
                {
                    foreach (var schema in contentType.Value.Schema.OneOf)
                    {
                        foreach (var property in schema.Properties)
                        {
                            properties.AddRange(GetPropertiesRecursively(property));
                        }
                    }
                }

                if (contentType.Value.Schema != null && contentType.Value.Schema.Items != null)
                {
                    if (contentType.Value.Schema.Items != null 
                        && contentType.Value.Schema.Items.Properties != null
                        && contentType.Value.Schema.Items.Properties.Count > 0)
                    {
                        foreach (KeyValuePair<string, OpenApiSchema> property in contentType.Value.Schema.Items
                            .Properties)
                        {
                            properties.AddRange(GetPropertiesRecursively(property));
                        }
                    }
                    else if (contentType.Value.Schema.Items != null 
                             && contentType.Value.Schema.Items.Properties != null
                             && contentType.Value.Schema.Items.Properties.Count == 0)
                    {
                        properties.Add(new Property()
                        {
                            Name = $"_{contentType.Value.Schema.Type}",
                            Format = contentType.Value.Schema.Items.Type,
                            Type = contentType.Value.Schema.Type
                        });
                    }
                }
            }

            return properties;
        }
    }
}