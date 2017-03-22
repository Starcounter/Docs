# Transactions

The secret to the speed of Starcounter lies in the fact that all database objects _live_ in the database all the time rather than being moved to and from the database.

Starcounter is fully [ACID](http://en.wikipedia.org/wiki/ACID) compliant and consequently transactions are consistent, isolated and atomic. But if our database objects are in the database at all times, how do we prevent other users and transactions to see our unfinished changes?  Starcounter solves this using transactional memory, such that each transaction sees its private snapshot of the database. This means that code inside and outside a transaction scope will potentially read a different value for the same property.

This works even if the database is in the terabytes. This means that nobody sees what you are doing while you are in your own transactional scope. You can even use SQL to query your database snapshot within the transaction while outside transactions will not be able to see your changes until they are done.

## What are transactions?

A database typically has many users or threads accessing its data simultaneously. Reads and writes occur in parallel. When reads and writes occur at the same time, [transactions](http://en.wikipedia.org/wiki/Database_transactions) are needed to ensure that the data stays correct.

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

## Transaction concurrency
When many users write to the database at the same time, the database engine must ensure that the data keeps consistent using atomicity and isolation. I.e. if an account reads 100 and you want to update it to 110 and another transaction is simultaneously reading a 100 and wants to update it to 120. Should the result be 110, 120 or 130? In order to resolve this the transaction must be able to resolve conflicts. The easiest way to do this is to use locking. However, if you want your database engine to serve large numbers of users and transactions, locking is slow and expensive and can lead to [deadlocks](http://en.wikipedia.org/wiki/Deadlock). Locking is efficient when you almost expect a conflict, i.e. when the probability is high that you will have a conflict. The slow nature of locking is that it always consumes time, even if there is no conflict. Another word for locking is 'pessimistic concurrency control'. A more efficient way of providing concurrency is to use 'optimistic concurrency control'. As the name implies, you don't expect a conflict, but you will still handle it correctly. The concurrency control in Starcounter is **optimistic concurrency control**.

## Optimistic transaction concurrency

Optimistic concurrency control makes an assumption that conflicts between transactions are unlikely. The database can therefore allow transactions to execute without locking the modified objects. If a conflict occurs Starcounter will restart the transaction until the transaction is successfully commited, or a maximum of 100 times. For long-running transactions, the developer has to implement the retry functionality him- or herself. 

## Pessimistic transaction concurrency
Pessimistic concurrency control locks objects when executing the transaction. This concurrency control often gives very negative performance hits.

{% import "../../macros.html" as macros %}

{{ macros.tocGenerator(page.title, summary.parts[0].articles[3].articles[2].articles) }}
