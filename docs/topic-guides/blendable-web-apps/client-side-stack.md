# Client-side stack

## Introduction

Apart from data sync with Palindrom, we propose the complete stack for maximum benefit of building thin client single page applications \(SPA\). The other important parts are: a template engine with two-way data binding and template loading infrastructure.

This stack lets you stay on the bleeding edge of development, using the new APIs such as Web Components that turn the web browser into a powerful, interactive presentation layer for apps.

All the client side libraries mentioned on this page come preinstalled with Starcounter. Starcounter auto-configures this stack for you.

The stack attempts to add as few new components as possible - established patterns and web standards are used when possible.

## Client-side libraries

### Data sync

Palindrom web apps use [Palindrom](https://github.com/Palindrom/Palindrom) for server-client data synchronization. Palindrom makes use of the JSON and [JSON-Patch](https://tools.ietf.org/html/rfc6902) web standards. It uses the JSON,  `XMLHttpRequest` and WebSocket APIs that are built into every modern web browser.

The implementation of JSON-Patch is provided by `json-patch-duplex.js`, which comes from the [fast-json-patch](https://github.com/Starcounter-Jack/JSON-Patch) library and provides tools for applying and generating patches.

The implementation of operational transformation in Palindrom is provided by [json-patch-queue](https://github.com/Palindrom/JSON-Patch-Queue), [json-patch-ot-agent](https://github.com/Palindrom/JSON-Patch-OT-agent), and [json-patch-ot](https://github.com/Palindrom/JSON-Patch-OT).

Palindrom and its dependencies are wrapped into the Custom Element [palindrom-polymer-client](https://github.com/Palindrom/palindrom-polymer-client) to make it easy to add. Adding this Custom Element into the DOM automatically loads and configures all the dependencies.

### Template engine

Since Palindrom provides a JavaScript object that reflects the server-side view-model, it needs a template engine to present the UI in the DOM. JavaScript libraries, such as D3 or React, can consume this object.

To render HTML templates in the DOM we recommend the [`dom-bind`](https://www.polymer-project.org/1.0/docs/devguide/data-binding) Custom Element that is part of the [Polymer](https://github.com/Polymer/polymer) library. It supports binding annotations using curly braces \(`{{}}`\), conditional fragments with [`dom-if`](https://www.polymer-project.org/1.0/docs/devguide/templates#dom-if) and loops with [`dom-repeat`](https://www.polymer-project.org/1.0/docs/devguide/templates#dom-repeat).

The solution is based on the Web Components APIs from the latest  HTML specifications. It's approved by all major browser vendors and is [widely adopted](https://www.webcomponents.org/). However, not everything is there yet, some browsers require the `webcomponents.js` polyfill to be loaded to support for these APIs.

### Template loading

To build modular apps out of many partial views, use the `imported-template` Custom Element. It loads the templates from separate HTML files and stamp them to the DOM, it plays well with Polymer's data-bindings and regular DOM APIs.

To make it easier to use with Starcounter, `imported-template` is wrapped in the [starcounter-include](https://github.com/Starcounter/starcounter-include) Custom Element, which also sets up the data binding between Palindrom and imported-template.

## The stack, visualized

The following chart shows the bird-eye's view on the client side libraries used.

![](../../.gitbook/assets/client-side-components.svg)

## Client side libraries distributed with Starcounter

The `StaticFiles` folder from Starcounter installation is automatically served as a static content folder. When Starcounter server receives a request for a static file, it searches for the file in all of the static content folders. The project folder has higher priority over internal folder.

See the file `ClientFiles\bower-list.txt` in your Starcounter installation directory (usually `C:\Program Files\Starcounter`) for the list of client side libraries bundled with your Starcounter instance, including their version numbers.

For Starcounter 2.4.0.5470, it looks like this:

```text
bower check-new     Checking for new versions of the project dependencies...
starcounter-clientfiles#2.1.1 C:\Work\Starcounter\StarcounterClientFiles\src\StarcounterClientFiles
├─┬ imported-template#3.2.0
│ └── juicy-html#4.0.0
├─┬ palindrom-client#5.0.0
│ ├── Palindrom#5.1.0
│ └── polymer not installed
├── palindrom-redirect#1.0.0
├─┬ polymer-source#2.5.0
│ ├── shadycss#1.1.1
│ └── webcomponentsjs#1.1.0
├─┬ starcounter-include#5.0.0
│ └── imported-template#3.2.0
├── underwear.css#0.0.7
├── uniform.css#0.0.9
└── webcomponentsjs#1.1.0
```

Read more on the [static file server page](../network/static-file-server.md).