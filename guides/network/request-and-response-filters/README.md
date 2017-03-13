# Request and Response filters

We are going through our central web API's in an attempt to make them clearer for application developers to comprehend. Our ambition is to keep them simple and non-verbose, but tweak them to also be a little more transparent on what is going on under the surface.

This page will describe the basics of handlers and glimpse on what we have in mind for the upcoming version.

<h2>Handler basics</h2>

In Starcounter, applications typically communicate by defining code snippets collectively referred to as <em>handlers</em>.

Handlers respond to requests for <strong>resources</strong>. The request include what <strong>representation </strong>are accepted, and preferred. The URI indicates the identify of the requested resource.

Handlers return <code>Response</code> objects. In their native form, application developers define handlers that return response objects constructed explicitly:

```cs
Handle.GET("/author", () =>
{
  return new Response()
  {
    StatusCode = 200,
    ContentType = "text/plain",
    Body = "Per Samuelsson"    
  };
});
```

<h2>The JSON data format</h2>

In Starcounter, JSON is a first-class citizen. As such, we provide built-in types that makes serving and accepting JSON simpler for the app developer.

Most notably, <a href="/guides/typed-json/typed-json">TypedJSON </a>allow you to define C# objects based on arbitrary JSON data, normally provided as a string. One variant of this allow classes to be defined by an example JSON. We call this, unsurprisingly, <a href="/guides/typed-json/json-by-example">JSON-by-example</a>.

With that, by defining this:

```json
{
  "Name": "Per",
  "Surname": "Samuelsson"
}
```

you can instantly enjoy this:

```cs
var a = new Author();
Console.WriteLine(a.Name + " " + a.Surname);
// Output: Per Samuelsson
```

and go from the instance back to the JSON text using this:

```cs
Console.WriteLine(a.ToJson());
// Output:
//{
//  "Name": "Per",
//  "Surname": "Samuelsson"
//}
```

Underneath, the generated class derives from a built-in class, <code>Json</code>.

```cs
class Author : Json
{
  // ...
}
```

Lets take a closer look to what that means in the context of handling requests.

<h2>JSON in request handlers</h2>

In the introduction, you learned that handlers return <code>Response</code> objects. You saw how to create a response and set its values explicitly.

Because we choose JSON as the dominant data exchange format in Starcounter, we've added convenience methods that allow you to return instances of Typed JSON. You'll often see handlers defined like this:

```cs
Handle.GET("/author", () =>
{
  return new Author();
});
```

Technically, it is enabled by a simple <em>implicit </em>cast, that allows any object of type <code>Json</code> to be cast to a <code>Response</code>, such as this:

```cs
Author a = new Author();
Response r = a;
```

Hence, the above translate approximately to this:

```cs
Handle.GET("/author", () =>;
{
  return new Response()
  {
    StatusCode = 200,
    ContentType = "application/json",
    Body = "{ Name: \"Per\", Surname: \"Samuelsson\"}"            
  };
});
```

Why approximately? Because there is another very important detour underneath, one we'll look at next.

<h2>Returning resources, not representations</h2>

Early in this text, we concluded that handlers accept URIs that identify a resource and that the request indicate what representations are accepted. In <a href="https://en.wikipedia.org/wiki/Representational_state_transfer">REST</a>, there is a distinct separation between a <em>resource </em>and its <em>representation</em>.

Still, you saw how to return JSON from a handler. And JSON is a data format, hence a representation? It is. How do we know the request accept that format, and what happens if it don't?

The answer is found by taking a closer look at the <code>Response</code> class. If you do, you'll see a property called <code>Resource</code>. The type is <code>IResource</code> and the idea is simple: any class that can act a <code>IResource</code> can be used to construct responses and is given the chance to return a representation of itself based on a requested <a href="https://en.wikipedia.org/wiki/Media_type">MIME type</a>.

Going back to returning JSON directly from handlers. When you do this:

```cs
Handle.GET("/author", () =>;
{
  return new Author();
});
```

what actually happens is this:

```cs
Handle.GET("/author", () =>;
{
  return new Response()
  {
    Resource = new Author();
  }
});
```

Simply put: any <code>Json</code> instance is a <strong>resource </strong>that can <strong>represent itself</strong> as <code>application/json</code>.

Certain kind of applications actually expose APIs supporting only that single data format. They would need nothing more, and could be designed using only the above features. Normally though, applications that wish to be good web citizen want to support at least a few more formats, and most notably <code>text/html</code>.

<h2>Resources with multiple representations</h2>

Let us expose the "author" resource in an additional web format: HTML. We'll keep it to just two different representations in this text, but it should be fairly easy to see how to extend that, supporting as many representations as the application require.

To support multiple representations, two alternatives exist: <em>inline</em>, normally using the <code>Resource</code> class, or by using <em>middleware</em>.

<h2>Multiple representations using the Resource class</h2>

<strong>NOTE</strong>:<em> The Resource class is not yet part of any version, but instead a proposal we are considering. See the next chapter on how to support multiple representations using middleware.</em>

The recommended pattern to define handlers that return multiple representations <em>inline </em>is by using the <code>Resource</code> class. At its core, it allows the developer to support a set of representations by mapping each MIME type to <em>providers</em>.

```cs
Handle.GET("/author", () =>
{
  var json = new Author();
  var html = "<b>Per</b><br><div>Samuelsson</div>";

  var r = new Resource();
  return r.Json(json).Html(() =&gt; return UTF8.GetBytes(html));
});
```

Taking a closer look at what goes on here, <code>Resource</code> expose as its central method <code>AsMime</code> and a few convenient-methods for the most common data types, like JSON:

```cs
class Resource : IResource
{
  public IResource AsMime(string mimeType, IResource provider)
  {
    return AsMime(mimeType, () => return resource.AsMimeType(mimeType));
  }

  public IResource AsMime(string mimeType, Func&lt;byte[]&gt; provider)
  {
    // ... store provider
  }

  public IResource Json(IResource provider)
  {
    return AsMime("application/json", provider);    
  }
}
```

Based on the incoming request, the server will then pick the appropriate provider(s) to invoke and construct the response from the result.

<h2>Multiple representations with middleware</h2>

With <em>middleware</em>, applications can customize the request pipeline. Using it, developers can move provision of multiple representations into general code snippets that are invoked by the server when resources returned from handlers fail to provide a certain, requested MIME type.

The <code>MimeProvider</code> class expose itself just like that.

```cs
void Main()
{
  var app = Application.Current;

  app.Use(MimeProvider.Html((context, next) =>
  {
    var json = context.Resource as Json;
    if (json != null)
    {
      context.Result = Self.GET&lt;byte[]&gt;(json["Html"]);
    }
    next();
  });

  Handle.GET("/author", () =>
  {
    return new Author();
  });
}
```

Middleware in the form of MIME providers accept as their input a <em>context </em>(<code>MimeProviderContext</code>) and a delegate that reference the next middleware in the chain. From the context, the <code>IResource</code> that has failed to represent itself in the requested type can be retrieived. The role of the middleware provider is to <em>extend </em>that resource by returning the requested representation.

Hence, the semantics of middleware MIME providers are:

<blockquote>"When a resource can't provide a certain MIME type representation, let me take a look at it and give it a try".</blockquote>

In the above snippet, the provider acts on the resource if it's of type <code>Json</code>. If so, it tries reading a "Html" property and take the value of that as a URI string, passing that URI to a <code>Self.GET</code> to invoke a fellow handler that can provide a byte stream of the HTML content.

<h2>JSON view models, HTML views</h2>

To wrap this text up, we'll take a look on a practical use case where all the above come together.

In MVVM, a<em> view model</em> lives between a <em>view </em>and a <em>model</em>. In Starcounter, we promote a simple yet powerful pattern to define HTML views, driven by an underlying view model, defined in JSON. The advantages of this approach are several, but out of scope for this text. You can read more about the details <a href="/guides/web-apps/starcounter-mvvm">here</a>.

The basic pattern of this is very close to where we ended up in the last section, where you saw how an individual handler returned a typed JSON object, and a middleware MIME provider was registered to read a HTML URI from any JSON object, returning a corresponding HTML view. What we didn't show earlier is how we utilize this pattern as the foundation for a template-driven MVVM engine.

Simply put, a <em>template engine</em> take some kind of <strong>template</strong>, combine that with some <strong>data </strong>and form it into a finished <strong>result</strong>. In this context, we use a <strong>HTML </strong>template, insert <strong>JSON </strong>data into it a return a <strong>webpage</strong>.

<div class="code-name">Author.html</div>

{% raw %}
```html
<html>
<head>
  <title>Author</title>
</head>
<body>
  Name: {{model.Name}},
  Surname: {{model.Surname}}
</body>
</html>
```
{% endraw %}

<div class="code-name">Author.json</div>

```json
{
  "Html": "/author.html",
  "Name": "Per",
  "Surname": "Samuelsson"
}
```

<div class="code-name">Program.cs</div>

```cs
void Main()
{
  var app = Application.Current;

  app.Use(new HtmlFromJsonProvider());

  Handle.GET("/author", () =>
  {
    return new Author();
  });
}
```

The <code>HtmlFromJsonProvider</code> middleware comes as part of Starcounter and, based on previous sections, you can probably guess what it's about. It's little more than this code, wrapped up in a class:

```cs
app.Use(MimeProvider.Html((context, next) =>
{
  var json = context.Resource as Json;
  if (json != null)
  {
    context.Result = Self.GET<byte[]>(json["Html"]);
  }
  next();
});
```

The focus here is to illustrate a pattern, not to provide code that can run out of the box. Still, what was just presented is very close to the code you'll see in our real-world sample apps.

Let's finally summarize this last section in a step-by-step to get the feeling of how it all fit together.

{% raw %}
<ol>
    <li>Client (e.g. browser) request <code>GET /author</code> with <code>text/html</code></li>

    <li>Starcounter invoke user handler and get back <code>Json</code> instance.</li>

    <li>The <code>Json</code> instance can't render <code>text/html</code>. Starcounter turn to middleware for MIME providers.</li>

    <li>The <code>HtmlFromJsonProvider</code> read <code>Json.Html</code> property, referencing <code>/author.html</code> and invoke an internal request using <code>Self.GET("/author.html").</code></li>

    <li>The built-in static file server respond with that file, and it's returned to the client.</li>

    <li>The client detect <code>{{model.Name}}</code>-like constructs.</li>

    <li>Client (using small client logic) request <code>GET /author</code> with <code>application/json</code></li>

    <li>Server return data from Author.json</li>

    <li>The resulting web page is rendered inserting JSON data into the template</li>
</ol>
{% endraw %}

<h2>What's next?</h2>

To get deeper into how Starcounter application development works, please head over to our <a href="/guides/">guides </a>for specific topics, tutorials and a lot more!

<h3>Implementation details</h3>

In the current implementation, MIME providers are only invoked on resources of type <code>Json</code>. In upcoming versions, we'll allow any customized <code>IResource</code> implementation to be able to trigger these.

Furthermore, registering a MIME provider for the JSON format itself, like this:

```cs
void Main()
{
  Application.Current.Use(MimeProvider.For("application/json", (context, next) =>
  {
    // Do our thing ...
    next();
  });
}
```

will currently <strong>not trigger</strong>. This is due to a legacy, where Starcounter treat JSON as a first-class citizen and optimize some flows for that. This will also be changed in upcoming versions.
