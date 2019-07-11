# Database classes

**Note**: Starcounter 3.0.0 is currently in preview stage. This API might be changed in the future releases without backwards compatibility.

## Introduction

The database schema in Starcounter is defined by C# classes with the `[Database]` attribute:

```cs
using Starcounter.Nova;

[Database]
public abstract class Person
{
    public abstract string FirstName { get; set; }
    public abstract string LastName { get; set; }
    public string FullName => $"{FirstName} {LastName}";
}
```

All instances of database classes created with the `Db.Insert` method are stored persistently.

## Constructors

Database classes support default constructors. Non-private default constructors are called when a new instance is created with `Db.Insert`.

For example, this is a valid database class with a constructor:

```cs
using Starcounter.Nova;

[Database]
public abstract class Order
{
    public Order()
    {
        this.Created = DateTime.Now;
    }

    public abstract DateTime Created { get; set; }
}
```

It's possible to have constructors with parameters, although, they are never called when using `Db.Insert`.
Constructors with parameters in database classes can be useful for unit testing purposes when you want to inject dependencies or other arguments into a class.
If you add a constructor with parameters to a database class, you also have to add a default constructor.

**Warning**: All C# access modifiers are accepted for constructors, except for `internal`. Using it will throw `ScErrSchemaCodeMismatch (SCERR4177)` exception.

## Fields and properties

Database classes should only use properties - either auto-implemented or with an explicitly declared body.

Properties should also be public and either `abstract` or `virtual`. It is recommended to use `abstract` properties to reduce application memory footprint.

### Collections

It's possible to have collections in the database class if the collection has an explicitly declared body.
For example, the following properties are allowed:

```cs
public List<string> Branches => new List<string>() { "develop", "master" };

public IEnumerable<Person> Friends => Db.SQL<Person>("SELECT p FROM Person p");
```

These properties and fields are not allowed:

```
public string[] Names { get; set; }
public List<Person> People { get; }
public IEnumerable Animals;
```

To access collections from database objects, first retrieve the object and then access the property that has the collection:

```
var person = Db.SQL<Person>("SELECT p FROM Person p").First();
IEnumerable<Person> friends = person.Friends;
```

## Indexing

Database indexes can be defined with [`CREATE INDEX`](https://www.w3schools.com/sql/sql_create_index.asp) SQL query. Unique and not unique indexes are supported.

```cs
Db.SQL("CREATE INDEX IX_Person_FirstName ON Person (FirstName)");
```

A single property index can be created with the `[Index]` attribute:

```cs
using Starcounter.Nova;

[Database]
public class Person
{
    [Index]
    public virtual string FirstName { get; set; }
    public virtual string LastName { get; set; }
}
```

## Limitations

### Property limit

Database classes can have a maximum of 112 properties for performance reasons. The limit applies to the total number of persistent properties (including all inherited) per class.

Thus, this is not allowed:

```cs
[Database]
public class LargeClass
{
    public virtual string Property1 { get; set; }
    public virtual string Property2 { get; set; }

    // ...

    public virtual string Property113 { get; set; }
}
```

If a database class has more than 113 properties, Starcounter throws `ScErrToManyAttributes (SCERR4013)`.

### Nested classes

Nested database classes are not supported. The limitation is that inner database classes cannot be queried with SQL.

## Relations

### One-to-many relations

We recommend modeling one-to-many relationships by having references both ways - the child has a reference to the parent and the parent has a reference to all the children.

In this example there is a one-to-many relationship between `Department` and `Employee`:

```cs
using Starcounter.Nova;

[Database]
public class Department
{
    public IEnumerable Employees
    {
        get => Db.SQL<Employee>("SELECT e FROM Employee e WHERE e.Department = ?", this);
    }
}

[Database]
public class Employee
{
    public virtual Department Department { get; set; }
}
```

### Many-to-many relations

We recommend modeling many-to-many relationships with an associative class.

In this example there is a many-to-many relation between `Person` and `Company` - to represent this many-to-many relationship we use the associative class `Shares`:

```cs
using Starcounter.Nova;

[Database]
public class Person
{
    public IEnumerable EquityPortfolio
    {
        get => Db.SQL<Shares>("SELECT s.Equity FROM Shares s WHERE s.Owner = ?", this);
    }
}

[Database]
public class Company
{
    public IEnumerable ShareHolders
    {
        get => Db.SQL<Shares>("SELECT s.Owner FROM Shares s WHERE s.Equity = ?", this);
    }
}

[Database]
public class Shares
{
    public virtual Person Owner { get; set; }
    public virtual Company Equity { get; set; }
    public virtual int Quantity { get; set; }
}
```

### Inheritance

Any database class can inherit from any other database class.

```cs
using Starcounter.Nova;

[Database]
public class Customer
{
   public virtual string Name { get; set; }
}

public class PrivateCustomer : Customer
{
   public virtual string Gender { get; set; }
}

public class CorporateCustomer : Customer
{
   public virtual string VatNumber { get; set; }
}
```

The `[Database]` attribute is inherited from base - to subclasses.
Any class that directly or indirectly inherits a class with the `[Database]` attribute becomes a database class.
In the example above, both `PrivateCustomer` and `CorporateCustomer` become database classes due to them inheriting `Customer`.

The table `Customer` will contain all `PrivateCustomers` and all `CorporateCustomers`.
So if there is a private customer called `"Goldman, Carl"` and a corporate customer called `"Goldman Sachs"`,
the result of `SELECT C FROM Customer c` will contain both of them.

#### Inheriting from non-database classes

A database class cannot inherit from a class that's not a database class.
This will throw, during compile-time, `System.NotSupportedException` or `ScErrSchemasDoNotMatch (SCERR15009)` depending on how the base class is defined.

It's also not possible to cast a non-database class to a database class.

## Comparing database objects

Database objects can be checked for equality with the `Equals` method.
Comparing database objects with `object.ReferenceEquals` or the `==` operator always returns `false` if any of the objects are retrieved from the database:

```cs
var firstProduct = Db.Insert<Product>();
var secondProduct = Db.Insert<Product>();
var anotherFirstProduct = Db.Get<Product>(Db.GetOid(firstProduct));

// Checks if two database objects are equal 
Console.WriteLine(firstProduct.Equals(secondProduct)); // => false
Console.WriteLine(firstProduct.Equals(anotherFirstProduct)); // => true

// Returns false for different object or objects retrieved from the database
Console.WriteLine(firstProduct == secondProduct); // => false
Console.WriteLine(firstProduct == anotherFirstProduct); // => false
Console.WriteLine(firstProduct == firstProduct); // => true
Console.WriteLine(object.ReferenceEquals(firstProduct, secondProduct)); // => false
Console.WriteLine(object.ReferenceEquals(firstProduct, anotherFirstProduct)); // => false
Console.WriteLine(object.ReferenceEquals(firstProduct, firstProduct)); // => true
```

## Database object identity

Starcounter automatically assigns an `UInt64` unique key for each database object. The key is unique across entire database not across one table.

### Get object's unique key

```cs
var p = Db.Insert<Product>();
ulong oid = Db.GetOid(p);
```

### Get object by unique key

```cs
var p = Db.Get<Product>(oid);
```

### Querying by object's unique key

```cs
var product = Db.SQL<Product>("SELECT p FROM Product p WHERE p.ObjectNo = ?", oid)
    .FirstOrDefault();
```

### Notes

- Zero (`0`) is not a valid key.
- Currently it is not possible to insert a database object with predefined unique key.
- It is possible to compare database objects by their unique keys.
