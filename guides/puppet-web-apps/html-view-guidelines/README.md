
# HTML Views Guidelines

In order to harness the full power of Starcounter, applications should be built to accomodate for complete visual and functional interoperability. To make this process easier for developers, we provide these guidelines for HTML views which, when followed, will allow applications to achieve seamless visual integration with other applications.

To get a technical background, the article [Layout compositions for HTML partials](https://starcounter.io/layout-compositions-html-partials/) covers more of the underlying ideas of what's presented here.

### Guideline 1: Separation of Presentation and Content

To make applications look great when running independently while also allowing them to visually blend with other applications, it is beneficial to separate the presentation and the content. This is accomplished using the `<template is="starcounter-composition">` element.

The basic boilerplate of a Starcounter HTML view, which is created by adding a `Starcounter HTML template with dom-bind` file in Visual Studio, looks like this:

```html
<link rel="import" href="/sys/polymer/polymer.html">

<template>
    <template is="dom-bind">

    </template>
</template>
```

To separate the presentation and content in this file, the element mentioned above, `<template is="starcounter-composition">` should be used. This element should contain the presentation of the HTML view while the `<template is="dom-bind">` should contain the content. In code, this is how it looks:

```html
<link rel="import" href="/sys/polymer/polymer.html">

<template>
    <template is="dom-bind">
        <!-- content goes here -->
    </template>
    <template is="starcounter-composition">
        <!-- presentation goes here-->
    </template>
</template>
```

The content are the elements that either contain information for the user, such as `<h1>`, `<span>`, and `<a>`, or elements that create some kind of interaction between the user and the application, such as `<button>` and `<input>`.

The content of the HTML view is distributed to the presentation using a Shadow DOM concept called slots, as explained in guideline 4.

### Guideline 2: Defining the Content

When defining the content of a view, it is important to keep in mind that the slotable elements, which are the ones that will be exposed for blending, have to be on the root of the HTML view. Consider the following HTML view:

```html
<template>
    <h1>A Headline</h1>
    <p>Some text for te user to read</p>
    <button>A button to click</button>
</template>
``` 

Here, every element is at the root of the document and will be exposed for blending after they are attached to slots and distributed in the Shadow DOM. In some cases, putting every element on the root of the view, like we do in the example above, might not be desired. Then, the goal should still be to place as many elements as possible on the root of the view, especially elements like `<img>`, `<table>`, and custom elements that are visually obtrusive and might require blending to create a high level of interoperability. 

Additionally, there might be situations where the developer would like to have a higher level of abstraction on some of his or her content. For example, consider this pagination bar:

<div>
<style>
.pagination {
    display: inline-block;
}

.pagination a {
    color: black;
    float: left;
    padding: 8px 16px;
    text-decoration: none;
}

.pagination a:hover {
    background-color: lightgray;
}
</style>

<div class="pagination">
  <a href="#">&laquo;</a>
  <a href="#">1</a>
  <a href="#">2</a>
  <a href="#">3</a>
  <a href="#">4</a>
  <a href="#">5</a>
  <a href="#">6</a>
  <a href="#">&raquo;</a>
</div>
</div>

Here, it would not make sense to break it up into the respective parts because they do not have any real meaning when presented individually. It would rather make sense to put the parent on the root level so that the whole bar is exposed for blending, and not the individual buttons.

### Guideline 3: Attaching the Content to Slots

{% raw %}

To give clearer semantic meaning to the content of one application when mixing with other applications, explicit slot names are used. When not using explicit slot names, the elements at the root will get implicit slot names and look like this: `<content select="[slot='MyApp/0']"></content>`, the zero is simply the index of the element in the view. With an explicit slot name, it becomes much clearer what kind of element it is: `<content select="[slot='MyApp/MainHeadline']"></content>`.

Slot names are added as attributes to the elements like so: `<button slot="MyApp/SubmitButton">Submit</button>`.

### Guideline 4: Create the Presentation in `starcounter-composition`

As outlined in guideline 1, the presentation of the HTML view should be included within the `<template is="starcounter-composition">`.

The following syntax is used to distribute the content in the Shadow DOM: `<content select="[slot='AppName/ElementName']"></content>`.

Consider the following HTML view with three elements at the root:

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

To add a `starcounter-composition` to this HTML view, something like this can be done:

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

Here, the elements are distributed in the way that the view will look when no blending is applied or when the app is running in a standalone mode. 

**Note:**
For Shadow DOM v1, the `slot` tag should be used instead of the `content` tag. Thus, the tag above would take this shape: `<slot name="AppName/ElementName"></slot>`.

### Guideline 5: Apply Styling to Avoid Conflicts and Allow Blending

Regarding styling, there are two ways to make the application easier to visually integrate with other apps:

1. Prefix the all class names with the name of the app. As outlined in [Avoiding CSS Conflicts](https://docs.starcounter.io/guides/mapping-and-blending/avoiding-css-conflicts/), the class should be prefixed with the name of the app to avoid CSS conflicts with classes from other apps.

2. Keep styling that will affect the layout inside the `starcounter-composition`.

{% endraw %}
