# Sharing screen

Sharing data and sharing screen are two unrelated concepts that go the best together.

Sharing of the screen happens when a JSON view-model of an app A is merged as a sibling to the JSON view-model of an app B. 

It is desired for apps A and B to share data (see the previous page) at the same time as they share screen. This results in a user experience of a well-knit app suite. 

## How not to share the screen between apps

An app cannot request a partial from another apps, because internal `Self.GET` requests cannot cross the app boundary.

## How to share the screen between apps

Sharing of the screen happens automatically when you use URI mapping. It boils down into two apps having their partials mapped to the same URI.

## More details, please

The apps share the screen when a user request gets a response composed from multiple sub-responses. 

![Sharing screen gif](/assets/105_5.gif)

This is how it works. Your app has a URL handler. That handler makes a request to another URL handler in your app, which is mapped to a specific database table name.

If in the database there is my app running and it happens to also have a URL handler mapped to the same database table name as your app, Starcounter will call that handler whenever your handler is called. The response JSON and HTML of my app will be merged with the response of your app. The client receives single merged JSON view-model and a single merged HTML template.

## Four steps for sharing the screen

![Product details screenshot](/assets/Screenshot-2015-11-17-22.51.15.png)

Our [developer samples](http://github.com/StarcounterSamples) use this in multiple places. One of our favourite examples is how products display images, when [Products](https://github.com/StarcounterSamples/Products) app shares the screen with [Images](https://github.com/StarcounterSamples/Images) app.

_Screenshow shows Products app sharing screen with the Images app_

### 1. Make a sub-request

In the request handler (C#), make an internal request using `Self.GET`.

Attach the result of this request to the view-model of your app. This becomes the **JSON insertion point**.

<div class="code-name"><a href="https://github.com/StarcounterSamples/Products/blob/master/src/Products/Api/MainHandlers.cs">MainHandlers.cs</a></div>

```cs
Handle.GET("/products/product/{?}", (string objectId) => {
    StandalonePage master = (StandalonePage)Self.GET("/Products/standalone");

    master.CurrentPage = Db.Scope<Json>(() => {
      var partialUrl = "/Products/partials/product/" + objectId;
      return Self.GET<EditProductPage>(partialUrl);
    });

    return master;
});
```

In the above example, we make an internal request to `"/Products/partials/product/" + objectId` and `master.CurrentPage` is the JSON insertion point for the result.

### 2. Create an insertion point in HTML

In the request handler HTML template, put a `<starcounter-include>` element that displays the HTML response of the internal request.

This becomes the **HTML insertion point** for the response of the internal request.

<div class="code-name"><a href="https://github.com/StarcounterSamples/Products/blob/master/src/Products/Api/MainHandlers.cs">LauncherWrapperPage.html</a></div>

```html
<template>
    <template is="dom-bind">
        <starcounter-include 
          partial="{{model.CurrentPage}}"></starcounter-include>
    </template>
</template>
```

In the above example, `<starcounter-include partial="{{model.CurrentPage}}"></starcounter-include>` is the HTML insertion point for the result.

### 3. Create the sub-request response

The internal request handler needs to return a `Starcounter.Page` with a HTML template.

This is the request handler in Products app:

<div class="code-name"><a href="https://github.com/StarcounterSamples/Products/blob/master/src/Products/Api/MainHandlers.cs">MainHandlers.cs</a></div>

```cs
Handle.GET("/products/partials/product/{?}", (string objectId) => {
    return Db.Scope(() => {
        var page = new EditProductPage() { 
            Html = "/Products/viewmodels/EditProductPage.html" 
        };
        UInt64 _objectId = DbHelper.Base64DecodeObjectID(objectId);
        var product = DbHelper.FromID(_objectId) as Product;
        page.Data = product;
        page.RefreshData();
        return page;
    });
});
```

And this is the internal request handler in Images app:

<div class="code-name"><a href="https://github.com/StarcounterSamples/Images/blob/master/src/Images/Api/MainHandlers.cs">MainHandlers.cs</a></div>

```cs
Handle.GET("/images/partials/concept/{?}", (string objectId) => {
    return Db.Scope<Json>(() => {
        Something something = Db.SQL<Something>("SELECT o FROM Simplified.Ring1.Something o WHERE ObjectID = ?", objectId).First;
        ConceptPage a = new ConceptPage() {
            Html = "/Images/viewmodels/ConceptPage.html",
            Data = something
        };
        return a;
    });
});

Handle.GET("/images/partials/concept-vendible/{?}", (string objectId) => {
    return Self.GET("/images/partials/concept/" + objectId);
});
```

### 4. Map using `UriMapping`

As said above, the request handlers in both apps need to be mapped to the same database table, if we want to see the result on the shared screen.

Thus, Products app maps the internal request handler the following way:
(NOTE: Curly braces syntax in mapping is available in Starcounter starting from version `2.1.878`.)

<div class="code-name"><a href="https://github.com/StarcounterSamples/Products/blob/master/src/Products/Api/OntologyHooks.cs">OntologyHooks.cs</a></div>

```cs
UriMapping.OntologyMap("/Products/partials/product/{?}", 
    typeof(Product).FullName, null, null);
```

Similarly, Images app maps its internal request handler:

<div class="code-name"><a href="https://github.com/StarcounterSamples/Images/blob/master/src/Images/Api/MainHandlers.cs">MainHandlers.cs</a></div>

```cs
UriMapping.OntologyMap("/images/partials/concept-vendible/{?}", 
    typeof(Product).FullName, null, null);
```

## Shared sessions and transactions

The apps are displayed on the same page because Starcounter merges their response into a single JSON. When you create a Session in one of the handlers, the other view-models become attached to the same session.

Long running transactions are shared for the same session. This gives out-of-the box real-time experience to your app. Making change to a data property in one app will result in data updating on the screen for all apps that are running (see the Barcodes video).