# Customizing Solution's Design


Building your app or solution design the way we suggest does not only help you create a consistent look and feel fast and easily. It also gives you means to customize it later, by applying global themes, switching color & configuration schemes, or replacing the definition of every component at once.

This page covers ways of customizing your **entire** app's  (solution's) design at once. You can always customize/edit every single view individually, by changing its composition, see [View Composing](view-composing.md)


## Themes

Theming is the very high-level way to change the look and feel of many UI elements and patterns with a single step, by just attaching already created theme to given layout.

This allows you to provide one package that can completely redefine styling for individual UI components no matter how deeply they are composed in the view.

We are trying to keep our theming solution closest to the upcoming standard: [CSS Shadow Themes & Parts](https://meowni.ca/posts/part-theme-explainer/). Unfortunately, there is no polyfill that is performant enough, so we are forced to fall back to the slightly more opinionated solution.

This requires an element to actively look up for a theme when it's defined. That's how Vaadin custom elements work, and that's how we will keep Uniform Components working. Therefore, as long as you stick to [Starcounter Design System](starcounter-design-system.md), you don't need to worry about those implementation details.

#### But why good old Bootstrap-like themes do not work?

That's because we put all presentation into Shadow DOM, and old, global CSS cannot pierce through this boundary.

We actually consider it a fundamental feature. It prevents the cases when tiny little CSS rule inserted in one part of big enterprise solution breaks the styling of something in the distant part, probably unrelated or delivered by a different party.

#### How to use it?

If you are not into the code, you can start the [Blending](https://github.com/Starcounter/Blending) apps suite, and use a [GUI to manage the themes](https://github.com/Starcounter/Blending/blob/master/docs/gui.md). There you can pick a layout, then add URLs to the themes you would like to add.

If you are more into the code, all you need is a URL to your theme, then you can edit layout's Shadow DOM composition and add a theme using
```html
<enlighted-link rel="import" name="your-theme" href="/path/to/theme.html"></enlighted-link>
```
`<enlighted-link>` is nothing more than HTML Import (`<link rel="import">`) that works in Shadow DOM.

#### How it works?

When a theme is inserted, it loads to the main document a piece of markup and CSS, which is later picked up by custom elements definitions, so they could look different.


{% hint style="warning" %} Themes are global, meaning they are not scoped to the subtree of partial views it's inserted in. By applying the theme, you are changing the look of all applicable components on the page.{% endhint %}

#### What is the theme?

It is an HTML document as any other that could be loaded via HTML Imports. That allows a theme to bundle more themes. For example, `material-theme.html` could import `vaadin-material-date-picker.html`, `vaadin-material-combo-box.html`, plus some global CSS variables and rules to make native elements look more material-ish.

But what is most important, it's a document that contains a set of CSS rules to be picked up by custom element definition to style its `::parts`. Until it's standardized, it's tightly related to individual element's implementation. See [Vaadin's `vaadin-material-theme/.../vaadin-date-picker.html`]( https://github.com/vaadin/vaadin-material-theme/blob/master/vaadin-date-picker.html) for example.

#### Where to get the theme from?

Usually, from your custom elements vendor, Starcounter will publish a list of known themes for Uniform components, you can check [Vaadin themes](https://vaadin.com/themes) as well.

Naturally, you can create your own, and ship it with an app.


## Collections of CSS Custom Properties

Slightly lower-level way to customize existing design is by using CSS Variables (Custom Properties). Well-behaved design systems, themes, custom elements and even individual CSS rules usually use CSS Variables which could be used to customize it.

This allows to change color schemes, or configure other values like font sizing, margins, etc.

Again [Blending](https://github.com/Starcounter/Blending) apps suite [delivers GUI](https://github.com/Starcounter/Blending/blob/master/docs/gui.md#css-variables-manager) to do that without touching any code. You can as well add it to you composition code manually.

{% hint style="warning" %} Attaching CSS Variables' collection through Blending app, allows you to set them on light DOM. Currently to the global scope - `body` (https://github.com/Starcounter/Blending/issues/232). While setting it in view's composition, attaches them in Shadow DOM to the scope you choose. {% endhint %}

## Overwriting `uni-elements` definitions

That is the lowest-level approach, that is slightly hacky, but this hackiness allows you to do any kind of changes and re-design.

It allows you to completely change not only look but also the behavior of any UI element.

Assuming, the solution of apps is created consistently using Starcounter Design System and Uniform Components, for example, all date pickers are presented using `<uni-date-picker>`.

Then if you want to drastically change its look or even behavior, you can do so, by creating your own custom element with the same name, but with more sophisticated behavior.

For example, an airline company could have high demands on the UX and feature set of the date picker. With Starcounter, it could still use an app which was created with regular date picker needs.

The requirements are that initial design author has stuck to the conventions of Starcounter Design System, and the new date picker supports the same API, which in case of Uniform Components is just nothing more than being able to wrap the native HTML elements. That means no specific API, just exchange data with native HTML API.

You can fork [`uni-element`](https://github.com/Starcounter/uniform.css/tree/master/components) or start from scratch.

Then you need to add this definition to any apps `/sys` folder under the same path as an overwritten element, for example `/sys/uniform.css/components/uni-date-picker/uni-date-picker.html`. You can use a fork of  [StarcounterClientFiles app](https://github.com/Starcounter/StarcounterClientFiles/) for this purpose.


{% hint style="info" %} The same technique works for any custom element, not only Uniform one. {% endhint %}

## Need more?

If any of the above fulfill your needs for general customization and theming, first of all, please let us know.

If you need to change more, you can always edit every view's presentation using [View Composition](view-composition.md), you can re-arrange HTML, change CSS, replace custom elements with your own, do any kind of HTML+CSS+JS magic you need.

If the apps were made according to our guidelines, you should have entire presentation available there for you to change. Nothing but presentation - the content, functionality, and data-binding should be safe in the light DOM, out of reach for encapsulated composition.
