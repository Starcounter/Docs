# Long running transactions

## Introduction

Starcounter supports long-running transactions. A long-running transaction, as its name indicates, spans over a longer period of time. To create a long-running transaction, use `Db.Scope(Action)` or `(new Transaction(...)).Scope(Action)` and attach it to a view-model.

When creating long-running transactions, it could contain objects that might be modified by other transactions during the transaction's lifetime. This is because the isolation level for long-running transactions is lower and it will therefore not detect conflicts. This is different from the behavior of `Db.Transact` and `Db.TransactAsync`, as described in the [Using Transactions page](using-transactions.md#dbtransact-and-dbtransactasync-usage).

A long-running transaction can be used on any scheduled thread, however, it cannot be used from more than one thread at a time. If two or more threads call `transaction.Scope(Action)`at the same time, an exception will be thrown.

 This document is divided into three parts:

1.  Creating and attaching long-running transactions
2.  Dealing with already attached long-running transactions in view-models
3.  Exceptions and how to solve them

## Create and Attach

### The Reason For Attaching View-Models to Transactions

A very important reason for associating view-models with transactions is that it allows you to put business logic in the database objects rather than in some code inside the form or some code that gets called when you save the form.

Let's say, for example, that the discount of an order changes when you change the quantity of any of its order items. The more you buy, the cheaper it gets. You would want this business logic should be visible to the order form presented to the user when he uses the mouse and keyboard to edit the order. To honor the DRY principle \(don't repeat yourself\) you should not have to repeat any business logic already in the domain model \(i.e. database object\). And if you put the code on the discount in the form, you would not handle the separation-of-concern very nicely as the rule is probably not connected with a certain piece of user interface or even a single specific application, but rather a generic business rule that should keep it's integrity no matter how many editors and different clients and application servers you throw at it.

By allowing the business objects to live inside a transactional scope, the form can view the world as if the changes are there while the outside world does not yet see them. When the form is saved, the transaction is committed or if the form is not saved, the transaction is simply aborted.

### Assigning JSON to a Transaction

Let's assume that you are composing an email in a mail program. You are entering a recipient that is not yet in your contact database. You would then create a new EmailAddress object and assign it to your email.

If the user elects to cancel the email, the EmailAddress should not be saved. If the user elects to send/save the email, the email address should be saved. But the EmailAddress is directly edited in the email form. How does Starcounter know that it should only be saved if the user saves the email?

A new transaction is created calling `Db.Scope` that takes a delegate to be executed as parameter. The transaction will then attach to the view-model when the \(view-model\) object is instantiated.

```csharp
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

### Using an Existing Transaction

Sometimes a transaction is already attached on another part of the view-model. To reuse it, it needs to be scoped before the new page is created.

```csharp
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

## Attaching a Transaction to an Existing JSON Object

If the part of the view-model that the transaction should attach to is already instantiated, for example a default value for a property of type `Json`, the transaction can attach manually.

Lets assume that in the previous example, the `CurrentPage` property was already instantiated.

```csharp
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

### Sharing Transactions

A transaction can attach and be used on more than one instance in the view-model. When a transaction has a scope, all calls inside the scope will use the same transaction.

In this example the call to the second handler with uri `/email-client/email/{emailId}` will use the transaction created in the first handler.

```csharp
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

### Making Sure to Create a New Transaction

To always create a new transaction, manually create and scope it:

```csharp
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

### Handling Long-Running Transactions in View-Models

When inside a view-model that's attached to a long-running transaction, it's possible to commit and rollback changes.

The syntax for these are `Transaction.Commit()` and `Transaction.Rollback()`.

`Transaction.Commit()` commits changes to the database which means that they will become visible for other transactions.

`Transaction.Rollback()` rolls back the state of the view-model. For example, with a commit that's immidiately followed by a rollback, no changes will roll back. Consider this scenario instead:

```csharp
void Handle(Input.CreateEmailTrigger action) 
{
  Transaction.Commit();
  new Email()
  {
    Address = this.Address
  };
  Transaction.Rollback();
}
```

In this scenario, the new `Email` that's created would roll back and the state of the view-model would return to the previous commit.

Most sample apps uses `Commit` and `Rollback` to allow the user to save or cancel change like in the following example:

```csharp
partial class MailPage : Json, IBound<Mail>
{
  void Handle(Input.RecipientAddress action)
  {
    var emailAddress = Db.SQL<EmailAddress>("SELECT e FROM EmailAddress e WHERE Address = ?", action.value).First;
    if (emailAddress == null)
    {
      emailAddress = new EmailAddress() 
      { 
        Address = action.value 
      };
      Data.RecipientAddress = emailAddress;
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

You can find an example of this in [step 6](/guides/tutorial/cancel-and-delete/README.md) of the tutorial.

## Exceptions

### ScErrIteratorClosed \(SCERR4139\)

Starcounter throws this exception when an iterator closes before finishing. An iterator will close if a long-running transaction commits, rolls back, or cancels while iterating.

This is the simplest way to close the iteration and throw the exception if we assume that this code is in a long-running transaction:

```csharp
foreach (var person in Db.SQL("SELECT p FROM Person p"))
{
    Transaction.Commit();
}
```

Due to this, we recommend not to execute code with side effects during the iteration since it might cause it to close. This is important when the developer does not have full control over the side effects, such as when making `Self.GET` calls that might have responses from other apps.

### ScErrTransactionModifiedButNotReferenced \(SCERR4287\)

`ScErr4287` is thrown when a long-running transaction that's not attached to a JSON object writes to the database without committing or rolling back at the end of the scope.

For example:

```csharp
using Starcounter;

[Database]
public class Person {}

class Program
{
    static void Main()
    {
        Db.Scope(() =>
        {
            new Person(); // ScErr4287
        });
    }
}
```

  
Here, a new object is written to the database but it's never committed or rolled back because long-running transactions don't automatically commit at the end of the scope. Starcounter throws an exception here to avoid confusion on what changes are commited. When using the long-running transaction, ensure that all writes are commited or rolled back before the end of the scope:

```csharp
using Starcounter;

[Database]
public class Person {}

class Program
{
    static void Main()
    {
        Db.Scope(() =>
        {
            new Person();
            // some more code

            Transaction.Current.Commit(); // Or Transaction.Current.Rollback();
        });
    }
}
```



