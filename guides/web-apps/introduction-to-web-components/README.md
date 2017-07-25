# Introduction to Web Components

If you are writing web applications and you're allowed to use modern browser features, Web Components is a very powerful ally. A very good introduction can be found on [WebComponents.org](https://www.webcomponents.org/introduction/), but we will provide you with a quick summary.

![web components breakdown](/assets/web-components-breakdown.png)

Web Components are a set of new features in HTML 5 and beyond. Each feature is useable on its own and is not dependent on the other features, but together they really have the potential to change the way web applications are developed.

For instance, Web Components allow us to render a list of products and their barcodes by executing this code in a web browser:

<div class="code-name">BarcodesPage.html</div>

{% raw %}
```html
<link rel="import" href="/sys/x-barcode/src/x-barcode.html">
<link rel="import" href="/sys/palindrom-client/palindrom-client.html">
<template id="root">
  <template is="dom-repeat" items="{{model.Products}}">
    <h1>{{item.Name}}</h1>
    <x-barcode code="{{item.Barcode}}"></x-barcode>
  </template>
</template>
<palindrom-client ref="root"></palindrom-client>
```
{% endraw %}

This is only a fraction of what would be required in traditional frameworks, such as Backbone.

## `<template>` tag

Starcounter let's you create JSON view-models that expose the current application state. To render it in the UI, it is the simplest to use a client-side framework that provides two-way data binding between HTML and JSON.

The `<template>` tag that was added to HTML5 as part of the Web Components spec family. It allows to define reusable chunks of HTML that work with any framework that supports this new web standard. Because it is now an established standard, we decided to use it to power [partials](/guides/web-apps/html-views).

## Custom Elements and Shadow DOM

We suggest to keep the business and application logic on the server. Yet, there is still plenty of room to use the capabilities of a modern web browser to run the *view* logic.

Custom Elements and Shadow DOM define how one can wrap a complex view functionality into a simple HTML tag, that requires no knowledge about the implementation details from the developer.

As can be seen in the above code snippet, to render a UPC-A barcode using SVG in a web browser, you can just import and use a MIT-licensed Custom Element called [x-barcode](https://github.com/girliemac/x-barcode). There is no learning curve and you don't need to care about the implementation details.

Thousands of open sourced Custom Elements can be found on [customelements.io](https://customelements.io).

## HTML Imports

Last but not least, the HTML Imports spec is the part of Web Components family that defines how to obtain fragments of code using a `<link>` tag. As can be seen on the above snippet, this is how a definition of a Custom Element is loaded.

In addition to loading Custom Elements, Starcounter takes benefit from HTML Imports to compose complex apps using small templates known as  [partials](/guides/web-apps/html-views/), but you don't need to care much about that because it is done behind the scenes.
