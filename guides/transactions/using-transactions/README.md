# Using Transactions

Starcounter implements transactions with `Db.Transact`, `Db.TransactAsync`, and `Db.Scope`. This page describes how to use `Db.Transact` and `Db.TransactAsync`. The page [Long Running Transactions](../long-running-transactions) covers `Db.Scope`.

## `Db.Transact`

`Db.Transact` is the simplest way to create a transaction in Starcounter. It declares a transactional scope and runs synchronously, as described on the [previous page](/guides/transactions). The argument passed to the `Db.Transact` method is a delegate containing the code to run within the transaction. In code, it looks like this:

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

Since `Db.Transact` is synchronous, it sometimes becomes a performance bottleneck. Starcounter handles this by automatically scaling the number of working threads to continue processing requests even if some handlers are blocked. The maximum number of working threads is the number of CPU cores multiplied by 254, so with four cores, there would be a maximum of 1016 working threads. When these threads are occupied, the next `Db.Transact` call in line will have to wait until a thread is freed. [`Db.TransactAsync`](#dbtransactasync) circumvents this.

`Db.Transact` is an implementation of `Db.TransactAsync` with a thin wrapper that synchronously waits for the returned `Task` object. 

## `Db.TransactAsync`

`Db.TransactAsync` is the asynchronous counterpart of `Db.Transact`. It allows developers to have more complicated application designs and higher throughput. It returns a `Task` object that is marked as completed when flushing the transaction log for this transaction.

`Db.Transact` and `Db.TransactAsync` are syntactically identical:

```cs
Db.TransactAsync(() => 
{
    // The code to run in the transaction
})
```

If a handler creates write transactions, use `Db.TransactAsync` and then wait for all transactions at once with `Task.WaitForAll` or `TaskFactory.ContinueWhenAll`. Otherwise, the latency of the handler will degrade.

### Limitations

With the `Db.TransactAsync` API, it's tempting to use async/await in applications. Syntactically it's possible, although it's not that useful due to these limitations:

1. Using async/await is not possible in a handler body as Handler API doesn't support async handlers.
2. No special measures have been taken to force after-await code to run on Starcounter threads, so manual `Scheduling.ScheduleTask()` might be required (see [Running background jobs](../running-background-jobs) for details).
3. use async/await with caution as they may inadvertently increase the latency. Say if user code runs transactions sequentially, putting await in front of every `Db.TransactAsync` will accumulate all the individual latencies. The right stategy in this case is to make a list of tasks and then await them at once.

## `Db.Transact` and `Db.TransactAsync` Usage

Use `Db.Transact` and `Db.TransactAsync` when changes should commit instantly, in comparison to [`Db.Scope`](../long-running-transactions) where changes wait until they are manually commited.

### Transaction Size

Code in `Db.Transact` and `Db.TransactAsync` should execute in as short time as possible because conflicts are likelier the longer the transaction is. Conflicts requires long transactions to run more times which can be expensive. The solution is to break the transaction into smaller transactions.

### Side Effects

Since `Db.Transact` and `Db.TransactAsync` can run more than once because of conflicts, they should not have any side effects, such as HTTP calls or writes to a file.  

### Rollbacks

The only way to rollback changes in `Db.Transact` and `Db.TransactAsync` is to throw an exception in the transaction. The alternative is to use `Db.Scope` with `Transaction.Rollback`.

### Conflicts

If conflicts are likely, use `Db.Transact` or `Db.TransactAsync` because these handle conflicts while `Db.Scope` doesn't.

## Nested transactions

When a transaction runs within another transaction, the changes will commit when the outermost transaction scope ends. Consider a situation like this: 

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

When executing `PaySalaries`, it creates an outer transaction scope. No changes will commit to the database until the scope ends. All the transactions created by `MakePayment` will commit at the same time. This protects the atomicity of the outer transaction in `PaySalaries`. 

## ScErrReadOnlyTransaction

If an operation is done on the database without a transaction an exception will be thrown:
```
The transaction is readonly and cannot be changed to write-mode. (ScErrReadOnlyTransaction (SCERR4093))
```

For example:

```cs
[Database]
public class Person {}

class Program
{
    static void Main()
    {
        new Person(); // SCERR4093
    }
}
```

To fix this, wrap the operation in a transaction:

```cs
[Database]
public class Person {}

class Program
{
    static void Main()
    {
        Db.Transact(() => new Person());
    }
}
```