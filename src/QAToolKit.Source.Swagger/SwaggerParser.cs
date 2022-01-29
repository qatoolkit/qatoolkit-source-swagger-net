using System.IO;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Readers.Exceptions;
using Microsoft.OpenApi.Writers;
using QAToolKit.Source.Swagger.Exceptions;

namespace QAToolKit.Source.Swagger
{
    /// <summary>
    /// Swagger parser based on Microsoft OpenApi library
    /// </summary>
    public static partial class SwaggerParser
    {
        /// <summary>
        /// Parse Swagger document (JSON, YML) from stream to the OpenApiDocument
        /// </summary>
        /// <param name="sourceStream"></param>
        /// <param name="strictParsing"></param>
        /// <returns></returns>
        /// <exception cref="InvalidSwaggerException"></exception>
        /// <exception cref="UnsupportedSwaggerException"></exception>
        public static OpenApiDocument GenerateOpenApiDocument(Stream sourceStream, bool strictParsing)
        {
            OpenApiDocument openApiDocument;
            OpenApiSpecVersion openApiSpecVersion;
            try
            {
                openApiDocument = new OpenApiStreamReader().Read(sourceStream, out var diagnostic);

                if (strictParsing)
                {
                    if (diagnostic.Errors is { Count: > 0 })
                    {
                        throw new SwaggerValidationException("Swagger validation failed.", diagnostic.Errors);
                    }
                }

                openApiSpecVersion = diagnostic.SpecificationVersion;
            }
            catch (OpenApiUnsupportedSpecVersionException e)
            {
                throw new UnsupportedSwaggerException("Unsupported Swagger version.", e);
            }

            var writer = new OpenApiJsonWriter(new StringWriter());

            switch (openApiSpecVersion)
            {
                case OpenApiSpecVersion.OpenApi2_0:
                    openApiDocument.SerializeAsV2(writer);
                    break;
                case OpenApiSpecVersion.OpenApi3_0:
                    openApiDocument.SerializeAsV3(writer);
                    break;
                default:
                    throw new UnsupportedSwaggerException("Unsupported Swagger version.");
            }

            if (openApiDocument.IsOpenApiDocumentEmpty())
            {
                throw new InvalidSwaggerException("Swagger document is invalid, check the input.");
            }
            
            return openApiDocument;
        }

        private static bool IsOpenApiDocumentEmpty(this OpenApiDocument openApiDocument)
        {
            return openApiDocument?.Components == null || openApiDocument?.Paths == null;
        }
        
        /// <summary>
        /// Swagger types to QAToolKit Swagger types mapper
        /// </summary>
        /// <param name="swaggerType"></param>
        /// <param name="swaggerFormat"></param>
        /// <returns></returns>
        public static PropertyType MapFromSwaggerType(string swaggerType, string swaggerFormat)
        {
            return swaggerType switch
            {
                "string" when swaggerFormat == "binary" => PropertyType.Binary,
                "string" when swaggerFormat == "date-time" => PropertyType.DateTime,
                "string" when swaggerFormat == "date" => PropertyType.Date,
                "string" when swaggerFormat == "url" => PropertyType.Uri,
                "string" when swaggerFormat == "uri" => PropertyType.Uri,
                "string" when swaggerFormat == null => PropertyType.String,
                "string" => PropertyType.String,
                "boolean" => PropertyType.Boolean,
                "integer" when swaggerFormat == "int32" => PropertyType.Int32,
                "integer" when swaggerFormat == "int64" => PropertyType.Int64,
                "integer" => PropertyType.Int32,
                "object" => PropertyType.Object,
                "number" when swaggerFormat == null => PropertyType.Int32,
                "number" when swaggerFormat == "float" => PropertyType.Float,
                "number" when swaggerFormat == "double" => PropertyType.Double,
                "array" when swaggerFormat == "double" => PropertyType.DoubleArray,
                "array" when swaggerFormat == "float" => PropertyType.FloatArray,
                "array" when swaggerFormat == "int32" => PropertyType.Int32Array,
                "array" when swaggerFormat == "int64" => PropertyType.Int64Array,
                "array" when swaggerFormat == "date" => PropertyType.DateArray,
                "array" when swaggerFormat == "date-time" => PropertyType.DateTimeArray,
                "array" when swaggerFormat == "binary" => PropertyType.BinaryArray,
                "array" when swaggerFormat == "string" => PropertyType.StringArray,
                "array" when swaggerFormat == "boolean" => PropertyType.BooleanArray,
                "array" when swaggerFormat == "object" => PropertyType.ObjectArray,
                _ => PropertyType.Unknown
            };
        }
    }
}