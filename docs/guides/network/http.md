# HTTP

## Introduction

There are two ways of doing routing in Starcounter:

1. With the built-in `Handle` API that handles HTTP
2. With the routing API found in the Authorization library

In most cases, it's recommended to use the API in the [Authorization library](https://github.com/Starcounter/authorization#routing-middleware-and-context---concepts) because of its ease of use. Although, there might be some cases where the `Handle` API is needed.  
This page describes the built-in `Handle` API

## Requests

### Catching Incoming Requests

Incoming HTTP 1.0/1.1 requests are caught using the static `Handle` class.

```csharp
Handle.GET("/hello", () =>
{
    return "Hello World";
});
```

Handlers can be registered anywhere and at any time. Though, in most cases, they are registered in the `Main` method

### Catching Common HTTP Verbs

The basic HTTP methods `GET`, `POST`, `PUT`, `DELETE` and `PATCH` can be caught by methods with the same name in the `Handle` class.

```csharp
Handle.GET("/hello", () =>
{
    return "Hello World";
});

Handle.POST("/hello", () =>
{
    return 500;
});

Handle.DELETE("/hello", () =>
{
    return 500;
});
```

### Accepting Parameters in Requests

When matching incoming requests, some parts of the URI may contain dynamic data. This is handled by Starcounter by allowing you to define parameters in handlers. This is done by marking the dynamic part of the URI template with curly braces. The simplest use of the curly brace syntax is a single question mark `{?}`. This indicates that there is a fragment of dynamic data in the URI. The type of the data is determined by the code delegate that follows.

```csharp
Handle.GET("/hello/{?}", (string name) => 
{         
    return "Hello " + name;
});
```

In the above example, the delegate accepts `string name`. This means that the parameter will be parsed as a string, for example: `/hello/albert`, `/hello/anna`. To accept an integer parameter, simply change the lambda parameter type.

```csharp
Handle.GET("/squared?{?}", (int num) => 
{         
    return num + " squared equals " + num * num;
});
```

The accepted URIs would, for example, be `/squared?123` and `/squared?-4321`

To accept multiple dynamic fragments, add more curly braces. For each dynamic parameter there should be a parameter in the delegate. They are enumerated from left to right, so be careful to put the parameters in the right order.

```csharp
Handle.GET("/{?}/{?}", (string list, int item) => 
{         
    return "List is " + list + " and item is " + item;
});
```

The accepted URIs would be, for example: `/serialnumbers/4534123`, `/itemid/34321`

#### Database object as parameter in handler

One can also expect a database object as a parameter to a handler:

```csharp
[Database]
public class Person
{
    public string Name { get; set; };
}
```

```csharp
Handle.GET("/People/{?}", (Person person) => person.Name);
```

Such handler can be called with database object ID as a parameter or using special `Self.GET` variant, which takes database class instance as a second parameter:

```csharp
var person = Db.SQL<Person>(...).FirstOrDefault();
Response response = Self.GET("/People/{?}", person);
```

The handler above can be also called with object ID as parameter:

```csharp
string objectId = DbHelper.GetObjectID(c);
Response response = Self.GET("/People/" + objectId);
```

### Catching Other Verbs

The `CUSTOM` verb in the `Handle` class makes it possible to register other HTTP methods or even catch all methods and URIs.

```csharp
Handle.CUSTOM("REPORT /hello/{?}", (string p1) =>
{
    return 500;
});

Handle.CUSTOM("{?} /hello/{?}", (string method, string p1) =>
{
    return p1;
});

Handle.CUSTOM("OPTIONS", "/hello/{?}", (string p1) =>
{
    return p1;
});

Handle.CUSTOM("{?}", (string methodAndUri) =>
{
    return "Caught: " + methodAndUri;
});
```

### The `Request` Object

A `Request` parameter can be declared together with the enumerated parameters. It encapsulates the entire request.

```csharp
Handle.GET("/hello", (Request request) =>
{
    return 500;
});

Handle.GET("/persons/{?}", (string name, Request request) =>
{
    return 500;
});
```

To access certain request HTTP headers, use `Headers[String]` accessor on a `Request` object \(same as for the `Response` object\):

```csharp
string mySuperHeader = req.Headers["MySuperHeader"];
string allRequestCookies = req.Headers["Set-Cookie"];
```

Request cookies are accessible from `Cookies` as a list of strings "name=value" \(same as for Response object\):

```csharp
List<String> allRequestCookies = req.Cookies;
```

To obtain client IP address, use `GetClientIpAddress()` on the `Request` object.

Use the `HandlerAppName` property to find out which application the request belongs to. This might be useful when working with request filters.

### Handler Options

When creating \(using the `Handle` interface\) and calling handlers \(using the `Self` interface\), one can supply last `HandlerOptions` parameter, which specifies certain options for handler calls or registration. Here are the notable handler options:

* `SkipRequestFilters`: used to declare a handler for which request filters will not be applied.
* `SkipResponsetFilters`: used to declare a handler for which response filters will not be applied.
* `SkipHandlersPolicy`: If the database flag "EnforceURINamespaces" is set to True, all application handlers are required to start with application name. In this case `SkipHandlersPolicy` flag allows to register any URI handler.
* `SelfOnly`: registered handler is going to be accessible only inside codehost using `Self` interface. `SelfOnly` handlers are not registered in gateway, in comparison with normal handlers.

**Examples**:

Registering a handler that skips request filters:

```csharp
Handle.POST("/myhandler", (Request request) =>
{
    return 204;
}, new HandlerOptions() { SkipRequestFilters = true });
```

### Exception Propagation Within Applications

Internal requests are requests made to handlers within same user application \(`sccode` instance\) using `Node` and `X`. Internal requests and handlers can be nested and create a call hierarchy. Sometimes its useful to cast a specific exception deep down in the hierarchy and handle it on another level or let the system handle it \(for example by automatically sending the response\). This can be achieved using `ResponseException` exception object. The following example illustrates this concept:

```csharp
Handle.GET("/exc1", (Request req) =>
{
    Response resp = Self.GET("/exc2");
    return resp;
});

Handle.GET("/exc2", (Request req) =>
{
    try
    {
        Response resp = Self.GET("/exc3");
        return resp;
    }
    catch (ResponseException exc)
    {
        exc.ResponseObject.StatusDescription = "Modified!";
        exc.ResponseObject.Headers["MyHeader"] = "Super value!";
        exc.UserObject = "My user object!";
        throw exc;
    }
});

Handle.GET("/exc3", (Request req) =>
{
    Response resp = new Response()
    {
        StatusCode = 404,
        StatusDescription = "Not found!"
    };

    throw new ResponseException(resp);
});
```

Handler `/exc3` constructs and throws an instance of `ResponseException` exception. Handler `/exc2` catches the exception, modifies some data and re-throws the exception. Eventually, the `ResponseException` is caught by outer system handler and `ResponseObject` is automatically sent on the original `Request req`. Note that `ResponseException` mechanisms are working only within one user application \(they are simple C\# exceptions\).

User can attach an arbitrary user object to `ResponseException` by either constructor or `UserObject` property.

### Unregistering HTTP Handlers

After an HTTP handler is created - it can be unregistered with the `Handle.UnregisterHttpHandler` method.

### Size Limit on Payloads

The default limit on payloads in requests is 1048576 bytes, exceeding this limit will prevent the request from going through and this warning will be printed in the [Administrator log](../working-with-starcounter/administrator-web-ui.md#log):

> Attempt to HTTP upload of more than 1048576 bytes. Closing socket connection

The limit can be increased to a maximum of 2048576 bytes by changing the value of `MaximumReceiveContentLength` in `%userprofile%\Documents\Starcounter\Personal\scnetworkgateway.xml`.

{% hint style="info" %}
When sending large files, we recommend to use WebSocket instead of HTTP. That pattern is demonstrated in the `FileUploadPage` \([code-behind](https://github.com/Starcounter/KitchenSink/blob/master/src/KitchenSink/FileUploadPage.json.cs), [HTML](https://github.com/Starcounter/KitchenSink/blob/master/src/KitchenSink/wwwroot/KitchenSink/FileUploadPage.html)\) in the sample app [KitchenSink](https://github.com/Starcounter/KitchenSink).
{% endhint %}

## Responses

When responding to a request from a handler such as `Handle.GET`, a `Response` object should be returned.

The `Response` class has many implicit cast operators to make this convenient.

* `string` \(mime type will be text/plain or text/html depending on the request `Accept` header\)
* `byte[]` \(mime type will be the first one in the `Accept` header\)
* `Json` object \(mime type will be `application/json`\)
* `int`, `uint`, `decimal`, `bool`, `double`, `long`, `ulong`, `DateTime` returns a Javascript literal \(JSON\)
* `null` \(no content\)

### Returning Different Types

#### Response Object

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

#### JSON Object

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

#### String

When returning a string, the returned mime type depends on the `Accept` header of the request. If the request prioritizes `text/html` or `application\json`, the HTTP response will use this type accordingly. If no Accept header was provided, the mime-type `text/plain` will be used.

```csharp
Handle.GET("/hello", () => "Hello World" );
```

#### Status Code and Status Description

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

#### Null

When null is returned from the handler, it's equal to returning the 404 `Not found` status code.

#### Streamed Body

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

### Serving Static Resources

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

### Delayed or Explicitly Handled Responses

Sometimes, the `Response` object cannot be returned immediately in the handler. One reason could be the access of third party resources or doing long-running jobs. By returning `HandlerStatus.Handled` in the handler, the user indicates that the response will be returned later or that it already has been returned another way.

For example:

```csharp
Handle.GET("/postponed", (Request req) =>
{
    Http.POST("/posttest", "Here I do a post!", null, null, (Response resp, Object userObject) =>
    {
        // Modifying the response object by injecting some data.
        resp.Headers["MySuperHeader"] = "Here is my header value!";
        resp.Headers["Set-Cookie"] = "MySuperCookie=CookieValue";
        req.Response = resp;
    }); // "resp" object will be automatically sent when delegate exits.
    return HandlerStatus.Handled;
});
```

Please refer to the [External HTTP calls and Node usage](http.md) article for more information.

### Summary of Adjustable Response Fields

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

Setting arbitrary HTTP headers on a `Response` object is straightforward using the `Headers` property:

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

Remarks:

* `StatusCode` default value is 200
* `StatusDescription` default value is _"OK"_
* `Server` default value is `Server: Starcounter/#starcounter_version (Windows)`, for example: `Server: Starcounter/2.3.1.7779 (Windows)`.
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

{% hint style="info" %}
Encode complex HTTP header values, such as those containing line separators, using base64 or similar encoding to reduce the chance of headers that are parsed the wrong way.
{% endhint %}

### Setting Properties on Outgoing Response

Sometimes, when deep in the call hierarchy of `Self.GET`, it's necessary to be able to set properties not on the current response, but directly on corresponding, not yet available, outgoing responses, like headers, status codes, messages, and cookies. To achieve that, the following static methods are available from the class `Handle`:

```csharp
void AddOutgoingCookie(String cookieName, String cookieValue);
void AddOutgoingHeader(String headerName, String headerValue);
void SetOutgoingStatusDescription(String statusDescription);
void SetOutgoingStatusCode(UInt16 statusCode);
```

