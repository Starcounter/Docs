# Declarative Shadow DOM

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

## Attaching Elements to Slots

When blending elements from different views, the elements on the root appear as `slot` elements in [CompositionEditor](https://github.com/starcounter/compositionEditor), the tool we use to rearange elements. These elements have names. By default, their names look like `<slot name="MyApp/0"></slot>`, where the `0` represents the index of the element in its view. 

By explicitly attaching elements to slots, we can give them meaningful names.  

### Locate the Files

The only files we will touch in this step are in the `src/[app-name]/wwwroot/[app-name]/views` directories, as marked in orange:

![Views marked in file strucutre](/assets/FileStrucutreBlending.PNG)

We will start out with these three files as they are the simplest ones to adapt:

* `RecordsList.html`
* `PetListPage.html` 
* `RecordSummary.html`

#### RecordsList

{% raw %}

<div class="code-name">RecordsList.html</div>

```html
<link rel="import" href="/sys/polymer/polymer.html">

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
<link rel="import" href="/sys/polymer/polymer.html">

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
<link rel="import" href="/sys/polymer/polymer.html">

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
<link rel="import" href="/sys/polymer/polymer.html">

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
<link rel="import" href="/sys/polymer/polymer.html">

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

{% endraw %}