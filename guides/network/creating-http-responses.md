# Creating HTTP responses

When you respond to a request (i.e. when you return from your delegate registered with `Handle.GET()`,`Handle.POST()` etc), you should return a `Response` object.

The `Response` class has many implicit cast operators to make this convenient.

* `string` (mime type will be text/plain or text/html depending on the request `Accept` header)
* `byte[]` (mime type will be the first one in the `Accept` header)
* `Json` object (mime type will be `application/json`)
* `int`, `uint`, `decimal`, `bool`, `double`, `long`, `ulong`, `DateTime` returns a Javascript literal (JSON)
* `null` (no content)

## Returning a Response object

When you create a Response object, you have the choice of setting the body to a `byte[]`, a `string`.

```cs
GET("/hello", () => {
    new Response() {
        ContentType="text/plain",
        Body="Hello World"
    };
});
```

## Returning a JSON object

If you return an instance of the `Json` class, the mime type will be `application/json` and the body will contain a JSON string.
```cs
GET("/hello", () => {
    return new PersonData() {
        FirstName="Joachim",
        LastName="Wester"
    };
});
```

## Returning a string

If you return a string, the returned mime type depends on the `Accept` header of the request. If the request prioritizes `text/html` or `application\json`, the HTTP response will use this type accordingly. If no Accept header was provided, the mime-type ```text/plain``` will be used.
```cs
GET("/hello", () => "Hello World" );
```

## Returning a Status Code and Status Description

Create a `Response` object:

```cs
GET("/hello", () => {
    return new Response() {
        StatusCode = 404,
        StatusDescription = "Not Found"
    };
});
```

If integer is returned from delegate that will result in automatic response with `StatusCode` equals to the integer and default status description.

## Returning null

When null is returned from the handler its equal to returning 404 `Not found` status code.

## Serving static resources

To resolve a static resource, there is a method `Handle.ResolveStaticResource` which takes a resource URI and incoming request and returns a response representing this resource. Response however can be a 404, so to return a "nice" 404 page user has to do the following code, for example:
```cs
Response resp = Handle.ResolveStaticResource(req.Uri, req);

if (404 == resp.StatusCode) {
    resp = Self.GET("/404.html");
}
return resp;
```
When codehost starts, Starcounter adds a static resource resolver on default user port (GET handler on URI "/{?}"). To register your own static resource resolver on default user port, you should supply `HandlerOptions.ReplaceExistingHandler` when registering such handler.

## Delayed or explicitly handled responses

Sometimes the `Response` object can not be returned immediately in the handler. One reason could be accessing third party resources, or doing long-running job. By returning `HandlerStatus.Handled` in the handler user indicates that response will be later or already returned by some other means.

For example:

```cs
Handle.GET("/postponed", (Request req) => {
    Http.POST("/posttest", "Here I do a post!", null, null, 
        (Response resp, Object userObject) => {
        
        // Modifying the response object by injecting some data.
        resp.Headers["MySuperHeader"] = "Here is my header value!";
        resp.Headers["Set-Cookie"] = "MySuperCookie=CookieValue";
        req.Response = resp;
    }); // "resp" object will be automatically sent when delegate exits.

    return HandlerStatus.Handled;
});
```

Please refer to [External HTTP calls and Node usage](/guides/network/external-http-calls-and-node-usage/) article for more information.

## Summary of adjustable Response fields

* `StatusCode`: 404, 501, etc
* `StatusDescription`: _"Not Found"_, _"Service Unavailable"_, etc
* `ContentType`: _"text/html"_, _"application/json"_, etc
* `ContentEncoding`: _"gzip"_, etc
* `Cookies`: _a list of entries like "MyCookie1=123", "MyCookie2=456"_, etc
* `Body`: _"Here is response body!"_, etc
* `BodyBytes`: _Byte[] bodyBytes = { 1, 2, 3, 4, 5};_, etc
* `ConnFlags`: used to manipulate the connection with client.

The following `Response.ConnectionFlags` values are available:
* `Response.ConnectionFlags.DisconnectImmediately`: immediately disconnects the associated connection with endpoint without sending any data first.
* `Response.ConnectionFlags.DisconnectAfterSend`: first sends given message to endpoint and then closes the corresponding connection.

Example:

```cs
Handle.GET(8080, "/shutdown", (Request req) => {
    return new Response() {
        Body = "Closing connection with you!",
        ConnFlags = Response.ConnectionFlags.DisconnectAfterSend
    };
});
```

Setting arbitrary HTTP headers on `Responce` object is straight forward using `Headers` property:

```cs
Handle.GET("/response1", () => {
    Response r = new Response() {
        StatusCode = 404,
        StatusDescription = "Not Found",
        ContentType = "text/html",
        ContentEncoding = "gzip",
        Cookies = { "MyCookie1=123", "MyCookie2=456" },
        Body = "response1"
    };

    r.Headers["MyHeader"] = "MyHeaderData";
    r.Headers["Location"] = "/newlocation";

    return r;
});
```

Remarks:

* `StatusCode` default value is 200
* `StatusDescription` default value is _"OK"_
* To access certain HTTP headers, use `Headers` accessor on `Response` object (same as for `Request`):

```cs
Response resp;
...
if ("SC" == resp.Headers["Server"])
  return resp.Headers["Accept-Ranges"];
```

Examples:

```cs
Handle.GET("/hello", () => {
  return new Response() {
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
 
When you create a Response object, you have the choice of setting the body to a `byte[]`, a `string` or a `Stream`.
As Starcounter schedules threads in a highly optimized way, it is recommended that you allow Starcounter to handle streaming. This is done by assigning you `stream` to the Response object and then returning the response, relying on Starcounter to read the stream and send the network traffic on its own accord. Streamed object should allow getting length of the stream in bytes (`Length` property).

```cs
GET("/movie", () => {
   FileStream stream = File.Open("bigfile.mpg", FileMode.Open, FileAccess.Read, FileShare.Read));
   Response r = new Response() {
	StreamedBody = stream,
	ContentType = "application/octet-stream"
   };
   return r;
});
```
Note that the stream object is automatically closed when the stream data is sent completely or if the connection is dropped.

## Setting properties on outgoing response

Sometimes being deep in call hierarchy of `Self.GET` its necessary to be able to set properties not on the current response, but directly on corresponding (but not yet available) outgoing response, like headers, status code and message, cookies. To achieve that, the following static methods are available on class `Handle`:

```cs
void AddOutgoingCookie(String cookieName, String cookieValue);
void AddOutgoingHeader(String headerName, String headerValue);
void SetOutgoingStatusDescription(String statusDescription);
void SetOutgoingStatusCode(UInt16 statusCode);
```