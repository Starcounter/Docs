# Database classes

## Introduction

The developer defines which classes should have their instances stored persistently by decorating the classes with the `Database` attribute. We call these classes database classes. The weaver, a tool that transforms the application code to work with the [VMDBMS](./), locates the database classes during compile time. The weaver then automatically rewrites the class to call into the database engine instead of the heap. Thus, the only thing stored in the object on the heap is the identification of a persistent object in the VMDBMS that holds the data. For the developer, it looks similar to the code-first approach in object-relational mappers such as Entity Framework where the developer defines C\# classes, and the framework derives a schema from it.

This is approximately how a class looks before and after weaving:

{% code-tabs %}
{% code-tabs-item title="Before weaving" %}
```csharp
[Database]
public class Person
{
    public string Name { get; set; }
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

{% code-tabs %}
{% code-tabs-item title="After weaving" %}
```csharp
[Database]
public class Person
{
    private ulong _id;
    public string Name
    {
        get => DbState.GetString(_id, "Name");
        set => DbState.SetString(_id, "Name", value);
    }
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

After the weaver weaves the class, whenever the app use the `Name` property, it’s retrieving or setting the value in the database and not the object on the heap. With that, persistent objects can be updated the same way as any other object.

Weaved code is not visible to the developer, even if it’s used by the compiler. This is by design so that the developer don’t have to worry about weaving.

## Creating database classes

Database classes are, for the most part, created the same way as any other class. The main difference is under the hood; public properties in the database class become database columns and properties with explicitly declared bodies, such as `FullName` in the example below become code properties, which are not stored as columns, but can be accessed in SQL queries:

```csharp
[Database]
public class Person
{
    public string FirstName { get; set; } // Column
    public string LastName { get; set; } // Column
    public string FullName => $"{FirstName} {LastName}"; // Property
}
```

{% hint style="warning" %}
Database classes have to be declared as `public`, otherwise, Starcounter throws `ScErrEntityClassNotPublic (SCERR4220)` in compile-time. 
{% endhint %}

### Properties and fields

Database classes only support public auto-implemented properties or public properties backed by private fields:

```csharp
[Database]
public class DatabaseFieldsAndProperties
{
    // Supported. Public properties are treated as columns
    // which means that their values are stored persistently
    // and they can be queried through SQL
    public string PublicAutoImplementedProperty { get; set; }

    // Supported. The property is treated as a column.
    // The property can't contain any logic.
    // The setter is optional
    private string privateField;
    public string PublicPropertyWithBackingField
    {
        get => privateField;
        set => privateField = value;
    }

    // Not supported. Throws ScErrNonPublicFieldNotExposed (SCERR4285)
    // or ScErrDebugSequenceFailUnexpect (SCERR12016) when you start the application.
    internal string InternalProperty { get; set; }

    // Not supported. Throws ScErrNonPublicCodeProperty (SCERR4306)
    // when you try to query it
    protected string ProtectedProperty { get; set; }

    // Not supported. Throws ScErrFieldInDatabaseType (SCERR4307)
    // in compile-time
    public string PublicField;

    // Supported. It's ignored by the weaver and thus
    // treated as a regular .NET field.
    // You can't query a transient field
    [Transient]
    public string PublicTransientField;
}
```

The data types supported and their limitation are listed on the page [Querying with SQL](querying-with-sql.md#data-types).

#### Preventing fields from becoming database columns

Use the `Transient` attribute to exclude properties and fields from becoming database columns. Properties and fields with the `Transient` attribute remain as regular .NET and properties or fields and their values are stored on the heap and garbage collected with the objects they belong to. These properties and fields can't be queried with SQL.

Since transient properties and fields are regular .NET properties, you can only retrieve their values with the initial object reference. Thus, transient properties and fields of objects that have been fetched from the database return the default value of the property or field:

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

Due to the way reference navigation works with database objects, transient properties of objects that are retrieved through reference navigation return the default value of the property or field:

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

### Column limit

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

## Nested classes

Nested database classes are not supported. The limitation is that inner database classes cannot be queried with SQL.

## Create database objects

Database objects are created with the native program code operator `new`. For example, consider the following database class:

```csharp
[Database]
public class Person
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
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

### Deserializing to database classes

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

### Casting From non-database class

It's not possible to cast from a non-database class to a database class. For example, this is not possible:

```csharp
public void UpdatePerson(ExternalApiModel data) 
{
    (data.ExternalApiPerson as Person).Name = "John";
}
```

Instead, database object creation should be done with the `new` operator.

## Inheritance

Any database object can inherit from any other database object. The `Database` attribute is inherited from base- to subclasses. Hence, any class that directly or indirectly inherits a class with the `Database` attribute becomes a database class.

### Example

In this example, both `PrivateCustomer` and `CorporateCustomer` become database classes due to them inheriting `Customer`:

```csharp
[Database]
public class Customer
{
   public string Name { get; set; }
}

public class PrivateCustomer : Customer
{
   public string Gender { get; set; }
}

public class CorporateCustomer : Customer
{
   public string VatNumber { get; set; }
}
```

The table `Customer` will contain all `PrivateCustomers` and all `CorporateCustomers`. So if there is a private customer called "Goldman, Carl" and a corporate customer called "Goldman Sachs", the result of `SELECT C FROM Customer c` will contain both of them.

### Base classes

A base class contains all instances of all derived classes in addition to the instances with the its own exact type.

```sql
SELECT C FROM Customer C WHERE Name LIKE 'Goldman%'
```

Returns `[ { Name:"Goldman Sachs" }, { Name:"Goldman, Carl" } ]`

### Derived classes

```sql
SELECT C FROM PrivateCustomer C WHERE Name LIKE 'Goldman%'
```

Returns `[{ Name:"Goldman, Carl", Gender:"Male" }]`

```sql
SELECT C FROM CorporateCustomer C WHERE Name LIKE 'Goldman%'
```

Returns `[{ Name:"Goldman Sachs", VatNumber:"1234" } ]`

### Inheriting from non-database classes

A database class cannot inherit from a class that's not a database class. This will throw an error when the application is weaved.

It's also not possible to cast a non-database class to a database class.

