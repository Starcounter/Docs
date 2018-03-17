# Querying with SQL

## Introduction

Data stored in the database can be queried for with SQL queries. The query is passed as a string to the `Db.SQL` method. The allowed SQL syntax is covered in the [SQL](../sql/) section.

In addition to querying the database with SQL, you can also use LINQ by using the package [Starcounter.Linq](https://www.nuget.org/packages/Starcounter.Linq/). Documentation for Starcounter.Linq is in in the [Starcounter.Linq repository on GitHub](https://github.com/Starcounter/Starcounter.Linq),

## Return Types

SQL queries are executed with the `Db.SQL` method. If the SQL command is `SELECT`, the function returns `Starcounter.QueryResultRows<T> : IEnumerable<T>`, it otherwise returns `null`.

```csharp
Db.SQL("SELECT p FROM Person p"); // => QueryResultRows<Person>
Db.SQL("DELETE FROM Person"); // => null
```

`T` in `QueryResultRows<T>` is the type of the object retrieved if the whole object is retrieved, otherwise, it's `Starcounter.Query.Execution.Row`.

```csharp
Db.SQL("SELECT p FROM Person p"); // => QueryResultRows<Person>
Db.SQL("SELECT p.Name FROM Person p"); // => QueryResultRows<Query.Execution.Row>
```

We recommend avoiding `Starcounter.Query.Execution.Row` when possible and instead retrieve the whole object and filter out the needed properties with Linq.

```csharp
Db.SQL("SELECT p.Name FROM Person p"); // Not recommended
Db.SQL("SELECT p FROM Person p")
    .Select(p => new { p.Name }); // Recommended
```

In addition to traditional SQL, Starcounter allows you to select objects in addition to primitive types such as strings and numbers. Also, it allows you to use C\# style path expressions such as `person.FullName`.

{% hint style="warning" %}
When writing queries with `Db.SQL`, keep in mind that there are [certain reserved words](../sql/reserved-words.md) that should be escaped. That is done by surrounding the reserved word in quotation marks.
{% endhint %}

{% hint style="warning" %}
The `QueryResultRows` class is deprecated from Starcounter 2.3.1. Read about the changes here: [https://starcounter.io/reducing-magic-increasing-familiarity-obsoleting-queryresultrows/](https://starcounter.io/reducing-magic-increasing-familiarity-obsoleting-queryresultrows/)
{% endhint %}

## Making Queries Less Fragile

Since the queries are strings, they are sensitive to refactorings, for example if a database class is renamed. To make the queries less fragile, `typeof` and `nameof` can be used:

```csharp
Db.SQL($"SELECT p FROM {typeof(Person)} p");
Db.SQL($"SELECT p.{nameof(Person.Name)} FROM {typeof(Person)} p");
```

If the query is executed many times, constructing the string every time can become a performance problem. In that case, the string can be constructed once and stored in a static property.

## Using Variables

SQL variables are represented by question marks \(?\) in the query string, and you pass the current values of the variables as parameters to the method `Db.SQL`.

```csharp
var employees = Db.SQL<Employee>(
  "SELECT e FROM Employee e WHERE e.FirstName = ?", "Joe");
  
foreach (var employee in employees)
{
  Console.WriteLine($"{employee.FirstName} {employee.LastName}");
}
```

`Db.SQL`takes an arbitrary number of variables as long as the number of variable values are the same as the number of variables in the query string. Otherwise, an `ArgumentException` will be thrown.

```csharp
var lastName = "Smith";
Employee manager; //Assume some value is assigned to the variable manager.

var employees = Db.SQL<Employee>(
  "SELECT e FROM Employee e WHERE e.LastName = ? AND e.Manager = ?", 
  lastName, manager);
  
foreach (var employee in employees)
{
  Console.WriteLine($"{employee.LastName}; {emp.Manager.LastName}");
}
```

Each variable has an implicit type depending on its context in the query string. For example, a variable that is compared with a property of type `string` will implicitly be of the type `string`. If a variable is given a value of some incompatible type, then an `InvalidCastException` will be thrown. All numerical types are compatible with each other.

{% hint style="info" %}
You can only use `?` for variables after the `WHERE` clause. You can't, for instance, use `?` to replace the class name of a query.
{% endhint %}

## Query Processing Error

If a query cannot be processed due to some syntax or type checking error then the method `Db.SQL` will throw the `SqlException` `ScErrSQLIncorrectSyntax (SCERR7021)`.

```csharp
try
{  
    var people = Db.SQL<Person>("SELECT e.NonExistingProperty FROM Person p");
    
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

