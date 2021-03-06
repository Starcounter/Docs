# HTML Views

## Introduction

This page explains what HTML views and view-models are and how they are used to create nested pages.

## HTML View Definition

An HTML view definition is a valid HTML document that contains at least one `<template>` tag and can be loaded with the [`<starcounter-include>`](https://github.com/Starcounter/starcounter-include) or [`imported-template`](https://github.com/Juicy/imported-template) web component. The `<template>` tag is cached on the client and stamped out as many times as needed.

{% hint style="info" %}
Read about the difference between `starcounter-include` and `imported-template` in the article "[&lt;starcounter-include&gt; and non-namespaced partial view-models](https://starcounter.io/starcounter-include-non-namespaced-partial-view-models/)".
{% endhint %}

The requirements and behaviors for the HTML view definition originate from the [HTML Imports](http://www.w3.org/TR/html-imports/) and [HTML Template](http://www.w3.org/TR/html-templates/) specifications. The most interesting aspects are:

* The document should be a valid HTML document and can contain anything that the browser allows
* The document may include dependencies such as stylesheets, HTML imports, scripts, etc.
* The content outside of the `<template>` tag is requested and executed once per session on partial load time
* The content inside the `<template>` is stamped out with the `<template>` itself every time the response is bound to a tree in the view-model
* Every node from the `<template>` will be stamped with an attached `model` property. Custom elements, template binding frameworks, or JavaScript scripts can use this data to populate the view

A partial HTML view is a view that has been nested as a part of a bigger view.

{% hint style="warning" %}
The Polymer version used by Starcounter was changed from Polymer 1 to Polymer 2 in Starcounter 2.4. This change requires certain aspects of the view to be adapted to work in Starcounter 2.4. Read more in this blog post: [https://starcounter.io/starcounter-2-4-gets-upgrade-polymer-2/](https://starcounter.io/starcounter-2-4-gets-upgrade-polymer-2/)
{% endhint %}

## View-Model Definition

A view-model definition is a valid JSON document with the filename extension `.json` that contains an `Html` property that points to the corresponding HTML view. It will also contain properties for the data that will be bound between the view and the database model.

The view-model definition can be combined with an optional view-model code-behind which defines the class name and contains input handlers. The filename extension for this file is `.json.cs`.

A partial view-model is a view-model that has been nested as a part of a bigger view-model.

A partial view-model together with its corresponding partial HTML view can simply be referred to as a partial.

## Using Partials

A partial can be accessed in two different ways:

1. Directly through an HTTP request that is handled in the application by a `Handle.GET`. The response to this request should be a complete HTML and JSON document. 
2. Through a blending point. A blending point is a combination of a `Self.GET` call, which acts as the partial view-model blending point, and a `<starcounter-include>` tag, which acts as a partial HTML view blending point. The `<starcounter-include>` tag determines where in the DOM tree the partial HTML view should be rendered.

## Examples

### Creating the Handler

No matter if the partial is accessed through an HTTP request from the browser or a `Self.GET` call, there always needs to be a handler that deals with the request. This handler returns a partial with the JSON and HTML. The handler, in its simplest form, looks something like this:

```csharp
Handle.GET("/your/partial/url", () => 
{
    return new YourPartialPage();
}
```

This handler only returns the JSON and HTML if the app uses the `HtmlFromJsonProvider` and `PartialToStandaloneHtmlProvider` [middmileware](../network/middleware.md).

### Adding the Blending Point

You can now create a blending point for this partial by attaching the partial to a parent partial using `Self.GET`. For example:

```csharp
mainPage.SubPage = Self.GET("/your/partial/url");
```

In the partial HTML view for the `mainPage` above, the HTML from the `SubPage` partial can be stamped in like so:

```markup
<starcounter-include partial="{{model.SubPage}}"></starcounter-include>
```

## Partial HTML View Example

A partial HTML view may look something like this:

{% code title="AddressPage.html" %}
```markup
<!--
Load you dependencies: <script>s, HTML Imports, CSS stylesheets etc.
Those dependencies will only be executed once when the partial is imported
-->
<!-- For example, to use Polymer's dom-bind custom element: -->
<link rel="import" href="/sys/polymer/polymer.html" />
<style>
    .myapp-address-entry-name {
        font-weight: bold;
    }
</style>
<!--
Everything from this template will be stamped into the parent page's DOM and <script>s will be executed for every instance of this partial.
All child nodes will receive a `model` property with JSON view-model,
which is automatically bound to the server-side.
-->
<template>
    <template is="dom-bind">
        <!--
            Now, the double curly brace syntax "{{}}" can be used 
            for two-way data bindings from the HTML view to serverside. 
            For example:
         -->
        <h2 class="myapp-address-entry-name">{{model.FullName}}</h2>
        <h4>Address</h4>
        <starcounter-include partial="{{model.Address}}">
        </starcounter-include>
    </template>
</template>
```
{% endcode %}

The example above uses Polymer's `dom-bind` element for data-bindings. Using Polymer in HTML views is not required, but we prefer it because it is a simple declarative alternative to imperative binding data to HTML elements using JavaScript.

