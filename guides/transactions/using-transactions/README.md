# Using Transactions

Transactions in Starcounter are implemented with `Db.Transact`, `Db.TransactAsync`, and `Db.Scope`. This page will describe how to use `Db.Transact` and `Db.TransactAsync`. `Db.Scope` is covered on the page [Long Running Transactions](../long-running-transactions).

## `Db.Transact`

`Db.Transact` is the simplest way to create a transaction in Starcounter. It declares a transactional scope as described on the [previous page](/guides/transactions) and is run synchronously.
The argument passed to the `Db.Transact` method is a delegate containing the code to be run within the transaction. In code, it looks like this:

```cs
Db.Transact(() =>
{
    new Employee
    {
        FirstName = "Samwise",
        LastName = "Gamgee"
    };
});
```

For your convenience there are some overloads of the `Db.Transact` function that allows you to specify delegates that have input and output parameters.

```cs
Db.Transact(Action action, ...);
TResult Db.Transact<TResult>(Func<TResult> func, ...);
```  

Since `Db.Transact` is synchronous, there might be cases where it becomes a performance bottleneck. Starcounter deals with this by automatically scaling the number of working threads to continue processing requests even if some handlers are blocked. The maximum number of working threads is the number of CPU cores multiplied by 254, so with four cores, there would be a maximum of 1016 working threads. When all these threads are occupied, the next `Db.Transact` call in line will have to wait until a thread is freed. This can be circumvented by using [`Db.TransactAsync`](#dbtransactasync).

Worth noting is that `Db.Transact` is actually `Db.TransactAsync` but with a thin wrapper that synchronously waits for the returned `Task` object. 

## `Db.TransactAsync`

`Db.TransactAsync` is the asynchronous counterpart of `Db.Transact`. It allows developers to have more complicated application designs and higher throughput. It returns a `Task` object that is marked as completed when flushing the transaction log for this transaction.

It is declared the same way as `Db.Transact`:

```cs
Db.TransactAsync(() => 
{
    // The code to be run in the transaction
})
```

It's worth nothing that if a handler issues several write transactions, then it's reasonable to use `Db.TransactAsync` and then wait for all transactions at once with `Task.WaitForAll` or `TaskFactory.ContinueWhenAll`. Otherwise, the latency of such handler would extremely degrade.

### Limitations

With Db.TransactAsync API it's tempting to employ async/await in the user app. Syntactically it's possible, although it's not that useful due to several shortcomings:

1. Using async/await is not possible in a handler body as Handler API doesn't support async handlers.
2. No special measures have been taken to force after-await code to run on Starcounter threads, so manual `Scheduling.ScheduleTask()` might be required (see [Running background jobs](../running-background-jobs) for details).
3. async/await should be used with caution as they may inadvertently increase the latency. Say if user code runs several transactions sequentially, putting await in front of every `Db.TransactAsync` will accumulate all the individual latencies. The right stategy in this case is to make a list of tasks and then await all of them at once.

## `Db.Transact` and `Db.TransactAsync` Usage

`Db.Transact` and `Db.TransactAsync` should be used when changes are to be commited instantly, in comparison to [`Db.Scope`](../long-running-transactions) where changes wait until they are manually commited. Due to this, if there's a need to rollback changes, `Db.Scope` should be used over `Db.Transact` or `Db.TransactAsync`. Also, keep in mind that `Db.Scope` does not handle conflicts, thus, if conflicts are likely, `Db.Transact` or `Db.TransactAsync` is better suited for the job.

It is also recommended that the code in `Db.Transact` and `Db.TransactAsync` can be executed in less than approximately two seconds. The reason for this is that the longer the transaction is, the more likely it is that there will be conflicts. Thus, long transactions might be required to run several times which can be expensive. The solution is to simply break the transaction into several smaller transactions. 

Since `Db.Transact` and `Db.TransactAsync` might run several times, it is also important that they do not have any side effects, such as HTTP calls or writes to a file.  

## Nested transactions

When a transaction is run within another transaction, the changes will be commited when the outermost transaction scope ends. Consider a situation like this: 

```cs
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

When executing `PaySalaries`, it creates an outer transaction scope. No changes will be commited to the database until the scope ends. Thus, all the transactions created by `MakePayment` will be commited at the same time. This is done to protect the atomicity of the outer transaction in `PaySalaries`. 