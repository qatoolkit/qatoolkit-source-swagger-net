# QAToolKit.Source.Swagger
[![Build .NET Library](https://github.com/qatoolkit/qatoolkit-source-swagger-net/workflows/.NET%20Core/badge.svg)](https://github.com/qatoolkit/qatoolkit-source-swagger-net/actions)
[![CodeQL](https://github.com/qatoolkit/qatoolkit-source-swagger-net/workflows/CodeQL%20Analyze/badge.svg)](https://github.com/qatoolkit/qatoolkit-source-swagger-net/security/code-scanning)
[![Sonarcloud Quality gate](https://github.com/qatoolkit/qatoolkit-source-swagger-net/workflows/Sonarqube%20Analyze/badge.svg)](https://sonarcloud.io/dashboard?id=qatoolkit_qatoolkit-source-swagger-net)
[![NuGet package](https://img.shields.io/nuget/v/QAToolKit.Source.Swagger?label=QAToolKit.Source.Swagger)](https://www.nuget.org/packages/QAToolKit.Source.Swagger/)
[![Discord](https://img.shields.io/discord/787220825127780354?color=%23267CB9&label=Discord%20chat)](https://discord.gg/hYs6ayYQC5)

## Description
`QAToolKit.Source.Swagger` is a .NET library, which generates `IEnumerable<HttpRequest>` object that is the input for other components.

Supported .NET frameworks and standards: `netstandard2.0`, `netstandard2.1`, `net7.0`

Major features:

- Parses `OpenAPI v3.0` Swagger files (JSON or YAML),
- swagger.json can be loaded from `disk` or from `URL`,
- access swagger.json from URL, which is protected by `basic authentication`,
- control which swagger endpoints are returned by specifying `request filters` (check below)

Get in touch with me on:

[![Discord](https://img.shields.io/discord/787220825127780354?color=%23267CB9&label=Discord%20chat)](https://discord.gg/hYs6ayYQC5)

## Video material

1. Introduction to Swagger Library

[![Start with Swagger Library](https://i9.ytimg.com/vi/EhQQMNZbwVY/maxresdefault.jpg?time=1607952000000&sqp=CIDN3f4F&rs=AOn4CLDcqo7vQ9jLTzLgCj2YdrPFPdVqZQ)](https://www.youtube.com/watch?v=EhQQMNZbwVY "Start with Swagger Library")

2. Swagger library request filters explained

[![Swagger library request filters](https://i9.ytimg.com/vi/-1hBRJLZcjY/maxresdefault.jpg?time=1607952000000&sqp=CIDN3f4F&rs=AOn4CLCJYrFjyO44DFjlTY1v6MlGe_hJRw)](https://www.youtube.com/watch?v=-1hBRJLZcjY "Swagger library request filters")

## Sample

This is a sample of instantiating a new Swagger Source object from URL.

```csharp
// create new Swagger URL source
SwaggerUrlSource swaggerSource = new SwaggerUrlSource(
    options =>
    {
        options.AddBasicAuthentication("myuser", "mypassword");
        options.AddRequestFilters(new RequestFilter()
        {
            AuthenticationTypes = new List<AuthenticationType.Enumeration>() { AuthenticationType.Enumeration.Customer },
            TestTypes = new List<TestType.Enumeration>() { TestType.Enumeration.LoadTest },
            EndpointNameWhitelist = new string[] { "GetCategories" },
            HttpMethodsWhitelist = new List<HttpMethod>() { HttpMethod.Put, HttpMethod.Post, HttpMethod.Get },
            GeneralContains = new string[] { "bicycle" }
        });
        options.AddBaseUrl(new Uri("https://dev.myapi.com"));
        options.UseSwaggerExampleValues = true;
    });

//To run the Swagger parser we need to pass an array of URLs
IEnumerable<HttpRequest> requests = await swaggerSource.Load(new Uri[] {
    new Uri("https://api.demo.com/swagger/v1/swagger.json"),
    new Uri("https://api2.demo.com/swagger/v1/swagger.json")
});
```

The above code is quite simple, but it needs some explanation.

## Description

#### 1. AddBasicAuthentication
If your Swagger.json files are protected by basic authentication, you can set those with `AddBasicAuthentication`.

#### 2. AddNTLMAuthentication
If your Swagger.json files are protected by Windows (NTLM) authentication, you can add it with `AddNTLMAuthentication`. There are two overrides which you can use.

```csharp
SwaggerUrlSource swaggerSource = new SwaggerUrlSource(
    options =>
    {
        options.AddNTLMAuthentication("myuser", "mypassword");
    ....

//or use default security context (logged in user)

SwaggerUrlSource swaggerSource = new SwaggerUrlSource(
    options =>
    {
        options.AddNTLMAuthentication();
    ...
```

#### 3. AddRequestFilters
Filters comprise of different types. Those are `AuthenticationTypes`, `TestTypes` and `EndpointNameWhitelist`. All are optional.

##### 3.1. AuthenticationTypes
Here we specify a list of Authentication types, that will be filtered out from the whole swagger file. This is where QA Tool Kit presents a convention.
The built-in types are:
- `AuthenticationType.Customer` which specifies a string `"@customer"`,
- `AuthenticationType.Administrator` which specifies a string `"@administrator"`,
- `AuthenticationType.Oauth2` which specifies a string `"@oauth2"`,
- `AuthenticationType ApiKey` which specifies a string `"@apikey"`,
- `AuthenticationType.Basic` which specifies a string `"@basic"`.

In order to apply filters, you need to tag your API endpoints with those strings.

We normally do that, by adding the tags in the Swagger endpoint description. An example might be: `Get categories from the system. @customer,@administrator,@oauth2.`

This is an example from swagger.json excerpt:

```json
"/v{version}/categories?parent={parentId}": {
    "get": {
        "tags": [
            "My endpoints"
        ],
        "summary": "Get categories",
        "description": "Get Categories, optionally filtered by parentId. TEST TAGS -> [@customer,@administrator,@oauth2]",
        "operationId": "GetCategories",
```

Parser then finds those string in the description field and populates the `RequestFilter` property.

##### 3.2 TestTypes
Similarly as in the `AuthenticationTypes` you can filter out certain endpoints to be used in different test scenarios. Currently library supports:

- TestType.LoadTest which specifies a string `"@loadtest"`,
- TestType.IntegrationTest which specifies a string `"@integrationtest"`,
- TestType.SecurityTest which specifies a string `"@securitytest"`

The same swagger.json excerpt which support test type tags might look like this:

```json
"/v{version}/categories?parent={parentId}": {
    "get": {
        "tags": [
            "My endpoints"
        ],
        "summary": "Get categories",
        "description": "Get Categories, optionally filtered by parentId. TEST TAGS -> [@loadtest,@integrationtest,@customer,@administrator,@oauth2]",
        "operationId": "GetCategories",
```

If you feed the list of `HttpRequest` objects with load type tags to the library like `QAToolKit.Engine.Bombardier`, only those requests will be tested.

##### 3.3 EndpointNameWhitelist
Another `RequestFilter` option is `EndpointNameWhitelist`. You can specify a list of endpoints that will be included in the results.

Every other endpoint will be excluded from the results. In the sample above we have set the result to include only `GetCategories` endpoint. 
That corresponds to the `operationId` in the swagger file above.

##### 3.4 HttpMethodsWhitelist
`HttpMethodsWhitelist` is a list of HTTP Methods that will be used in the `RequestFilter`.

Every other endpoint, that does not have a HTTP method that is in the `HttpMethodsWhitelist` will be excluded from the results.

##### 3.5 GeneralContains
Final `RequestFilter` option is `GeneralContains`, which is an array of strings.

Every other endpoint, that does not contain an array from the list will be excluded from the results. Every string in the array is compared with Swagger `Description`, `Summary` or `Tags` and is case insensitive.

#### 4. AddBaseUrl
Your swagger file has a `Server section`, where you can specify an server URI and can be absolute or relative. An example of relative server section is:
```json
"servers": [
    {
        "url": "/api/v3"
    }
],
```
In case of relative paths you need to add an absolute base URL to `Swagger Processor` with `AddBaseUrl`, otherwise the one from the `Servers section` will be used.

#### 5. UseSwaggerExampleValues
You can set `UseSwaggerExampleValues = true` in the SwaggerOptions when creating new Swagger source object. This will
check Swagger for example files and populate those.

By default this option is set to false.

## To-do

- **This library is an early alpha version**
- Swagger processor only returns `application/json` content type. Support for other might come later.
- Data generation and replacement is only supported for Path, Url and Json Body properties.

## License

MIT License

Copyright (c) 2020-2023 Miha Jakovac

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.