# Datatypes

This page describes the expected datatypes of the `Db.SQL` query results for the database object properties, fields and arithmetic operations.

## Properties and fields

Your object properties and fields may have the following datatypes (`DbTypeCode`):

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
* `SByte`
* `Single`
* `String`
* `UInt16`
* `UInt32`
* `UInt64`

The datatypes `Boolean`, `Byte`, `DateTime`, `Decimal`, `Double`, `Int16`, `Int32`, `Int64`, `SByte`, `Single`, `String`, `UInt16`, `UInt32`, `UInt64` correspond to the .NET datatypes with the same names.

The datatype `object` represents a reference to a database object, i.e. an instance of a class, directly or by inheritance having the `Database` attribute set.

The datatype `Binary` is for representing binary data up to 8 kB. Note that in Starcounter there is also another binary datatype `LargeBinary` for storing larger binary data. However, `LargeBinary` cannot be indexed and is not supported in Starcounter SQL.

All signed integers, `Int64`, `Int32`, `Int16` and `SByte` are represented as `Int64` internally in Starcounter SQL. The unsigned integers, `UInt64`, `UInt32`, `UInt16` and `Byte` are represented as `UInt64`. The approximate numerical types `Single` and `Double` are represented as `Double`. `DateTime` is represented as an `Int64` of the number of .Net ticks from `DateTime.MinValue.Ticks`.

If you want to store `null` values for datatypes that essentially are value types, you can instead use the corresponding nullable datatypes:

* `Nullable<Boolean>`,
* `Nullable<Byte>`,
* `Nullable<DateTime>`,
* `Nullable<Decimal>`,
* `Nullable<Double>`,
* `Nullable<Int16>`,
* `Nullable<Int32>`,
* `Nullable<Int64>`,
* `Nullable<SByte>`,
* `Nullable<Single>`,
* `Nullable<UInt16>`,
* `Nullable<UInt32>`,
* `Nullable<UInt64>`.

## Arithmetic operations

The datatype of the result of an [arithmetic operation](/guides/SQL/data-operators/) is one of the following:

1. `Double` (representing approximate numeric values) [highest precedence],
2. `Decimal` (representing exact numeric values),
3. `Int64` (representing signed integers),
4. `UInt64` (representing unsigned integers - the natural numbers) [lowest precedence]

In general the datatype of the result of an arithmetic operation is the datatype with the highest precedence of the datatypes of the operands.

However, in the following special cases you need a datatype with higher precedence to appropriately represent the result:

- A subtraction between `UInt64`'s (unsigned integers) has a result of datatype `Int64` (signed integer).
- A division between any combination of `UInt64`'s and `Int64`'s (unsigned and signed integers) has a result of datatype `Decimal`

## Collections

It is possible to have collections in the database class if the collection has an explicitly declared body. For example, the following propeperties are allowed:

```cs
public List<string> Branches 
{
    get { return new List<string>{ "develop", "master" } };
}

public IEnumerable<Person> Friends
{
    get { return Db.SQl<Person>("SELECT p FROM Person p") };
}
```

These properties are not allowed:

```cs
public string[] Names { get; set; }
public List<Person> People { get; }
public IEnumerable Animals;
```

These cannot be queried for with SQL, though, they can still be accessed from C# after they have been retrieved from the database. Imagine that a `Person` class has the property `Friends` friends above, then `Friends` can be accessed like so:
```cs
var person = Db.SQL<Person>("SELECT p FROM Person p").First;
IEnumerable<Person> friends = person.Friends;
```