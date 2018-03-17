# Creating HTTP Responses

When responding to a request from a handler such as `Handle.GET`, a `Response` object should be returned.

The `Response` class has many implicit cast operators to make this convenient.

* `string` \(mime type will be text/plain or text/html depending on the request `Accept` header\)
* `byte[]` \(mime type will be the first one in the `Accept` header\)
* `Json` object \(mime type will be `application/json`\)
* `int`, `uint`, `decimal`, `bool`, `double`, `long`, `ulong`, `DateTime` returns a Javascript literal \(JSON\)
* `null` \(no content\)

## Returning a Response object

When creating a `Response` object, you have the choice of setting the body to a `byte[]`, a `string`.

```csharp
Handle.GET("/hello", () =>
{
    new Response()
    {
        ContentType = "text/plain",
        Body = "Hello World"
    };
});
```

## Returning a JSON object

When returning an instance of the `Json` class, the mime type will `application/json` and the body will contain a JSON string.

```csharp
Handle.GET("/hello", () =>
{
    return new PersonData()
    {
        FirstName = "Joachim",
        LastName = "Wester"
    };
});
```

## Returning a string

When returning a string, the returned mime type depends on the `Accept` header of the request. If the request prioritizes `text/html` or `application\json`, the HTTP response will use this type accordingly. If no Accept header was provided, the mime-type `text/plain` will be used.

```csharp
Handle.GET("/hello", () => "Hello World" );
```

## Returning a Status Code and Status Description

Create a `Response` object:

```csharp
GET("/hello", () =>
{
    return new Response()
    {
        StatusCode = 404,
        StatusDescription = "Not Found"
    };
});
```

If an integer is returned from a delegate that will result in automatic response with a `StatusCode` equal to the integer and default status description.

## Returning null

When null is returned from the handler, it's equal to returning the 404 `Not found` status code.

## Serving static resources

To resolve a static resource, there is a method `Handle.ResolveStaticResource` which takes a resource URI and incoming request and returns a response representing this resource. Response however can be a 404, so to return a "nice" 404 page user has to do the following code, for example:

```csharp
Response resp = Handle.ResolveStaticResource(req.Uri, req);

if (404 == resp.StatusCode)
{
    resp = Self.GET("/404.html");
}
return resp;
```

When the codehost starts, Starcounter adds a static resource resolver on the default user port \(GET handler on URI "/{?}"\).

## Delayed or explicitly handled responses

Sometimes, the `Response` object cannot be returned immediately in the handler. One reason could be the access to third party resources or performing long-running jobs. By returning `HandlerStatus.Handled` in the handler, the user indicates that the response will be returned later or that it already has been returned. To accomplish such behaviour there is a special method on `Request`: `void SendResponse(Response resp)` which sends given `Response` object. Sending response using `Request.SendResponse` must be done on Starcounter scheduler. Since HTTP is a request-response protocol, only one response can be send per request \(in comparison with WebSocket protocol\).

For example:

```csharp
Handle.GET("/postponed", (Request req) =>
{
    // Scheduling some job here, which will send the response.
    Scheduling.ScheduleTask(() => {
        // Do something external, like accessing third party resources or
        // performing long-running jobs.
        ...
        // Now send the response for the original "/postponed" request.
        req.SendResponse(new Response { BodyBytes = myBodyData });
    });

    // Immediately returning from the HTTP handler.
    // The actual response will be sent later.
    return HandlerStatus.Handled;
});
```

Please refer to the [External HTTP calls and Node usage](external-http-calls-and-node-usage.md) article for more information.

## Summary of adjustable Response fields

* `StatusCode`: 404, 501, etc
* `StatusDescription`: _"Not Found"_, _"Service Unavailable"_, etc
* `ContentType`: _"text/html"_, _"application/json"_, etc
* `ContentEncoding`: _"gzip"_, etc
* `Cookies`: _a list of entries like "MyCookie1=123", "MyCookie2=456"_, etc
* `Body`: _"Here is response body!"_, etc
* `BodyBytes`: _Byte\[\] bodyBytes = { 1, 2, 3, 4, 5};_, etc
* `ConnFlags`: used to manipulate the connection with client.

The following `Response.ConnectionFlags` values are available:

* `Response.ConnectionFlags.DisconnectImmediately`: immediately disconnects the associated connection with endpoint without sending any data first.
* `Response.ConnectionFlags.DisconnectAfterSend`: first sends given message to endpoint and then closes the corresponding connection.

Example:

```csharp
Handle.GET(8080, "/shutdown", (Request req) =>
{
    return new Response()
    {
        Body = "Closing connection with you!",
        ConnFlags = Response.ConnectionFlags.DisconnectAfterSend
    };
});
```

Setting arbitrary HTTP headers on a `Responce` object is straightforward using the `Headers` property:

```csharp
Handle.GET("/response", () =>
{
    var response = new Response()
    {
        StatusCode = 404,
        StatusDescription = "Not Found",
        ContentType = "text/html",
        ContentEncoding = "gzip",
        Cookies = { "MyCookie1=123", "MyCookie2=456" },
        Body = "response1"
    };

    response.Headers["MyHeader"] = "MyHeaderData";
    response.Headers["Location"] = "/newlocation";

    return response;
});
```

Its recommended to encode complex HTTP header values \(for example, containing line separators\) using `base64` or similar encoding.

Remarks:

* `StatusCode` default value is 200
* `StatusDescription` default value is _"OK"_
* `Server` default value is "Server: Starcounter/\#starcounter\_version \(Windows\)", for example: `Server: Starcounter/2.3.1.7779 (Windows)`.
* To access certain HTTP headers, use the `Headers` accessor on the `Response` or `Request` object:

```csharp
Response response;
...
if ("SC" == response.Headers["Server"])
{
  return response.Headers["Accept-Ranges"];  
}
```

Examples:

```csharp
Handle.GET("/hello", () =>
{
  return new Response()
  {
    StatusCode = 404,
    StatusDescription = "Not Found",
    ContentType = "text/html",
    ContentEncoding = "gzip",
    Cookies = { "MyCookie1=123", "MyCookie2=456" },
    Body = "response1"
  };
});
```

## Returning a streamed body

When creating the `Response` object, the body can be set to a `byte[]`, `string`, or `Stream`.  
As Starcounter schedules threads in a optimized way, it is recommended to allow Starcounter to handle streaming. This is done by assigning the `stream` to the `Response` object and then returning the response, relying on Starcounter to read the stream and send the network traffic on its own accord. Streamed object should allow getting length of the stream in bytes \(`Length` property\).

```csharp
Handle.GET("/movie", () =>
{
   FileStream stream = File.Open("bigfile.mpg", FileMode.Open, FileAccess.Read, FileShare.Read));
   var response = new Response()
   {
        StreamedBody = stream,
        ContentType = "application/octet-stream"
   };
   return response;
});
```

Note that the `stream` object is automatically closed when the stream data is sent completely or if the connection is dropped.

## Setting properties on outgoing response

Sometimes, when deep in the call hierarchy of `Self.GET`, it's necessary to be able to set properties not on the current response, but directly on corresponding, not yet available, outgoing responses, like headers, status codes, messages, and cookies. To achieve that, the following static methods are available from the class `Handle`:

```csharp
void AddOutgoingCookie(String cookieName, String cookieValue);
void AddOutgoingHeader(String headerName, String headerValue);
void SetOutgoingStatusDescription(String statusDescription);
void SetOutgoingStatusCode(UInt16 statusCode);
```

