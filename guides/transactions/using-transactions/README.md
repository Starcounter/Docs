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

When executing `PaySalarlies`, it creates an outer transaction scope. No changes will be commited to the database until the scope ends. Thus, all the transactions created by `MakePayment` will be commited at the same time. This is done to protect the atomicity of the outer transaction in `PaySalaries`. 

## A more complete example

In Starcounter, you declare transaction scopes. A transaction scope surrounds its code and declares that the database operations within the scope should be atomic and isolated. The following example shows how to declare transaction scope:

```cs
using Starcounter;

[Database]
public class Account
{
   public string AccountId;
   public decimal Amount;
}

public class MoneyTransfer
{
  public Account FromAccount;
  public Account ToAccount;
  public decimal Amount;

  public static void MoveMoney(Account fromAccount, Account toAccount, decimal amount)
  {
      Db.Transact(() =>
      {
         MoneyTransfer a = new MoneyTransfer();
         a.FromAccount = fromAccount;
         a.ToAccount = toAccount;
         a.Amount = amount;
         fromAccount.Amount -= amount;
         toAccount.Amount += amount;
     };
  }
}
```

## Transaction scopes

If you have worked with older SQL databases, you might be familiar with transactions but not with transaction scopes. In modern databases, transaction scopes allows transactions to be nested. For those of you who have been using Microsoft SQL Server, you might be familiar with the "BEGIN TRANSACTION" command. Starcounter's transaction scopes works in the same way. Whereas the old school SQL COMMIT always commits a transaction, the end of a transaction scope might not depending on if you are within a nested transaction or not.

## Nested transaction scopes
When you nest transactions scopes, only the topmost transaction scope will actually commit any changes and make them visible to outside transactions (other users). Consider the following example:

```cs
[Database]
public class Account
{
   public string AccountId;
   public decimal Amount;
   public static Account GetAccount(string id)  
   {
      return Db.SQL<Account>("SELECT A FROM Account A WHERE AccountId=?", id ).First;
   }
}

public class MoneyTransfer
{
  public Account FromAccount;
  public Account ToAccount;
  public decimal Amount;

  public static void MoveMoney(Account fromAccount, Account toAccount, decimal amount)
  {
      Db.Transact(() =>
      {
         MoneyTransfer a = new MoneyTransfer();
         a.FromAccount = fromAccount;
         a.ToAccount = toAccount;
         a.Amount = amount;
         fromAccount.Amount -= amount;
         toAccount.Amount += amount;
      };
  }

  public static void HelloScope(decimal amount)  
  {
     Db.Transact(() =>
     {
        MoveMoney( GetAccount("Client A"), GetAccount("Client D"), 100 );
        MoveMoney( GetAccount("Client B"), GetAccount("Client D"), 100 );
        MoveMoney( GetAccount("Client C"), GetAccount("Client D"), 100 );
     };
  }
```

At the end of the MoveMoney function, the transaction scope ends. Should we commit? If we do, we have ensured the atomicity of MoveMoney, but we have violated the atomicity of HelloScope(). In hello scope, we have been instructed that all three money transfers are atomic. It is a contract we need to abide if in order to be ACID compliant. Either everything in the scope has happened, or nothing. Before we are finished, we must not show any changes to any viewer outside of the transaction. What if we commit the changes at the end of HelloScope() where the parent transaction scope ends? In this case, we have honored both contracts. Both transactions are ACID. So transaction scope is a declaration of atomicity, not an instruction to commit. Actually it would be possible to leave out the possibility of nested transaction scopes, but then you would have to have two MoveMoney() functions or provide some parameter to instruct it to commit or not to commit. By using transaction scopes however, you can make encapsulated transactions that can be used inside any other transaction without compromising their atomicity.