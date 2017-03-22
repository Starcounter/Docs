# Using Transactions

## The basics

Write access to the database can only be done within the scope of a transaction.
You get a transaction scope by calling the method <code>Transact</code> of the
static class <code>Db</code> (namespace <code>Starcounter</code>).
To the method <code>Transact</code> you pass a delegate with the code that
should be run within the transaction.

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

## Nested transactions

Code within nested transaction scopes will be run within the same transaction,
and the transaction is comitted when the outermost transaction scope ends.
Consider the example code below. When executing the method <code>PaymentOfSalaries</code>
there will be nested transaction scopes since <code>PaymentOfSalaries</code> has one
transaction scope and that method calls <code>Payment</code> which has another transaction scope.
Thus when executing <code>PaymentOfSalaries</code> all calls of <code>Payment</code> will
be executed within the same transaction as <code>PaymentOfSalaries</code>.

```cs
public class Administration
{
    Company company;
    ...

    public void Payment(Account from, Account to, Decimal amount)
    {
        Db.Transact(() =>
        {
            from.Balance = from.Balance - amount;
            to.Balance = to.Balance + amount;
        };
    }

    public void PaymentOfSalaries()
    {
        Db.Transact(() =>
        {
            QueryResultRows<Employee> result = Db.SQL<Employee>("SELECT e FROM Employee e");
            foreach(Employee emp in result)
            {
                Payment(company.Account, emp.Account, emp.Salary);
            }
        };
    }
    ...
}  
```

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

## Synchronous vs Asynchronous transactions
As was mentioned, Db.Transact performs blocking wait on IO and has non-blocking counterpart - Db.TransactAsync. The reason for this is to let application code means to know when transaction is secured and its safe for the application to cause any side-effects based on that fact. Blocking call to Db.Transact could be a problem depending on application design, throughput and latency requirements. In certain scenarios it's not that critical - Starcounter can automatically scale the number of working threads, so it will continue processing requests despite some handlers are blocked. Number of threads are limited by ```number_of_cpu*254```. If achieved throughput is not sufficient then handlers are to be redesigned to use Db.TransactAsync() instead of Db.Transact().

Instead:
```cs
Db.Transact(...);
return new Response(...);
```

Do:
```cs
Db.TransactAsync(...).ContinueWith(
    () => Scheduling.ScheduleTask(
      () => request.SendResponse(new Response(...))
    )
);
return HandlerStatus.Handled;
```

Worth to note that if handler issues several write transactions, then it's always reasonable to use Db.TransactAsync() and then wait for all Transactions at once with Task.WaitForAll(), TaskFactory.ContinueWhenAll() or similar. Otherwise, the latency of such handler would extremely degrade.

Under the hood, `Db.Transact` is implemented in terms of `Db.TransactAsync` functions. `Db.TransactAsync` returns a `Task` object that is marked completed on flushing transaction log for this transaction. The `Db.Transact` family is a thin wrapper around `Db.TransactAsync` that effectively just calls `Db.TransactAsync` and synchronously waits for returned Task. Thus, `Db.Transact` is a blocking call that waits for IO on write transactions.
```cs
System.Threading.Tasks.Task TransactAsync(Action action, ...);
```  

## Db.TransactAsync and async/await

With Db.TransactAsync API it's tempting to employ async/await in the user app. Syntactically it's possible, although it's not that useful due to several shortcomings:

1. Using async/await is not possible in a handler body as Handler API doesn't support async handlers.
2. No special measures have been taken to force after-await code to run on a Starcounter thread, so manual `Scheduling.ScheduleTask()` might be required (see [Running background jobs](../running-background-jobs) for details).
3. async/await should be used with caution as they may inadvertently increase the latency. Say if user code runs several transactions sequentially, putting await in front of every `Db.TransactAsync()` will accumulate all individual latencies. Right stategy in this case is to make a list of tasks and the await all of them at once.
