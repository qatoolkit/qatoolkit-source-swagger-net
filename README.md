# QAToolKit.Source.Swagger
![.NET Core](https://github.com/qatoolkit/qatoolkit-source-swagger-net/workflows/.NET%20Core/badge.svg)

`QAToolKit.Source.Swagger` is a .NET standard library, which generates `IList<HttpRequest>` object that is the input for other components.

Major features:

- Parses `OpenAPI v3.0` Swagger files,
- swagger.json can be loaded from `disk` or from `URL`,
- access swagger.json from URL, which is protected by `basic authentication`,
- control which swagger endpoints are returned by specifying `request filters` (check below)

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
            AuthenticationTypes = new List<AuthenticationType>() { AuthenticationType.Customer },
            TestTypes = new List<TestType>() { TestType.LoadTest },
            EndpointNameWhitelist = new string[] { "GetCategories" }
        });
        options.AddBaseUrl(new Uri("https://dev.myapi.com"));
        options.UseSwaggerExampleValues = true;
    });

//To run the Swagger parser we need to pass an array of URLs
IList<HttpRequest> requests = await swaggerSource.Load(new Uri[] {
    new Uri("https://api.demo.com/swagger/v1/swagger.json"),
    new Uri("https://api2.demo.com/swagger/v1/swagger.json")
});
```

The above code is quite simple, but it needs some explanation.

## Description

#### 1. AddBasicAuthentication
If your Swagger.json files are protected by basic authentication, you can set those with `AddBasicAuthentication`.

#### 2. AddRequestFilters
Filters comprise of different types. Those are `AuthenticationTypes`, `TestTypes` and `EndpointNameWhitelist`. All are optional.

##### 2.1. AuthenticationTypes
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

##### 2.2 TestTypes
Similarly as in the `AuthenticationTypes` you can filter out certain endpoints to be used in different test scenarios. Currently library supports:

- TestType.LoadTest which specifies a string `"@loadtest"`,
- TestType.IntegrationTest which specifies a string `"@integrationtest"`,
- TestType.SecurityTest which specifies a string `"@securitytest"`,

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

##### 2.3 EndpointNameWhitelist
Final `RequestFilter` option is `EndpointNameWhitelist`. With it you can specify a list of endpoints that will be included in the results.

Every other endpoint will be excluded. In the sample above we have set the result to include only `GetCategories` endpoint. 
That corresponds to the `operationId` in the swagger file above.

#### 3. AddBaseUrl
Your swagger file has a `Server section`, where you can specify an server URI and can be absolute or relative. An example of relative server section is:
```json
"servers": [
    {
        "url": "/api/v3"
    }
],
```
In case of relative paths you need to add an absolute base URL to `Swagger Processor` with `AddBaseUrl`, otherwise the one from the `Servers section` will be used.

#### 4. UseSwaggerExampleValues
You can set `UseSwaggerExampleValues = true` in the SwaggerOptions when creating new Swagger source object. This will
check Swagger for example files and populate those.

By default this option is set to false.

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