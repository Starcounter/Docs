# URI mapping

Starcounter applications are isolated, meaning that they don't know about each other's presence, registered handlers, database classes and so on. However when composing several applications together, one would like to display each application's elements on the shared screen, for example, in menu section or in search results. URI mapping allows mapping application handlers onto common URIs. Let's take an example where two applications, People and Products would like to display their elements in the Menu section of the wrapping (or also known as Launcher) application:

Menu partial from the People application:

```cs
Handle.GET("/people/menu", () =>
{
    return new Json() { Html = "/People/viewmodels/Menu.html" };
});
```

Menu partial from the Products application:

```cs
Handle.GET("/products/menu", () =>
{
    return new Json() { Html = "/products/viewmodels/Menu.html" };
});
```

Now using URI mapping we can map both of the above to common URI "/sc/mapping/menu":

```cs
UriMapping.Map("/people/menu", "/sc/mapping/menu");
UriMapping.Map("/products/menu", "/sc/mapping/menu");
```

Now by calling the common URI "/sc/mapping/menu" (from the common wrapping application) and serializing the response to the client (e.g. browser) we will get a merged JSON response from both applications and a link to merged HTML response in common HTML property:

```json
{
"People": {
    "Html": "/People/viewmodels/Menu.html"
},
"Products": {
    "Html": "/Products/viewmodels/Menu.html"
},
"Html": "/sc/htmlmerger?Launcher=/Launcher/viewmodels/LauncherMenu.html
    &People=/People/viewmodels/Menu.html
    &Products=/Products/viewmodels/Menu.html",
"AppName": "Launcher",
"PartialId": "/Launcher/viewmodels/LauncherMenu.html"
}
```

In conclusion, by calling any of mapped URIs ("/people/menu" or "/products/menu" or "/sc/mapping/menu") - all mapped handlers are called and the results are merged during serialization to the client.

The syntax for `UriMapping.Map` is the following:
```cs
void Map(String appProcessedUri,
         String mapProcessedUri,
         String method = Handle.GET_METHOD)
```

where `appProcessedUri` and `mapProcessedUri` can contain last parameter of the type string, for example, to map a search query URI:

```cs
UriMapping.Map("/Products/search?query={?}", "/sc/mapping/search?query={?}");
```
to a Products application search handler:
```cs
Handle.GET("/products/search?query={?}", (string query) => { ... }
```
