# Transactions

The secret to the speed of Starcounter lies in the fact that all database objects _live_ in the database all the time rather than being moved to and from the database.

Starcounter is fully [ACID](http://en.wikipedia.org/wiki/ACID) compliant and consequently transactions are consistent, isolated and atomic. But if our database objects are in the database at all times, how do we prevent other users and transactions to see our unfinished changes?  Starcounter solves this using transactional memory, such that each transaction sees its private snapshot of the database. This means that code inside and outside a transaction scope will potentially read a different value for the same property.

This works even if the database is in the terabytes. This means that nobody sees what you are doing while you are in your own transactional scope. You can even use SQL to query your database snapshot within the transaction while outside transactions will not be able to see your changes until they are done.

### The Db.Transact function
You define transactional scope using the `Db.Transact` function. The scoped transaction is placed in a delegate.

```cs
using Starcounter;

class Hello
{
   static void Main()
   {
      Db.Transact(() =>
      {
         var albert = new Person()
         {
           FirstName = "Albert",
           LastName = "Einstein"
         };

         new Quote()
         {
           Person = albert,  
           Text = "Make things as simple as possible, but not simpler"
         };
      });
   }
}

[Database]
public class Person
{
   public string FirstName;
   public string LastName;
   public string FullName { get { return FirstName + " " + LastName; } }
}

[Database]
public class Quote
{
   public Person Person;
   public string Text;
}
```

For your convenience there are some overloads of the `Db.Transact` function that allows you to specify delegates that have input and output parameters.

```cs
Db.Transact(Action action, ...);
TResult Db.Transact<TResult>(Func<TResult> func, ...);
```  

### The Db.TransactAsync function
Under the hood `Db.Transact` is implemented in terms of `Db.TransactAsync` functions. `Db.TransactAsync` returns Task object, that is marked completed on flushing transaction log for this transaction. `Db.Transact` family is a thin wrapper around `Db.TransactAsync`, that effectively just calls Db.TransactAsync() and synchronously waits for returned Task. Thus Db.Transact() is a blocking call that waits for IO on write transactions. Take a look at the corresponding section at [More on transactions](more-on-transactions/) for reasoning and possible performance implications.

```cs
System.Threading.Tasks.Task TransactAsync(Action action, ...);
```  

{% import "../../macros.html" as macros %}

{{ macros.tocGenerator(page.title, summary.parts[0].articles[3].articles[2].articles) }}
