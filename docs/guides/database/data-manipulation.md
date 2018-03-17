# Data manipulation

There are three data manipulation statements in SQL92: `INSERT`, `UPDATE` and `DELETE`. `INSERT`and `UPDATE` are not supported in Starcounter SQL while `DELETE` is available through `Db.SQL`. Objects are created and updated in the programming code.

All modifications have to be wrapped in a transaction. These modifications are visible to other transaction after the changes have been commited.

### Create Database Objects

Database objects are created with the native program code operator `new`. For example, consider the following database class:

```csharp
[Database]
public class Person
{

    public String FirstName { get; set; }
    public String LastName { get; set; }
}
```

To create a new instance of this class, the syntax `new Person()` would be used, like this:

```csharp
new Person()
{
    FirstName = "John",
    LastName = "Doe"
};
```

### Update Database Objects

A database object can be updated using the native program code assign operator `=`.

For example, instead of instantiating an object like in the example above, it's possible to create the object and then update its properties:

```csharp
var person = new Person();
person.FirstName = "John";
person.LastName = "Doe";
```

To update the `LastName` of all the `Person` objects in the database, they would be looped through and updated, like so:

```csharp
var people = Db.SQL<Person>("SELECT p FROM Person p");
foreach (var person in people)
{
    person.LastName = person.LastName.ToUpper();
}
```

### Delete Database Objects

There are two ways to delete database objects:

1. Using the `Delete` method on an object
2. Using `DELETE FROM`

`Delete` is used for single objects and `DELETE FROM` is used for many objects.

They look like this:

```csharp
var john = new Person();
john.Delete();

Db.SQL("DELETE FROM Person");
```

`person.Delete()` will just delete `john` while `DELETE FROM Person` will delete all objects of the `Person` class.

**Note**: To delete database objects that are bound to the view-model, the view-model object should be deleted before the database object is deleted.

