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

To make it possible to arrange views in meaningful ways, which we will do in the last step [Client-Side Blending](../client-side-blending), we have to define what elements in the view that can be rearranged. We also have to create a default view that applies when there is no arrangement or if there are no other views to combine with.

This is done using Declarative Shadow DOM. 

The description for applying Declarative Shadow DOM is in the [guides](/guides/web-apps/html-view-guidelines). For our purposes, it's enough to know that content and presentation should be separated and that root level elements should be given a `slot` attribute.


