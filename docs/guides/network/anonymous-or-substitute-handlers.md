# Anonymous or Substitute Handlers

Sometimes, it is required to get a partial without declaring a handler. This is when anonymous handlers became handy.

```csharp
model.Partial = Self.GET("/custom-url", () =>
{
    return new Page()
    {
        Html = "/custom-page.html"
    };
});
```

The `model.Partial` will contain the returned `new Page()` and all the responses from all the apps mapped to the `"/custom-url"`. The anonymous handler can be called to any url.

### Things to know about anonymous handlers

* The anonymous handlers should never be used for direct calls to handlers defined in other apps. Follow the practice that the apps should not be aware of each other.
* An app can only have one response per URL, that is why anonymous handlers can't be combined with declared ones.
* The `Html` property is not required, it may be blank if the calling app does not have any response.
* The anonymous handler response should be of type `Starcounter.Page` otherwise it won't be possible to merge with other responses.

### When to use anonymous handlers

### Shared UI sections

The most common case is the [Launcher](https://github.com/starcounterapps/launcher). The Launcher defines different UI sections and populates them with anonymous `Self.GET` requests.

```csharp
//UriMapping.MappingUriPrefix - a constant, equals to "/sc/mapping"
model.Menu = Self.GET(UriMapping.MappingUriPrefix + "/menu", () => new Page());
```

The `model.Menu` contains merged responses from all handlers mapped to `/sc/mapping/menu`. The mapping would look like this:

```csharp
UriMapping.Map("/Products/menu", UriMapping.MappingUriPrefix + "/menu");
```

Read more about mapping here: [Mixing apps](../mapping-and-blending/).

### UI blending with layout editor

Blending technology requires some extra HTML elements and JSON wrapping which are injected per partial [using middleware](middleware.md). This means that partials which are created without a request won't be available for layout blending.

```csharp
// The IndexPage is available for layout blending.
// It is retrieved with a request.
Handle.GET("/my-app", () => new IndexPage());
```

```csharp
Handle.GET("/my-app", () => new MasterPage()
{
    // The IndexPage is not available for layout blending.
    // It is retrieved without a request.
    CurrentPage = new IndexPage()
});
```

```csharp
// The MasterPage and the IndexPage are both retrieved
// with a request and available for layout blending.
Handle.GET("/my-app", () => MasterPage()
{
    CurrentPage = Self.GET("/my-app/partials/index")
});

Handle.GET("/my-app/partials/index", () => new IndexPage());
```

An anonymous handler can be used instead of the custom partial handler definition.

```csharp
// The IndexPage is still available for layout blending,
// It is retrieved with an anonymous request.
Handle.GET("/my-app", () => MasterPage()
{
    CurrentPage = Self.GET("/my-app/partials", () => new IndexPage())
});
```

