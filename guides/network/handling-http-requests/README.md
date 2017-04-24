# Handling HTTP requests

## Catching incoming requests

Incoming HTTP 1.0/1.1 requests are caught using the static `Handle` class.

```cs
class Hello
{
   static void Main()
   {
      Handle.GET("/hello", () =>
      {
         return "Hello World";
      });
   }
}
```

You can register new handlers at any time, so there is no need to do it in the `Main()` method.

## Catching the common HTTP verbs (methods)

The basic HTTP methods `GET`, `POST`, `PUT`, `DELETE` and `PATCH` can be caught by convenient methods with the same name in the `Handle` class.

```cs
class Hello
{
   static void Main()
   {
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
   }
}
```

## Catching other verbs (methods)

Using the `CUSTOM` method in `Handle`, you can register other HTTP methods (verbs) or even catch all methods and URIs.

```cs
class Hello
{
   static void Main()
   {
      Handle.CUSTOM("REPORT /hello/{?}", (String p1) =>
      {
         return 500;
      });

      Handle.CUSTOM("{?} /hello/{?}", (String method, String p1) =>
      {
         return p1;
      });

      Handle.CUSTOM("OPTIONS", "/hello/{?}", (String p1) =>
      {
         return p1;
      });

      Handle.CUSTOM("{?}", (String methodAndUri) =>
      {
         return "Catched: " + methodAndUri;
      });
   }
}
```

## Dealing with the `Request` object

Together with the enumerated parameters, you can also declare a `Request` parameter. It encapsulates the entire request.

```cs
class Hello
{
   static void Main()
   {
      Handle.GET("/hello", (Request request) =>
      {
         return 500;
      });

      Handle.GET("/persons/{?}", (string name, Request request) =>
      {
         return 500;
      });
   }
}
```

To access certain request HTTP headers, use `Headers[String]` accessor on a `Request` object (same as for the `Response` object):
```cs
String mySuperHeader = req.Headers["MySuperHeader"];
String allRequestCookies = req.Headers["Set-Cookie"];
```

Request cookies are accessible from `Cookies` as a list of strings "name=value" (same as for Response object):
```cs
List<String> allRequestCookies = req.Cookies;
```

To obtain client IP address use `GetClientIpAddress()` on the `Request` object.

Use the `HandlerAppName` property to find out which application the request belongs to. This might be useful when working with request filters.

## Notable handler options

When creating (using `Handle.` interface) and calling handlers (using `Self.` interface) one can supply last `HandlerOptions` parameter, which specifies certain options for handler calls or registration. Here are the notable handler options:

* `SkipRequestFilters`: used to declare a handler for which request (previously middleware) filters will not be applied.
* `ReplaceExistingHandler`: replace an existing handler if it was registered.
* `HandlerLevel`: level on which the handlers should be registered or called.
* `SkipHandlersPolicy`: If database flag "EnforceURINamespaces" is set to True, all application handlers are required to start with application name. In this case `SkipHandlersPolicy` flag allows to register any URI handler.
* `SubstituteHandler`: specifies a delegate to be called to replace call for existing handler or to provide a call when handler does not exist.
* `SelfOnly`: registered handler is going to be accessible only inside codehost using `Self` interface. `SelfOnly` handlers are not registered in gateway, in comparison with normal handlers.

**Examples**:

Registering handler that skips middleware filters and is being called directly externally:
```cs
Handle.POST("/myhandler", (Request request) =>
{
	return 204;
}, new HandlerOptions() { SkipMiddlewareFilters = true });
```

Calling URI handler "/MyPostHandler" on handlers level `ApplicationLevel`:
```cs
Self.POST("/MyPostHandler", null, null, null, 0, new HandlerOptions()
{
	HandlerLevel = HandlerOptions.HandlerLevels.ApplicationLevel
});
```


## Exception propagation within same application

Internal requests are requests made to handlers within same user application (`sccode` instance) using `Node` and `X`. Internal requests and handlers can be nested and create a call hierarchy. Sometimes its useful to cast a specific exception deep down in the hierarchy and handle it on another level or let the system handle it (for example by automatically sending the response). This can be achieved using `ResponseException` exception object. The following example illustrates this concept:

```cs
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

Handler `/exc3` constructs and throws an instance of `ResponseException` exception. Handler `/exc2` catches the exception, modifies some data and re-throws the exception. Eventually, the `ResponseException` is caught by outer system handler and `ResponseObject` is automatically sent on the original `Request req`. Note that `ResponseException` mechanisms are working only within one user application (they are simple C# exceptions).

User can attach an arbitrary user object to `ResponseException` by either constructor or `UserObject` property.

## Unregistering existing HTTP handlers

After an HTTP handler is created - it can be unregistered. For this use `Handle.UnregisterHttpHandler` method.
