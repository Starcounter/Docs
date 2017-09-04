# Data Types

This page describes the expected data types of the `Db.SQL` query results for the database object properties, fields and arithmetic operations.

## Properties and Fields

Your object properties and fields may have the following data types (`DbTypeCode`):

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

### String

The `String` data type can store data up to 1 MB of encoded string data. Thus, all strings with a length of less than 270600 will fit into the string datatype. Strings longer than 270600 might fit depending on string content.

### Decimal

The data type `Decimal` is stored as a 64-bit integer and has a precision of six decimals and a range between `4398046511104.999999` amd `-4398046511103.999999`. Trying to set the `Decimal` data type to a more precise value or to a value outside of the range throws `ScErrCLRDecToX6DecRangeError (SCERR4246)`. In those cases, `Double` can be used if data loss is acceptable.

### Object

The data type `object` represents a reference to a database object, i.e. an instance of a class, directly or by inheritance having the `Database` attribute set.

### Binary

The data type `Binary` represents binary data up to 1 MB.

### Numerical Types

All signed integers, `Int64`, `Int32`, `Int16` and `SByte` are represented as `Int64` internally in Starcounter SQL. The unsigned integers, `UInt64`, `UInt32`, `UInt16` and `Byte` are represented as `UInt64`. The approximate numerical types `Single` and `Double` are represented as `Double`.

### DateTime

`DateTime` is represented as an `Int64` of the number of .Net ticks from `DateTime.MinValue.Ticks`.

### enum

`enum` is supported as a data type. It's stored as a number in the database. Queries on `enum` will return a number which can be cast to an `enum`. 

```cs
using Starcounter;
using System.Linq;
using System.Diagnostics;

public enum Hairstyle { good, bad, ugly };

[Database]
public class Person
{
    public Hairstyle HairStyle { get; set; }
}

class Program
{
    static void Main()
    {
        Db.Transact(() =>
        {
            var person = new Person() { HairStyle = Hairstyle.bad };
            var hairstyle = Db.SQL("SELECT p.Hairstyle FROM Person p").First();
            Debug.Write(hairstyle); // => 1
            Debug.Write((Hairstyle)hairstyle); // => bad
            Debug.Write(Db.FromId<Person>(person.GetObjectID()).HairStyle); // => bad
        });
    }
}
```

### Nullable Types

To store `null` values, use the corresponding nullable data types:

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

## Arithmetic Operations

The data type of the result of an [arithmetic operation](/guides/SQL/data-operators/) is one of the following:

1. `Double` (representing approximate numeric values) [highest precedence]
2. `Decimal` (representing exact numeric values)
3. `Int64` (representing signed integers)
4. `UInt64` (representing unsigned integers - the natural numbers) [lowest precedence]

In general the data type of the result of an arithmetic operation is the data type with the highest precedence of the data types of the operands.

However, in the following special cases you need a data type with higher precedence to appropriately represent the result:

- A subtraction between `UInt64`'s (unsigned integers) has a result of data type `Int64` (signed integer).
- A division between any combination of `UInt64`'s and `Int64`'s (unsigned and signed integers) has a result of data type `Decimal`

## Collections

It is possible to have collections in the database class if the collection has an explicitly declared body. For example, the following properties are allowed:

```cs
public List<string> Branches => new List<string>(){ "develop", "master" };

public IEnumerable<Person> Friends => Db.SQL<Person>("SELECT p FROM Person p");
```

These properties and fields are not allowed:

```cs
public string[] Names { get; set; }
public List<Person> People { get; }
public IEnumerable Animals;
```

The properties with explicitly declared bodies cannot be queried for with SQL, but they can be accessed from the application code after they have been retrieved from the database. If a `Person` class has the property `Friends` with a declared body, then `Friends` can be accessed like so:
```cs
var person = Db.SQL<Person>("SELECT p FROM Person p").FirstOrDefault();
IEnumerable<Person> friends = person.Friends;
```