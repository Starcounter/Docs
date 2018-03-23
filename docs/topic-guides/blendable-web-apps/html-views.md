# HTML views

## Introduction

This page explains how view-models and HTML views are used to create blendable web apps.

## HTML view 

An HTML view is a valid HTML document that contains at least one `<template>` tag and can be loaded with the [`<starcounter-include>`](https://github.com/Starcounter/starcounter-include) or [`imported-template`](https://github.com/Juicy/imported-template) web component. The `<template>` tag is cached on the client and stamped out as many times as needed.

{% hint style="info" %}
Read about the difference between `starcounter-include` and `imported-template` in the article "[&lt;starcounter-include&gt; and non-namespaced partial view-models](https://starcounter.io/starcounter-include-non-namespaced-partial-view-models/)".
{% endhint %}

The requirements and behaviors for the HTML views originate from the [HTML Imports](http://www.w3.org/TR/html-imports/) and [HTML Template](http://www.w3.org/TR/html-templates/) specifications. The most interesting aspects are:

* The document should be a valid HTML document and can contain anything that the browser allows
* The document may include dependencies such as stylesheets, HTML imports, scripts, etc.
* The content outside of the `<template>` tag is requested and executed once per session on partial load time
* The content inside the `<template>` is stamped out with the `<template>` itself every time the response is bound to a tree in the view-model
* Every node from the `<template>` will be stamped with an attached `model` property. Custom elements, template binding frameworks, or JavaScript scripts can use this data to populate the view

A partial HTML view is a view that has been nested as a part of a bigger view.

{% hint style="warning" %}
The Polymer version used by Starcounter was changed from Polymer 1 to Polymer 2 in Starcounter 2.4. This change requires certain aspects of the view from earlier versions of Starcounter to be adapted to work in Starcounter 2.4. Read more in this blog post: [https://starcounter.io/starcounter-2-4-gets-upgrade-polymer-2/](https://starcounter.io/starcounter-2-4-gets-upgrade-polymer-2/)
{% endhint %}

## View-model class

A view-model class is a valid JSON document with the filename extension `.json` that contains an `Html` property that points to the corresponding HTML view. It will also contain properties for the data that will be bound between the view and the database model. Starcounter compiles view-models defined in `.json` files to a `Json` class in C#.

The view-model class can be extended with an optional code-behind in a file with the extension `.json.cs`. The code-behind defines the class name and contains input handlers.

A partial view-model is a view-model that has been nested as a part of a bigger view-model.

A partial view-model together with its corresponding partial HTML view can simply be referred to as a "partial".

## Rendering HTML views

A view is rendered when the client obtains a view-model with a view. This can happen in two ways:

1. On a root level, through an HTTP request that is handled in the application by a `Handle.GET`. The response to this request is a view-model. The Starcounter [app shell](app-shell.md) renders it automatically. 
2. On a nested level, through a pair of insertion points in a view-model and a view:
   - a blended view-model insertion point is represented by a property in the view-model. The property inserts the result of a `Self.GET` call to a partial view-model in a parent view-model.
   - a blended view insertion point is represented by a `<starcounter-include>` tag in HTML. The tag inserts the partial view in the parent view.

## Examples

### Creating the handler

No matter if the view-model is accessed through an HTTP request from the browser or a `Self.GET` call, there always needs to be a handler that deals with the request. This handler returns a view-model. The handler, in its simplest form, looks something like this:

```csharp
Application.Current.Use(new HtmlFromJsonProvider());
Application.Current.Use(new PartialToStandaloneHtmlProvider());

Handle.GET("/your/partial/url", () => 
{
    return new YourPartialPage();
}
```

The view-model will be rendered by the Starcounter app shell only if the app uses the `HtmlFromJsonProvider` and `PartialToStandaloneHtmlProvider` [middleware](../network/middleware.md).

### Adding the blended view-model insertion point

You can now create a blended view-model insertion point for this partial by inserting the view-model in a parent view-model using `Self.GET`. For example:

```csharp
mainPage.SubPage = Self.GET("/your/partial/url");
```

In the view for the `mainPage` above, the view from the `SubPage` property can be rendered by using:

```markup
<starcounter-include view-model="{{model.SubPage}}"></starcounter-include>
```

## View example

A HTML view may look like this:

{% code-tabs %}
{% code-tabs-item title="AddressPage.html" %}
```markup
<!--
Load your dependencies: <script>s, HTML Imports, CSS stylesheets etc.
Those dependencies will only be executed once when the partial is imported
-->
<!-- For example, to use Polymer's dom-bind custom element: -->
<link rel="import" href="/sys/polymer/polymer.html">
<style>
    .myapp-address-entry-name {
        font-weight: bold;
    }
</style>
<!--
Everything from this template will be stamped into the parent page's DOM and <script>s will be executed for every instance of this view.
All child nodes will receive a `model` property with view-model,
which is automatically bound to the server-side.
-->
<template>
    <dom-bind>
        <template>
            <!--
                Now, Polymer's double curly brace syntax "{{}}" can be used 
                for two-way data bindings between the view and the view-model
             -->
            <h2 class="myapp-address-entry-name">{{model.FullName}}</h2>
            <h4>Address</h4>
            <starcounter-include view-model="{{model.Address}}">
            </starcounter-include>
        </template>
    </template>
</template>
```
{% endcode-tabs-item %}
{% endcode-tabs %}

The example above uses Polymer's `dom-bind` element for data-bindings. Using Polymer in views is not required, but we prefer it because it is a simple declarative alternative to imperative binding data to HTML elements using JavaScript.

