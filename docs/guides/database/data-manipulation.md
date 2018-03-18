# Data manipulation

## Introduction

There are three data manipulation statements in SQL92: `INSERT`, `UPDATE` and `DELETE`. `UPDATE` is not supported in Starcounter SQL, `DELETE` is available through `Db.SQL`, and `INSERT` is available with [reload](../working-with-starcounter/using-unloadreload-to-modify-database-schema.md). Objects are otherwise created and updated in the programming code.

All modifications have to be wrapped in a transaction. These modifications are visible to other transaction after the changes have been commited.

## Create

Database objects are created with the native program code operator `new`:

```csharp
new Person()
{
    FirstName = "John",
    LastName = "Doe"
};
```

{% hint style="info" %}
Read more about creating database object on the [database classes page](database-classes.md#create-database-objects)
{% endhint %}

## Update

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

## Delete

There are two ways to delete database objects:

1. Using the `Delete` method on an object
2. Using `DELETE FROM`

`Delete` is used for single objects and `DELETE FROM` is used for many objects.

They look like this:

```csharp
var john = new Person();
john.Delete();

Db.SQL("DELETE FROM Person");

Db.SQL("DELETE FROM Person WHERE Name = ?", "John");
```

`person.Delete()` will just delete `john` while `DELETE FROM Person` will delete all objects of the `Person` class.

To delete database objects that are bound to the view-model, the view-model object should be deleted before the database object is deleted.

### ScErrMemoryLimitReachedAbort \(SCERR8036\)

Deleting many records with `DELETE FROM` might breach the size limit for a single transaction which will cause Starcounter to throw `ScErrMemoryLimitReachedAbort`. This can be fixed by using `Delete` and splitting the deletion in smaller transactions: 

```csharp
var people = Db.SQL($"SELECT p FROM {typeof(Person)} p");
foreach (var person in people)
{
    Db.Transact(() => person.Delete());
}
```

Read more in the [kernel Q&A](../working-with-starcounter/kernel-questions-and-answers.md).

