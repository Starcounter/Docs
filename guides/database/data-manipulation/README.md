# Data manipulation

There are three data manipulation statements in SQL92: `INSERT`, `UPDATE` and `DELETE`. `INSERT`and `UPDATE` are not supported in Starcounter SQL while `DELETE` is available through `Db.SlowSQL`. Objects are instead created, updated, and deleted directly in the programming code.

All modifications have to be wrapped in a transaction. These modifications are visible to other transaction after the changes have been commited.

## Create Database Objects

Database objects are created with the native program code operator `new`. For example, consider the following database class:

```cs
[Database]
public class Person
{
    public String FirstName;
    public String LastName;
}
```

To create a new instance of this class, the syntax `new Person()` would be used, like this:

```cs
var person = new Person()
{
    FirstName = "John",
    LastName = "Doe"
};
```

## Update Database Objects

A database object can be updated using the native program code assign operator `=`.

For example, instead of instantiating an object like in the example above, it's possible to create the object and then update its properties and fields:

```cs
var person = new Person();
person.FirstName = "John";
person.LastName = "Doe";
```

To update the `LastName` of all the `Person` objects in the database, they would be looped through and updated, like so:

```cs
var people = Db.SQL<Person>("SELECT p FROM Person p);
foreach (var person in people)
{
    person.LastName = person.LastName.ToUpper();
}
```

## Delete Database Objects

There are two ways to delete database objects in Starcounter:

1. Using the `Delete` method on a specific object
2. Using `DELETE FROM` in `Db.SlowSQL`

Essentially, `Delete` is used for single objects and `DELETE FROM` is used for many objects. 

In code, they look like this:

```cs
person = new Person();
person.Delete();

Db.SlowSQL("DELETE FROM Person");
```

Here, `person.Delete()` will just delete that single object while `DELETE FROM Person` will delete all objects belonging to the `Person` class. 

It's worth keeping in mind that using `DELETE FROM` might create a massive transaction that can hit the size limit for transactions as explained in the [Kernel Q&A](guides/working-with-starcounter/kernel-q-and-a/). The reason for this is that `DELETE FROM` only creates one transaction which scales with the number of columns to delete. To work around this limit, create a loop that deletes each object using the `Delete` method and commit the changes in smaller chunks. The performance loss for this is small since `DELETE FROM` is implemented in a similar fashion except that it does not break up the transaction into smaller transactions.

**Note**: To delete database objects that are bound to the view-model, the view-model object should be deleted before the database object is deleted.


