# Querying with SQL

## Introduction

You can query data stored in the database with SQL queries. The query is passed as a string to the `Db.SQL` method. The allowed SQL syntax is covered in the [SQL](../sql/) section.

## Return types

SQL queries are executed with the `Db.SQL` method. If the SQL command is `SELECT`, the function returns `IEnumerable<object>` or `IEnumerable<T>` depending on if you use the generic method or not, it otherwise returns `null`.

```csharp
Db.SQL($"SELECT p FROM {typeof(Person)} p"); // => IEnumerable<Person>
Db.SQL($"DELETE FROM {typeof(Person)}"); // => null
```

In addition to traditional SQL, Starcounter lets you select objects in addition to primitive types such as strings and numbers. Also, it allows you to use C\# style path expressions such as `person.FullName`.

The `Db.SQL<T>` method supports the `System.Tuple` result type. You can use it for aggregations and properties:

```csharp
var maxAndMinAgeByGender = Db.SQL<Tuple<long, long>>(
    $"SELECT MAX(p.Age), MIN(p.Age) FROM {typeof(Person)} p GROUP BY p.{nameof(Gender)}");
    
var ageHeightAndWeight = Db.SQL<Tuple<int, short?, byte?>>(
    $"SELECT p.Age, p.Height, p.Weight FROM {typeof(Person)} p");
```

The tuple argument has to be consistent with the type of the corresponding queried property. Type casting is only supported for the types that can be implicitly cast.

For queries with more than 7 properties, use nested tuples:

```csharp
Db.SQL<Tuple<int, int, int, int, int, int, int, Tuple<int, int>>>(
    $"SELECT p.Attr1, p.Attr2, p.Attr3, p.Attr4, p.Attr5, p.Attr6, p.Attr7, p.Attr8, p.Attr9 FROM {typeof(Person)} p");

Db.SQL<Tuple<int, int, int, int, int, int, int,
             Tuple<int, int, int, int, int, int, int,
                   Tuple<int, int, int, int>>>>(
    "SELECT p.Attr1, p.Attr2, p.Attr3, p.Attr4, p.Attr5, p.Attr6, p.Attr7, p.Attr8, p.Attr9, p.Attr10, " +
    $"p.Attr11, p.Attr12, p.Attr13, p.Attr14, p.Attr15, p.Attr16, p.Attr17, p.Attr18 FROM {typeof(Person)} p");
```

{% hint style="warning" %}
When writing queries with `Db.SQL`, keep in mind that there are [certain reserved words](../sql/reserved-words.md) that should be escaped. That is done by surrounding the reserved word in quotation marks.
{% endhint %}

## Using variables

SQL variables are represented by question marks \(?\) in the query string, and you pass the current values of the variables as parameters to the method `Db.SQL`.

```csharp
var employees = Db.SQL<Employee>(
  $"SELECT e FROM {typeof(Employee)} e WHERE e.{nameof(FirstName)} = ?", "Joe");
  
foreach (var employee in employees)
{
  Console.WriteLine($"{employee.FirstName} {employee.LastName}");
}
```

`Db.SQL`takes an arbitrary number of variables as long as the number of variable values are the same as the number of variables in the query string. Otherwise, an `ArgumentException` will be thrown.

```csharp
var lastName = "Smith";
var manager = new Employee("John", "Doe", bigBoss);

var employees = Db.SQL<Employee>(
  $"SELECT e FROM {typeof(Employee)} e WHERE e.{nameof(LastName)} = ? AND e.{nameof(Manager)} = ?", 
  lastName, manager);
  
foreach (var employee in employees)
{
  Console.WriteLine($"{employee.LastName}; {emp.Manager.LastName}");
}
```

Each variable has an implicit type depending on its context in the query string. For example, a variable that is compared with a property of type `string` will implicitly be of the type `string`. If a variable is given a value of some incompatible type, then Starcounter throws an `InvalidCastException`. All numerical types are compatible with each other.

{% hint style="info" %}
You can only use `?` for variables after the `WHERE` clause. You can't, for instance, use `?` to replace the class name of a query.
{% endhint %}

## Query processing error

If a query cannot be processed due to some syntax or type checking error then the method `Db.SQL` will throw the `SqlException` `ScErrSQLIncorrectSyntax (SCERR7021)`.

```csharp
try
{  
    var people = Db.SQL<Person>($"SELECT e.NonExistingProperty FROM {typeof(Person)} p");
    
    foreach(Person person in people)  
    {    
        Console.WriteLine(person.Name);  
    }
}
catch (SqlException exception)
{  
    Console.WriteLine("Incorrect query: " + exception.Message);
}
```

## Data types

This page describes the expected data types of the `Db.SQL` query results for the database object properties, fields and arithmetic operations.

### Field and property data types

Your object properties and fields may have the following data types \(`DbTypeCode`\):

* `Binary`
* `Boolean`
* `Byte`
* `DateTime` 
* `Decimal`
* `Double`
* `Int16`
* `Int32`
* `Int64`
* `object`
* `enum`
* `SByte`
* `Single`
* `String`
* `UInt16`
* `UInt32`
* `UInt64`

The data types `Boolean`, `Byte`, `DateTime`, `Double`, `Int16`, `Int32`, `Int64`, `SByte`, `Single`, `String`, `UInt16`, `UInt32`, `UInt64` correspond to the .NET data types with the same names.

#### String

The `string`data type can store data up to 1 MB of encoded string data. Thus, all strings with a length of less than 270600 will fit into the string data type. Strings longer than 270600 might fit depending on string content.

#### Decimal

The data type `Decimal` is stored as a 64-bit integer and has a precision of six decimals and a range between `4398046511104.999999` and `-4398046511103.999999`. Trying to set the `Decimal` data type to a more precise value or to a value outside of the range throws `ScErrCLRDecToX6DecRangeError (SCERR4246)`. In those cases, `Double` can be used if data loss is acceptable.

#### Object

The data type `object` represents a reference to a database object - an object that has the `Database` attribute or inherits it.

#### Binary

The data type `Binary` represents binary data up to 1 MB.

#### Numerical types

All signed integers, `Int64`, `Int32`, `Int16` and `SByte` are represented as `Int64` internally in Starcounter SQL. The unsigned integers, `UInt64`, `UInt32`, `UInt16` and `Byte` are represented as `UInt64`. The approximate numerical types `Single` and `Double` are represented as `Double`. 

#### DateTime

`DateTime` is represented as an `Int64` of the number of .Net ticks from `DateTime.MinValue.Ticks`.

#### Enum

`enum` is supported as a data type. It's stored as a number in the database. Queries on `enum` will return a number which can be cast to an `enum`.

```csharp
using System;
using System.Linq;
using Starcounter;

public enum House { Targaryen, Tyrell, Baratheon, Greyjoy };

[Database]
public class Person
{
    public House House { get; set; }
}

class Program
{
    static void Main()
    {
        Db.Transact(() =>
        {
            var person = new Person() { House = House.Tyrell };
            var house = Db.SQL($"SELECT p.{nameof(House)} FROM {typeof(Person)} p").FirstOrDefault();
            Console.Write(house); // => 1
            Console.Write((House)house); // => Tyrell
            Console.Write(Db.FromId<Person>(person.GetObjectID()).House); // => Tyrell
        });
    }
}
```

#### Nullable types

If you want to store `null` for value types, you can use the corresponding nullable data types:

* `Nullable<Boolean>`
* `Nullable<Byte>`
* `Nullable<DateTime>`
* `Nullable<Decimal>`
* `Nullable<Double>`
* `Nullable<Int16>`
* `Nullable<Int32>`
* `Nullable<Int64>`
* `Nullable<SByte>`
* `Nullable<Single>`
* `Nullable<UInt16>`
* `Nullable<UInt32>`
* `Nullable<UInt64>`

### Arithmetic operations

The data type of the result of an [arithmetic operation](../sql/data-operators.md) is one of the following:

1. `Double` \(representing approximate numeric values\) \[highest precedence\]
2. `Decimal` \(representing exact numeric values\)
3. `Int64` \(representing signed integers\)
4. `UInt64` \(representing unsigned integers - the natural numbers\) \[lowest precedence\]

In general, the data type of the result of an arithmetic operation is the data type with the highest precedence of the data types of the operands.

However, in the following special cases you need a data type with higher precedence to appropriately represent the result:

* A subtraction between `UInt64`'s \(unsigned integers\) has a result of data type `Int64` \(signed integer\).
* A division between any combination of `UInt64`'s and `Int64`'s \(unsigned and signed integers\) has a result of data type `Decimal`

### Collections

It is possible to have collections in the database class if the collection has an explicitly declared body. For example, the following properties are allowed:

```csharp
public List<string> Branches => new List<string>(){ "develop", "master" };

public IEnumerable<Person> Friends => Db.SQL<Person>($"SELECT p FROM {typeof(Person)} p");
```

These properties and fields are not allowed:

```csharp
public string[] Names { get; set; }
public List<Person> People { get; }
public IEnumerable Animals;
```

The properties with explicitly declared bodies cannot be queried for with SQL, but they can be accessed from the application code after they have been retrieved from the database. If a `Person` class has the property `Friends` with a declared body, then `Friends` can be accessed like so:

```csharp
var person = Db.SQL<Person>($"SELECT p FROM {typeof(Person)} p").FirstOrDefault();
IEnumerable<Person> friends = person.Friends;
```

