# Long running transactions

Starcounter supports long-running transactions. A long-running transaction, as its name indicates, spans over a longer period of time. To create a long-running transaction, use `Db.Scope(Action)` or `(new Transaction(...)).Scope(Action)` and attach it to a view-model.

When creating long-running transactions, it could contain objects that might be modified by other transactions during the transaction's lifetime. This is because the isolation level for long-running transactions is lower and it will therefore not detect conflicts. This is different from the behavior of `Db.Transact` and `Db.TransactAsync`, as described in the [Using Transactions page](using-transactions/#dbtransact-and-dbtransactasync-usage). 

A long-running transaction can be used on any scheduled thread, however, it cannot be used from more than one thread at a time. If two or more threads call `transaction.Scope(Action)`at the same time, an exception will be thrown.

This document is divided into two parts:
1. [Creating and attaching long-running transactions](#create-and-attach)
2. [Dealing with already attached long-running transactions in view-models](#handling-long-running-transactions-in-view-models)

## Create and Attach

### The Reason For Attaching View-Models to Transactions

A very important reason for associating view-models with transactions is that it allows you to put business logic in the database objects rather than in some code inside the form or some code that gets called when you save the form.

Let's say, for example, that the discount of an order changes when you change the quantity of any of its order items. The more you buy, the cheaper it gets. You would want this business logic should be visible to the order form presented to the user when he uses the mouse and keyboard to edit the order. To honor the DRY principle (don't repeat yourself) you should not have to repeat any business logic already in the domain model (i.e. database object). And if you put the code on the discount in the form, you would not handle the separation-of-concern very nicely as the rule is probably not connected with a certain piece of user interface or even a single specific application, but rather a generic business rule that should keep it's integrity no matter how many editors and different clients and application servers you throw at it.

By allowing the business objects to live inside a transactional scope, the form can view the world as if the changes are there while the outside world does not yet see them. When the form is saved, the transaction is committed or if the form is not saved, the transaction is simply aborted.

### Assigning JSON to a transaction

Let's assume that you are composing an email in a mail program. You are entering a recipient that is not yet in your contact database. You would then create a new EmailAddress object and assign it to your email.

If the user elects to cancel the email, the EmailAddress should not be saved. If the user elects to send/save the email, the email address should be saved. But the EmailAddress is directly edited in the email form. How does Starcounter know that it should only be saved if the user saves the email?

A new transaction is created calling `Db.Scope` that takes a delegate to be executed as parameter. The transaction will then be attached to the view-model when the (view-model) object is instantiated.

```cs
Handle.GET("/email-client/new-email", () =>
{
  return Db.Scope(() => 
  {
    var emailPage = new EmailPage()
    {
      Data = new Email()
    }
    return emailPage;
  })
});
```

When Starcounter executes the `Handle` function or when it otherwise operates on the object set in the `Data` property, it will first set the current transaction scope to the transaction set in the `Transaction` property in the view-model or its nearest parent view-model.

Inside your form, the changes are all there and the information appears updated on the user screen. For the outside world, no unsaved changes are visible to disturb the consistency of your database.

### Using an existing transaction

Sometimes a transaction is already attached on another part of the view-model. To reuse it it needs to be scoped before the new page is created.

```cs
Handle.GET("/email-client/new-email", () =>
{
  var masterPage = Self.GET<MasterPage>("/email-client");
  masterPage.Transaction.Scope(() =>
  {
    masterPage.CurrentPage = new MailPage()
    {
      Data = new Email()
    };
  });
  return masterPage;
});
```

### Attaching a transaction to an existing json-object

If the part of the view-model that the transaction should be attached to is already instantiated, for example a default value for a property of type Json, the transaction can be attached manually.

Lets assume that in the previous example, the `CurrentPage` property was already instantiated.

```cs
Handle.GET("/email-client/new-email", () =>
{
  var masterPage = Self.GET<MasterPage>("/email-client");
  masterPage.Transaction.Scope(() =>
  {
    masterPage.CurrentPage.AttachCurrentTransaction();
  });
  return masterPage;
});
```

### Sharing transactions

A transaction can be attached and used on more than one instance in the view-model. When a transaction is scoped, all subsequent calls inside the scope will use the same transaction.

In this example the call to the second handler with uri `/email-client/email/{emailId}` will use the transaction created in the first handler.

```cs
Handle.GET("/email-client/new-email", () =>
{
  var masterPage = Self.GET<MasterPage>("/email-client");
  Db.Scope(() =>
  {
    var email = new Email();
    var mailPage = Self.GET<MailPage>("/email-client/email/" + email.GetObjectID());
    masterPage.CurrentPage = page;
  });
  return masterPage;
});

Handle.GET("/email-client/email/{?}", (string emailId) =>
{
  Email email = Db.SQL<Email>("SELECT e FROM Email e WHERE ObjectId=?", emailId).First;
  return new MailPage()
  {
    Data = email
  };
});
```

Scopes are nested, so if in the example the second rest-handler, `Handle.Get("/email-client/email/{?}", ...)` would also declare a scope it will still use the same transaction as created by the caller, `GET("/email-client/new-email", ...)`.

### Making sure a new transaction is created

If, for some reason, that it's vital that a new transaction always is created it is possible to manually create and scope a transaction.

```cs
Handle.GET("/email-client/new-email", () =>
{
  var masterPage = Self.GET<MasterPage>("/email-client");
  var transaction = new Transaction(false, false);
  transaction.Scope(() =>
  {
    var email = new Email();
    masterPage.CurrentPage = Self.GET<MailPage>("/email-client/email/" + email.GetObjectID());
  });
  return masterPage;
});
```

## Handling Long-Running Transactions in View-Models

```cs
partial class MailPage : Json, IBound<Mail>
{
  void Handle(Input.To action)
  {
    var emailAddress = Db.SQL<EmailAddress>("SELECT e FROM EmailAddress e WHERE Address = ?", action.value).First;
    if (emailAddress == null)
    {
      emailAddress = new EmailAddress() 
      { 
        Address = action.value 
      };
      Data.To = emailAddress;
    }
  }

  void Handle(Input.SaveTrigger action)
  {
    Transaction.Commit();
  }

  void Handle(Input.CancelTrigger action)
  {
    Transaction.Rollback();
  }
}
```