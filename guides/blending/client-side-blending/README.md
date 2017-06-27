# Client-Side Blending

When merging responses from different apps with [server-side blending](server-side-blending), the views are stacked on top of each other. In most cases, that's not what we want. Instead, we would like to compose elements in the views to make the result look like one page. That's what client-side blending does:

![Blending comparison](/assets/BlendingComparison2.png)

## Composing Views
 
To solve this, we could edit the source code of the apps involved. This is impractical, and in some cases even impossible. The main reason is that the apps in a system, or application suite, are often created by different developers and it's not always possible to get access to their source code. A secondary reason is versatility. If an app is modified to work well with your application suite, it might not work well with other application suites.

Client-side blending lets you change the structure of blended apps without touching the source code. It does this by swapping the default composition of HTML that's sent to the client with an alternative composition. This requires a couple of things:
1. Alternative compositions should be defined with the default composition as a starting point. Otherwise, the HTML would have to be rewritten which would be inefficient, especially since alternative compositions are often similar to default compositions.
2. Alternative compositions have to be tied to a specific set of responses. For example, if you have four apps in your app suite and you swap out one of them, you wouldn't want the composition for the previous set of apps to still apply.
3. Alternative compositions have to be stored persistently. If an app is restarted, the defined compositions should still be applied.
4. Alternative compositions have to be applied when the different responses are merged and not on application start since server-side blending can change in runtime.

### 1. Defining Alternative Compositions

Alternative compositions are defined in the browser when the apps are running with the [CompositionEditor](https://github.com/StarcounterApps/CompositionProvider). It looks like this:

![Composition editor](/assets/CompositionEditor.gif)

The editor highlights the area that is changed and displays the changes in real time. There's no need to write the HTML from scratch, instead, the alternative composition is created from the default composition. 

### 2. Tying to Specific Responses

Every composition is tied to an identification. All identifications for the compositions in a page can be found by double clicking on the top input field in the CompositionEditor. They look like this:

```
[partial-id="/sc/htmlmerger?PetList=/PetList/views/PetListPage.html&MedicalRecordProvider=/MedicalRecordProvider/views/RecordSummary.html"]
[partial-id="/PetList/views/MasterPage.html"]
```

The first one is the indentification for the merged response of `PetListPage.html` from PetList and `RecordSummary.html` from MedicalRecordProvider. Keys of merged responses are always prefixed with `/sc/htmlmerger`.

The second is the composition for `MasterPage.html` from PetList. It has not been merged.

Starcounter automatically creates these identifications.    

### 3. Storing Compositions

Saved compositions from the CompositionEditor end up in the database. The specific class is `HTMLComposition`. Thus, it can be queried: `SELECT * FROM Starcounter.HTMLComposition`. 

The class has two properties, `Key` and `Value`. `Key` is the identification mentioned previously and `Value` is the HTML. 

### 4. Serving Compositions

[CompositionProvider](https://github.com/starcounterapps/compositionprovider) is an app that provides compositions when responses are merged. It does this by checking if there are any compositions in the database that match the identification of the merged response and serves the composition if that's the case. Otherwise, the default composition is served.

## Summary

With this, apps that use server-side blending can be changed without touching the source code and the changes will be applied every time that combination of apps is used.