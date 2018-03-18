# Post-commit hooks

## Introduction

Post-commit hooks is a way to guarantee that some code executes after committing a transaction. This functionality is, for example, useful if you want to send out confirmation emails after you've created orders in the database:

```csharp
using Starcounter;

[Database]
public class Order
{
    // Properties
}

public class Program
{
    static void Main()
    {
        Hook<Order>.AfterCommitInsert += (sender, id) =>
        {
            // Executes after you commit a new Order
            var order = Db.FromId<Order>(id);
            SendConfirmationEmail(order);
        };

        // Create a new order and trigger the hook
        Db.Transact(() => new Order());
    }

    private static void SendConfirmationEmail(Order order) 
    { 
        // Implementation for sending a confirmation email   
    }
}
```

With this, you're guaranteed to send out a confirmation email after someone creates a new order. The hook is triggered no matter what app creates the order, as long as the app is in the same database, the app with the hooks runs, and the app has registered the hooks.

The fact that post-commit hooks execute after the transaction differentiates it from [normal commit hooks ](commit-hooks.md)where the hook executes as a part of the transaction.

## API

There are three different post-commit hooks:

```csharp
Hook<DatabaseClass>.AfterCommitInsert += (sender, id) => { /* Implementation */ };
Hook<DatabaseClass>.AfterCommitUpdate += (sender, id) => { /* Implementation */ };
Hook<DatabaseClass>.AfterCommitDelete += (sender, id) => { /* Implementation */ };
```

Each event uses the standard `EventHandler<T>` delegate, where `T` is `ulong`. The ID provided to the hook is the unique object ID of the triggering object. The sender is an `object` that you can cast to a `Task` that represents the asynchronous operation for the transaction that triggered the hook. Read more about how you can use this with a custom scheduler in the [post-commit hooks with a custom scheduler](post-commit-hooks.md#advanced:-post-commit-hooks-with-a-custom-scheduler) section.

The `DatabaseClass` in the code sample above is the database class that should trigger the hook when an instance from the class is inserted, updated, or deleted.

## Invocation

Starcounter invokes the correct hooks when an instance of the specified class is inserted, updated, or deleted. This statement assumes that the hook is registered and that the app with the hook is running.

### Triggering hooks

For example, say that we have these four post-commit hooks:

```csharp
Hook<Order>.AfterCommitInsert += (sender, id) => Debug.WriteLine("AfterCommitInsert-Order");
Hook<Person>.AfterCommitInsert += (sender, id) => Debug.WriteLine("AfterCommitInsert-Person");
Hook<Person>.AfterCommitUpdate += (sender, id) => Debug.WriteLine("AfterCommitUpdate-Person");
Hook<Person>.AfterCommitDelete += (sender, id) => Debug.WriteLine("AfterCommitDelete-Person");
```

To trigger each of these, we write something like this:

```csharp
// Insert order
Db.Transact(() => new Order());

// Insert person
var person = Db.Transact(() => new Person());

// Update person
Db.Transact(() => person.Name = "Someone");

// Delete person
Db.Transact(() => person.Delete());
```

{% code-tabs %}
{% code-tabs-item title="Debug console" %}
```text
AfterCommitInsert-Order
AfterCommitInsert-Person
AfterCommitUpdate-Person
AfterCommitDelete-Person
```
{% endcode-tabs-item %}
{% endcode-tabs %}

Notice how each of these operations is in a separate transaction. If we put them in the same transaction, the result is different:

```csharp
Db.Transact(() =>
{
    // Insert order
    new Order();

    // Insert person
    var person = new Person();

    // Update person
    person.Name = "Someone";

    // Delete person
    person.Delete();
});
```

{% code-tabs %}
{% code-tabs-item title="Debug console" %}
```text
AfterCommitInsert-Order
```
{% endcode-tabs-item %}
{% endcode-tabs %}

This code only triggers the `AfterCommitInsert` hook for the `Order` class. The reason for this is that the hooks are triggered based on the final result of the transaction, not the individual operations themselves. The only state change after the transaction was that the app created an `Order` since the transaction created and then immediately deleted the `Person`.

Post-commit hooks trigger multiple times if there are several operations in one transaction:

```csharp
Db.Transact(() =>
{
    new Person();
    new Person();
}); 
```

{% code-tabs %}
{% code-tabs-item title="Debug console" %}
```text
AfterCommitInsert-Person
AfterCommitInsert-Person
```
{% endcode-tabs-item %}
{% endcode-tabs %}

### Infinite loops 

If you invoke a post-commit hook from inside a commit hook, it runs in an infinite loop because you're scheduling a task from inside that task:

```csharp
// This will cause an infinite loop once invoked
Hook<Order>.AfterCommitInsert += (sender, id) =>
{
    Db.Transact(() => new Order());
};
```

### Invocation timing

Also, post-commit hooks are not guaranteed to execute right after the transaction. It's possible that another transaction executes before executing the hook. Thus, you can't assume that the database state that existed after the transaction that triggered the hook is still the same:

```csharp
Hook<Person>.AfterCommitInsert += (sender, id) => 
{
    var person = Db.FromId<Person>(id);
    // Here, person can be null
};

var person = Db.Transact(() => new Person());

// This may run before the hook
Db.Transact(() => person.Delete());
```

If another thread deletes the person between the time the transaction completes and before the hook is invoked, the object will no longer exist.

## Durability

Post-commit hooks execute after the transaction is committed to memory but before Starcounter flushes the commit to the transaction log. That the post-commit hooks execute before the transaction log flush means that the transaction is not guaranteed to be durable in the hook:

```csharp
Hook<Order>.AfterCommitInsert += (sender, id) =>
{
    // Executes after you commit a new Order
    var order = Db.FromId<Order>(id);
    SendConfirmationEmail(order);

    // POWER OUTAGE
};

// Create a new order and trigger the hook
Db.Transact(() => new Order());
```

If the transaction with the new order has not been flushed to the transaction log where the power outage happens, then the customer gets the confirmation email but the order has not been stored in the database.

You can guarantee that the transaction is durable by executing the hook after the task with `ContinueWith`:

```csharp
Hook<Order>.AfterCommitInsert += (sender, id) =>
{
    var task = (System.Threading.Tasks.Task)sender;

    // Flush the transaction to the log
    task.ContinueWith((t) => 
    {
        // The transaction is now durable
        // Put the code for the hook here
    });
};
```

## Registering hooks

You can almost register hooks anywhere. A common practice is to have them in a specific class with a `Register` method and invoke this method in the entry point:

```csharp
public static class PostCommitHooks
{
    public static void Register()
    {
        Hook<Order>.AfterCommitInsert += (sender, id) => { /* */ };
        // The other post-commit hooks
    }
}

public class Program
{
    static void Main()
    {
        PostCommitHooks.Register();
    }
}
```

When you register post-commit hooks, avoid two things: registering the hooks in a transaction or in another hook. The reason for this is that the transaction may have to restart if there's a conflict which would register the hook several times. If you register it in another hook the inner hook will be registered every time the outer hook is called. If you register a hook many times, it's called as many times as its registered:

```csharp
public class Program
{
    static void Main()
    {
        for (int i = 0; i < 3; i++)
        {
            Hook<Order>.AfterCommitInsert += (sender, id) => Debug.WriteLine("In insert hook");
        }

        Db.Transact(() => new Order());
    }
}
```

{% code-tabs %}
{% code-tabs-item title="Debug console" %}
```text
In insert hook
In insert hook
In insert hook
```
{% endcode-tabs-item %}
{% endcode-tabs %}

## Advanced: post-commit hooks with a custom scheduler

Starcounter uses its default scheduling mechanism to schedule tasks that the post-commit hooks execute in. Currently, that's an instance of `DbTaskScheduler`. A complementary API can provide a custom scheduler when you register post-commit hooks, forcing Starcounter to utilize that instead of the built-in default. The example below shows a custom task scheduler that extends the Starcounter database task scheduler, executing a callback every time the hook in which it is installed in is queued:

```csharp
using Starcounter;
using System;
using System.Threading.Tasks;

[Database]
public class Order
{
    // Properties that are included in an order
}

public class Program
{
    static void Main()
    {
        Hook<Order>.OnAfterCommitInsert(
            (sender, id) => { /* */ }, 
            new NotifyingScheduler(() => Console.Write("A task was scheduled")));

        Db.Transact(() => new Order());
    }
}

class NotifyingScheduler : DbTaskScheduler
{
    readonly Action callback;

    public NotifyingScheduler(Action action)
    {
        callback = action;
    }

    protected override void QueueTask(Task task)
    {
        base.QueueTask(task);
        callback?.Invoke();
    }
}
```

Corresponding APIs are available for all other hooks:

* `Hook<DatabaseClass>.OnAfterCommitInsert`
* `Hook<DatabaseClass>.OnAfterCommitUpdate`
* `Hook<DatabaseClass>.OnAfterCommitDelete`

