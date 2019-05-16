# How to attach an HTTP request to an existing long-running transaction

When you are building a multi-page application, it might be useful to create an object in a transaction without committing it.

If you want to provide the user with a URL to an uncommitted object, you will need to attach the view-model of the request handler to the existing long-running transaction.

The following button handler creates a `Person` object and redirects to its page, without committing the object:

```
public void Handle(Input.CreatePerson action)
{
    var person = new Person()
    {
        FirstName = "Uncommitted Guy"
    };

    RedirectUrl = "/people/persons/" + person.GetObjectID();
}
```

When the request handler for `"/people/persons/{?}"` catches the request after the redirection, it needs to search for the object by its ID in the existing long-running transaction:

```
Handle.GET("/people/persons/{?}", (string id) =>
{
    MasterPage master = GetMasterPageFromSession();

    Action runPartialInScope = () =>
    {
        master.CurrentPage = Self.GET("/people/partials/persons/" + id);
    };

    if (master.CurrentPage.Transaction != null)
    {
        // Attach to a long running transaction of the existing CurrentPage
        master.CurrentPage.Transaction.Scope(runPartialInScope);
    }
    else
    {
        // If there was no existing CurrentPage, create a new transaction
        Db.Scope(runPartialInScope);
    }

    return master;
});
```

### Read more

[Guide: Long-running transactions](../topic-guides/transactions/long-running-transactions.md)

