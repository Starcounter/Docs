# Middleware

Middleware is code that affects the *request pipeline*. Essentially, it enables applications to customize how the server handles requests. This customization comes in three different shapes:

1. request filters
2. response filters
3. response and request filters wrapped up in *middleware classes*.

These can help with a range of issues, such as security, logging, wrapping, request modification, and more.

An example of this is the [Launcher](https://github.com/StarcounterPrefabs/Launcher) which uses request filters to wrap responses from other applications into its own response.

All middleware is registered with the `Application.Current.Use` API which has three overloads corresponding to the different types of middleware listed above. This registration is usually done at the entry point of the application and can look something like this:

```cs
static void Main()
{
  Application.Current.Use((request) => {
    // Request filter goes here
  });

  Application.Current.Use((request, response) => {
    // Response filter goes here
  });

  Application.Current.Use(new SomeMiddleware());
}
```

Middleware does not impact internal `Self.GET` calls.

## Request filters

When allowing external HTTP requests, it might be useful to pre-process or filter out certain requests before the designated handler is called. Request filters make it possible to do exactly that. They are lists of user-supplied delegates, or filters, that are executed on external requests before the actual handlers are called. These filters are executed one by one until one of the filters returns a non-null `Response`. If a `Response` was returned from the request filter, then this response is returned to the client without calling the handler. If none of the filters returned a `Response` object, then the request will be passed on and dealt with by the handler.

![Middleware example](../assets/Middleware-example.png)

An example of this can be an extremely basic spam filter:

<div class="code-name code-title">Request spam filter</div>
```cs
Application.Current.Use((req) =>
{
    if (req.Uri.Contains("spam"))
    {
        return new Response()
        {
            StatusCode = 406,
            StatusDescription = "Well, thanks, but no thanks!"
        };
    }
    return null;
});
```

When there is an incoming request, this request filter checks if the URI contains the string "spam", and returns a `Response` object if that's the case which means that the request will be blocked without reaching the handler. Otherwise, it returns `null`, which allows the request to move on to the next request filter if there are more of them or go to the handler if there was only one request filter.

To let requests to a certain handler bypass all request filters, use the class `HandlerOptions` and set `SkipRequestFilters` to `true`. Like this:

```cs
Handle.GET("/my-url", () => new Page(), new HandlerOptions() { SkipRequestFilters = true });
```

As an important distinction from response filters, request filters does only catch requests to handlers in the current application. That means that request filters running in one application will not interfere with requests coming to other applications running simultaneously.

## Response filters

Response filters do the opposite of request filters; they make alterations to outgoing responses. For example, response filters makes it possible to add a certain HTTP header to responses for requests with a `/special` URI prefix after the request has been dealt with by the handler:

```cs
Application.Current.Use((request, response) =>
{
    if (request.Uri.StartsWith("/special"))
    {
        resp.Headers["MyHeaderName"] = "MyHeaderValue";
        return response;
    }
    return null;
});
```

In this case, a new header would be added to the response if the URI of the incoming request started with `/special`. It would then return the response and no other response filters would be called. If the request URI did not start with `/special`, then the next response filter would be called or the response would be returned if there were no more response filters to call. Take a look at this response filter by comparison:

```cs
Application.Current.Use((request, response) =>
{
    if (request.Uri.StartsWith("/special"))
    {
        resp.Headers["MyHeaderName"] = "MyHeaderValue";
    }
    return return;
});
```

In this case, the next response filter would never be called since a response is always returned. That is important to keep in mind when building response filters.

In the examples above, the response filter checks for information in the request. It is also possible to check for information in the response, such as in this example:

```cs
Application.Current.Use((Request request, Response response) =>
{
    if (response.StatusCode == 404)
    {
        return new Response()
        {
            StatusCode = 404,
            StatusDescription = "Not Found",
            Body = Self.GET("/myapp/404.html").Body
        };
    }
    return null;
});
```

Here, the response filter makes it possible to return a descriptive `404` page by checking the outgoing responses for the `404` status code and in that case return a response containing an HTML page.

## Middleware classes

In addition to request and response filters, `Application.Current.Use` allow for custom middleware classes that can be exposed and used by all applications. These custom middleware classes implement the `IMiddleware` interface and do not look much different from the response and request filters explained above.

Here's an example of an application using a custom middleware class:
```cs
class Blocker : IMiddleware
{
    void IMiddleware.Register(Application application)
    {
        application.Use((request) =>
        {
            return new Response()
            {
                StatusCode = 500,
                StatusDescription = "Blocker doesn't allow ANYTHING to get through!"
            };
        });
    }
}

class Program {

    static void Main()
    {
        Application.Current.Use(new Blocker());

        Handle.GET("/blocked", () =>
        {
            return "I'll never be called :( ";
        });
    }
}
```

In this case, a custom middleware class is created using the `IMiddleware` interface. The middleware created in this case is a request filter that simply returns an unhelpful response no matter what the incoming request is.

By using custom middleware, it is possible to achieve a higher level of abstraction by hiding the implementation of the middleware.

### HtmlFromJsonProvider

`HtmlFromJsonProvider` is a custom middleware class provided in the `Starcounter` namespace. It acts as a response filter by intercepting outgoing responses containing JSON objects and instead responding with the corresponding HTML. For example, look at the following application:

<div class="code-name">Person.html</div>
```html
<template>
    <template is="dom-bind">
        <h1>{{model.FirstName}}</h1>
        <h3>{{model.LastName}}</h3>
    </template>
</template>
```

<div class="code-name">Person.json</div>
```cs
{
  "Html": "/person.html",
  "FirstName": "John",
  "LastName": "Doe"
}
```

<div class="code-name">Program.cs</div>
```cs
void Main() {
  Application.Current.Use(new HtmlFromJsonProvider());

  Handle.GET("/person", () =>
  {
    return new Person();
  });
}
```

When a call for `/person` is made, this is what will happen:

1. The JSON object `Person` will be returned by the handler
2. The `HtmlFromJsonProvider` will intercept the object
3. It will look in the JSON file for the `Html` property and find `/person.html`
4. It will return the HTML found at that path

If the HTML at the path would be a complete HTML document, this would be sufficient. Though, because the HTML provided in this example is a HTML template, it is necessary to add another layer of middleware to convert the template to a full HTML document that can be rendered by the browser. That's where `PartialToStandaloneHtmlProvider` enters the picture.

### PartialToStandaloneHtmlProvider

This middleware class checks if the HTML is a full document, or essentially if it starts with a `doctype`. If it is not a full HTML document, it wraps the existing HTML inside the body of an HTML document that contains the following:

1. A `puppet-client` element to create a WebSocket connection
2. Import links to the Starcounter custom elements `starcounter-include` and `starcounter-debug-aid`
3. Import links to the outside libraries Polymer and Bootstrap
4. The session URL which makes it possible for PuppetJs to request the relevant JSON in a future request

It is possible to override this default HTML by passing a string containing HTML as a parameter.

Since `PartialToStandaloneHtmlProvider` wraps the actual response from the handler, it will also have the HTTP status code that was returned.

When building Starcounter applications the way that is advised, the two above middleware classes should always be provided in order to send HTML that can be parsed to the client.

This is how it should look:

```cs
static void Main()
{
  Application.Current.Use(new HtmlFromJsonProvider());
  Application.Current.Use(new PartialToStandaloneHtmlProvider());

  // Rest of the application
}
```