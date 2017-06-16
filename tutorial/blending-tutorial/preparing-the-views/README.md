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

The [guides](/guides/web-apps/html-view-guidelines) describes how to use Declarative Shadow DOM. For our purposes it's enough to know that content and presentation should be separated and that root level elements should be given a `slot` attribute.


