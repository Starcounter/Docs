# Uniform Design System

## Introduction

Uniform design system is a user interface toolkit created by Starcounter. It allows authoring of blendable apps that have a common look and feel. 

Uniform is applied by the Starcounter host. It controls the look and feel of native HTML5, typical UI patterns, and components. It is a link between app authors and solution owners.

Unlike many design systems, Uniform is open-ended and themeable. It can be configured, extended or replaced in parts to achieve anything that is possible with HTML, CSS, and JavaScript.

## Mental model of client-side rendering by the Starcounter host

Starcounter host follows a layered design that allows controlling the look and feel of all apps. These layers are:

1. Native HTML5 and Shadow DOM
2. Uniform design system
3. Theme configuration through CSS custom properties
4. Replacing parts of Uniform with HTML, CSS, and JavaScript

### Native HTML5 and Shadow DOM

As an app author, you provide the views for your apps with a clear separation between the content part and the presentation part of the view.

In the content part, you provide the UI components that should be rendered by the Starcounter host as unstyled native HTML5 elements. 

To prevent the Starcounter host from controlling the rendering of a UI component, you use shadow DOM, preferably in a custom element. Shadow root creates a boundary that blocks the Starcounter host's attempt to force a look and feel on your components.

### Uniform design system

Uniform design system is the toolkit used by the Starcoutner host to give a common look and feel to all apps.

It consists of a CSS custom properties declaration, CSS reset, a pattern library and a component library.

The CSS custom properties declaration is a basic set of primitives. It expresses the fonts, colors and white space values that are used by Uniform.

The CSS reset, nicknamed "Underwear," overwrites the default user agent stylesheet. It provides the look of the native HTML5 elements such as the headings (H1-H6), links, inputs, buttons, and tables. For more details and preview, see [Underwear.css](https://github.com/Starcounter/underwear.css) on GitHub.

The pattern library is a set of CSS classes that implement visual hierarchy and give specific roles to native elements. The visual hierarchy is represented by sections, cards, titles and column layouts to work with. The specific roles of native elements are achieved by different kinds of alerts,  buttons, etc. For more details and preview, see [Uniform.css](https://github.com/Starcounter/uniform.css) on GitHub.

The component library is a set of custom elements that implement interactive UI controls. For the beginning, we provide a form item, a date picker, a data table and a pagination control. For more details and preview, see [Uniform components](https://github.com/Starcounter/uniform.css/tree/master/components) on GitHub.

The CSS reset is implicitly loaded for any blendable web app by the Starcounter app shell, so there is no need to load it explicitly in the views. 

To use the patterns and components, you must import them explicitly. They are used in the shadow root of the view container by wrapping distributed native HTML5 elements. That allows to serve real progressive enhancement, and what is crucial for a system of blended apps: keep app content's markup semantic and unopinionated, giving solution owner a way to apply own opinion on top of functional elements without changing of the app source code.

The names of the CSS custom properties, CSS classes, and custom elements have a `uni-` prefix.

Our [KitchenSink](https://kitchensink.starcounter.io/) app ([source](https://github.com/Starcounter/KitchenSink)) demonstrates the usage of Uniform design system as well as commonly used 3rd party custom elements that we can endorse, such as a Google map or a combo box.

### Theme configuration through CSS custom properties

Uniform design system comes with a modern-looking default theme.

As a solution owner, you can customize the look and feel of the theme by providing new values for the [CSS Custom Properties](https://developer.mozilla.org/en-US/docs/Web/CSS/Using_CSS_variables).

One of the tools in the [Blending](https://github.com/Starcounter/Blending) app suite is a CSS custom properties manager. It stores theme configurations based on CSS custom properties in the database. It provides simple to use way of providing a theme configuration conditionally in specific layouts.

For more sophisticated theming we would love to use native [CSS shadow parts and themes](https://meowni.ca/posts/part-theme-explainer/). Unfortunately, it's not supported yet by any browser.

To read more on customizing the design of the blended solution, please check [the dedicated page](customizing-solutions-design.md).

### Replacing parts of Uniform with HTML, CSS and JavaScript

We are using all those just-released-features of Web not just because we love using the bleeding edge of the latest technology. It is mostly to serve the best functionality, with solid base and interoperability without limiting the freedom of web development.

Uniform design system delivers an easy to use abstraction layer and building blocks to create unified, consistent UI and UX for your app and solution of apps without a hassle. However, because we are building our features on top of the open Web Platform, at any point, you are free to add, enhance, replace or hack the presentation layer using your  HTML, CSS or JavaScript code. The only technical limitation is Shadow DOM encapsulation.

## UI Kit

To make it easier for you to kickstart app design, we have made available a UI kit for Adobe XD that utilizes Uniform design system. The kit contains the assets used in Uniform and sample design files of an app \([BlendingEditor](https://github.com/Starcounter/Blending)\).

[Download the kit from our Google Drive](https://drive.google.com/drive/folders/1-71NMTdjGFo4IizBfKdvl2oi93z1RUoH?usp).

![Example from the UI kit](../../.gitbook/assets/uikit1.png)

![Sample design](../../.gitbook/assets/uikit2%20%281%29.png)
