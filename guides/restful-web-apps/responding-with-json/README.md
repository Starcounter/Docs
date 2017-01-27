# Responding with JSON

Typed JSON objects are serialized automatically to `application/json` format when returned from request a handler.

<div class="code-name">PersonJson.json</div>

```json
{
    "FirstName": "Jocke",
    "LastName": "Wester"
}
```

<div class="code-name">Hello.cs</div>

```cs
using Starcounter;

class Hello
{
    static void Main()
    {
        Handle.GET("/hello", () =>
        {
            var json = new PersonJson();
            return json;
        });         
    }
}
```

The default HTTP status code for responses is 200 OK.

To achieve a different HTTP status code, define it explicitly in a `Response` object:

<div class="code-name">ErrorJson.json</div>

```json
{
    "ErrorDescription": ""
}
```

<div class="code-name">Hello.cs</div>

```cs
using Starcounter;

class Hello
{
    static void Main()
    {
        Handle.GET("/hello", () =>
        {
            var json = new ErrorJson()
            {
              ErrorDescription = "Hello not found"
            };

            return new Response()
            {
                StatusCode = 404,
                StatusDescription = "Not Found"
                Body = json
            };
        });         
    }
}
```

> Browse the guide to find more information about [JSON](/guides/typed-json/) and [Response](/guides/network/handling-http-requests)
