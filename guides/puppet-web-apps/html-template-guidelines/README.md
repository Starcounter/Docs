
# HTML Template Guidelines

In order to harness the full power of Starcounter, applications should be built to accomodate for complete visual and functional interoperability. To make this process easier for developers, we provide these guidelines for HTML templates which, when followed, will allow applications to achieve seamless visual integration with other applications.

For applications that have previously been developed without following these guidelines, take a look at the article [Convert Existing HTML Templates to starcounter-include](https://starcounter.io/making-apps-blendable/). To get a technical background, read the article [Layout compositions for HTML partials](https://starcounter.io/layout-compositions-html-partials/).

### Guideline 1: Separate Functionality and Layout

To make applications look great when running independently while also allowing them to visually blend with other applications, it is beneficial to separate the functionality and layout in the HTML template. This is easily accomplished using the `<template is="starcounter-composition">` element.

The basic boilerplate of a Starcounter HTML template, which is created by adding a `Starcounter HTML template with dom-bind` file in Visual Studio, looks like this:

```html
<link rel="import" href="/sys/polymer/polymer.html">

<template>
    <template is="dom-bind">

    </template>
</template>
```

To separate functionality and layout in this html file, the element mentioned above, `<template is="starcounter-composition">` should be used. This element should contain the layout of the application while the `<template is="dom-bind">` should contain the functional elements. In code, this is how it looks:

```html
<link rel="import" href="/sys/polymer/polymer.html">

<template>
    <template is="dom-bind">
        <!-- functional elements go here -->
    </template>
    <template is="starcounter-composition">
        <!-- layout goes here -->
    </template>
</template>
```

A more in-depth explanation of `starcounter-composition` and why functionality and layout should be separated can be found in the [article linked above](https://starcounter.io/layout-compositions-html-partials/).

### Guideline 2: Define the Functional Elements

Functional elements are elements that either interact with the user visually, such as `<h1>`, `<p>` and `<span>` elements, or provides some kind of interaction between the user and the application, such as `<input>` and `<button>` elements. To start building the HTML template, these elements should be added to the `dom-bind` template as seen in Guideline 1.

To have complete clarity in what counts as a functional elements, here are some examples:

{% raw %}

**Simple elements:**

* `<button value="{{model.Submit::click}}">Submit</button>`
* `<h1>Very Important Headline</h1>`
* `<a href="/here/there">There</a>`
* `<starcounter-include partial="{{model.MyPartial}}"></starcounter-include>`

**Nested elements:**

There are two kinds of nested elements that count as functional elements:

1. Elements such as `<table>`, `<nav>`, and `<ul>` which are not just pure container elements but also provide some kind of functionality to it's children.
```html
<table>
      <tr>
          <th>Name</th>
          <th>Age</th>
      </tr>
      <tr>
          <td>Alice</td>
          <td>42</td>
      </tr>
</table>
```
2. All elements, including `<div>` elements, when the child is a `<template>`.
```html
<div>
      <template is="dom-if" if="{{model.Person.Alive}}">
          <strong>Still going strong</strong>
      </template>
</div>
```

All these element should be put on the root level of the `dom-bind` template.

### Guideline 3: Add Slots to Functional Elements

To give clearer semantic meaning to functional elements when mixing with other application, explicit slot names are used. This is how elements look when blending without explicit slot names: `<content select="[slot='MyApp/0']"></content>`, the zero is simply the index of the element in the template. With an explicit slot name, it becomes much clearer what kind of element it is: `<content select="[slot='MyApp/MainHeadline']"></content>`.

Slot names are added as attributes to the functional elements like so:

```html
<button slot="MyApp/SubmitButton" value="{{model.Submit::click}}">Submit</button>

<div slot="MyApp/LifeReport">
    <template is="dom-if" if="{{model.Person.Alive}}">
        <strong>Still going strong</strong>
    </template>
</div>
```

### Guideline 4: Create Layout in `starcounter-composition`

As outlined in guideline 1, the layout of the HTML template should be included within the `<template is="starcounter-composition">`.

The elements with a slot are grabbed into the `starcounter-composition` using the following syntax: `<content select="[slot='AppName/ElementName']"></content>`.

Consider the following template only containing functional elements:

```html
<link rel="import" href="/sys/polymer/polymer.html">

<template>
    <template is="dom-bind">
        <h1 slot="MyApp/ImportantHeadline">Very Important Headline</h1>
        <div slot="MyApp/LifeReport">
            <template is="dom-if" if="{{model.Person.Alive}}">
                <strong>Still going strong</strong>
            </template>
        </div>
        <input slot="MyApp/StatusInput" value="{{model.Status$::input}}" />
    </template>
</template>
```

To add a `starcounter-composition` to this template, something like this can be done:

```html
<link rel="import" href="/sys/polymer/polymer.html">

<template>
    <template is="dom-bind">
        <h1 slot="MyApp/ImportantHeadline">Very Important Headline</h1>
        <div slot="MyApp/LifeReport">
            <template is="dom-if" if="{{model.Person.Alive}}">
                <strong>Still going strong</strong>
            </template>
        </div>
        <input slot="MyApp/StatusInput" value="{{model.Status$::input}}" />
    </template>
    <template is="starcounter-composition">
        <content select="[slot='MyApp/ImportantHeadline']"></content>
        <div class="myapp-life-report">
            <content select="[slot='MyApp/LifeReport']"></content>
            <content select="[slot='MyApp/StatusInput']"></content>
        </div>
    </template>
</template>
```

### Guideline 5: Apply Styling to Avoid Conflicts and Allow Blending

Regarding styling, there are two ways to make the application easier to visually integrate with other apps:

1. Prefix the class with the name of the app. As outlined in [Avoiding CSS Conflicts](https://docs.starcounter.io/guides/mapping-and-blending/avoiding-css-conflicts/), the class should be prefixed with the name of the app to avoid CSS conflicts with classes from other apps.

2. Keep styling that will affect the layout inside the `starcounter-composition`. This includes CSS attributes such as `padding`, `margin`, `display` and `float`.

{% endraw %}
