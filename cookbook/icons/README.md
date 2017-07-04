# Icons

The preferred icon format is SVG.

Why SVG? With SVG you can do things like change the color `:hover`, animate, etc.

Why not other formats? We do not use icon fonts, because they have conflicts when multiple apps use them, they require CSS hacks to restyle them. We do not use PNGs because they are look bad on retina screens and are hard to change, style and animate.

Every app should have a black and white icon that appears:

* as the package icon in the Warehouse
* as the link to the default entrypoint in the [Launchpad](https://github.com/starcounterapps/launchpad)

## Icon size

The default icon size should be **24x24** pixels. 

This happens to be the default size in Polymer ([`iron-icon`](https://elements.polymer-project.org/elements/iron-icon)). It is one of the standard icon sizes in many icon packs, such as [Visualpharm](http://www.visualpharm.com/articles/icon_sizes.html) and [Icons8](https://icons8.com/articles/choosing-the-right-size-and-format-for-icons/).

## How to insert an SVG button icon and restyle it

**Warning**: The icon should not have a `fill` attribute anywhere inside of the SVG file. Otherwise, it will be harder to style it from CSS.

In the `dom-bind` part of your partial define a `<button>` that includes an inline SVG or and inline SVG with `<use>` reference to an external file:

```html
<button class="CompositionEditor-resetbutton" slot="CompositionEditor/icon" on-click="toggleEditing" aria-label="Brush icon">
    <svg modified$="{{model.layoutModified$}}" active$="{{model.editingMode$}}" viewBox="0 0 74 76" version="1.1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink">
        <use xlink:href="/CompositionEditor/images/layout-editor.svg#Page-1"/>
    </svg>
</button>
```

Now, the above SVG and its paths can be styled using CSS in the `starcounter-composition` part. The below code includes a fix that makes SVG content styleable in browsers other than Chrome:

```html
<template is="starcounter-composition">
    <style>
        .icon-holder {
            width: 24px;
            height: 24px;
            background: #FFFFFF;
            font-size: 0;
            padding: 3px;
            box-sizing: border-box;
            fill: #000000;
        }

        .icon-holder ::content button {
            width: 100%;
            height: 100%;
        }

        .compositioneditor-shadowcss-fix > .icon-holder button {
            width: 100%;
            height: 100%;
        }

        .icon-holder ::content [active],
        .icon-holder:hover {
            cursor: pointer;
            fill: #E0115F;
        }

        .compositioneditor-shadowcss-fix > .icon-holder [active],
        .compositioneditor-shadowcss-fix > .icon-holder:hover {
            cursor: pointer;
            fill: #E0115F;
        }
    </style>
    <div class="icon-holder">
        <content select="[slot='CompositionEditor/icon']"></content>
    </div>
</template>
<script>
    (function() {
        // :host and ::content selectors are not supported by WC v0 polyfill
        // so a custom script is needed to make it work in browsers other than Chrome
        // TODO: Re-evaluate the need once webcomponents.js#V1 is shipped.
        // see https://github.com/StarcounterPrefabs/Launcher/pull/266#issuecomment-273434475
        if (window.ShadowDOMPolyfill) {
            var template = document.currentScript.previousElementSibling;
            template.parentNode.classList.add('compositioneditor-shadowcss-fix');
        }
    })();
</script>
```
Reference example:

- [CompositionEditor `Master.html`](https://github.com/Starcounter/CompositionEditor/blob/41f49ddf3337579dfacd6b11639f8105c2f9aeae/src/CompositionEditor/wwwroot/CompositionEditor/Master.html)
- [CompositionEditor `layout-editor.svg`](https://github.com/Starcounter/CompositionEditor/blob/41f49ddf3337579dfacd6b11639f8105c2f9aeae/src/CompositionEditor/wwwroot/CompositionEditor/images/layout-editor.svg)