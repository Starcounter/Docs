# Client-Side Blending

When merging responses from different apps with [server-side blending](server-side-blending), the views are stacked on top of each other. In most cases, that's not what we want. Instead, we would like to compose elements in the views to make the result look like one page. That's what client-side blending does:

![Blending comparison](/assets/SimpleBlendingDemo.PNG)

In the example above, there are two apps, PetList and MedicalRecord. By default, they are stacked on top of each other. This makes it seem like they are not connected, when they actually are. With client-side blending, we can move the table of examinations into the card from the PetList app and make it look like one coherent concept. In essence, we are **changing the layout but not the content** to combine apps that were not explicitly built to share screen. This is done without touching the source code of the individual apps.

This ability of modifying the layout of views coming from different apps is crucial, especially when working with many apps. Without it, there would just be a stack of views with no meaningful layout.  

## Layout and Content Separation

Client-side blending works by replacing or modifying the default layout. For this to work, content and layout has to be separated. [Shadow DOM](https://www.html5rocks.com/en/tutorials/webcomponents/shadowdom/) handles this separation - the content is in light DOM and the layout is in Shadow DOM. 

The structure of this separation looks like this:
```html
<template>
    <h1 slot="myapp/main-heading">My heading</h1>
    <button slot="myapp/left-button">Go left</button>
    <button slot="myapp/right-button">Go right</button>
    <template is="declarative-shadow-dom">
        <style>
            .myapp-direction-controls {
                display: flex;
                justify-content: center;
            }
        </style>
        <slot name="myapp/main-heading"></slot>
        <div class="myapp-direction-controls">
            <slot name="myapp/left-button"></slot>
            <slot name="myapp/right-button"></slot>
        </div>
    </template>
</template>
```

Here, the content of the view is defined on the root level and the layout is defined inside the `declarative-shadow-dom`. With client-side blending, the code in the `declarative-shadow-dom` can be modified or replaced.

## Composing Layouts

With the example at top from MedicalProvider and PetList we have two layouts that we would like to blend:

<div class="code-name">PetList layout</div>

```html 
<style>
    @import '/PetList/style.css';
</style>
<div class="pet-list-wrapper">
    <div class="pet-list-wrapper__row">
        <slot name="petlist/details-name"></slot>
        <slot name="petlist/details-age-and-animal"></slot>
    </div>
    <div class="pet-list-wrapper__row">
        <slot name="petlist/details-owner-name"></slot>
        <slot name="petlist/details-weight"></slot>
    </div>
    <slot name="petlist/details-list-link"></slot>
</div>
```

<div class="code-name">MedicalProvider layout</div>

```html
<slot name="medicalrecord/records-list-headline"></slot>
<slot name="medicalrecord/records-list-table"></slot>
```

When these two layouts are merged with server-side blending, the resulting layout looks like this:

```html
<style>
    @import '/PetList/style.css';
</style>
<div class="pet-list-wrapper">
    <div class="pet-list-wrapper__row">
        <slot name="petlist/details-name"></slot>
        <slot name="petlist/details-age-and-animal"></slot>
    </div>
    <div class="pet-list-wrapper__row">
        <slot name="petlist/details-owner-name"></slot>
        <slot name="petlist/details-weight"></slot>
    </div>
    <slot name="petlist/details-list-link"></slot>
</div>
<slot name="medicalrecord/records-list-headline"></slot>
<slot name="medicalrecord/records-list-table"></slot>
```

The layout from MedicalRecord is appended at the end of the PetList wrapper. To blend it, we will move the MedicalRecord table and headline into the `div class="pet-list-wrapper"` and expand the width of the wrapper to fit the table:

```html
<style>
    @import '/PetList/style.css';
    .pet-list-wrapper {
        max-width: 750px;
    }
</style>
<div class="pet-list-wrapper">
    <div class="pet-list-wrapper__row">
        <slot name="petlist/details-name"></slot>
        <slot name="petlist/details-age-and-animal"></slot>
    </div>
    <div class="pet-list-wrapper__row">
        <slot name="petlist/details-owner-name"></slot>
        <slot name="petlist/details-weight"></slot>
    </div>
    <slot name="medicalrecord/records-list-headline"></slot>
    <slot name="medicalrecord/records-list-table"></slot>
    <slot name="petlist/details-list-link"></slot>
</div>
```

We have now produced the result shown in the image above; the view from the MedicalRecord app has been neatly integrated with the view from PetList.

## Creating Layouts

The tool for creating these layouts is the [CompositionEditor](https://github.com/starcounterapps/CompositionEditor). 

## Providing Layouts

Layouts are provided by the [CompositionProvider](https://github.com/starcounterapps/compositionprovider) when the responses are merged. 