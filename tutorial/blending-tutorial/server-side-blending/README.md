# Server-Side Blending

Blending happens both on the server-side and on the client-side.

Server-side blending is about taking the response from handlers in different apps and merging them. 

## Blending the List of Pets

In the PetList app, we have a view where we can see the complete list of pets:

![Pet List View](/assets/PetListImage.PNG)

We want to combine this view with the summary view from the MedicalRecordsProvider to create an overview of all the pets and their combined medical examinations.

For this to happen, we need to blend the responses from two handlers. The first one is the handler that returns the list page in the PetList app:

<div class="code-name">PartialHandlers.cs (PetList)</div>

```cs
Handle.GET("/PetList/partial/petlist", () =>
{
    var page = new PetListPage();

    page.Pets.Data = Db.SQL<Pet>($"SELECT p FROM {typeof(Pet)} p");

    return page;
});
```

The other handler is the one that returns the summary from the MedicalRecordProvider:

<div class="code-name">PartialHandlers.cs (MedicalRecordProvider)</div>

```cs
Handle.GET("/MedicalRecordProvider/partial/recordssummary?name={?}", (string patientName) => 
    new RecordsSummary() { Data = GetExaminations(patientName) });
```

When blending is done properly, apps should not know of each other. For this to work, we use tokens. When a handler that has a token is called, the `Blender` API checks if any other handlers has the same token. If that's the case, the call to the first handler also returns the response from the other handlers with the same token.

A token can be either a string or a class. Using strings is the simplest, but also the least flexible approach. In this case, both handlers belong to the list, so we'll connect them to a `list` token.

<div class="code-name">PartialHandlers.cs (PetList)</div> 

```cs
public void Register()
{
    Handle.GET("/PetList/partial/petlist", () =>
    {
        var page = new PetListPage();

        page.Pets.Data = Db.SQL<Pet>($"SELECT p FROM {typeof(Pet)} p");

        return page;
    });

    // ...

    Blender.MapUri("/PetList/partial/petlist", "list");
}
```

<div class="code-name">PartialHandlers.cs (MedicalRecordProvider)</div>

```cs
public void Register()
{
    // ...

    Handle.GET("/MedicalRecordProvider/partial/recordssummary?name={?}", (string patientName) => new RecordsSummary() { Data = GetExaminations(patientName) });

    Blender.MapUri("/MedicalRecordProvider/partial/recordssummary?name=", "list");
}
```

Now, if you build, run and open `http://localhost:8080/PetList`, you should see this:

![Blended list](/assets/BlendedSummaryList.PNG)

It's not beautiful, but it works. One request to `/PetList` returns two responses from different apps that are not aware of each other.