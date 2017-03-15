# Long running transactions

Starcounter supports long-running transactions. A long-running transaction, as its name indicates, spans over a longer period of time. To create a long-running transaction you use <code>Db.Scope(<var>Action</var>)</code> or <code>(new Transaction(...)).Scope(<var>Action</var>)</code> and attach it to a view-model.

There are some things that you need to consider when creating long-running transactions. You are opening a transaction that could contain objects that might be modified by others (other transactions) during the transactions lifetime. The concistency is default set to a lower isolation level for your transaction, which means that conflicts will not be detected, it is possible though to enable conflict detection if it's needed. This is different from the transaction scopes described earlier in this document that will retry the commits themselves numerous times.

A long-running transaction can be used on any scheduled thread, however it cannot be used from more than one thread at a time. If two or more threads call `transaction.Scope(Action)`at the same time an exception will be thrown.

## Assigning JSON to a transaction

A server-side JSON object can be associated with a transaction. This is an important concept in Starcounter view-models.

Let's assume that you are composing an email in a mail program. You are entering a recipient that is not yet in your contact database. You would then create a new EmailAddress object and assign it to your email.

```cs
partial class MailPage : Json, IBound<Mail>
{
  void Handle(Input.To input)
  {
    EmailAddress a = Db.SQL<EmailAddress>("SELECT e FROM EmailAddress e WHERE Address=?", input.value).First;
    if (a == null)
    {
      a = new EmailAddress() { Address = input.value };
      Data.To = a;
    }
  }

  void Handle(Input.SaveTrigger input)
  {
    this.Transaction.Commit();
  }

  void Handle(Input.CancelTrigger input)
  {
    this.Transaction.Rollback();
  }
}
```

If the user elects to cancel the email, the EmailAddress should not be saved. If the user elects to send/save the email, the email address should be saved. But the EmailAddress is directly edited in the email form. How does Starcounter know that it should only be saved if the user saves the email?

The way this is done in Starcounter is to assign a transaction to the view-model. In this way, changes that pertains to the actions performed in the scope of the form editing can be kept together as a single transaction. A new transaction is created calling `Db.Scope` that takes a delegate to be executed as parameter. The transaction will then be attached to the view-model when the (view-model) object is instantiated.

```cs
class Program
{
  static void Main()
  {
    Handle.GET("/new-email", () =>
    {
      MailPage p = null;
      Db.Scope(() => 
      {
        p = new MailPage()
        {
          Html = "email.html",
          Data = new Email()
        };
      });
      return p;
    });
  }
}
```

When Starcounter executes the `Handle` function or when it otherwise operates on the object set in the `Data` property, it will first set the current transaction scope to the transaction set in the `Transaction` property in the view-model or its nearest parent view-model.

Inside your form, the changes are all there and the information appears updated on the user screen. For the outside world, no unsaved changes are visible to disturb the consistency of your database.

## Using an existing transaction

Sometimes a transaction is already attached on another part of the view-model. To reuse it it needs to be scoped before the new page is created.

```cs
class Program
{
  static void Main()
  {
    Handle.GET("/new-email", () =>
    {
      Master m = Self.GET<Master>("/Master");
      m.Transaction.Scope(() =>
      {
        m.FocusedPage = new MailPage()
        {
          Html = "email.html",
          Data = new Email()
        };
      });
      return m;
    });
  }
}
```

## Attaching a transaction to an existing json-object

If the part of the view-model that the transaction should be attached to is already instantiated, for example a default value for a property of type Json, the transaction can be attached manually.

Lets assume that in the previous example, the `FocusedPage` property was already instantiated.

```cs
class Program
{
  static void Main()
  {
    Handle.GET("/new-email", () =>
    {
      Master m = Self.GET<Master>("/Master");
      m.Transaction.Scope(() =>
      {
        m.FocusedPage.AttachCurrentTransaction();
      });
      return m;
    });
  }
}
```

#### Sharing transactions

A transaction can be attached and used on more than one instance in the view-model. When a transaction is scoped, all subsequent calls inside the scope will use the same transaction.

In this example the call to the second handler with uri `/email/{emailId}` will use the transaction created in the first handler.

```cs
class Program
{
  static void Main()
  {
    Handle.GET("/new-email", () =>
    {
      Master m = Self.GET<Master>("/Master");
      Db.Scope(() =>
      {
        Email email = new Email();
        MailPage page = Self.GET<MailPage>("/email/" + email.GetObjectID());
        m.FocusedPage = page;
      });
      return m;
    });

    Handle.GET("/email/{?}", (string emailId) =>
    {
      return new MailPage()
      {
        Html = "email.html",
        Data = Db.SQL<Email>("SELECT e FROM Email e WHERE ObjectId=?", emailId).First
      };
    });
  }
}
```

Scopes are nested, so if in the example the second rest-handler, `Handle.Get("/email/{?}", ...)` would also declare a scope it will still use the same transaction as created by the caller, `GET("/new-email", ...)`.

## Making sure a new transaction is created

If, for some reason, that it's vital that a new transaction always is created it is possible to manually create and scope a transaction.

```cs
class Program
{
  static void Main()
  {
    Handle.GET("/new-email", () =>
    {
      Master m = Self.GET<Master>("/Master");
      Transaction t = new Transaction(false, false);
      t.Scope(() =>
      {
        Email email = new Email();
        MailPage page = Self.GET<MailPage>("/email/" + email.GetObjectID());
        m.FocusedPage = page;
      });
      return m;
    });
  }
}
```

## Why is this important

A very important reason for associating view-models with transactions is that it allows you to put domain logic (aka. business logic) in the domain objects (aka database objects) rather than in some code inside the form or some code that gets called when you save the form.

Let's say, for example, that the discount of an order changes when you change the quantity of any of its order items. The more you buy, the cheaper it gets. You would want this business logic should be visible to the order form presented to the user when he uses the mouse and keyboard to edit the order. To honour the DRY principle (don't repeat yourself) you should not have to repeat any business logic already in the domain model (i.e. database object). And if you put the code on the discount in the form, you would not handle the separation-of-concern very nicely as the rule is probably not connected with a certain piece of user interface or even a single specific application, but rather a generic business rule that should keep it's integrity no matter how many editors and different clients and application servers you throw at it.

By allowing the business objects to live inside a transactional scope, the form can view the world as if the changes are there while the outside world does not yet see them. When the form is saved, the transaction is committed or if the form is not saved, the transaction is simply aborted.
