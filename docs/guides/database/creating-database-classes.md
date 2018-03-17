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

Use the `Transient` attribute to exclude fields and properties from becoming database columns. Fields or properties with the `Transient` attribute remain as regular .NET fields and properties and their values are stored on the heap and garbage collected with the objects they belong to. These fields and properties can't be queried with SQL.

Since transient properties and fields are regular .NET fields and properties, you can only retrieve their values with the initial object reference. Thus, transient fields or properties of objects that have been fetched from the database return the default value of the fields or properties:

```csharp
using System;
using System.Linq;
using Starcounter;

namespace TransientSampleApp1
{
    [Database]
    public class Person
    {
        public string Name { get; set; }
        
        [Transient]
        public int ProcessSessionNumber { get; set; }
    }

    class Program
    {
        static void Main()
        {
            // Create the object of the Person class defined above
            var person = Db.Transact(() => new Person()
            {
                Name = "Jane Doe",
                ProcessSessionNumber = 1234
            });

            // It's the initial reference, so retrieving 
            // the value of the transient property works
            Console.Write(person.ProcessSessionNumber); // => 1234
            
            // Fetch the object from the database. 
            // The reference is not the initial reference anymore
            person = Db.SQL<Person>(
                "SELECT p FROM Person p WHERE p.Name = ?", "Jane Doe")
                .First();

            // Retrieving the non-transient property works
            Console.Write(person.Name); // => "Jane Doe"

            // Retrieving the transient property gives back the default value
            Console.Write(person.ProcessSessionNumber); // => 0
        }
    }
}
```

Due to the way reference navigation works with database objects, transient fields or properties of objects that are retrieved through reference navigation return the default value of the field or property:

```csharp
using System;
using Starcounter;

namespace TransientSampleApp2
{
    [Database]
    public class Parent
    {
        [Transient] public string Note { get; set; }
    }

    [Database]
    public class Child
    {
        public Parent Parent { get; set; }
    }

    class Program
    {
        static void Main()
        {
            var child = new Child
            {
                Parent = new Parent() { Note = "A note" }
            };

            // Note will be null, since the child.Parent getter 
            // always returns a new instance of the Parent class.
            Console.Write(child.Parent.Note); // => null
        }
    }
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

