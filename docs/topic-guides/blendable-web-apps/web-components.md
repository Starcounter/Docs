# Web Components

## Introduction

If you are writing web applications and you're allowed to use modern browser features, Web Components is a powerful ally. A good introduction can be found on [MDN](https://developer.mozilla.org/en-US/docs/Web/Web_Components) and [WebComponents.org](https://www.webcomponents.org/introduction/), but we will give you a summary and how to use Web Components in Starcounter web apps.

## Summary of Web Components

Web Components are a set of new features in HTML 5 and beyond. Each feature is useable on its own and is not dependent on the other features, but together they really have the potential to change the way web applications are developed.

For instance, Web Components allow us to render a list of products and their barcodes by executing this code in a web browser:

```markup
<link rel="import" href="/sys/x-barcode/src/x-barcode.html">

<template is="dom-repeat" items="{{model.Products}}">
  <h1>{{item.Name}}</h1>
  <x-barcode code="{{item.Barcode}}"></x-barcode>
</template>
```

This is only a fraction of what would be required in traditional frameworks, such as Backbone.

### `<template>` tag

The `<template>` tag that was added to HTML5 to define reusable chunks of HTML that work with any framework.

### Custom Elements and Shadow DOM

We suggest to keep the business and application logic on the server. Yet, there is still plenty of room to use the capabilities of a modern web browser to run the _view_ logic.

Custom elements and Shadow DOM define how one can wrap a complex view functionality into a simple HTML tag, that requires no knowledge about the implementation details from the developer.

As can be seen in the above code snippet, to render a UPC-A barcode using SVG in a web browser, you can just import and use a MIT-licensed custom element called [x-barcode](https://github.com/girliemac/x-barcode). There is no learning curve and you don't need to care about the implementation details.

Thousands of open sourced custom elements can be found on [webcomponents.org](https://webcomponents.org).

### HTML Imports

Last but not least, the HTML Imports spec is the part of Web Components family that defines how to obtain fragments of code using a `<link>` tag. As can be seen on the above snippet, this is how a definition of a custom element is loaded.

In addition to loading custom elements, Starcounter takes benefit from HTML Imports to compose complex apps using small [views](html-views.md), but you don't need to care much about that because it is done behind the scenes.

## Using Web Components

Web Components are loaded by browser with HTML Imports. HTML Import for Polymer in Starcounter should look like this:

```markup
<link rel="import" href="/sys/polymer/polymer.html">
```

### Avoiding loading the same files multiple times {#avoiding-loading-the-same-files-multiple-times}

Browser loads HTML Imports only once, not like scripts and styles, which are loaded as many times as many references page has.

How does the browser know which of the components has already been imported then? Browser distinguishes Web Components one from another by their location.

This will be loaded twice and then lead to a Polymer error:

```markup
<link rel="import" href="/bower_components/polymer/polymer.html"><link rel="import" href="/sys/polymer/polymer.html">
```

It is a good practice to avoid double loading of external libraries. Hence, you need to use the same URL pattern as other apps. The recommended way is to use the pattern: `/sys/<dependency-name>/<dependency-files>`.

Some common components such as Polymer and Palindrom are included in Starcounter. See the [Client-side stack](client-side-stack.md) page for information what client side libraries are included in that directory by default.

The benefit of this is that you can rely on having a specific version of Starcounter to include a specific version of Polymer, Palindrom, etc.

### Adding external dependencies to apps {#adding-external-dependencies-to-your-apps}

In order to add files that match this pattern, simply put a `sys` folder in your `wwwroot` folder that holds the static files for your project.

You should not do that automatically, but use Bower to install such dependencies. A correct Bower configuration consists of two files in your project: `bower.json` and `.bowerrc`.

#### bower.json {#bower.json}

The `bower.json` file keeps the list of your app's client side dependencies. This file should not be created and maintained manually. It should be modified using the command line tool: `bower init`, `bower install paper-dialog --save`.

A sample file can be found in the [UniformDocs app](https://github.com/Starcounter/UniformDocs/blob/master-2.4/src/UniformDocs/bower.json).

#### .bowerrc {#.bowerrc}

Keep in mind that you can use another version of Polymer by putting it into your local `sys` folder. This will affect all other apps, though. To prevent that, use a `.bowerrc` file.

The `.bowerrc` file contains the Bower configuration. It specifies the destination directory and what dependencies should be ignored, because they are delivered with Starcounter. An example of this can be found in the [UniformDocs app](https://github.com/Starcounter/UniformDocs/blob/master-2.4/src/UniformDocs/.bowerrc).

