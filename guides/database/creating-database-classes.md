# Database Classes

## Introduction

Marking a class in the code as a database class is done by setting the `[Database]` attribute. This class becomes a part of the database schema and all instances of the class are stored in the database. 

## Creating Database Classes

Database classes are, for the most part, created the same way as any other class. The main difference is under the hood; Public fields and public auto-created properties in the database class become database columns and properties with explicitly declared bodies, such as `FullName` in the example below become code properties, which are not stored as columns, but can be accessed in SQL queries:

```csharp
[Database]
public class Person
{
    public string FirstName { get; set; } // Column
    public string LastName { get; set; } // Column
    public string FullName => $"{FirstName} {LastName}"; // Property
}
```

Database classes have to be declared as `public`, otherwise Starcounter will throw `ScErrEntityClassNotPublic (SCERR4220)` in weave-time. 

### Properties and Fields

We recommend using auto-implemented properties instead of fields in database classes because Starcounter will only allow auto-implemented properties in future versions to reduce maintenance and make it easier to be cross-platform. For developers this also means that weave-time will be faster and that error messages for edge cases will be clearer.

#### Access Levels

Properties and fields have to be public, otherwise, `ScErrNonPublicFieldNotExposed` will be thrown with `ScErrCantBindAppWithPrivateData (SCERR2149)`. This also applies to properties with the [`Transient`](creating-database-classes.md#preventing-fields-from-becoming-database-columns) attribute.

#### Preventing Fields From Becoming Database Columns

Use the `Transient` attribute to exclude fields and properties from becoming database columns. Fields or properties with the `Transient` attribute will remain as regular .NET fields and properties and their values will be stored on the head and be garbage collected with the objects they belong to. These fields and properties can't be queried with SQL.

```csharp
[Database]
public class Person
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName => $"{FirstName} {LastName}";
    [Transient]
    public int ProcessSessionID { get; set; }
    [Transient]
    public int ProcessSessionNumber { get; set; }
}
```

### Constructors

Constructors in database classes work the same way as they do in any other class. For example, this works as expected:

```csharp
[Database]
public class Person
{
    public Person(string name)
    {
        this.Name = name;
        this.Created = DateTime.Now;
    }

    public string Name { get; set; }
    public DateTime Created { get; set; }
}
```

### Column Limit

Database classes can have a maximum of 112 columns for performance reasons. Thus, this is not allowed:

```csharp
[Database]
public class LargeTable
{
    public string Column1 { get; set; }
    public string Column2 { get; set; }
    // ...
    public string Column113 { get; set; }
}
```

## Nested Classes

Nested database classes are not supported. The limitation is that inner database classes cannot be queried with SQL.

## Create Database Objects

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

{% hint style="info" %}
All database write operations, such as creating new database objects have to be wrapped in a [transaction](../transactions/).
{% endhint %}

### Deserializing to Database Classes

When deserializing to a database class, the deserialization should be wrapped in a transaction since it creates a new database object:

```csharp
using Starcounter;
using Newtonsoft.Json;

namespace DeserializeDemo
{
    [Database]
    public class Person
    {   
        public string Name { get; set; }
    }

    class Program
    {
        static void Main()
        {
            DeserializePerson(@"{""Name"": ""Gimli""}");
        }

        public static void DeserializePerson(string json)
        {
            Db.Transact(() =>
                JsonConvert.DeserializeObject<Person>(json));
        }
    }
}
```

### Casting From Non-Database Class

It's not possible to cast from a non-database class to a database class. For example, this is not possible:

```csharp
public void UpdatePerson(ExternalApiModel data) 
{
    (data.ExternalApiPerson as Person).Name = "John";
}
```

Instead, database object creation should be done with the `new` operator.

