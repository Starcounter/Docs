# Partials

This page describes the recommended way to create partials or, in other words, nested pages. Particularly, the following questions are answered:

1. What is as partial?
2. How to create a partial that uses Starcounter features?
3. What are the benefits that Starcounter gives for partials?

The examples take benefit of the features of [Web Components](/guides/web/introduction-to-web-components/) and work best in Google Chrome. Other browsers are supported using a [polyfill](http://webcomponents.org/polyfills/).

# Partials in Starcounter

It is good and common practice to divide the app into conceptually separated UI parts.

> We do not force developers to build apps our way, developers has complete freedom to use any C#/JS/HTML magic that they want.

Here is the way we recommend, that uses Starcounter features to make it easy, fast, and clean.

### Insertion point

A page of a Starcounter app can be accessed either:

1. directly - meaning that a web browser is the origin of the HTTP request handled by `Handle.GET`, in which case the response should be a complete HTML (and JSON) document,
2. or through a **insertion point** - meaning that in this or another app, there is both an `Self.GET` call (**view-model insertion point**) to the handler as well as accompanying [`<starcounter-include>`](https://github.com/Polyjuice/starcounter-include) tag (**HTML insertion point**) that indicates the place in the DOM tree where this response will be rendered, in which case the response should be a __partial HTML__.

### Partial

**Partial** is a conceptually separated UI part (like person form, map for address, etc.) returned as the response to an HTTP request. The request may come from the inside of an app as well as from the outside (through [mapping](http://starcounter.io/guides/apps/)).

Technically, a partial is a triplet consisting of the following:

- `.json` - view-model with a `Html` property, which value is the path to a Partial HTML
- `.json.cs` - optional view-model code-behind, which defines the class name, hints the data binding and contains input handlers
- `.html` - **Partial HTML**, the UI representation of the view-model. See below for the definition

### Partial HTML

**Partial HTML** is a valid HTML document that contains at least one `<template>` tag, which is cached on the client and stamped out as many times as needed.

This is a document that could be loaded with [`<imported-template>`](https://github.com/Juicy/imported-template).

So the requirements and behavior, comes from [HTML Imports](http://www.w3.org/TR/html-imports/) and [HTML Template](http://www.w3.org/TR/html-templates/) specs, the most interesting are:
- The document should be valid HTML document and can contain literally anything that the browser allows,
- The document may include dependencies such as stylesheets, HTML imports, scripts, etc.
- The content outside the `<template>` tag is requested and executed on partial load time (once per session).
- The content inside the `<template>` is stamped out with `<template>` itself - every time the response is bound to a tree in the view-model.
- Every node from `<template>` will be stamped with model property attached. Then custom element, template binding framework, or just a JS script may use this data to populate the view.

##### Example:
```html
<!-- 
    Load you dependencies: <script>s HTML Imports, CSS stylesheets
    That data will only be executed once, when the partial is imported
-->
<!-- For example you may like to use Polymer's `dom-bind` custom element -->
<link rel="import" href="/sys/polymer/polymer.html" />

<!-- 
    Everything from this template will be stamped into the parent page's DOM,
    and <script>s inside will be executed for every instance of this partial.
    All child nodes will receive `.model` property with JSON view model,
    which is bound to server-side automatically.
--> 
<template>
    <template is="dom-bind">
        <!-- 
            Now, you can use mustache {{}} syntax for two-way data binding
            HTML <-> serverside, for example:
         -->
        <h2>{{model.FullName}}</h2>
        <h4>Address</h4>
        <starcounter-include partial="{{model.Address}}"></starcounter-include>
    </template>
</template>
```
## Partial Usage
Once you create handler for your partial page:

```csharp
Handle.GET("/your/partial/url", () => //...
```
To use a partial out simply need, to place _insertion point_ in your view-model:

```csharp
mainPage.SubPage = Self.GET("/your/partial/url");
```
and _HTML insertion point_ in your HTML markup

```html
<starcounter-include partial="{{SubPage}}"></starcounter-include>
```
Thanks to that, HTML markup will get stamped from external file, and applicable view model will be attached.

## Advanced Partials usage
If you want to read in more detail how to use partials, how it technically work, how to extend it, or use outside check at [More on partials](/guides/web/more-on-partials/).

-----

# Partials and Mixed Apps

With small enhancement to simple partials shown above, Starcounter gives a huge experience improvement to the entire ecosystem of [mixed apps](http://starcounter.io/guides/apps/).

By using _partials_ the way we suggest, you support unlimited UI integration between all other apps, without any API pain.

## Definitions:

### Merged Partial
**merged partial** is a _partial_ from given main app (origin of `Self.GET` request) concatenated with partial sibling responses from other apps [mapped](http://starcounter.io/guides/apps/) to the same Starcounter ontology

##### Example:
**Merged JSON view model** send to the browser. Individual view-models are name-spaced. However, you should not bother about it too much - `<starcounter-include>` will resolve it for you.

```js
{
    "AppA": {
        "Html": "/AppA/viewmodels/APage.html",
        "Text$": "Some value that will be bound"
    },
    "AppB": {
        "Html": "/AppB/viewmodels/BPage.html",
        "Properties": "That will be bound to BPage HTML Elements"
    },
    "Launcher": {
        "That": "object may be attached by a Launcher/Layout app to improve visual appearance and mixing features"
    },
    // Path to Merged Partial HTML
    "Html": "/sc/htmlmerger?AppA=/AppA/viewmodels/APage.html&AppB=/AppB/viewmodels/BPage.html"
}
```

**Merged Partial HTML**. Individual Partial HTMLs are wrapped into `<imported-template-scope>`, so `<starcounter-include>`/`imported-template` could bind correct view-model to each part, then HTML is just concatenated together.

```html
<!-- a hint added automatically by Launcher, so imported-template could bind app's scope to the template -->
<imported-template-scope scope="AppA">
    <!-- App A Partial HTML -->
    <link rel="import" href="/sys/some-dependency/of-app-A.html" />

    <template>
        <template is="dom-bind">
            <div>Any node given in</div>
            <custom-element>App A HTML Partial</custom-element>
            <!-- it will point to the `Text` property in App A JSON view-model,
                 `AppA.Text` in merged view-model -->
            <p>{{model.Text}}</p>
        </template>
    </template>
</imported-template-scope>
<imported-template-scope scope="AppB">
    <!-- App B Partial HTML -->
    <link rel="import" href="/sys/other-dep/for-app-B.html">

    <template>
        <div>Nodes</div>
        <p>from App B HTML Partial</p>
        <appb-element>
            All those elements will receive reference to synced view-model in
            `model` property, which they could consume in JS+DOM manner
        </appb-element>
    </template>
</imported-template-scope>
```

## Partial Usage
In echosystem of Starcounter apps _partials_ are used and created exactly as in any other standalone Starcounter app, the only difference is that _merged partial_ may get included into _insertion point_, so the children list of HTML node `<starcounter-include partial="{{SubPage}}"></starcounter-include>` may contain nodes from other apps as well, and JSON subtree at this node may contain namespaced view models from other apps.

For more details check [More on partials](/guides/web/more-on-partials/).

## Features for Partials in Starcounter

To enhance experience even more, every Starcounter Launcher/Layout app may [extend](https://github.com/Polyjuice/Launcher/wiki/include-template-in-Polyjuice) `<starcounter-include>` with additional features like [layout composition](https://github.com/Polyjuice/Launcher/wiki/Layout-setup), so multiple results from different apps may get arranged in UI nicely.

On how `<starcounter-include>` is extended, you can read [here](https://github.com/Polyjuice/Launcher/wiki/include-template-in-Polyjuice).

<i class="fa fa-lightbulb-o button-icon-left"></i> To use/stamp HTML partial with Starcounter UI mixing, but without layout editing features, app dev could use just `imported-template`, for example `<template is="imported-template" model="{{SubPage}}" content$="{{SubPage.Html}}">`

<i class="fa fa-lightbulb-o button-icon-left"></i> To use/stamp HTML partial without any Starcounter features, app dev could use just `imported-template` custom element with path to given html file, for example `<template is="imported-template" content="/MyApp/file.html"></template>`
