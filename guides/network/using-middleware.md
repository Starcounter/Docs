# Using middleware

## Request filters

Sometimes for external HTTP requests one would like to have some request pre-processing or simply filter-out certain requests completely. For this purpose we introduced so-called "request filters". Its basically a list of user-supplied delegates that are executed for external requests before the actual handlers are called. Each filter in the list is executed one by one, until some filter returns a non-null `Response`. If `Response` object was created in request filter then its returned to the client and the actual handler will not be called. If none of the filters returned a response object then execution proceeds to the actual handler.

**Note:** internal `Self.GET` requests do not go through request or response filters.

Each application can use zero or many filters. User `Application.Use` method to register filters.

```cs
// Register a request filter
Application.Current.Use(request =>
{
    return null;
});

// Register a response filter
Application.Current.Use((request, response) =>
{
    return response;
});
```

Sometimes one would however like to completely skip all request filters and process the external request "as-is". For this, when declaring handler, user has to supply `HandlerOptions`:

```cs
Handle.GET("/my-url", () => new Page(), new HandlerOptions() { SkipRequestFilters = true });
```

There are various applications for request filters: security, logging, wrapping, request modification, etc.

One notable example is the [Launcher](https://github.com/StarcounterPrefabs/Launcher) application that wraps responses from all running applications into its own response.

## Response filters

Sometimes its needed to add some post processing for outgoing responses. For example, one would like to add a certain HTTP header to responses for requests having a "/special" URI prefix. To add response filters, we provide a static method `AddResponseFilter(Func<Request, Response, Response> filter)` in `Handle` class. Response filters are executed in a list for every outgoing response until one filter returns a non-null response. To support the given example, here is how the response filter will look like:

```cs
Application.Current.Use((Request request, Response response) =>
{
    // Checking if request URI starts with /special
    if (request.Uri.StartsWith("/special"))
    {
        // Adding desired header.
        response.Headers["MyHeaderName"] = "MyHeaderValue";
    }
    return response;
});
```

Using the same approach one can post-process static files responses as well. If a response is a static file `Response.FileExists` will return true.
