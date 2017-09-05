# Using Transactions

## Choosing Transaction

It's important to chose the right transaction. In most situation, the choice is clear - if you are going to attach a transaction to a view-model, use a long-running transaction, otherwise, use a short-running transaction. When the choice is not clear, consider these factors:

### Side Effects

Since `Db.Transact` and `Db.TransactAsync` can run more than once because of conflicts, they should not have any side effects, such as HTTP calls or writes to a file. `Db.Scope` can have side effects, as long as it's [not in an iterator](../long-running-transactions/#scerriteratorclosed-scerr4139). 

### Rollbacks

The only way to rollback changes in `Db.Transact` and `Db.TransactAsync` is to throw an exception in the transaction. The alternative is to use `Db.Scope` with `Transaction.Rollback`.

### Conflicts

If conflicts are likely, use `Db.Transact` or `Db.TransactAsync` because these handle conflicts while `Db.Scope` doesn't.

## Mixing Transactions

Transactions can be mixed as outer and inner transactions - one transaction wraps around the other. These are the possible combinations and their effects:

| Outer | Inner | Effect                                  |
|-------|-------|-----------------------------------------|
| Long  | Long  | Execute inner as a part of outer        |
| Long  | Short | Execute inner as a separate transaction |
| Short | Long  | Not supported. Run-time error           |
| Short | Short | Execute inner as part of outer          |

### Long-Running in Long-Running

With a long-running transaction inside a long-running transaction, they act as if they were one transaction:

```cs
[Database]
public class Person {}

[Database]
public class Animal {}

class Program
{
    static void Main()
    {
        Db.Scope(() =>
        {
            new Person();

            Db.Scope(() =>
            {
                Transaction.Current.Commit(); // Commits the Person
                new Animal();
            });

            Transaction.Current.Commit(); // Commits the Animal
        });
    }
}
```

### Short-Running in Long-Running

Short-running transactions in long-running transactions are executed separately: 

```cs
using Starcounter;
using System.Linq;

[Database]
public class Person {}

[Database]
public class Animal
{
    public string Specie { get; set; }
}

class Program
{
    static void Main()
    {
        Db.Scope(() =>
        {
            new Person();

            Db.Transact(() =>
            {
                new Animal() { Specie = "Dog" };
            }); // Animal is commited to the database - transaction is done

            // The Animal committed can be accessed in the outer transaction
            var animal = Db.SQL("SELECT a FROM Animal a").First();

            // Rolls back the Person but not the Animal
            Transaction.Current.Rollback();
        });
    }
}
```

### Long-Running in Short-Running

Using long-running transactions in short-running transactions is not supported, it will throw `ScErrTransactionLockedOnThread (SCERR4031)`:

```cs
Db.Transact(() =>
{
    Db.Scope(() => // SCERR4031
    {
        new Person(); 
    });
}); 
```

## Short-Running in Short-Running

Short-running in short-running transactions work the same as with long-running in long-running transactions: the inner transaction is executed as a part of the outer:

```cs
using Starcounter;

[Database]
public class Person {}

[Database]
public class Animal {}

class Program
{
    static void Main()
    {
        Db.Transact(() =>
        {
            Db.Transact(() =>
            {
                new Person();
            }); // Person is not commited

            new Animal();
        }); // Animal and Person are commited
    }
}
```

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