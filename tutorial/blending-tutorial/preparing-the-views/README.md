# Preparing the Views

When blending, we combine view-models and views from different apps. Combining view-models is easy:

```json
"PetList": {
    // view-model from PetList 
},
"MedicalRecordProvider": {
    // view-model from MedicalRecordProvider
}
```

Combining views is harder because views become useful when arranged in a meaningul way. 

To enable arranging views in meaningful ways, as we will do in the last step [Client-Side Blending](../client-side-blending), we have to define **which elements can be rearranged** and create a **default arrangement** that applies when there are no other views to combine with.

Declarative Shadow DOM helps us define this. 

The [guides](/guides/web-apps/html-view-guidelines) describes how to use Declarative Shadow DOM. For our purposes it's enough to know that we should separate content and presentation and that root level elements should be given a `slot` attribute.

## Adapting Views to Declarative Shadow DOM

When blending elements from different views, the elements on the root appear as `slot` elements in [CompositionEditor](https://github.com/starcounter/compositionEditor), the tool we use to rearange elements. These elements have names. By default, their names look like `<slot name="MyApp/0"></slot>`, where the `0` represents the index of the element in its view. 

By explicitly attaching elements to slots, we can give them meaningful names.  

### Locate the Files

The only files we will touch in this step are in the `src/[app-name]/wwwroot/[app-name]/views` directories, as marked in orange:

![Views marked in file strucutre](/assets/FileStrucutreBlending.PNG)

### Adapting Simple Views

We will start out with these three files as they are the simplest ones to adapt:

* `RecordsList.html`
* `PetListPage.html` 
* `RecordSummary.html`

#### RecordsList

{% raw %}

<div class="code-name">RecordsList.html</div>

```html
<template>
    <template is="dom-bind">
        <h2>Examinations</h2>
        <table class="table">
            <thead>
                <tr>
                    <th>Date</th>
                    <th>Clinic</th>
                    <th>Patient</th>
                    <th>Note</th>
                </tr>
            </thead>
            <tbody>
                <template is="dom-repeat" items="{{model.Records}}" as="record">
                    <tr>
                        <td>{{record.Date}}</td>
                        <td>{{record.Clinic}}</td>
                        <td>{{record.Patient.Name}}</td>
                        <td>{{record.NoteFromExaminer}}</td>
                    </tr> 
                </template>
            </tbody>
        </table>
    </template>
</template>
```

The only elements that are possible to rearrange are the ones on the **root level**. In this case, they are the `h2` and `table`. If we would wrap these two elements in another element, such as a `div`, they would always be arranged with the `h2` first, immidiately followed by the `table`, without the ability to rearrange. 

To see how it looks in the editor, follow these steps:

* Build and run MedicalRecordProvider, either through Visual Studio or by using `build.bat` and `run.bat`

* Run CompositionEditor and CompositionProvider
    1. Go to `http://localhost:8181/#/databases/default/appstore`
    2. Find CompositionEditor and CompositionProvider
    3. If you can't find them, uncheck the box "Show Compatible" at the top of the page
    4. Download the latest version of CompositionEditor and CompositionProvider
    5. Go to `http://localhost:8181/#/databases/default`
    6. Click "Start" for both of them
    7. It should look like this:

    ![Right apps running](/assets/AppsRunningCorrectly.PNG)

* Find the current layout
    1. Go to `http://localhost:8080/MedicalRecordProvider`
    2. Click `Ctrl + E`
    3. Click the textfield in the top of the editor and choose `[partial-id="/sc/htmlmerger?MedicalRecordProvider=/MedicalRecordProvider/views/RecordsList.html"]`
    4. It should look like this:

    ![Correct Layout](/assets/CorrectLayout.PNG)

As you can see, these are the elements:

```html
<slot name="MedicalRecordProvider/0"></slot>
<slot name="MedicalRecordProvider/1"></slot>
```

`<slot name="MedicalRecordProvider/0"></slot>` is the `h2` element and `<slot name="MedicalRecordProvider/1"></slot>` is the `table` element.

With more elements, this becomes harder to manage. Because of that, we assign explicit slot names.

This is how we would assign explicit slot names in this view:

```html
<h2 slot="medicalrecordprovider/records-list-headline">Examinations</h2>
<table slot="medicalrecordprovider/records-list-table" class="table">...</table>
```

The elements in the editor will now look like this:

```html
<slot name="medicalrecordprovider/records-list-headline"></slot>
<slot name="medicalrecordprovider/records-list-table"></slot>
```

For this view, that's all we have to do. 

#### RecordSummary

<div class="code-name">RecordSummary.html</div>

```html
<template>
    <template is="dom-bind">
        <h3>Summary</h3>
        <p>Number of examinations: {{model.NumberOfExaminations}}</p>
        <p>Percentage of routine visits: {{model.PercentageOfRoutineVisits}}</p>
        <p>Most visited clinic: {{model.MostVisitedClinic}}</p>
        <p>Latest Examination: {{model.LatestExamination}}</p>
    </template>
</template>
```

As with `RecordList`, `RecordSummary` only needs explicit slot names since all its rearrangeable elements are on the root level. The result looks like this:

<div class="code-name">RecordSummary.html</div>

```html
<template>
    <template is="dom-bind">
        <h3 slot="medicalrecordprovider/summary-headline">Summary</h3>
        <p slot="medicalrecordprovider/summary-examination-number">Number of examinations: {{model.NumberOfExaminations}}</p>
        <p slot="medicalrecordprovider/summary-percentage">Percentage of routine visits: {{model.PercentageOfRoutineVisits}}</p>
        <p slot="medicalrecordprovider/summary-most-visited">Most visited clinic: {{model.MostVisitedClinic}}</p>
        <p slot="medicalrecordprovider/summary-latest">Latest Examination: {{model.LatestExamination}}</p>
    </template>
</template>
```

#### PetListPage

<div class="code-name">PetListPage.html</div>

```html
<template>
    <template is="dom-bind">
        <template is="dom-repeat" items="{{model.Pets}}" as="pet">
            <div class="pet-list-wrapper">
               <h4>{{pet.Name}} - {{pet.Animal}}</h4>
               <a href$="/PetList/Details?name={{pet.Name}}">View details</a>
            </div>
        </template>
    </template>
</template>
```

This view is a simple list. Since the elements in the list should not be rearranged, we will wrap it in a div with a slot attribute:

<div class="code-name">PetListPage.html</div>

```html
<template>
    <template is="dom-bind">
        <div slot="pet-list/list">
            <template is="dom-repeat" items="{{model.Pets}}" as="pet">
                <div slot="pet-list/list-item" class="pet-list-wrapper">
                   <h4>{{pet.Name}} - {{pet.Animal}}</h4>
                   <a href$="/PetList/Details?name={{pet.Name}}">View details</a>
                </div>
            </template>
        </div>
    </template>
</template>
```

### Adapting Complex views

The views in the previous section only needed explicit slots since all their rearrangeable elements were on the root level. Most of the time, that's not the case.

#### PetDetails

Consider this file:

<div class="code-name">PetDetails.html</div>

```html
<template>
    <template is="dom-bind">
        <div class="pet-list-wrapper">
            <div class="pet-list-wrapper__row">
                <h3>{{model.Name}}</h3>
                <h4>{{model.Age}} years old {{model.Animal}}</h4>
            </div>
            <div class="pet-list-wrapper__row">
                <p>Owner: {{model.OwnerName}}</p>
                <p>Weight: {{model.Weight}} kg</p>
            </div>
            <a href="/PetList">Back to list</a>
        </div>
    </template>
</template>
```

Since there is only one root element, the layout of this view can't be edited. We want to be able to edit the layout of this view to include information from MedicalRecordProvider. To do this, we have to separate the content and the layout. 

This will not be covered in length here since it's already covered in the blog post [How to Apply New Shadow DOM Layouts to Existing HTML Templates](https://starcounter.io/making-apps-blendable/). When those instructions are applied to this view, it should look like this:

<div class="code-name">PetDetails.html</div>

```html
<template>
    <template is="dom-bind">
        <h3 slot="petlist/details-name">{{model.Name}}</h3>
        <h4 slot="petlist/details-age-and-animal">{{model.Age}} years old {{model.Animal}}</h4>
        <p slot="petlist/details-owner-name">Owner: {{model.OwnerName}}</p>
        <p slot="petlist/details-weight">Weight: {{model.Weight}} kg</p>
        <a slot="petlist/details-list-link" href="/PetList">Back to list</a>
    </template>
    <template is="declarative-shadow-dom">
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
    </template>
</template>
```

The code inside the `declarative-shadow-dom` template will be the exact code that will show up in the editor as the default layout. 

#### MasterPage (MedicalRecordProvider)

<div class="code-name">MasterPage</div>

```html
<link rel="import" href="/sys/polymer/polymer.html">

<template>
    <style>
        .mrp-wrapper {
            width: 900px;
            margin: 0 auto;
            box-shadow: 0 1px 3px #ccc;
            padding: 20px;
        }
    </style>
    <template is="dom-bind">
        <div class="mrp-wrapper">
            <starcounter-include partial="{{model.RecordsList}}"></starcounter-include>
            <starcounter-include partial="{{model.RecordsSummary}}"></starcounter-include>
        </div>
    </template>
</template>
```

As above, apply the steps in the [blog post](https://starcounter.io/making-apps-blendable/) and the result looks like this:

```html
<link rel="import" href="/sys/polymer/polymer.html">

<template>
    <template is="dom-bind">
        <starcounter-include slot="medicalrecordprovider/master-records-list" partial="{{model.RecordsList}}"></starcounter-include>
        <starcounter-include slot="medicalrecordprovider/master-records-summary" partial="{{model.RecordsSummary}}"></starcounter-include>
    </template>
    <template is="declarative-shadow-dom">
        <style>
            .mrp-wrapper {
                width: 900px;
                margin: 0 auto;
                box-shadow: 0 1px 3px #ccc;
                padding: 20px;
            }
        </style>
        <div class="mrp-wrapper">
            <slot name="medicalrecordprovider/master-records-list"></slot>
            <slot name="medicalrecordprovider/master-records-summary"></slot>
        </div>
    </template>
</template>
```

## Summary

These changes makes it possible to combine views from multiple apps and arrange the elements so that the views look as if they were from one app. Without this change only the root level element would be possible to move and we would be stuck with a structure where views from different apps would have to be stacked on top of each other. 

{% endraw %}