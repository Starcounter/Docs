# Common look and feel of apps

## Introduction

Good user experience is significant for a successful solution of apps. An author of a single app aims to balance a good default look that matches other apps and to have distinctive features that are both easy to use and pleasing to the eye of the end user.

Starcounter offers a layered approach that allows app authors to adapt to a common look and feel for the typical parts of the UI and to provide their own components for the unique parts. Solution owners can theme or replace the look and feel without changing of the app source code.

## Mental model of client-side rendering by the Starcounter host

Starcounter host follows a standards-based layered approach that separates the responsibilities of the app author, the design system, and the solution owner. 

The full design consists of:

1. Content defined by native HTML
2. Common UI design patterns in Uniform Design System
3. Theme configuration through custom CSS properties
4. Extend or replace the theme with HTML, CSS, and JavaScript

### Content defined by native HTML

As an app author, you provide the views for your apps with a clear separation of the content and the presentation.

In the content part, you provide the native semantic HTML elements that can be rendered on the screen directly or with their appearance changed in the presentation layer.

### Common UI design patterns in Uniform Design System

As the basic way for providing a common look and feel in the presentation layer, Starcounter provides [Uniform Design System](https://uniform.starcounter.io/). It consists of theme primitives, CSS reset, and a library of user interface design patterns implemented using a stylesheet and components.

Uniform follows the inversion of control principle, which allows app authors to delegate the implementation and themeing of common parts of user interface to the Starcoutner host and the solution owner.

Unlike many design systems, Uniform is open-ended and themeable. It can be configured, extended or replaced in parts to achieve anything that is possible with HTML, CSS, and JavaScript.

The theme primitives define the fonts, sizes, colors, margins and other values used throughout the reset and components.

The CSS reset overwrites the default user agent stylesheet. It provides the look of the native HTML5 elements such as the headings (H1-H6), links, inputs, buttons, and tables. It is loaded by the Starcounter app shell, so there is no need to load it explicitly in the views. 

The stylesheet part of Uniform implements visual hierarchy using sections, cards, and titles. It also allows giving specific roles to native elements, for example, to designate "primary" or "danger" buttons.

The component part of Uniform is a set of custom elements that implement interactive UI controls. The first version includes a form item, a date picker, a data table and a pagination control. 

To use the Uniform stylesheet and components, you must import them explicitly in the presentation layer of the view.

The names of the theme primitives, CSS classes, and custom elements begin with a `uni-` prefix.

Our [UniformDocs](https://uniform.starcounter.io/) app demonstrates Uniform. It also shows how to use common third-party custom elements such as a Google Map or a Vaadin combo box element.

As an app author, you can implement specialized components in your app that use Uniform internally. This lets you adapt specialized components to the solution’s look and feel.

### Theme configuration through custom CSS properties

Uniform comes with a modern-looking default theme.

As a solution owner, you can customize the theme primitives by providing new values using [custom CSS properties](https://developer.mozilla.org/en-US/docs/Web/CSS/Using_CSS_variables). This allows changing the fonts, colors, and other values without replacing the stylesheet.

You can provide new values directly in a stylesheet in a custom composition. It requires some skill in CSS and HTML but allows for spectacular results such as providing a theme configuration only for a specific part of the app.

If you don't need that much of control and prefer a simple to use GUI, use the "Theme configurations" feature of the [Blending](https://github.com/Starcounter/Blending) app suite. It is an administration tool that stores the values of custom CSS properties in the database. It can provide theme configurations for specific layouts that affect all the Uniform components on the screen.

{% hint style="warning" %} Attaching custom CSS properties through "Theme configurations" sets them on the global scope - the `body` of the light DOM (https://github.com/Starcounter/Blending/issues/232). Providing custom CSS properties values in custom compositions attaches them in shadow DOM to the scope that you choose. {% endhint %}

{% hint style="info" %} For more sophisticated themeing we would love to use native [CSS shadow parts and themes](https://meowni.ca/posts/part-theme-explainer/). Unfortunately, it's not supported yet by any browser. {% endhint %}

### Extend or replace the theme with HTML, CSS and JavaScript

The rendering of apps in a Starcounter host is based on the inversion of control. This means that all parts of the presentation can be extended or replaced by the solution owner when needed. It allows you to completely change the look or behavior without changing the app source code.

As a solution owner, you are free to add, enhance, replace or hack the presentation layer using your HTML, CSS or JavaScript code. The only constraint is that the presentation is encapsulated in shadow DOM. 

For example, when all apps consistently use Uniform components, all the date pickers are presented using `<uni-date-picker>`. You can drastically change its look or behavior by creating your own custom element.

To apply the new behavior for all date pickers in the solution, you would simply replace the definition of `<uni-date-picker>`. The new definition must be provided by running an app that registers it at the same URI, for example `/sys/uniform.css/components/uni-date-picker/uni-date-picker.html`.

To apply the new behavior just for some date pickers, you would fork `<uni-date-picker>` and give it a new name, such as `<your-date-picker>`. To use it, you would change `<uni-date-picker>` to `<your-date-picker>` in selected custom compositions.

Create your own components by forking [`uni-element`](https://github.com/Starcounter/uniform.css/tree/master/components) or starting from scratch. In your components, you can use Uniform's custom CSS properties to apply common fonts, colors, and other theme primitives' values.

You must provide your own components to the Starcounter host by running an app that contains the static files. You can fork [StarcounterClientFiles app](https://github.com/Starcounter/StarcounterClientFiles/) for this purpose.

{% hint style="info" %} The same technique works for any custom element, not only Uniform one. {% endhint %}

The following diagram presents a decision tree of what technique of themeing to use in your case.

![Decision tree of themeing Uniform](../../.gitbook/assets/uniform-decision-tree.png)
