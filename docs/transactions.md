# Transactions

**Note**: Starcounter 3.0.0 is currently in preview stage. This API might be changed in the future releases without backwards compatibility.

Starcounter uses transactions to ensure that database operations are [ACID](https://en.wikipedia.org/wiki/ACID).

All database reads and writes must be wrapped in transactions.

## Achieving ACID Compliance

### Atomicity

As defined on [Wikipedia](https://en.wikipedia.org/wiki/Atomicity_%28database_systems%29), an atomic transaction "is an indivisible and irreducible series of database operations such that either all occur, or nothing occurs."
Starcounter ensures atomicity by wrapping changes of one transaction within a transaction scope. The changes commit simultaneously at the end of the scope.
If something interrupts the transaction before the end of the scope is reached, none of the changes will commit to the database.

### Consistency

A consistent DBMS ensures that all the data written to the database follow the defined contraints of the database. 
In Starcounter, this is solved by raising exceptions when an "illegal" action is carried out within a transaction, such as commiting non-unique values to a field that requires unique values.
The exception will in turn make the transaction roll back so that none of the changes are applied.

### Isolation

To make transaction isolated, Starcounter uses [snapshot isolation](https://en.wikipedia.org/wiki/Snapshot_isolation).
This means that when a transaction initializes, it takes a snapshot of the database and stores it in a transactional memory.
Every transaction sees its own snapshot of the database. For example,
an SQL query that executes before a parallel transaction commits will not be able to see the changes made by the transaction because the changes are isolated to that transaction's snapshot of the database.
This works no matter how large the database is.

### Durability

Durability ensures that commited transactions will survive permanently. Starcounter solves this by writing transactions to a transaction log after commits.

### Concurrency Control

When multiple users write to the database at the same time, the database engine must ensure that the data is consistent by using atomicity and isolation.
For example, if an account reads 100 and you want to update it to 110 and another transaction is simultaneously reading a 100 and wants to update it to 120.
Should the result be 110, 120 or 130?

To resolve the problem with multiple transactions accessing the same data, the transaction must be able to handle conflicts.
The easiest way to do this is to use locking.
If you want your database engine to serve large numbers of users and transactions, locking is slow and expensive and can lead to [deadlocks](http://en.wikipedia.org/wiki/Deadlock).
Locking is efficient when conflicts are likely, but is otherwise slow because it always consumes time, even if there are no conflicts.
Another word for locking is "pessimistic concurrency control".

A more efficient way of providing concurrency than "pessimistic concurrency control" is "optimistic concurrency control".
As the name implies, this concurrency mechanism assumes that conflicts are unlikely, but if conflicts happen, they are still handled.
Starcounter uses optimistic concurrency control. Thus, the Starcounter database handles transactions without locking the modified objects.
If there are conflicts, the developer can either provide a delegate to execute on conflict or use the `Db.TryTransact` method to retry when there is a conflict.

### `Db.Transact`

`Db.Transact` is the simplest way to create a transaction in Starcounter.
It declares a transactional scope and runs synchronously, as described above.
The argument passed to the Db.Transact method is a delegate containing the code to run within the transaction. In code, it looks like this:

```cs
Db.Transact(() =>
{
    // Adds a row to the Person table.
    var person = Db.Insert<Person>();
    person.Name = "Gandalf";
});
```

Since `Db.Transact` is synchronous, it blocks the executing thread until the transaction completes.
Thus, if the transaction takes more than a few milliseconds to run, it might prevent your application's performance from scaling with CPU core counts.
In those cases, use `Db.TransactAsync` instead. `Db.TransactAsync` returns a `Task` that completes when the transaction commits or rolls back which lets you avoid blocking.

### `Db.TransactAsync`

`Db.TransactAsync` is the asynchronous counterpart of `Db.Transact`.
It gives the developer more control to balance throughput and latency.
The function returns a `Task` that is marked as completed and successful with the property `IsCompletedSuccessfully` when the database operations are written to the transaction log which persists the changes.

`Db.Transact` and `Db.TransactAsync` are syntactically identical, but semantically different since `Db.TransactAsync` is used with `await`:

```cs
await Db.TransactAsync(() =>
{
    // The code to run in the transaction.
});
```

While waiting for the write to the transaction log to finish, it's possible to do other things, such as sending an email:

```cs
Order order = null;

Task task = Db.TransactAsync(() =>
{
    order = Db.Insert<Order>();
});

// Order has been added to the database.

SendConfirmationEmail(order);

// Wait until transaction is persisted.
await task;
```

This is more flexible and performant than Db.Transact, but it comes with certain risks;
for example, if there's a power outage or other hardware failure after the email is sent but before writing to the log,
the email will be incorrect - even if the user got a confirmation, the order will not be in the database since it was never written to the transaction log.

`Db.TransactAsync` is useful when creating many transactions in sequence:

```cs
var coupon = GetPromotionalCoupon();

Customer[] customers = GetAllCustomers();

Task[] tasks = customers
    .Select(c => Db.TransactAsync(() => c.AddCoupon(coupon)))
    .ToArray();

// Alternatively, use Task.WaitAll(tasks) to block until tasks are completed.
await Task.WhenAll(tasks);
```

This speeds up the application since the thread is free to handle the next transaction even if the database operations in the previous transactions are not written to the transaction log yet.

### Nested Transactions

Transactions can't be nested unless you specify what to do on commit for the inner transaction.
To understand why it's this way, take a look at this example:

```cs
public void OrderProduct(long productId, Customer customer)
{
    Db.Transact(() =>
    {
        SendInvoice(productId, customer);
        RemoveFromInventory(productId);
    });
}

public void SendInvoice(long productId, Customer customer)
{
    var invoice = new Invoice(productId, customer);
    Db.Transact(() => AddInvoiceToDb(invoice));
    invoice.Send();
}
```

After the `SendInvoice` transaction, you'd expect that the invoice has been committed to the database.
That is not the case.
To preserve the atomicity of the outer transaction, the changes have to be committed at the same time at the end of the outer transaction's scope.
Thus, if the invoice was sent on line 14 and then the whole transaction could roll back in `RemoveFromInventory` and undo `AddInvoiceToDb`.
This would cause the customer to receive an invoice that is not stored in the database.

Due to this, inner transactions have to specify what to do on commit. To adapt the previous example to specify what to do on commit, we would do this to `SendInvoice`:

```cs
public void SendInvoice(long productId, Customer customer)
{
    var invoice = new Invoice(productId, customer);
    Db.Transact(() => AddInvoiceToDb(invoice),
        new TransactOptions(() => invoice.Send()));
}
```

With this change, the invoice would be sent whenever the changes in `AddInvoiceToDb` are committed and you can be sure that the invoice would be sent first when the invoice is safely in the database.
In this case, it would be when the outer transaction scope terminates. If there was no outer transaction, the invoice would be sent when the transaction with `AddInvoiceToDb` terminates.
Thus, `onCommit` ensures that the calls are made in the correct order no matter what.

If you don't know if a transaction will be nested within another transaction, it's always safe to add an empty `onCommit` delegate:

```cs
Db.Transact(() =>
{
    // Read and write to database
}, new TransactOptions(() => {}));
```

With an empty `onCommit` delegate, you acknowledge that the changes are not guaranteed to commit after the transaction scope.

If an inner transaction doesn't have an onCommit delegate, Starcounter throws `ArgumentNullException`.

### Transaction Size

Code in `Db.Transact` and `Db.TransactAsync` should execute in as short time as possible because conflicts are more likely the longer the transaction is.
Conflicts requires long transactions to run more times which can be expensive.
The solution is to break big transactions into smaller ones.

### Threading

Transactions serialize database access because the database kernel is not thread-safe. Since database access is serialized, database operations in the same transaction can't be parallelized.

For example, if you try to use PLINQ to parallelize database operations in a transaction, it will fail with `ScErrNoTransactionAttached`:


```cs
Db.Transact(() =>
{
    var customers = Db.SQL<Customer>("SELECT p FROM Customer p");

    // Fails with ScErrNoTransactionAttached
    customers.AsParallel().ForAll(customer =>
    {
        customer.DiscountRate += 5;
    });
});
```

Even if database operations in the same transactions aren't parallelized, database operations from multiple transactions are run either concurrently or in parallel, depending on the available database resources.

It's possible to use `async`/`await` in transactions. To perform database operations asynchronously in transactions, you have to make sure it uses the enclosing context:

```cs
await Db.TransactAsync(async () =>
{
    await Task.Factory.StartNew(() =>
        {
            // Database operations
        },
        CancellationToken.None,
        TaskCreationOptions.None,
        TaskScheduler.FromCurrentSynchronizationContext());
});
```