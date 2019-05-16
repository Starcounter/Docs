# Handling HTTP Requests

There are two ways of doing routing in Starcounter:

1. With the built-in `Handle` API
2. With the routing API found in the Authorization library

In most cases, it's recommended to use the API in the [Authorization library](https://github.com/Starcounter/authorization#routing-middleware-and-context---concepts) because of its superior ease of use. Although, there might be some cases where the `Handle` API is needed.  
This page describes the built-in `Handle` API

### Catching Incoming Requests

Incoming HTTP 1.0/1.1 requests are caught using the static `Handle` class.

```
Handle.GET("/hello", () =>
{
    return "Hello World";
});
```

Handlers can be registered anywhere and at any time. Though, in most cases, they are registered in the `Main` method

### Catching Common HTTP Verbs

The basic HTTP methods `GET`, `POST`, `PUT`, `DELETE` and `PATCH` can be caught by methods with the same name in the `Handle` class.

```
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

```
Handle.GET("/hello/{?}", (string name) => 
{         
    return "Hello " + name;
});
```

In the above example, the delegate accepts `string name`. This means that the parameter will be parsed as a string, for example: `/hello/albert`, `/hello/anna`. To accept an integer parameter, simply change the lambda parameter type.

```
Handle.GET("/squared?{?}", (int num) => 
{         
    return num + " squared equals " + num * num;
});
```

The accepted URIs would, for example, be `/squared?123` and `/squared?-4321`

To accept multiple dynamic fragments, add more curly braces. For each dynamic parameter there should be a parameter in the delegate. They are enumerated from left to right, so be careful to put the parameters in the right order.

```
Handle.GET("/{?}/{?}", (string list, int item) => 
{         
    return "List is " + list + " and item is " + item;
});
```

The accepted URIs would be, for example: `/serialnumbers/4534123`, `/itemid/34321`

### Catching Other Verbs

The `CUSTOM` verb in the `Handle` class makes it possible to register other HTTP methods or even catch all methods and URIs.

```
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

```
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

```
string mySuperHeader = req.Headers["MySuperHeader"];
string allRequestCookies = req.Headers["Set-Cookie"];
```

Request cookies are accessible from `Cookies` as a list of strings "name=value" \(same as for Response object\):

```
List<String> allRequestCookies = req.Cookies;
```

To obtain client IP address, use `GetClientIpAddress()` on the `Request` object.

Use the `HandlerAppName` property to find out which application the request belongs to. This might be useful when working with request filters.

### Handler Options

When creating \(using the `Handle` interface\) and calling handlers \(using the `Self` interface\), one can supply last `HandlerOptions` parameter, which specifies certain options for handler calls or registration. Here are the notable handler options:

* `SkipRequestFilters`: used to declare a handler for which request \(previously middleware\) filters will not be applied.
* `ReplaceExistingHandler`: replace an existing handler if it was registered.
* `HandlerLevel`: level on which the handlers should be registered or called.
* `SkipHandlersPolicy`: If database flag "EnforceURINamespaces" is set to True, all application handlers are required to start with application name. In this case `SkipHandlersPolicy` flag allows to register any URI handler.
* `SubstituteHandler`: specifies a delegate to be called to replace call for existing handler or to provide a call when handler does not exist.
* `SelfOnly`: registered handler is going to be accessible only inside codehost using `Self` interface. `SelfOnly` handlers are not registered in gateway, in comparison with normal handlers.

**Examples**:

Registering a handler that skips middleware filters and is being called directly externally:

```
Handle.POST("/myhandler", (Request request) =>
{
    return 204;
}, new HandlerOptions() { SkipMiddlewareFilters = true });
```

Calling the URI handler `/MyPostHandler` on handlers level `ApplicationLevel`:

```
Self.POST("/MyPostHandler", null, null, null, 0, new HandlerOptions()
{
    HandlerLevel = HandlerOptions.HandlerLevels.ApplicationLevel
});
```

### Exception Propagation Within Applications

Internal requests are requests made to handlers within same user application \(`sccode` instance\) using `Node` and `X`. Internal requests and handlers can be nested and create a call hierarchy. Sometimes its useful to cast a specific exception deep down in the hierarchy and handle it on another level or let the system handle it \(for example by automatically sending the response\). This can be achieved using `ResponseException` exception object. The following example illustrates this concept:

```
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

