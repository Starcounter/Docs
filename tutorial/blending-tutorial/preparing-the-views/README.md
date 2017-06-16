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

When we blend elements from different views, the elements appear as `slot` elements in the blending tool. These elements have names. By default, their names look like `<slot name="MyApp/0"></slot>`, where the `0` represents the index of the element in its view. 

By explicitly attaching elements to slots, we can give them meaningful names.  

### Locate the Files

The only files we will touch in this step are in the `src/[app-name]/wwwroot/[app-name]/views` directories, as marked in orange:

![Views marked in file strucutre](/assets/FileStrucutreBlending.PNG)

We will start out with these three files as they only require slots to blend properly:

* `RecordsList.html`
* `PetListPage.html` 
* `RecordSummary.html`