# Short-Running Transactions

## `Db.Transact`

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

Since `Db.Transact` is synchronous, it sometimes becomes a performance bottleneck. Starcounter handles this by automatically scaling the number of working threads to continue processing requests even if some handlers are blocked. The maximum number of working threads is the number of CPU cores multiplied by 254, so with four cores, there would be a maximum of 1016 working threads. When these threads are occupied, the next `Db.Transact` call in line will have to wait until a thread is freed. [`Db.TransactAsync`](short-running-transactions.md#dbtransactasync) circumvents this.

`Db.Transact` is an implementation of `Db.TransactAsync` with a thin wrapper that synchronously waits for the returned `Task` object.

## `Db.TransactAsync`

`Db.TransactAsync` is the asynchronous counterpart of `Db.Transact`. It allows developers to have more complicated application designs and higher throughput. It returns a `Task` object that is marked as completed when flushing the transaction log for this transaction.

`Db.Transact` and `Db.TransactAsync` are syntactically identical:

```csharp
Db.TransactAsync(() => 
{
    // The code to run in the transaction
})
```

If a handler creates write transactions, use `Db.TransactAsync` and then wait for all transactions at once with `Task.WaitForAll` or `TaskFactory.ContinueWhenAll`. Otherwise, the latency of the handler will degrade.

### Limitations

With the `Db.TransactAsync` API, it's tempting to use async/await in applications. Syntactically it's possible, although it's not that useful due to these limitations:

1. Using async/await is not possible in a handler body as Handler API doesn't support async handlers.
2. No special measures have been taken to force after-await code to run on Starcounter threads, so manual `Scheduling.ScheduleTask()` might be required \(see [Running background jobs](running-background-jobs.md) for details\).
3. use async/await with caution as they may inadvertently increase the latency. Say if user code runs transactions sequentially, putting await in front of every `Db.TransactAsync` will accumulate all the individual latencies. The right stategy in this case is to make a list of tasks and then await them at once.

## Transaction Size

Code in `Db.Transact` and `Db.TransactAsync` should execute in as short time as possible because conflicts are likelier the longer the transaction is. Conflicts requires long transactions to run more times which can be expensive. The solution is to break the transaction into smaller transactions.

