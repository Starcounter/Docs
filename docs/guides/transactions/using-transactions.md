# Using Transactions

Starcounter implements transactions with `Db.Transact`, `Db.TransactAsync`, and `Db.Scope`. This page describes how to use `Db.Transact` and `Db.TransactAsync`. The page [Long Running Transactions](long-running-transactions.md) covers `Db.Scope`.

### `Db.Transact`

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

Since `Db.Transact` is synchronous, there are cases where it becomes a performance bottleneck. Starcounter handles this by automatically scaling the number of working threads to continue processing requests even if some handlers are blocked. The maximum number of working threads is the number of CPU cores multiplied by 254, so with four cores, there would be a maximum of 1016 working threads. When these threads are occupied, the next `Db.Transact` call in line will have to wait until a thread is freed. [`Db.TransactAsync`](../../#dbtransactasync) circumvents this.

Worth noting is that `Db.Transact` is actually `Db.TransactAsync` but with a thin wrapper that synchronously waits for the returned `Task` object.

### `Db.TransactAsync`

`Db.TransactAsync` is the asynchronous counterpart of `Db.Transact`. It allows developers to have more complicated application designs and higher throughput. It returns a `Task` object that is marked as completed when flushing the transaction log for this transaction.

`Db.Transact` and `Db.TransactAsync` are syntactically identical:

```csharp
Db.TransactAsync(() => 
{
    // The code to run in the transaction
})
```

It's worth nothing that if a handler issues write transactions, then it's reasonable to use `Db.TransactAsync` and then wait for all transactions at once with `Task.WaitForAll` or `TaskFactory.ContinueWhenAll`. Otherwise, the latency of such handler would degrade.

### Limitations

With the `Db.TransactAsync` API, it's tempting to use async/await in applications. Syntactically it's possible, although it's not that useful due to these shortcomings:

1. Using async/await is not possible in a handler body as Handler API doesn't support async handlers.
2. No special measures have been taken to force after-await code to run on Starcounter threads, so manual `Scheduling.ScheduleTask()` might be required \(see [Running background jobs](running-background-jobs.md) for details\).
3. use async/await with caution as they may inadvertently increase the latency. Say if user code runs transactions sequentially, putting await in front of every `Db.TransactAsync` will accumulate all the individual latencies. The right stategy in this case is to make a list of tasks and then await them at once.

### `Db.Transact` and `Db.TransactAsync` Usage

Use `Db.Transact` and `Db.TransactAsync` when changes are to commit instantly, in comparison to [`Db.Scope`](long-running-transactions.md) where changes wait until they manually commit. Due to this, if there's a need to rollback changes, use `Db.Scope` over `Db.Transact` or `Db.TransactAsync`. Also, keep in mind that `Db.Scope` does not handle conflicts, thus, if conflicts are likely, `Db.Transact` or `Db.TransactAsync` is better suited for the job.

Code in `Db.Transact` and `Db.TransactAsync` should execute in as short time as possible because conflicts are likelier the longer the transaction is. Conflicts requires long transactions to run more times which can be expensive. The solution is to break the transaction into smaller transactions.

Since `Db.Transact` and `Db.TransactAsync` can run more than once, they should not have any side effects, such as HTTP calls or writes to a file.

### Nested transactions

When a transaction runs within another transaction, the changes will commit when the outermost transaction scope ends. Consider a situation like this:

```csharp
public void MakePayment(Account payerAccount, Account receiverAccount, Decimal amount)
{
    Db.Transact(() => // Inner transaction
    {
        payerAccount.Balance -= amount;
        receiverAccount.Balance += amount;
    };
}

public void PaySalaries()
{
    Db.Transact(() => // Outer transaction
    {
        QueryResultRows<Employee> employees = Db.SQL<Employee>("SELECT e FROM Employee e");
        foreach(var employee in employees)
        {
            MakePayment(company.Account, employee.Account, employee.Salary);
        }
    };
}
```

When executing `PaySalaries`, it creates an outer transaction scope. No changes will commit to the database until the scope ends. All the transactions created by `MakePayment` will commit at the same time. This protects the atomicity of the outer transaction in `PaySalaries`.

