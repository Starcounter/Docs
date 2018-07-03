# Starcounter Design System

## Introduction

Starcounter Design System is a set of tools maintained by Starcounter to allow app developers to create blendable web apps with a common look and feel. These tools are: Underwear.css, Uniform.css, custom elements and themes.

## Is this really a design system?

Sarcounter Design System follows the “convention over configuration” paradigm. If you want to quickly set up an app prototype, you can do it by copy and paste from the reference material, as if Starcounter Design System was your comprehensive toolbox.

Unlike many design systems, the Starcounter Design System is open ended and themable. It can be extended to make anything that is possible with HTML, CSS and JS. It can be adapted by a designer working with BlendingEditor to the extent allowed by Shadow DOM.

## Mental model in 4 layers

Starcounter Design System consists of 4 layers:

1. Underwear.css - a modern-looking default stylesheet
2. Uniform.css - UI patterns in a separated presentation layer
3. Component library based on custom elements
4. Themes based on CSS custom properties

### Underwear.css - a modern-looking default stylesheet

For the most basic layer, Starcounter provides a CSS library [Underwear.css](https://github.com/Starcounter/underwear.css). This small CSS file is based on Normalize.css. It provides a modern-looking alternative for the default user agent stylesheet.

As unopinionated as it gets, Underwear.css provides look and feel only for native HTML. It does not include any CSS classes.

Underwear.css is implicitly loaded for any blendable web app by the Starcounter app shell, so there is no need to load it explicitly in the views.

Underwear.css is configurable using CSS custom properties.

Head to the [demo page of Underwear.css](https://starcounter.github.io/underwear.css/) for a preview.

### Uniform.css - UI patterns in a separated presentation layer

The developer of a blendable web app should apply specific UI patterns, such as sections, cards, alerts, primary buttons in the presentation layer. The presentation layer, can be altered by a designer without touching the app source code through [View composing](view-composing.md).

For this layer, Starcounter Design System uses the CSS library [Uniform.css](https://github.com/Starcounter/uniform.css) to implement basic UI patterns. Apps that use these patterns will get a common look and feel that is changeable in compositions.

Uniform.css must be explicitly imported in any view shadow root that uses it.

Uniform.css is configurable using CSS custom properties.

Head to the [demo page of Uniform.css](https://starcounter.github.io/uniform.css/) for a preview.

### Component library based on custom elements

Your blendable web app can use any custom elements. However, finding or implementing a good custom element and creating a view-model for it takes time.

Our [UniformDocs](https://github.com/Starcounter/UniformDocs) app demonstrates implementation of some commonly used components, such as a datepicker or a combo box, using custom elements that we can endorse.

Head to the [hosted version of UniformDocs](https://uniform.starcounter.io/) for a preview.

### Themes based on CSS custom properties

Thanks to CSS custom properties, it is possible to externally configure values such as color schemes to Underwear.css, Uniform.css, your app custom stylesheets, and custom elements.

One of the tools in the [Blending](https://github.com/Starcounter/Blending) apps suite  is a CSS custom properties manager, that stores  color schemes in the database. The CSS custom properties can be stored in collections and provided conditionally in specific layouts.

## UI Kit

To make it easier for you to kickstart app design, we have made available a UI kit for Adobe XD that utilizes Starcounter Design System. The kit contains the assets used in Uniform.css and sample design files of an app \([BlendingEditor](https://github.com/Starcounter/Blending)\).

[Download the kit from our Google Drive](https://drive.google.com/drive/folders/1-71NMTdjGFo4IizBfKdvl2oi93z1RUoH?usp).

![Example from the UI kit](../../.gitbook/assets/uikit1.png)

![Sample design](../../.gitbook/assets/uikit2%20%281%29.png)

