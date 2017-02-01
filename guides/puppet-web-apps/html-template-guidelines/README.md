
# HTML Template Guidelines

In order to harness the full power of Starcounter, applications should be built to accomodate for complete visual and functional interoperability. To make this process easier for developers, we provide these guidelines for HTML templates which, when followed, will allow applications to achieve seamless visual integration with other applications.

For applications that have previously been developed without following these guidelines, take a look at the article [Convert Existing HTML Templates to starcounter-include](https://starcounter.io/making-apps-blendable/). To get a technical background, read the article [Layout compositions for HTML partials](https://starcounter.io/layout-compositions-html-partials/).

### Guideline 1: Separate Functionality and Layout

To make applications look great when running independently while allowing them to visually blend with other applications, it is beneficial to separate the functionality and layout in the HTML template. This is easily accomplished using the `<template is="starcounter-composition">` element.

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
<!-- div's count as functional elements if the child element is a template element -->
<div>
      <template is="dom-if" if="{{model.Person.Alive}}">
          <strong>Still going strong</strong>
      </template>
</div>

These element should all be put on the root level of the `dom-bind` template.
```

### Guideline 3: Add Slots

To give clearer semantic meaning to functional elements when mixing with other application, explicit slot names are used. This is how elements look when blending without explicit slot names: `<content select="[slot='0']"></content>`. With an explicit slot name, it becomes much clearer what kind of element it is: `<content select="[slot='MyApp/MainHeadline']"></content>`.

Slot names are added as attributes to the functional elements like so:
```html
<button slot="MyApp/SubmitButton" value="{{model.Submit::click}}">Submit</button>

<div slot="MyApp/LifeReport">
    <template is="dom-if" if="{{model.Person.Alive}}">
        <strong>Still going strong</strong>
    </template>
</div>
```

### Guideline 4: Create the Layout

As outlined in guideline 1, the layout of the HTML template should be included within the `<template is="starcounter-composition">`.

### Guideline 5: Apply Styling

Regarding styling, there are two ways to make the application easier to visually integrate with other apps:

1.

{% endraw %}
