# QAToolKit.Source.Swagger
![.NET Core](https://github.com/qatoolkit/qatoolkit-source-swagger-net/workflows/.NET%20Core/badge.svg)

`QAToolKit.Source.Swagger` is a .NET standard library, which generates `IList<HttpTestRequest>` object that is the input for other components.

Major features:

- Parses `OpenAPI v3.0` Swagger files,
- swagger.json can be loaded from `disk` or from `URL`,
- path parameters, URL paramters and JSON body models are replaced with custom values,
- access swagger.json from URL, which is protected by `basic authentication`,
- control which swagger endpoints are called by specifying `request filters` (check below)
- data generation for missing values (Experimental)

## Sample

This is a sample of instantiating a new Swagger Source object from URL.

```csharp
// create new Swagger URL source
SwaggerUrlSource swaggerSource = new SwaggerUrlSource(
    options =>
    {
        options.AddReplacementValues(new ReplacementValue[] {
        new ReplacementValue()
            {
                Key = "version",
                Value = "1"
            },
            {
                Key = "parentId",
                Value = "4"
            }
        });
        options.AddBasicAuthentication("myuser", "mypassword");
        options.AddRequestFilters(new RequestFilter()
        {
            AuthenticationTypes = new List<AuthenticationType>() { AuthenticationType.Customer },
            TestTypes = new List<TestType>() { TestType.LoadTest },
            EndpointNameWhitelist = new string[] { "GetCategories" }
        });
        options.AddBaseUrl(new Uri("https://dev.myapi.com"));
        options.AddDataGeneration();
    });

//To run the Swagger parser we need to pass an array of URLs
IList<HttpTestRequest> requests = await swaggerSource.Load(new Uri[] {
    new Uri("https://api.demo.com/swagger/v1/swagger.json"),
    new Uri("https://api2.demo.com/swagger/v1/swagger.json")
});
```

The above code is quite simple, but it needs some explanation.

## Description

#### 1. AddReplacementValues
This is a method that will add key/value pairs for replacement values you need to replace in the Swagger requests.

In the example above we say: "Replace `{version}` placeholder in Path and URL parameters and JSON body models."

In other words, if you have a test API endpoint like this: https://api.demo.com/v{version}/categories?parent={parentId} that will be set to https://api.demo.com/v1/categories?parent=4.

That, does not stop there, you can also populate JSON request bodies in this way:

[TODO sample JSON Body]

`ReplcementValue[]` has precedence over data generation (check below chapter 5).

#### 2. AddBasicAuthentication
If your Swagger.json files are protected by basic authentication, you can set those with `AddBasicAuthentication`.

#### 3, AddRequestFilters
Filters comprise of different types. Those are `AuthenticationTypes`, `TestTypes` and `EndpointNameWhitelist`.

##### 3.1. AuthenticationTypes
Here we specify a list of Authentication types, that will be filtered out from the whole swagger file. This is where QA Tool Kit presents a convention.
The built-in types are:
- `AuthenticationType.Customer` which specifies a string `"@customer"`,
- `AuthenticationType.Administrator` which specifies a string `"@administrator"`,
- `AuthenticationType.Oauth2` which specifies a string `"@oauth2"`,
- `AuthenticationType ApiKey` which specifies a string `"@apikey"`,
- `AuthenticationType.Basic` which specifies a string `"@basic"`.

In order to apply filters, you need to tag your API endpoints with those strings.

We normally do it in Swagger endpoint description. An example might be: `Get categories from the system. @customer,@administrator,@oauth2.`

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
Similarly as in the `AuthenticationTypes` you can filter out certain endpoints to be used in certain test scenarios. Currently libraray supports:

- TestType.LoadTest which specifies a string `"@loadtest"`,
- TestType.IntegrationTest which specifies a string `"@integrationtest"`,
- TestType.SecurityTest which specifies a string `"@securitytest"`,

The same swagger-json excerpt which allows load and integration tests.

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

##### 3.3 EndpointNameWhitelist
Final `RequestFilter` option is `EndpointNameWhitelist`. With it you can specify a list of endpoints that will be included in the results.

Every other endpoint will be excluded. In the sample above we have set the result to include only `GetCategories` endpoint. 
That corresponds to the `operationId` in the swagger file above.

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

#### 5. AddDataGeneration
##### !! EXPERIMENTAL !!
This is an experimental feature. It will generate the missing data in the `List<HttpTestRequest>` object from the swagger models, uri and query parameters.
`ReplcementValue[]` has precedence over data generation.

## Limitations

- Swagger processor only returns `application/json` content type. Support for other might come later.
- Data generation and replacement is only supported for Path, Url and Json Body properties.

## License

MIT License

Copyright (c) 2020 Miha Jakovac

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