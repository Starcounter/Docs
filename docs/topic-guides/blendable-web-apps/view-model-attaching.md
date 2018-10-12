# View-model Attaching

{% hint style="working" %} Starcounter developers and invited partners have access to [Docs erratum](https://github.com/Starcounter/RebelsLounge/wiki/Docs-erratum) which lists important changes in the upcoming version. Please request your access. {% endhint %}

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

The process of defining the attachment rules and attaching the responses is handled by the `blend.json` file described below.

Attaching works well with [view composing](view-composing.md), which makes the views representing the attached view-models look like one app.

{% hint style="info" %}
Attaching was previously called "server-side blending"
{% endhint %}

## Attachment rule components

Attachment rules use tokens. Tokens are case insensitive strings. Handlers (identified by their URIs) with the same token are called on internal `Self.GET` calls or external URI that matches one of the handlers. Once the handler with a token is called, it will not trigger further calls mapped to that handler directly, only when a new `Self` call is made.

Token matching can be made more fine-grained by using contexts. They are composed of a list of strings that acts as a bit map when matched with other contexts.

Starcounter came up with few predefined **size factor contexts** that have shared meaning which can be understood by all app authors:

* `page` - the view contains full information that the app has about a concept. The view renders in multiple lines. The view is suitable to be attached in full pages about a concept;
* `thumbnail` - the view contains basic information that the app has about a concept. The view renders in multiple lines. The view is suitable to be attached in side information or teasers about a concept;
* `row` - the view contains basic information that the app has about a concept. The view renders in a single line. The view is suitable to be attached in lists;
* `icon` - view contains the smallest unit of information that the app has about a concept. The view renders in a single element that is a link to a bigger view. The view is suitable to be attached in menu bars;

{% hint style="info" %} Every attachment rule should have exactly one size factor context. {% endhint %}

In addition, there are also two kinds of predefined **meta contexts**:

* `search` - the view is suitable for search results;
* `app` - the view represents the app itself, rather than a concept;

There are some compound contexts, that have a meaning of their own:

* `search, row` - a single-line row that is suitable for search results;
* `app, icon` - an icon that opens the main page of the app;

As an app author, you should avoid coming up with your own contexts that are unknown to other app authors. In case you give a context that is only known to you, there isn't much chance that your view-model will be attached to a request that originates from another app.

As a solution owner, you can replace the original contexts provided by an app author with custom contexts for fine tuning of the view-model attachment rules.

Contexts are case-insensitive. No context \(`null` value\) means **match any context**. Otherwise, two attachment rules are matched if the source context has all elements of the destination context (superset matching). Examples:

* Source context `{ "raw", "page" }` is NOT matched with `{ "search", "page" }`.
* Source context `null` is matched with `{ "raw", "page" }` and `{ "search", "page" }` and any other context.
* Source context `{ "page", "search" }` is matched with `{ "search", "page" }` and vice versa.
* Source context `{ "page", "search", "raw" }` is matched with `{ "search", "page" }` but NOT vice versa.

Consider contexts as an additional matching criteria for handlers with the same token.

Attachment system needs to know if you try to add a rule for the handler with substituted parameter, or a handler template, or a non-parametrized handler:
1. Handler with substituted parameter. For example, for the handler `/people/{?}` the URI with substituted parameter will be `/people/john`, `/people/bob`, etc. You should have a rule in `blend.json` with parameters `/people/{john}` and `/people/{bob}` correspondingly. Mind the braces.
2. Handler template. For example, for the handler `/people/{?}` you should have a rule in `blend.json` with parameter `/people/{?}`.
3. Non-parametrized handler. For example, we have a handler without any parameters, for example, `/people/john`. You should should have a rule in `blend.json` with parameter `/people/john`.

## Separate attachment rules in JSON

Attachment rules should be described in JSON, separate from the application code. Upon application startup, `blend.json` is parsed and the extracted attachment rules are applied right after the `Main` method. The same happens when the application restarts. With this mechanism, attachment rules can be changed for shipped application, where you only have the binaries. 

{% hint style="warning" %} `blend.json` file should be placed in the same directory as the starting application assembly. In Visual Studio you can click on `Properties` of this file and set `Copy to Output Directory` to `Copy if newer`. {% endhint %}

Here is an example of the content of a `blend.json` file:

```javascript
[
  {
     "Uri": "/someuri1",
     "Token": "sometoken1",
     "Contexts":[],
     "AllowFromDirection": true,
     "AllowToDirection": false,
     "Active": true
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
     "Contexts":["context1","context2", "context3"]
  }
]
```

Each element of the array contains the following fields:

```csharp
string Uri; 
string Token;
Boolean Active;
string[] Contexts;
bool AllowFromDirection;
bool AllowToDirection;
```

Here is the explanation of each field:
* `Uri`: URI which is either the handler with substituted parameter, or a handler template, or a non-parametrized handler. Must be defined.
* `Token`: Attachment rule token described above. Default value is an empty string which is treated as "DefaultToken".
* `Active`: Defines default attachment rule rule activeness. Default value is "True".
* `Contexts`: Attachment rule contexts (if any). Default value is "null".
* `AllowFromDirection`: Allows attachment rule calls from this URI (it defines if this URI can trigger attachment process). That means if it's set to true, when an app calls `Self.GET()` on an attachment rule URI it will request other apps for content with the same contexts, otherwise it will not. Default value is "True".
* `AllowToDirection`: Allows attachment rule calls to this URI (it defines if this URI can be triggered during attachment process). That means if it's set to true, when other app requests on attachment rule with the same contexts as one of the attachment rules described in your app, your app will respond, otherwise it will not. Default value is "True".


## Troubleshooting

If the view attachment rules are not working as you expect, here is the list of things that might cause this:
- The contexts in the attachment rules do not match.
- The handler does not return a response of type `Json`.
- Your view-model `Json` object does not have `Html` property.
- Your attachment point is not expressed via `Self.GET`.
- There is no handler in the app that responds to the attachment point `Self.GET`.