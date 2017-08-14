# Creating Database Classes

Starcounter does not support SQL92's INSERT statement. Instead, objects are created directly in the programming code. Marking a class in the code as a database class is done by setting the `[Database]` attribute. This class becomes a part of the database schema.

New records are created with the `new` operator. All instances of a database class are database objects and are stored in the database.
Public fields (e.g., `Person.FirstName` and `Quote.Person`), public auto-created properties (e.g., `Person.LastName`) and public properties getting and setting private fields (e.g., `Quote.Text`) become database columns. More complex public properties become code properties, which are not stored as columns, but can be accessed in SQL queries (e.g., `Person.FullName`).

```cs
using Starcounter;

[Database]
public class Person
{
    public string FirstName;
    public string LastName { get; set; }
    public string FullName => FirstName + " " + LastName;
}

[Database]
public class Quote
{
    public Person Person;
    private string _Text { get; set; }
    public string Text 
    { 
        get 
        { 
            return _Text; 
        } 
        set 
        { 
            _Text = value; 
        } 
    }
}
```

## Properties VS Fields

We recommend using auto-implemented properties instead of fields in database classes. This is mainly because Starcounter will only allow auto-implemented properties in future versions to reduce maintenance and make it easier to be cross-platform. For developers this also means that weave-time will be faster and that error messages for edge cases will be clearer.

## Column Limit

Database classes can have a maximum of 112 columns for performance reasons. Thus, this is not allowed:

```cs
[Database]
public class LargeTable
{
    public string Column1 { get; set; }
    public string Column2 { get; set; }
    // ...
    public string Column113 { get; set; }
} 
```

## Preventing Fields From Becoming Database Columns
Using the `Transient` attribute, it's possible to exclude fields and auto-implemented properties of a database class from becoming database columns. A field or auto-implemented property with this attribute will remain as a regular .NET field/property and its value will be stored on the CLR heap and be garbage collected along with the object it belongs to. Starcounter ignores these fields and properties which means that they are not available using SQL.

```cs
using Starcounter;

[Database]
public class Person
{
    public string FirstName;
    public string LastName { get; set; }
    public string FullName => FirstName + " " + LastName;
    [Transient]
    public int ProcessSessionID { get; set; }
    [Transient]
    public int ProcessSessionNumber { get; set; }
}
```

## Deserializing to Database Classes

When deserializing to a database class, the deserialization should be wrapped in a transaction since it creates a new database object:

```cs
using Starcounter;

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
            {
                Newtonsoft.Json.JsonConvert.DeserializeObject<Person>(json);
            });
        }
    }
}
```

## Casting From Non-Database Class

It's not possible to cast from a non-database class to a database class. Instead, database object creation should be done with the `new` operator. For example, this is not possible:

```cs
public void UpdatePerson(ExternalApiModel data) 
{
    (data.ExternalApiPerson as Person).Name = "John";
}
```