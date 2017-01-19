# Making Apps Blendable

### Introduction
In Starcounter, applications should be built to handle one task, and handle that task really well. This leads to a situation where the developer would like to use several simple but powerful applications to build a more flexible application with many different functionalities. As you may know, the way to accomplish this in Starcounter is using mapping and blending. Here, the process of making applications easily blendable will be explained. The reason to do this is that it makes it easier for the author of an application to __visually integrate applications from other developers without breaking the functionality of the application__.

The writing here is partly built upon the concepts explained in the blog post <a href="https://starcounter.io/layout-compositions-html-partials/">Layout Composition for HTML Partials</a>. Reading that post will make it easier to understand the practices explained here.

### Step 1: Separate Functionality and Layout
To make applications blendable it's necessary to separate the functional elements from the layout elements. As explained in the post linked above, that makes it possible to specify blendable elements to other applications.

Here's an HTML template where the functional and layout elements are not separated:

{% raw %}
```html
<template>
  <style>
    .AnApp-article {
      border: 1px solid #F7F7F7;
      border-radius: 3px;
      margin: 1em;
      padding: 1em;
    }
  </style>
  <div style="background-color: #FF0000;">
    <h2>Subpage</h2>
  </div>
  <template is="dom-bind">
    <div class="AnApp-article">
      <p>
        Something bound with Polymer's dom-bind so you can use <span>{{mustaches}}</span>
      </p>
    </div>
  </template>
  <div class="AnApp-article">
    <my-custom-element>...</my-custom-element>
  </div>
</template>
```
{% endraw %}

In this template, the functional elements are:
1. `<h2>Subpage</h2>`
2. `<p>something bound with Polymer´s dom-bind so you can use <span>{{mustaches}}</span></p>`
3. `<my-custom-element>...</my-custom-element>`

Start by copying these elements to the top of the template. When that is done the top of the template should look like this:

{% raw %}
```html
<template>
  <h2>Subpage</h2>
  <template is="dom-bind">
      <p>
        Something bound with Polymer's dom-bind so you can use <span>{{mustaches}}</span>
      </p>
  </template>
  <my-custom-element>...</my-custom-element>
  <style>
    .AnApp-article {
      border: 1px solid #F7F7F7;
      border-radius: 3px;
      margin: 1em;
      padding: 1em;
    }
  </style>
```
{% endraw %}

The rest will look the same as before. Notice that the `<template is="dom-bind">` is included with the `<p>` element, but the `<div>` is not. If there would be more elements that would require the use of `dom-bind` for Polymer two-way bindings, then these would all be put within the same `<template is="dom-bind"`.

Now, wrap everything, except for the functional elements in a `<template is="starcounter-composition">`.

### 2. Assign Elements to Slots and Add Slot Elements
Currently, the elements are in two places. That's not the way it should be. To fix this issue, slot elements should be utilized. Simply replace the duplicate elements within `starcounter-composition` with `<content select='[slot="AnApp/ElementDescription"]'>`. If you use Shadow DOM v1 instead of v0, then <slot name="AnApp/ElementDescription> should be used.

* `<h2>Subpage</h2>` would become `<content select='["AnApp/header"]'>`
* `<p>something bound... </p>` would become `<content select='["AnApp/description"]'>`
* `<my-custom-element>...</my-custom-element>` would become `<content select='["AnApp/element"]'>`

In a real scenario, more descriptive names would be beneficial.

Now that slot elements are added, slot attributes should be added to the functional elements. The way to do this is by adding the attribute `slot="AnApp/ElementDescription"` to those elements.

At this point, the document should look like this:

{% raw %}
```html
<template>
  <h2 slot="AnApp/header">Subpage</h2>
  <template is="dom-bind">
      <p slot="AnApp/description">
        Something bound with Polymer's dom-bind so you can use <span>{{mustaches}}</span>
      </p>
  </template>
  <my-custom-element slot="AnApp/element">...</my-custom-element>
  <style>
    .AnApp-article {
      border: 1px solid #F7F7F7;
      border-radius: 3px;
      margin: 1em;
      padding: 1em;
    }
  </style>
  <template is="starcounter-composition">
    <div style="background-color: #FF0000;">
      <content select='[slot="AnApp/header"]'></content>
    </div>
    <template is="dom-bind">
      <div class="AnApp-article">
        <content select='[slot="AnApp/description"]'></content>
      </div>
    </template>
    <div class="AnApp-article">
      <content select='[slot="AnApp/element"]'></content>
    </div>
  </template>
</template>
```
{% endraw %}

In the case of a document that does not require a `starcounter-composition` because there are no layout elements, it would still be beneficial to add slot attributes to create semantic clarity. For example:
```html
<template>
  <h1>Some big text</h1>
  <h5>Some smaller text</h5>
</template>
```
Should be written as:
```html
<template>
  <h1 slot="AnApp/bigheader">Some big text</h1>
  <h5 slot="AnApp/smallerheader">Some smaller text</h5>
</template>
```

### 3. Rearrange styling
As a final step, the styling pertaining to the layout of the page (margin, padding etc.) should be put within the `starcounter-composition` together with styling that might be changed when blending. It's encouraged to put styles pertaining to the functional elements in a separate stylesheet and to use internal style sheets inside `starcounter-composition`. Regarding the naming of CSS classes, they should be prefixed with the app name to ensure clarity when blended with other applications.

The final result should look like this:

{% raw %}
```html
<template>
  <h2 slot="AnApp/header">Subpage</h2>
  <template is="dom-bind">
      <p slot="AnApp/description">
        Something bound with Polymer's dom-bind so you can use <span>{{mustaches}}</span>
      </p>
  </template>
  <my-custom-element slot="AnApp/element">...</my-custom-element>
  <template is="starcounter-composition">
    <style>
      .AnApp-article {
        border: 1px solid #F7F7F7;
        border-radius: 3px;
        margin: 1em;
        padding: 1em;
      }

      .AnApp-article__header {
        background-color: #FF0000;
      }
    </style>
    <div class="AnApp-article__header">
      <content select='[slot="AnApp/header"]'></content>
    </div>
    <template is="dom-bind">
      <div class="AnApp-article">
        <content select='[slot="AnApp/description"]'></content>
      </div>
    </template>
    <div class="AnApp-article">
      <content select='[slot="AnApp/element"]'></content>
    </div>
  </template>
</template>
```
{% endraw %}

### Conclusion
After following these steps, the application should look identical to how it was before. The only difference is that the functional elements will be exposed to blending and the slot names will be more semantic than in the previous case when the slot names would be set implicitly to a non-descriptive name. Making this change will also prepare the application better for future versions of Starcounter where the process of blending will be more streamlined and user-friendly.
