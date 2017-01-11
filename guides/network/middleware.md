# Middleware

Middleware is code that affects the *request pipeline*. Essentially, it enables applications to customize how the server handles requests. This customization comes in three different shapes:

1. request filters
2. response filters
3. response and request filters wrapped up in *middleware classes*.

These can help with a range of issues, such as security, logging, wrapping, request modification, and more.

An example of this is the [Launcher](https://github.com/StarcounterPrefabs/Launcher) which uses request filters to wrap responses from other applications into its own response.

All middleware is registered with the `Application.Current.Use` API which has three overloads corresponding to the different shapes of middleware listed above. This registration is usually done at the entry point of the application. It can look something like this:

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

When allowing external HTTP requests, it might be useful to pre-process or filter out certain requests before the designated handler is called. Request filters make it possible to do exactly that. They are lists of user-supplied delegates, or filters, that are executed on external requests before the actual handlers are called. These filters are executed one by one until one of the filters returns a non-null `Response`. If a `Response` object was created, then it's returned to the client without calling the handler. If none of the filters returned a `Response` object, then the request will be passed on and dealt with by the handler.

An example of this can be an extremely basic spam filter:

<div class="code-name code-title">Request spam filter</div>
```cs
Application.Current.Use((req) => {
    if (req.Uri.Contains("spam")) {
        return new Response() {
            StatusCode = 406,
            StatusDescription = "Well, thanks, but no thanks!"
        };
    }
    return null;
});
```

When there is an incoming request, this request filter checks if the URI contains the string "spam", and returns a `Response` object if that's the case which means that the request will be blocked without reaching the handler. Otherwise, it returns `null`, which allows the request to move on to the next request filter if there are more of them or go to the handler if there was only one request filter.

To let a handler bypass all request filters, use the class `HandlerOptions` and set `SkipRequestFilters` to `true`. Like this:

```cs
Handle.GET("/my-url", () => new Page(), new HandlerOptions() { SkipRequestFilters = true });
```

## Response filters

Response filters do the opposite of request filters; they make alterations to outgoing responses. For example, response filters makes it possible to add a certain HTTP header to responses for requests with a `/special` URI prefix:

<div class="code-name code-title">Response filter header changer</div>
```cs
Application.Current.Use((req, resp) => {
    if (req.Uri.StartsWith("/special")) {
        resp.Headers["MyHeaderName"] = "MyHeaderValue";
    }
    return resp;
});
```

In this case, the response filter will check outgoing responses to requests where the URI starts with `/special`. If that's the case it will set a header and return the response.

## Middleware classes

In addition to request and response filters, `Application.Current.Use` allow for custom middleware classes that can be exposed and used by all applications. These custom middleware classes implement the `IMiddleware` interface and don't look much different from the response and request filters explained above.

Here's an example of an application using a custom middleware class:
```cs
class Blocker : IMiddleware {

    void IMiddleware.Register(Application application) {
        application.Use((req) => {
            return new Response() {
                StatusCode = 500,
                StatusDescription = "Blocker doesn't allow ANYTHING to get through!"
            };
        });
    }
}

class Program {

    static void Main() {
        Application.Current.Use(new Blocker());

        Handle.GET("/blocked", () => {
            return "I'll never be called :( ";
        });
    }
}
```

### HtmlFromJsonProvider

`HtmlFromJsonProvider` is a custom middleware class provided in the `Starcounter` namespace. It acts as a response filter by intercepting JSON objects and instead sending the corresponding HTML. For example, look at the following application:

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

  Handle.GET("/person", () => {
    return new Person();
  });
}
```

When a call for `/person` is made, this is what will happen:

1. The JSON object `Person` will be returned by the handler
2. The `HtmlFromJsonProvider` will intercept the object
3. It will look in the JSON file for the `Html` property and find `/person.html`
4. It will return the HTML found at that path

If the HTML at the path would be a complete HTML document, this would be sufficient. Because the HTML provided is a template, it is necessary to add another layer of middleware to convert the template to a full HTML document that can be rendered by the browser. That's where `PartialToStandaloneHtmlProvider` enters the picture.

### PartialToStandaloneHtmlProvider

This middleware class checks if the HTML is a full document, or essentially that it starts with a `doctype`. If it is not a full HTML document, it wraps the template inside the body of an HTML document that contains the `puppet-client` element to create a WebSocket connection, import links to the Starcounter custom elements `starcounter-include` and `starcounter-debug-aid`, import links to the outside libraries Polymer and Bootstrap, and the session URL which makes it possible for PuppetJs to request the relevant JSON in a future request.

Since `PartialToStandaloneHtmlProvider` wraps the actual response from the handler, it will also have the HTTP status code that was returned.

When building Starcounter applications the way that is advised, the two above middleware classes should always be provided in order to send parseable HTML to the client.

This is how it should look:

```cs
static void Main()
{
  Application.Current.Use(new HtmlFromJsonProvider());
  Application.Current.Use(new PartialToStandaloneHtmlProvider());

  // Rest of the application
}
