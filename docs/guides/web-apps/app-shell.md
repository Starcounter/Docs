# App Shell

## Introduction

A view exposed from your app should follow the [inversion of control](https://en.wikipedia.org/wiki/Inversion_of_control) principle. It needs to import the dependencies and be able to render itself. It must not assume how and where it is inserted in the main document and must not expect the presence of any APIs other than `<starcounter-include>` and [DOM](https://dom.spec.whatwg.org/#shadow-trees).

The view is inserted in the main document by another view or, if it is at the root of the view-model, by the Starcounter app shell.

{% hint style="info" %}
 "App shell" is a term coined by Google that is explained well at their web page [The App Shell Model](https://developers.google.com/web/fundamentals/architecture/app-shell).
{% endhint %}

## Starcounter app shell

The Starcounter app shell is a minimalist, configurable HTML document that imports the prerequisites for all blendable web apps:

*  a polyfill for the browsers that do not have a native implementation of the Web Components specs
*  Palindrom JS library for communication with the server
*  the `<starcounter-include>` custom element
*  Bootstrap, a CSS framework

On page load, the app shell establishes the Palindrom session with the server, obtains the JSON view-model tree, inserts the root view using `<starcounter-include>` and connects the root view to the view-model.

## Obtaining and invoking the app shell

The default content of the app shell is tied to the StarcounterClientFiles version that is bundled with your Starcounter installation. The current version can be previewed in the [StarcounterClientFiles repository on GitHub](https://github.com/Starcounter/StarcounterClientFiles/blob/3.x/src/StarcounterClientFiles/wwwroot/sys/app-shell/app-shell.html). You can upgrade or downgrade the app shell by installing another version of StarcounterClientFiles from the App Warehouse.

The app shell is invoked automatically for any `Json` with a `Html` property that is returned from your app. The only provision is that your app needs to register the following middleware:

```csharp
Application.Current.Use(new HtmlFromJsonProvider());
Application.Current.Use(new PartialToStandaloneHtmlProvider());
```

The [middleware](../network/middleware.md#middleware-classes) page explains the APIs presented above. In short, the `HtmlFromJsonProvider` middleware fetches the view associated with a view-model, and `PartialToStandaloneHtmlProvider` wraps the view in the app shell.

## Altering the app shell

It is strongly advised to use the default, unmodified app shell. The default app shell is the lowest common denominator for all apps. If you change it, you might introduce side effects that make your app incompatible with other vendor's apps.

If you need to modify the app shell, follow the instructions on the [middleware](../network/middleware.md#partialtostandalonehtmlprovider) page.  


