# Attach an HTTP request to an existing long running transaction

When you are building a multi-page application, it might be useful to create an object in a transaction without committing it.

If you want to provide the user with a URL to an uncommitted object, you will need to attach the view-model of the request handler to the existing long-running transaction.

The following button handler creates a `Person` object and redirects to its page, without committing the object:

```csharp
public void Handle(Input.CreatePerson action)
{
    var person = new Person()
    {
        FirstName = "Uncommitted Guy"
    };
    var id = person.GetObjectID();
    RedirectUrl = "/people/persons/" + id;
}
```

When the request handler for `"/people/persons/{?}"` catches the request after the redirection, it needs to search for the object by its ID in the existing long-running transaction:

```csharp
Handle.GET<string>("/people/persons/{?}", (string id) =>
{
    var master = Self.GET<StandalonePage>("/people/standalone");

    Action runPartialInScope = () =>
    {
        var person = DbHelper.FromID(DbHelper.Base64DecodeObjectID(id)) as Person;
        master.CurrentPage = Self.GET<Json>("/people/partials/persons/" + id);
    };

    if (master.CurrentPage.Transaction != null)
    {
        //attach to a long running transaction of the existing CurrentPage
        master.CurrentPage.Transaction.Scope(runPartialInScope);
    }
    else
    {
        //if there was no existing CurrentPage, create a new transaction
        Db.Scope(runPartialInScope);
    }

    return master;
});
```

### Read more

[Guide: Long-running transactions](../guides/transactions/long-running-transactions.md)

### See the full example

[github.com/StarcounterPrefabs/People](https://github.com/StarcounterSamples/People)

