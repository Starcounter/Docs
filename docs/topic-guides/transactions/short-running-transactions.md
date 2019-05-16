# Short-running transactions

## Introduction

Short-running transaction creates a transaction that commits to the database when the scope runs out. These transactions are ACID. There are two types of short-running transactions - synchronous and asynchronous.

## Db.Transact

`Db.Transact` is the simplest way to create a transaction in Starcounter. It declares a transactional scope and runs synchronously, as described on the [previous page](./). The argument passed to the `Db.Transact` method is a delegate containing the code to run within the transaction. In code, it looks like this:

```csharp
Db.Transact(() =>
{
    new Employee
    {
        FirstName = "Samwise",
        LastName = "Gamgee"
    };
});
```

Since `Db.Transact` is synchronous, it sometimes becomes a performance bottleneck. Starcounter handles this by automatically scaling the number of working threads to continue processing requests even if some handlers are blocked. The maximum number of working threads is the number of CPU cores multiplied by 254, so with four cores, there would be a maximum of 1016 working threads. When these threads are occupied, the next `Db.Transact` call in line will have to wait until a thread is freed. [Db.TransactAsync](short-running-transactions.md#db.transactasync) circumvents this.

`Db.Transact` is an implementation of `Db.TransactAsync` with a thin wrapper that synchronously waits for the returned `Task` object.

## Db.TransactAsync

`Db.TransactAsync` is the asynchronous counterpart of `Db.Transact`. It gives the developer more control to balance throughput and latency. The function returns a `Task` that is marked as completed when flushing the transaction log for the transaction. Thus, the database operation itself is synchronous while flushing to the transaction log is asynchronous.

`Db.Transact` and `Db.TransactAsync` are syntactically identical:

```csharp
Db.TransactAsync(() => 
{
    // The code to run in the transaction
})
```

While waiting for the write to the transaction log to finish, it's possible to do other things, such as sending an email:

```csharp
Order order = null;
Task task = Db.TransactAsync(() =>
{
    order = new Order();
}); // Order has been added to the database

SendConfirmationEmail(order);

task.Wait(); // Wait for write to log to finish
```

This is more flexible and performant than `Db.Transact`, but it comes with certain risks; for example, if there's a power outage or other hardware failure after the email is sent but before writing to the log, the email will be incorrect - even if the user got a confirmation, the order will not be in the database since it was never written to the transaction log.

If a handler creates multiple write transactions, use `Db.TransactAsync` and then wait for all transactions at once with `Task.WaitAll` or `TaskFactory.ContinueWhenAll`. Otherwise, the latency of the handler will degrade.

Consider the following example:

```csharp
for (int i = 1; i <= 100000; i++)
{
    Db.Transact(() =>
    {
        new Person();
    });
}

// Execute some code after commiting and flushing all transactions
```

The result of the following code is exactly the same as the code above, but it's executed many times faster:

```csharp
var tasks = new List<Task>();
for (int i = 1; i <= 100000; i++)
{
    tasks.Add(Db.TransactAsync(() =>
    {
        // All transactions up to this point have already been committed
        new Person();
    }));
}
Task.WaitAll(tasks.ToArray());
// Execute some code after commiting and flushing all the transactions
```

In this case, each iteration of the loop doesn't require the previous transaction to be flushed to the disk. The transaction execution is still synchronous.

### Limitations

With the `Db.TransactAsync` API, it's tempting to use async/await in applications. Syntactically it's possible, although it's not that useful due to these limitations:

1. Using async/await is not possible in a handler body as Handler API doesn't support async handlers.
2. No special measures have been taken to force after-await code to run on Starcounter threads, so manual `Scheduling.ScheduleTask` might be required \(see [Running background jobs](running-background-jobs.md) for details\).
3. Use async/await with caution as they may inadvertently increase the latency. Say if user code runs transactions sequentially, putting await in front of every `Db.TransactAsync` will accumulate all the individual latencies. The right stategy in this case is to make a list of tasks and then await them at once.

## Transaction size

Code in `Db.Transact` and `Db.TransactAsync` should execute in as short time as possible because conflicts are likelier the longer the transaction is. Conflicts requires long transactions to run more times which can be expensive. The solution is to break the transaction into smaller transactions.

