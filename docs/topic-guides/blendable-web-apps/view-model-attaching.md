# View-model Attaching

{% hint style="warning" %}
Converter functions are obsoleted and will be removed from Blender.MapUri in Starcounter 2.4.
{% endhint %}

## Introduction

When the browser sends a request to the server, the app with a URI handler that matches the request responds with a view-model. Starcounter has a unique feature of _attaching_ view-models from other apps to the main view-model in the same response using a system based on attachment rules.

For example, if a user wants to see a profile of a person, the browser makes a request to the People app: `GET http://localhost:8080/people/person/4782`. The response will include not only the view-model from the People app but also view-models from other apps that are attached to it.

An attachment rule consists, of a URI, a token and a context.

On a high level, these are the specific steps involved in sending the response:

1. The browser makes a request for a resource, such as:  `GET http://localhost:8080/people/person/4782`.
2. The server receives the request and routes it to the corresponding handler in an app \(People\).
3. The handler has an attachment rule that maps the URI to a token that represents the concept, in this case, it's the person table in the database \(`simplified.ring2.person`\). 
4. The view-model attaching engine requests responses from all the other handlers registered in the code host that have an attachment rule with the same token.
5. During the serialization process, the server attaches the responses from other apps that are view-models to the main view-model from the initial handler.
6. The server sends the response, which now contains view-models from multiple apps, back to the client.

By using attachment rules, the apps don't need to know anything about other apps in the code host - they don't even need to know if there are other apps - they only have to communicate what concept the handlers deal with. The apps should be built to not depend on, but expect, attaching.

The process of defining the attachment rules and attaching the responses is handled by the `Blender` class in the `Starcounter` namespace.

Attaching works well with [view composing](view-composing.md), which makes the views representing the attached view-models look like one app.

{% hint style="info" %}
Attaching was previously called "server-side blending"
{% endhint %}

## Tokens

Attachment rules use tokens. These tokens are either strings or classes, that are also converted to strings using full class names. Tokens are case insensitive. Handlers with the same token are called on internal `Self.GET` calls or external URI that matches one of the handlers. Once the handler with a token is called, it will not trigger further calls mapped to that handler directly, only when a new `Self` call is made.

The first parameter is either a handler URI or a specific URI. If the token is a string, it's defined as the second parameter. If it's one or more classes, they are defined in the template or as a `Type` array parameter.

```csharp
Blender.MapUri("/Products/settings", "settings");
Blender.MapUri<Product>("/Products/partials/product/{?}");
Blender.MapUri("/Products/menu", "menu");
Blender.MapUri("/people/persons/34623", "person"); // Specific URI attachement rule.
Blender.MapUri<Person>("/people/persons/{34623}"); // Specific URI attachement rule using so-called mixed URI.
```

An arbitrary number of classes are allowed as tokens \(up to 3 in a template, more in an array of class `Type`\). Here are the `MapUri` signature examples \(same exist for removing the token mapping from a handler\):

```csharp
static void MapUri<T>(String uri, String[] contexts = null);
static void MapUri<T1, T2, T3>(String uri, String[] contexts = null);
static void MapUri(String uri, Type[] types, String[] contexts = null);
```

Handlers can be mapped to empty tokens:

```csharp
static void MapUri(String uri, String[] contexts = null);
static void MapUri(String uri, Boolean allowBlendingFromUri, Boolean allowBlendingToUri, String[] contexts = null);
```

Handlers with empty tokens are called with other handlers with the same empty token.

## Contexts

Token matching can be made more fine-grained by using contexts. They are composed of a list of strings that acts as a bit map when matched with other contexts. This list isn't materialized anywhere, it is just an un-written contract between the apps.

Starcounter came up with few predefined contexts that have shared meaning which can be understood by all app authors:
* `page` - a size factor context. Use it if the view contains full information that the app has about a concept. The view renders in multiple lines. The view is suitable to be attached in full pages about a concept;
* `thumbnail` - a size factor context. Use it if the view contains basic information that the app has about a concept. The view renders in multiple lines. The view is suitable to be attached in side information or teasers about a concept;
* `row` - a size factor context. Use it if the view contains basic information that the app has about a concept. The view renders in a single line. The view is suitable to be attached in lists;
* `icon` - a size factor context. Use it if the view contains the smallest unit of information that the app has about a concept. The view renders in a single element that is a link to a bigger view. The view is suitable to be attached in menu bars;
* `search` - a meta context. Use it if the view is suitable for search results;
* `app` - a meta context. Use it if the view represents the app itself, rather than a concept;

There are some compound contexts, that have a meaning of their own:

* `search, row` - a single-line row that is suitable for search results;
* `app, icon` - an icon that opens the main page of the app;

As an app author or solution owner, you can come up with your own contexts. Keep in mind that such contexts are unknown to other app authors, so they are less likely to attach view-models from other apps.

As a solution owner, you can replace the original contexts provided by an app author with custom contexts for fine tuning of the view-model attachment rules.

No context \(`null` value\) means **match any context**. Otherwise, two handlers are matched if source context contains same elements as destination context. Examples:

* Source context `{ "raw", "page" }` is NOT matched with `{ "search", "page" }`.
* Source context `null` is matched with `{ "raw", "page" }` and `{ "search", "page" }` and any other context.
* Source context `{ "page", "search" }` is matched with `{ "search", "page" }` and vice versa.

Consider contexts as an additional matching rule for handlers with the same token.

### Attaching Specific URIs

Specific URI is the handler URI with parameters supplied. For example, for the handler `/people/{?}` the specific URIs will be `/people/john`, `/people/bob`, etc. When calling `Blender.MapUri` for a specific URI, you should pass a mixed URI which indicates what handler it belongs to. For the previous examples, the mixed URI will be `/people/{john}` and `/people/{bob}`, so the parameter in specific URI is wrapped into curly brackets. `Blender` class has helper methods to construct such mixed URIs: `Blender.GetMixedUriFromHandlerAndParameters`, `Blender.TryGetMixedUriFromSpecific`. The last method tries to find corresponding handler for the given specific URI, which might not be determined correctly \(for example, in case when the corresponding handler is not yet registered\). Mixed URIs in `MapUri` calls are needed so the underlying handler for a specific URI can be identified. Other `Blender` methods like `UnmapUri`, `IsMapped`, etc. can still use specific URIs and not mixed.

## Dynamic Addition and Removal

Handlers that are registered and mapped to a token can dynamically have the mapping removed with `UnmapUri`:

```csharp
Blender.UnmapUri("/app4/{?}", token2);
Blender.UnmapUri("/twoparams3/{?}/{?}", token);
Blender.UnmapUri<MyClass>("/app1/{?}");
Blender.UnmapUri<Person>("/people/persons/34623"); // Unmapping specific URI (not that specific URI is used, not mixed).
Blender.UnmapUriForAllTokens("/app1/param1/{?}"); // Unmapping URI for all tokens.
```

`IsMapped` is used to check if a handler is mapped to a token:

```csharp
isInBlender = Blender.IsMapped("/noparam1", token3);
isInBlender = Blender.IsMapped<MyClass>("/noparam1");
isInBlender = Blender.IsMapped<Person>("/people/persons/34623"); // Checking if specific URI is blended.
```

To get a dictionary of all handlers and their respective tokens, use `ListAllByTokens` or `ListAllByUris`:

```csharp
static Dictionary<string, List<BlendingInfo>> ListAllByTokens();
static Dictionary<string, List<BlendingInfo>> ListAllByUris();
```

The dictionary returned by `ListAllByTokens` has the token as the key. The entries in the dictionary are sorted alphabetically based on the token. The same applies for `ListByAllUris` but for URIs.

To get the list of attachment candidates that are going to run for a given request URI:

```csharp
BlendingInfo[] GetRunCandidatesForUri(String uri);
```

To list registered attachment rules for a given URI:

```csharp
BlendingInfo[] ListByUri(String uri)
```

and the same but for a given token:

```csharp
BlendingInfo[] ListByToken(String token)
```

As you might have noticed, a special attachment rule information structure is used here: `BlendingInfo`.  
It contains the following methods/properties:

```csharp
String AppName; // Returns the application name to which the handler belongs.
Boolean IsActive; // Shows if this attachment rule is active.
String Token; // Returns the token.
String Uri; // Returns URI given in MapUri (which was either mixed, specific or handler URI).
String SpecificUri; // Returns a specific blended URI or null if MapUri was called with handler URI.
String HandlerUri; // Returns the handler URI.
Boolean IsSpecificUri; // Is it a specific URI attached, not a handler URI.
String[] Contexts; // Returns context (if any).
Boolean IsFromUriConverterOn; // Returns True if the "FromUriConverter" is set/allowed.
Boolean IsToUriConverterOn; // Returns True if the "ToUriConverter" is set/allowed.
```

## Parameters in Handlers

Handlers are allowed to have an arbitrary number of parameters. When there is at least one parameter, the conversion functions are used. The first converter translates handler arguments to token arguments, while the second converter does the opposite. Both converters are taking and returning an array of strings:

```csharp
Blender.MapUri("/twoparams1/{?}/{?}", token,
(String[] from) => {
   return from;
}, (String[] to) => {
   return to;
});
```

In the example above, converter passes handler parameters to token parameters, as is. There are signatures of `Blender.MapUri` with no converters, which means that parameters are simply passed through, like above.

Often it's needed to trigger attachment on a specific URI. To achieve this, the first converter should return non-null string array on certain parameters.

The URI matcher selects the most concrete URI handler possible, among all choices. It's not related to the number of parameters. So below, for `/op2/first/second`, the more concrete handler is `/op2/{?}/{?}` and not `/op1/{?}`.

 ```csharp
Handle.GET("/op1/{?}", (string first) =>
{
    return first;
});
Handle.GET("/op2/{?}/{?}", (string first, string second) =>
{
    return first + " and " + second;
});

Blender.MapUri("/op1/{?}", "token1");
Blender.MapUri("/op2/{?}/{?}", "token1");

Response resp = Self.GET("/op2/first/second"); //returns "/op2/{?}/{?}", not "/op1/{?}"
```

## Call Direction

You can specify the direction of which handlers should be called. This is needed to trigger attaching in a certain direction: from the handler or to handler. The direction is determined by the value that corresponding converter is returning: `null` converter or `null` string array returned in converter blocks the direction of the call. In case of zero parameters, there is a special `Blender.MapUri` overrides with corresponding boolean parameters to determine the allowed call directions.

```csharp
Blender.MapUri("/twoparams1/{?}/{?}", myToken, null, (String[] to) => {
    return to;
});
Blender.MapUri("/noparam1", token3, true, false);
```

In the first example above, handler "/twoparams1/{?}/{?}" should not trigger calls on the `myToken` token, but it can be called when attaching is triggered by other handlers on the same token.  
In the second example above, handler "/noparam1" can trigger other handlers on the same token, but can't be triggered by them.

## Same Handler - Different Tokens

Handlers can be mapped to multiple tokens:

```csharp
Blender.MapUri("/app1/{?}", token1);
Blender.MapUri("/app2/{?}", token1);
Blender.MapUri("/app3/{?}", token1);

Blender.MapUri("/app1/{?}", token2);
Blender.MapUri("/app4/{?}", token2);
```

Here, the handler `/app1/{?}` is mapped to the `token1` and `token2`.

When an app requests `Self.GET("/app1/xyz")`, both handlers mapped to `token1` and `token2` will be triggered.

## Separate attachment rules in JSON

You can describe attachment rules in JSON, separate from the application code. If the file `blend.json` is in the same directory as the starting application assembly - the file is parsed and the extracted attachment rules are applied right after the `Main` method. The same happens when the application restarts. With this mechanism, attachment rules can be changed for shipped application, where you only have the binaries. Here is an example of the content of a `blend.json` file:

```javascript
[
  {
     "Uri": "/someuri1",
     "Token": "sometoken1",
     "Contexts":[],
     "AllowFromDirection": true,
     "AllowToDirection": false
  },
  {
     "Uri": "/someuri2/{?}/bla",
     "Token": "",
     "Contexts":["context1","context2"],
     "AllowFromDirection": false,
     "AllowToDirection": true
  },
  {
     "Uri": "/someuri3/{name}/xxx",
     "Token": "sometoken3",
     "Contexts":["context1","context2", "context3"],
     "AllowFromDirection": true,
     "AllowToDirection": true
  }
]
```

Each element of the array contains the following fields:

```csharp
string Uri; // Mixed, specific or handler URI.
string Token; // Token.
string[] Contexts; // Contexts (if any).
bool AllowFromDirection; // Allows attachment rule calls from this URI.
bool AllowToDirection; // Allows attachment rule calls to this URI.
```

In short, the meaning of the above fields is the same as in `Blender.MapUri` functions variations.

