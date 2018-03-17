# Server-Side Blending

From Starcounter 2.3.1/2.4, the Blender API described here replaces the previous OntologyMap and UriMapping API's. To view the old API's, go to an earlier version of the documentation with the drop down on the left side of the screen

## Introduction

Blending visually combines partial responses from different apps. It merges JSON/HTML responses. The JSON merging happens during the serialization process. The browser requests HTML partials through a special HTML-merging URI, it then displays combined partials from the different apps. Blending happens outside of apps and should not affect how they are functioning, essentially, apps should neither depend on, nor expect, blending. Blending applies to the JSON that GET handlers return. Apps should not know/assume neither about blender presence, nor other apps presense.

Blending uses tokens. These tokens are either strings or classes. Handlers blended on the same token are called on internal `Self.GET` calls or external URI that matches one of the handlers. Once the blended handler is called, it will not trigger further blending calls mapped to that handler directly, only when a new `Self` call is made.

The class `Blender` in the Starcounter namespace does the blending. The blender API does dynamic addition and removal of blended handlers during the lifetime of the app.

## Blending token

The first parameter is either a handler URI or a specific URI. If the token is a string, it's defined as the second parameter. If it's one or more classes, they are defined in the template or as a `Type` array parameter.

```csharp
Blender.MapUri("/Products/settings", "settings");
Blender.MapUri<Product>("/Products/partials/product/{?}");
Blender.MapUri("/Products/menu", "menu");
Blender.MapUri("/people/persons/34623", "person"); // Specific URI blending.
Blender.MapUri<Person>("/people/persons/{34623}"); // Specific URI blending using so-called mixed URI.
```

An arbitrary number of classes are allowed as a blending token \(up to 3 in template, more in array of class `Type`\). Here are the `MapUri` signature examples \(same exist for removing handler from blender\):

```csharp
static void MapUri<T>(String uri, String[] contexts = null);
static void MapUri<T1, T2, T3>(String uri, String[] contexts = null);
static void MapUri(String uri, Type[] types, String[] contexts = null);
```

One can even register a blending with an empty token:

```csharp
static void MapUri(String uri, String[] contexts = null);
static void MapUri(String uri, Boolean allowFromUriConverter, Boolean allowToUriConverter, String[] contexts = null);
```

which is blended with other handlers with the same empty token.

## Blending contexts

Within the same token blending can have a more fine-grained matching. This is where blending contexts are useful. They are composed of a list of strings that acts as a bit map when matched with other contexts. No blending context \(`null` value\) means **match any context**. Otherwise two blendings are matched if source context contains same elements as destination context. Examples:

* Source context `{ "Readable", "View" }` is NOT matched with `{ "Writable", "View" }`.
* Source context `null` is matched with `{ "Readable", "View" }` and `{ "Writable", "View" }` and any other context.
* Source context `{ "View", "Writable" }` is matched with `{ "Writable", "View" }` and vice versa.

Consider contexts as an additinal matching rule among blendings on the same token.

## Bledning specific URIs

Specific URI is the handler URI with parameters supplied. For example, for the handler `/people/{?}` the specific URIs will be `/people/john`, `/people/bob`, etc. When calling `Blender.MapUri` for a specific URI, you should pass a mixed URI which indicates what handler it belongs to. For the previous examples, the mixed URI will be `/people/{john}` and `/people/{bob}`, so the parameter in specific URI is wrapped into curly brackets. `Blender` class has helper methods to construct such mixed URIs: `Blender.GetMixedUriFromHandlerAndParameters`, `Blender.TryGetMixedUriFromSpecific`. The last method tries to find corresponding handler for the given specific URI, which might not be determined correctly \(for example, in case when the corresponding handler is not yet registered\). Mixed URIs in `MapUri` calls are needed so the underlying handler for a specific URI can be identified. Other `Blender` methods like `UnmapUri`, `IsMapped`, etc. can still use specific URIs and not mixed.

## Dynamic addition and removal

It is possible to get a list of handlers and tokens that are in Blender and then remove some of them or add new ones.

```csharp
Blender.UnmapUri("/app4/{?}", token2);
Blender.UnmapUri("/twoparams3/{?}/{?}", token);
Blender.UnmapUri<MyClass>("/app1/{?}");
Blender.UnmapUri<Person>("/people/persons/34623"); // Unmapping specific URI (not that specific URI is used, not mixed).
Blender.UnmapUriForAllTokens("/app1/param1/{?}"); // Unmapping URI for all blending tokens.
```

To check if certain handler is in blender:

```csharp
isInBlender = Blender.IsMapped("/noparam1", token3);
isInBlender = Blender.IsMapped<MyClass>("/noparam1");
isInBlender = Blender.IsMapped<Person>("/people/persons/34623"); // Checking if specific URI is blended.
```

To get a list of all blended handlers in a dictionary \(keyed by tokens and URIs respectively\):

```csharp
static Dictionary<String, List<BlendingInfo>> ListAllByTokens();
static Dictionary<String, List<BlendingInfo>> ListAllByUris();
```

To get the list of blending candicates that are going to run for a given URI handler:

```csharp
BlendingInfo[] GetRunCandidatesForUri(String uri);
```

To list registered blending rules for a given URI:

```csharp
BlendingInfo[] ListByUri(String uri);
```

and the same but for a given token:

```csharp
BlendingInfo[] ListByToken(String blendingToken)
```

As you might noticed, a special blending information structure is used here: `BlendingInfo`.  
It contains the following methods/properties:

```csharp
String AppName; // Returns the application name to which the blended handler belongs.
Boolean IsActive; // Shows if this blending is active.
String Token; // Returns blending token.
String Uri; // Returns URI given in MapUri (which was either mixed, specific or handler URI).
String SpecificUri; // Returns a specific blended URI or null if MapUri was called with handler URI.
String HandlerUri; // Returns blended handler URI.
Boolean IsSpecificUri; // Is it a specific URI blended, not a handler URI.
String[] Contexts; // Returns blending context (if any).
Boolean IsFromUriConverterOn; // Returns True if the "FromUriConverter" is set/allowed.
Boolean IsToUriConverterOn; // Returns True if the "ToUriConverter" is set/allowed.
```

## Parameters in handlers for Blending

Blending handlers are allowed to have arbitrary amount of parameters. When there is at least one parameter, the blender convertion functions are used. First converter translates handler arguments to token arguments, while the second converter does the opposite. Both converters are taking and returning an array of strings:

```csharp
Blender.MapUri("/twoparams1/{?}/{?}", token,
(String[] from) => {
   return from;
}, (String[] to) => {
   return to;
});
```

In the example above, converter passes handler parameters to token parameters, as is. There are signatures of `Blender.MapUri` with no converters, which means that parameters are simply passed through, like above.

Often it's needed to trigger blending on a specific URI. To achieve this, first converter should return non-null string array on certain parameters.

## Direction of calls when blending

Blender allows to specify the direction in which blended handlers would be called. This is needed to trigger blending in a certain direction: from handler or to handler. The direction is determined by the value that corresponding converter is returning: `null` converter or `null` string array returned in converter blocks the direction of the call. In case of zero parameters, there is a special `Blender.MapUri` override with corresponding boolean parameters to determine the allowed call directions.

```csharp
Blender.MapUri("/twoparams1/{?}/{?}", myToken, null, (String[] to) => {
    return to;
});
Blender.MapUri("/noparam1", token3, true, false);
```

In the first example above, handler "/twoparams1/{?}/{?}" should not trigger blending calls on token `myToken`, but it can be called when blending is triggered by other handlers on the same token.  
In the second example above, handler "/noparam1" can trigger other handlers on the same token, but can't be triggered by them.

## Same handler - different tokens

It is allowed to add blending for the same handler but on different tokens.

```csharp
Blender.MapUri("/app1/{?}", token1);
Blender.MapUri("/app2/{?}", token1);
Blender.MapUri("/app3/{?}", token1);

Blender.MapUri("/app1/{?}", token2);
Blender.MapUri("/app4/{?}", token2);
```

Here handler "/app1/{?}" participates in blending with `token1` and `token2` handlers.

When an app requests `Self.GET("/app1/xyz")`, both handlers mapped to `token1` and `token2` will be triggered.

