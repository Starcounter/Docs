# Database classes

## Defining database classes

We can define the schema of a Starcounter database table in an application by defining **public abstract C\# classes** decorated with the `DatabaseAttribute` attribute. We call these types **database classes**. The names and namespaces we give to these C\# classes will be the names and namespaces of the corresponding database tables.

```csharp
using System;
using Starcounter.Database;

[Database]
public abstract class Person
{
    public abstract string Name { get; set; }
}
```

All instances of these database classes created with the `IDatabaseContext.Insert<T>()` method are stored in the Starcounter database persistently.

## Constructors

Database classes support default constructors, which is useful for adding logic that is run each time an instance of the database class is created. Public default constructors of database classes are called whenever a new instance of the class is created with `IDatabaseContext.Insert<T>()`.

For exaαmple, here is our `Person` class with a custom default constructor:

```csharp
using System;
using Starcounter.Database;

[Database]
public abstract class Person
{
    public abstract string Name { get; set; }
    public abstract DateTime CreatedAtUtc { get; set; }

    public Person()
    {
        CreatedAtUtc = DateTime.UtcNow;
    }
}
```

## Fields and properties

Table columns are defined in database classes by abstract instance auto-implemented properties with public `get` and `set` accessors and with one of the [supported data types](database-types.md). The `Person` database class below defines two such columns, `Name` and `CreatedAtUtc`:

```csharp
using System;
using Starcounter.Database;

[Database]
public abstract class Person
{
    public abstract string Name { get; set; }
    public abstract DateTime CreatedAtUtc { get; set; }
}
```

Database classes can also define calculated properties, which are properties that do not expose an instance field. Commonly, we use them to reflect an aspect of the state of a database class instance. Let's say we want a shorthand for calculating the length of the `Name` string of a `Person`. We could do this by introducing a calculated property. These computed properties do not have the same restrictions as column properties, since they are transient and not bound to the corresponding database table.

```csharp
using System;
using Starcounter.Database;

[Database]
public abstract class Person
{
    public abstract string Name { get; set; }
    public abstract DateTime CreatedAtUtc { get; set; }

    public int NameLength => Name.Length;
}
```

**Note**: If a database class definition of a Starcounter application contains any non-computed instance properties that are not declared as abstract auto-implemented properties with public get and set accessors and with one of the [supported data types](database-types.md), an exception will be thrown when the application starts.

### Database queries in computed properties

Sometimes it's very useful to make database queries inside the `get` accessors of computed properties of database classes. This way we can extend object-oriented principles like encapsulation onto the persistent nature of database objects. Let's expand our `Person` class with a `Mother` one-to-many relational property:

```csharp
using System;
using Starcounter.Database;

[Database]
public abstract class Person
{
    public abstract string Name { get; set; }
    public abstract DateTime CreatedAtUtc { get; set; }
    public abstract Person Mother { get; set; }

    public IEnumerable<Person> Children
    {
        get
        {
            var db = DbProxy.GetContext(this);
            return db.Sql<Person>("SELECT p FROM Person p WHERE p.Mother = ?", this);
        }
    }

    public int NameLength => Name.Length;
}
```

We can then introduce a computed property `Children` with the purpose of listing, for each Person `m`, all `Person` instances `p` in the database where `m` is the value of the `Mother` property for `p`.

To implement this, we need access to the database context that the given `Person` instance lives in. We can achieve this by calling the static method `DbProxy.GetContext(this)`.

## Indexing

To achieve the full performance potential of the Starcounter database, it's crucial to register appropriate indexes for database classes. Database indexes can be defined with [`CREATE INDEX`](https://www.w3schools.com/sql/sql_create_index.asp) SQL queries. Both unique and not unique indexes are supported. Since `CREATE INDEX` is a DDL statement, we use the `IDdlExecutor` to perform it, outside an active transaction. We can obtain the `IDdlExecutor` from the [service provider](dependency-injection.md).

```csharp
var ddlExecutor = services.GetRequiredService<IDdlExecutor>();

ddlExecutor.Execute("CREATE INDEX IX_Person_FirstName ON Person (FirstName)");
```

A single property index can also be registered for a column by decorating its associated C\# property with the `IndexAttribute` attribute:

```csharp
using System;
using Starcounter.Database;

[Database]
public abstract class Person
{
    [Index]
    public abstract string Name { get; set; }
}
```

**Note**: DDL statements can only be performed from `IDdlExecutor`.

## Relations

### One-to-many relations

It's recommended to model one-to-many relations using references both ways, with the child entity having a reference to the parent and the parent having an instance method or computed property that selects all the children \(like in the example with the `Mother`/`Children` relation above\).

### Many-to-many relations

Many-to-many relations are best modelled using an **association class**.

Let's say we want to model many-to-many relation of **share ownership**, as understood as a relation between `Person` and `Company` entities such that a single person can hold multiple shares in multiple companies, and a single company can have multiple shareholders. For this, let's use the association class `ShareOwnership`. We can then make queries to this database class to calculate useful information in `Person` and `Company` that is exposed using computed properties. Lastly, we make sure that indexes are registered for the `Owner` and `Equity` properties of `ShareOwnership` since we will be quering them a lot.

```csharp
using Starcounter.Database;

[Database]
public abstract class Person
{
    public abstract string Name { get; set; }

    public IEnumerable<Company> OwnsSharesInCompanies
    {
        get
        {
            var db = DbProxy.GetContext(this);
            return db.Sql<Company>("SELECT s.Equity FROM ShareOwnership s WHERE s.Owner = ?", this);
        }
    }
}

[Database]
public abstract class Company
{
    public abstract string Name { get; set; }
    public abstract Person Ceo { get; set; }

    public IEnumerable<Person> Shareholders
    {
        get
        {
            var db = DbProxy.GetContext(this);
            return db.Sql<Person>("SELECT s.Owner FROM ShareOwnership s WHERE s.Equity = ?", this);
        }
    }
}

[Database]
public abstract class ShareOwnership
{
    [Index]
    public abstract Person Owner { get; set; }

    [Index]
    public abstract Company Equity { get; set; }

    public abstract int Quantity { get; set; }
}
```

### Inheritance

Inheritance is supported between database classes and has the same general semantics as class inheritence in C\#.

```csharp
using Starcounter.Database;

[Database]
public abstract class Customer
{
   public abstract string Name { get; set; }
}

public abstract class PrivateCustomer : Customer
{
   public abstract string Gender { get; set; }
}

public abstract class CorporateCustomer : Customer
{
   public abstract string VatNumber { get; set; }
}
```

The `DatabaseAttribute` attribute decoration is inherited from any base class to its subclasses, meaning that any class that directly or indirectly inherits a class that is decorated with the `DatabaseAttribute` attribute becomes a database class. In the example above, both `PrivateCustomer` and `CorporateCustomer` become database classes due to them inheriting `Customer`.

This also means that all rows in the `CorporateCustomer` table also are contained in the `Customer` table. The result of the `SELECT C FROM Customer c` SQL query will contain all rows from `Customer` as well as all rows from subclasses of `Customer`.

A database class cannot inherit from a class that's not a database class. It's also not possible to cast a non-database class to a database class.

## Database object identity

Starcounter automatically assigns an unique integer identifier called `Oid` to each database object. The key is unique across all table rows in the entire database. Any id value is used only once, and not reused in the future, even if the original object was deleted. We can configure the range to use for oid's using the `FirstObjectId` and `LastObjectId` options in the [database creation options](database-creation-options.md).

The `IDatabaseContext` type defines methods for working with object `Oid`'s. For the examples above, let's assume that `db` holds a reference to an instance of `IDatabaseContext`, recieved from an `ITransactor` when starting a new database transaction.

### Get the `Oid` from a database object

```csharp
var newObject = db.Insert<Product>();
ulong oid = db.GetOid(newObject);
```

### Find an object in the database by its unique key

```csharp
var oid = 459123UL;
var product = db.Get<Product>(oid);
```

### Notes

* Zero \(`0`\) is not a valid `Oid`.
* It's not yet possible to insert a database object with a predefined `Oid`.

## Comparing database objects

Database objects can be checked for equality with the `object.Equals` method or the `Equals` instance method for each database class instance. Comparing database objects with `object.ReferenceEquals` or the `==` operator always returns `false` if any of the objects are retrieved from the database.

We can also compare database objects by comparing their `Oid`'s.

Example:

```csharp
var transactor = services.GetRequiredService<ITransactor>();

transactor.Transact(db =>
{
    var firstProduct = db.Insert<Product>();
    var firstProductOid = db.GetOid(firstProduct);
    var secondProduct = db.Insert<Product>();
    var anotherFirstProduct = db.Get<Product>(firstProductOid);

    firstProduct.Equals(secondProduct); // false
    firstProduct.Equals(anotherFirstProduct); // true

    _ = firstProduct == secondProduct; // false
    _ = firstProduct == anotherFirstProduct; // false
    _ = firstProduct == firstProduct; // true

    object.ReferenceEquals(firstProduct, secondProduct); // false
    object.ReferenceEquals(firstProduct, anotherFirstProduct); // false
    object.ReferenceEquals(firstProduct, firstProduct); // true
});
```

## Discovery

Starcounter automatically discovers all database classes in the application assembly, which does not include all referenced assemblies. Manual configuration is required to use database classes from referenced assemblies.

### Example

The `Something` database class definition below is contained in an assembly that our app references:

```cs
[Database]
public abstract class Something
{
}
```

And here's the code we need to have in our application for it to detect the database class definition above, as well as any other database types contained in the same assembly:

```cs
using var services = new ServiceCollection()
    .AddStarcounter($"Database=./.database/test;OpenMode=CreateIfNotExists;StartMode=StartIfNotRunning;StopMode=IfWeStarted")
    .Configure<Starcounter.Database.Binding.TypeBindingOptions>(o =>
    {
        Type[] extraDatabaseTypes = typeof(Something) // Getting type of a database class.
            .Assembly // Getting assembly which defines the type.
            .GetTypes() // Getting all types in the assembly.
            .Where(x => x.GetCustomAttribute<DatabaseAttribute>(inherit: true) != null) // Filtering the types by the `DatabaseAttribute` attribute.
            .ToArray();

        // The `o.DefaultTypes` property contains all automatically discovered database classes.
        // The `o.Types` property defines which classes will be treated as database classes.
        // It shall contain all automatically discovered database classes plus extra classes from the referenced assembly.
        o.Types = o.DefaultTypes.Concat(extraDatabaseTypes).ToArray();
    })
    .BuildServiceProvider();
```

## Limitations

* Database classes can have a maximum of 112 properties for performance reasons. The limit applies to the total number of persistent properties \(including all inherited\) per class.
* Nested database classes are not supported in SQL queries.
