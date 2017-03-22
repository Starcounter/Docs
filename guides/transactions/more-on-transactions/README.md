# More on transactions

Creating an enterprise database management system would be easy if it was not for high concurrency. This means that a lot of users are reading and writing simultaneously while the system should guarantee atomicity, consistency, durability and isolation (ACID). This would be easy enough if you could simply use locking, but dealing with millions of users in this way works poorly. This is an area were Starcounter has its roots. It provides a lock free ACID engine that is based on transactional RAM and highly concurrent scheduling and memory management.

## What are transactions?

A database typically has many users (or threads) accessing its data simultaneously. Reads and writes occur in parallel. When reads and writes occur at the same time, [transactions](http://en.wikipedia.org/wiki/Database_transactions) are needed to ensure that the data stays correct.

## What are transactions good for?

To understand why transactions are so important, you should first be familiar with the kind of problems that would arise without them. Unless your database is read-only or unless only one user (or thread) accesses the database at a time, you need transaction management. Fortunately, it is very easy to use transactions in Starcounter. But before showing you how, it is vital to understand why transaction management is so important.

## The challenge

Let's say that your code is moving money from one account to another. Each account is stored in a database record. When you move money you perform three operations. You decrease money from the first account record. You increase money to the second account record. Finally, you add a new database record representing the event. This records stores the date and time, the amount and references to the two accounts. If another execution thread (another user) would write to the second account after you read the first account and before you wrote to the second account, the result would be incorrect. Also, if there was an error after you deducted the money but before money was added to the second account, the money would disappear into thin air.

## The solution

The solution is transaction atomicity and isolation. Atomicity makes sure that either everything has happened or nothing has happened. When one transaction is in the middle of changing data, only it sees the new information. The same records read by other transactions will show the previous state. This is called transaction isolation. Before and after a change, data should always be in a state that follows the integrity rules set up by the database. This is called transaction consistency. Also, when a transaction is done, the information should be secured. In Starcounter this is done by writing the data to one or more transaction log files. This ensures that the data is secured even if the database disk crashes.

## Transaction atomicity

By declaring all three operations as part of a single transaction you ensure that they happen as one atomic operation. If the receipt exists, both accounts are guaranteed to have changed. If it doesn't, both are guaranteed not to. Atomicity is the A of the important ACID properties of an enterprise database.

## Transaction isolation

So atomicity guarantees that either everything has happened, or nothing has happened. But what if another user sums up all accounts while they are being updated. Lets say that he or she reads the first account after it has been decreased but before the second account has been increased. The sum will be wrong. Money will be missing. The transaction will solve this. It will make sure that you only read a snapshot of the database. Only completed atomic transactions will be visible to you. While you are changing data, other transactions will still read the older versions until you are completely done. Isolation is the I in ACID.

## Transaction consistency and transaction durability

Starcounter uses transaction logs (a.k.a. redo logs) to ensure that transactions are durable. If the database disk crashes, the disk or disks containing the transaction log will be redone on top of the last backed up database image. This is called a recovery. The backup of a database image is called a checkpoint. If the data is not consistent (i.e. follows the rules) at the end of a transaction, it will be rolled back or redone. This means that noone will ever see any changes at all to the database. The situations where a transaction is rolled back includes the following:

* A field that is required to contain a unique value contains a non unique value
* There was an unhandled error during the transaction

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

## Conflicts

In case of a transaction conflict, the method <code>Db.Transact(<var>Action</var>)</code>
will automatically restart the transaction. The maximum times a transaction will be restarted is a 100 times.

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

## Transaction concurrency
When many users write to the database at the same time, the database engine must ensure that the data keeps consistent using atomicity and isolation. I.e. if an account reads 100 and you want to update it to 110 and another transaction is simultaneously reading a 100 and wants to update it to 120. Should the result be 110, 120 or 130? In order to resolve this the transaction must be able to resolve conflicts. The easiest way to do this is to use locking. However, if you want your database engine to serve large numbers of users and transactions, locking is slow and expensive and can lead to [deadlocks](http://en.wikipedia.org/wiki/Deadlock). Locking is efficient when you almost expect a conflict, i.e. when the probability is high that you will have a conflict. The slow nature of locking is that it always consumes time, even if there is no conflict. Another word for locking is 'pessimistic concurrency control'. A more efficient way of providing concurrency is to use 'optimistic concurrency control'. As the name implies, you don't expect a conflict, but you will still handle it correctly. The concurrency control in Starcounter is **optimistic concurrency control**.

## Optimistic transaction concurrency

Optimistic concurrency control makes an assumption that conflicts between transactions are unlikely. The database can therefore allow transactions to execute without locking the modified objects. If a conflict occurs Starcounter will retry the changes numerous times, if you are not using long-running transactions in which case you need to implement the retry functionality yourself

## Pessimistic transaction concurrency
Pessimistic concurrency control locks objects when executing the transaction. This concurrency control often gives very negative performance hits.

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

## Db.TransactAsync and async/await

With Db.TransactAsync API it's tempting to employ async/await in the user app. Syntactically it's possible, although it's not that useful due to several shortcomings:

1. Using async/await is not possible in a handler body as Handler API doesn't support async handlers.
2. No special measures have been taken to force after-await code to run on a Starcounter thread, so manual `Scheduling.ScheduleTask()` might be required (see [Running background jobs](../running-background-jobs) for details).
3. async/await should be used with caution as they may inadvertently increase the latency. Say if user code runs several transactions sequentially, putting await in front of every `Db.TransactAsync()` will accumulate all individual latencies. Right stategy in this case is to make a list of tasks and the await all of them at once.
