# Transactions

**Note**: Starcounter 3.0.0 is currently in preview stage. This API might be changed in the future releases without backwards compatibility.

Starcounter uses transactions to ensure that database operations are [ACID](https://en.wikipedia.org/wiki/ACID).

All database reads and writes must be wrapped in transactions.

## Achieving ACID compliance

### Atomicity

As defined on [Wikipedia](https://en.wikipedia.org/wiki/Atomicity_%28database_systems%29), an atomic transaction "is an indivisible and irreducible series of database operations such that either all occur, or nothing occurs." Starcounter ensures atomicity by wrapping changes of one transaction within a transaction scope. The changes commit simultaneously at the end of the scope. If something interrupts the transaction before the end of the scope is reached, none of the changes will commit to the database.

### Consistency

A consistent DBMS ensures that all the data written to the database follow the defined contraints of the database. In Starcounter, this is solved by raising exceptions when an "illegal" action is carried out within a transaction, such as commiting non-unique values to a field that requires unique values. The exception will in turn make the transaction roll back so that none of the changes are applied.

### Isolation

To make transaction isolated, Starcounter uses [snapshot isolation](https://en.wikipedia.org/wiki/Snapshot_isolation). This means that when a transaction initializes, it takes a snapshot of the database and stores it in a transactional memory. Every transaction sees its own snapshot of the database. For example, an SQL query that executes before a parallel transaction commits will not be able to see the changes made by the transaction because the changes are isolated to that transaction's snapshot of the database. This works no matter how large the database is.

### Durability

Durability ensures that commited transactions will survive permanently. Starcounter solves this by writing transactions to a transaction log after commits.

### Concurrency control

When multiple users write to the database at the same time, the database engine must ensure that the data is consistent by using atomicity and isolation. For example, if an account reads 100 and you want to update it to 110 and another transaction is simultaneously reading a 100 and wants to update it to 120\. Should the result be 110, 120 or 130?

To resolve the problem with multiple transactions accessing the same data, the transaction must be able to handle conflicts. The easiest way to do this is to use locking. If you want your database engine to serve large numbers of users and transactions, locking is slow and expensive and can lead to [deadlocks](http://en.wikipedia.org/wiki/Deadlock). Locking is efficient when conflicts are likely, but is otherwise slow because it always consumes time, even if there are no conflicts. Another word for locking is "pessimistic concurrency control".

A more efficient way of providing concurrency than "pessimistic concurrency control" is "optimistic concurrency control". As the name implies, this concurrency mechanism assumes that conflicts are unlikely, but if conflicts happen, they are still handled. Starcounter uses optimistic concurrency control. Thus, the Starcounter database handles transactions without locking the modified objects. If there are conflicts, the developer can either provide a delegate to execute on conflict or use the `ITransactor.TryTransact` method to retry when there is a conflict.

### `ITransactor.Transact`

`ITransactor.Transact` is the simplest way to create a transaction in Starcounter. It declares a transactional scope and runs synchronously, as described above. The argument passed to the `ITransactor.Transact` method is a delegate containing the code to run within the transaction. We can get the `ITransactor` instance from the application [service provider](dependency-injection.md):

```csharp
var transactor = services.GetRequiredService<ITransactor>();
```

Then we can use the `ITransactor` to create new database transactions.

```csharp
transactor.Transact(db =>
{
    var person = db.Insert<Person>(); // Adds a row to the Person table.
    person.Name = "Gandalf";
});
```

Since `ITransactor.Transact` is synchronous, it blocks the executing thread until the transaction completes. Thus, if the transaction takes more than a few milliseconds to run, it might prevent your application's performance from scaling with CPU core counts. In those cases, it's better to use `ITransactor.TransactAsync`. `ITransactor.TransactAsync` returns a `Task` that completes when the transaction commits or rolls back, which lets you avoid thread blocking.

### `ITransactor.TransactAsync`

`ITransactor.TransactAsync` is the asynchronous counterpart of `ITransactor.Transact`. It gives the developer more control of balance throughput and latency. The function returns a `Task` that is marked as completed and successful with the property `IsCompletedSuccessfully` when the database operations are written to the transaction log which persists the changes.

`ITransactor.Transact` and `ITransactor.TransactAsync` are syntactically identical, but semantically different since `ITransactor.TransactAsync` is used with `await`. While awaiting the transaction log write operation, it's possible to do other things:

```csharp
Order order = null;

Task task = transactor.TransactAsync(db =>
{
    order = db.Insert<Order>();
});

// Order has been added to the database.

SendConfirmationEmail(order);

// Await the transaction log write operation.
await task;
```

This is more flexible and performant than `ITransactor.Transact`, but it comes with certain risks. For example, if there's a power outage or other hardware failure after the email is sent but before writing to the log, the email will be incorrect. Even if the user got a confirmation, the order will not be in the database since it was never written to the transaction log.

`ITransactor.TransactAsync` is also useful when creating many transactions in sequence:

```csharp
var coupon = GetPromotionalCoupon();
var customers = GetAllCustomers();

Task[] tasks = customers
    .Select(c => transactor.TransactAsync(db => c.AddCoupon(coupon)))
    .ToArray();

await Task.WhenAll(tasks);
```

This speeds up the application since the thread is free to handle the next transaction even if the database operations in the previous transactions are not written to the transaction log yet.

### Nested transactions

A nested transaction is a transaction that is run within the scope of another transaction. Starcounter does not support such transactions. Attempting to start a new transaction within an active transaction's scope will throw a `NotSupportedException` exception.

```csharp
transactor.Transact(db1 =>
{
    var examplePerson = db1.Insert<Person>();
    transactor.Transact(db2 => // this call to Transact will fail
    {
        examplePerson.Name = "John";
    });
});
```

Instead, in the common cases where such an inner transaction would be used, we should instead reuse the outer transaction or move the inner transaction logic to a new transaction that is started before or after the outer transaction.

### Transaction size

The logic we provide in delegates in calls to `ITransactor.Transact` and `ITransactor.TransactAsync` should execute in as short time as possible, since conflicts are more likely to occur the longer the transaction is. And retrying long transactions due to conflicts can be expensive resource-wise. Therefore it's best practice to break big transactions into smaller ones.

### Threading

Transactions serialize database access because the database kernel is not thread-safe. Since database access is serialized, database operations in the same transaction can't be parallelized.

For example, if you try to use PLINQ to parallelize database operations in a transaction, it will fail with `ScErrNoTransactionAttached`:

```csharp
transactor.Transact(db =>
{
    var customers = db.Sql<Customer>("SELECT p FROM Customer p");

    // Fails with ScErrNoTransactionAttached
    customers.AsParallel().ForAll(customer =>
    {
        customer.DiscountRate += 5;
    });
});
```

Even if the database operations of a transaction aren't parallelized, database operations from multiple transactions are executed either concurrently or in parallel, depending on the available database resources.

#### Async operations within a transaction

It is not recommended to execute any asynchronous operations within a database transaction, especially not those which have side effects. Still, Starcounter supports asynchronous transaction delegates.

The following example demonstrates how to asynchronously save a query result into a file.

```csharp
string fileName = await transactor.TransactAsync(async db =>
{
    string _fileName = Path.GetTempFileName();
    using TextWriter writer = new StreamWriter(_fileName);

    foreach (var person in db.Sql<Person>("SELECT p FROM Person p"))
    {
        await writer.WriteLineAsync($"{db.GetOid(person)}: {person.Name}");
    }

    return _fileName;
});

Console.WriteLine("Query result has been saved into " + fileName);
```

**Note**: Starcounter uses a custom synchronization context to serialize database access, that is why awaiter has to always continue on the captured context.
