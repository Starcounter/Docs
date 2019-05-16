# Web Components

## Introduction

If you are writing web applications and you're allowed to use modern browser features, Web Components is a very powerful ally. A good introduction can be found on [WebComponents.org](https://www.webcomponents.org/introduction/), but we will give you a summary and how to use Web Components in Starcounter web apps.

## Summary of Web Components

Web Components are a set of new features in HTML 5 and beyond. Each feature is useable on its own and is not dependent on the other features, but together they really have the potential to change the way web applications are developed.

For instance, Web Components allow us to render a list of products and their barcodes by executing this code in a web browser:

```markup
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

This is only a fraction of what would be required in traditional frameworks, such as Backbone.

### `<template>` Tag

Starcounter let's you create JSON view-models that expose the current application state. To render it in the UI, it is the simplest to use a client-side framework that provides two-way data binding between HTML and JSON.

The `<template>` tag that was added to HTML5 as part of the Web Components spec family. It allows to define reusable chunks of HTML that work with any framework that supports this new web standard. Because it is now an established standard, we decided to use it to power [partials](html-views.md).

### Custom Elements and Shadow DOM

We suggest to keep the business and application logic on the server. Yet, there is still plenty of room to use the capabilities of a modern web browser to run the _view_ logic.

Custom Elements and Shadow DOM define how one can wrap a complex view functionality into a simple HTML tag, that requires no knowledge about the implementation details from the developer.

As can be seen in the above code snippet, to render a UPC-A barcode using SVG in a web browser, you can just import and use a MIT-licensed Custom Element called [x-barcode](https://github.com/girliemac/x-barcode). There is no learning curve and you don't need to care about the implementation details.

Thousands of open sourced Custom Elements can be found on [customelements.io](https://customelements.io).

### HTML Imports

Last but not least, the HTML Imports spec is the part of Web Components family that defines how to obtain fragments of code using a `<link>` tag. As can be seen on the above snippet, this is how a definition of a Custom Element is loaded.

In addition to loading Custom Elements, Starcounter takes benefit from HTML Imports to compose complex apps using small templates known as [partials](html-views.md), but you don't need to care much about that because it is done behind the scenes.

## Using Web Components

Web Components are loaded by browser with HTML Imports. HTML Import for Polymer in Starcounter should look like this:

```markup
<link rel="import" href="/sys/polymer/polymer.html" />
```

### Avoiding Loading the Same Files Multiple Times <a id="avoiding-loading-the-same-files-multiple-times"></a>

Browser loads HTML Imports only once, not like scripts and styles, which are loaded as many times as many references page has.

How does the browser know which of the components has already been imported then? Browser distinguishes Web Components one from another by their location.

This will be loaded twice and then lead to a Polymer error:

```markup
<link rel="import" href="/bower_components/polymer/polymer.html" /><link rel="import" href="/sys/polymer/polymer.html" />
```

It is a good practice to avoid double loading of external libraries. Hence, you need to use the same URL pattern as other apps. The recommended way is to use the pattern: `/sys/<dependency-name>/<dependency-files>`.

Starcounter has system folder called `StaticFiles` and located in the installation folder: `C:\Program Files\Starcounter\ClientFiles\StaticFiles`. There you can find `sys` folder, which includes some common components such as Polymer and Palindrom. You can also look at the code example under `PartialToStandaloneHtmlProvider` on the [middleware page](https://docs.starcounter.io/guides/network/middleware) to see what client side libraries are included in that directory by default.

The benefit of this is that you can rely on having a specific version of Starcounter to include a specific version of Polymer, Palindrom, etc.

### Adding External Dependencies to Your Apps <a id="adding-external-dependencies-to-your-apps"></a>

In order to add files that match this pattern, simply put a `sys` folder in your `wwwroot` folder that holds the static files for your project.

You should not do that automatically, but use Bower to install such dependencies. A correct Bower configuration consists of two files in your project: `.bowerrc` and \`bower.json.

#### .bowerrc <a id=".bowerrc"></a>

The `.bowerrc` file contains the Bower configuration. It specifies the destination directory and what dependencies should be ignored, because they are delivered with Starcounter. An example of this can be found in the [KitchenSink app](https://github.com/Starcounter/KitchenSink/blob/master/src/KitchenSink/.bowerrc).

To find the specific dependencies that are delivered with Starcounter, go to `C:\Program Files\Starcounter\ClientFiles\bower-list.txt`. For Starcounter 2.3.1.6694, it looks like this:

```text
bower check-new     Checking for new versions of the project dependencies...
sys#1.0.0 D:\repos\Starcounter\src\BuildSystem\ClientFiles
├─┬ PuppetJs#2.5.2 extraneous (latest is 3.0.4)
│ ├── fast-json-patch#1.2.2 (latest is 2.0.5)
│ ├── json-patch-ot#61992acdfd extraneous
│ ├─┬ json-patch-ot-agent#49d5d2cee5 extraneous
│ │ └── json-patch-queue#2a579f94db
│ └── json-patch-queue#2a579f94db
├── array.observe#0.0.1 extraneous
├─┬ bootswatch#3.3.7 (latest is 4.0.0-alpha.6)
│ └── bootstrap not installed
├── document-register-element#1.7.0
├── dom-bind-notifier#a43453ceb9
├─┬ imported-template#1.5.0
│ └── juicy-html#1.2.0
├── json-patch-ot#61992acdfd extraneous
├─┬ json-patch-ot-agent#49d5d2cee5 extraneous
│ └── json-patch-queue#2a579f94db
├── juicy-redirect#0.4.2 extraneous
├── object.observe#0.2.6 extraneous
├─┬ palindrom-client#4.0.0
│ ├── Palindrom#3.0.3 (3.0.4 available)
│ └── polymer#1.9.3 (latest is 2.0.2)
├── polymer#1.9.3 (latest is 2.0.2)
├─┬ puppet-client#4.0.0
│ ├── Palindrom#3.0.3
│ └─┬ polymer#1.9.3
│   └── webcomponentsjs#0.7.24
├── puppet-redirect#0.4.3 (latest is 0.5.0)
├── shadycss#1.0.5 extraneous
├─┬ starcounter-debug-aid#2.0.11
│ ├─┬ juicy-jsoneditor#1.1.0 (1.2.0 available)
│ │ ├── fast-json-patch#1.2.2 incompatible with ~1.0.0 (1.0.1 available, latest is 2.0.5)
│ │ ├── jsoneditor#5.5.11 (5.9.5 available)
│ │ └── polymer#1.9.3 (2.0.2 available)
│ └── polymer#1.9.3 (2.0.2 available)
├─┬ starcounter-include#3.0.0-rc.3
│ ├── imported-template#1.5.0
│ └── translate-shadowdom#0.0.5
└── webcomponentsjs#0.7.24 (latest is 1.0.10)
```

#### bower.json <a id="bower.json"></a>

`bower.json` file that keeps the list of your app's client side dependencies. This file should not be created and maintained manually. It should be modified using the command line tool: `bower init`, `bower install paper-dialog --save`.

A sample file can be found in the [KitchenSink app](https://github.com/Starcounter/KitchenSink/blob/master/src/KitchenSink/bower.json).

## Static File Server <a id="starcounter-static-file-server"></a>

The `StaticFiles` folder from Starcounter installation is automatically served as a static content folder. When Starcounter server receives a request for a static file, it searches for the file in all of the static content folders. The project folder has higher priority over internal folder.

Read more [here](https://docs.starcounter.io/guides/network/static-file-server).

Keep that in mind you can simply use another version of Polymer by putting it into your local `sys` folder. This will affect all other apps, though.

