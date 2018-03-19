# Relations

## Introduction

Relations between database classes are modeled similar to how it's done in relational databases while maintaining a object oriented implementation with references both ways.

## One-to-Many Relations

In a relational database you implement a one-to-many relation between two tables using a primary key of the first table and a foreign key of the second table. In object oriented programming code, you typically implement a one-to-many relation with a collection of object references in the first class and an object reference in the second class.

Using Starcounter we recommend that you model a one-to-many relation as in a relational database, i.e. an object reference in the second class, but also add a property \(or method\) in the first class that returns a collection of object references. In this way you both support the relational model and the typical object oriented implementation at the same time.

In the code example below there is a one-to-many relation between the entities/classes `Department` and `Employee` regarding employment. This one-to-many relation is stored in the object references `Department` in the class `Employee`.

```csharp
[Database]
public class Department
{
  public IEnumerable Employees => 
    Db.SQL<Employee>(
      "SELECT e FROM Employee e WHERE e.Department = ?", this);
}

[Database]
public class Employee
{
  public Department Department { get; set; }
}
```

In the code above, all instances of the `Employee` class has a `Department` reference to the department where they are employed and all instances of the `Department` class has a `Employees` collection of all employees of that particular department. However, within a `Department` object the collection is not stored internally in any data structure but instead is represented by an SQL query.

## Many-to-Many Relations

In a relational database you implement a many-to-many relation between two tables as two one-to-many relations to an associative table. In object oriented programming code, you typically implement a many-to-many relation between two classes as one collection of object references in each of the two classes.

Using Starcounter we recommend that you model a many-to-many relation as in a relational database, i.e. introducing a new associative class, but also add properties \(or methods\) that returns collections of object references in the two original classes. In this way you both support the relational model and the typical object oriented implementation at the same time.

In the code example below there is a many-to-many relation between the classes `Person` and `Company` regarding shares of the company. To represent this many-to-many relation we introduce the associative class `Shares`.

```csharp
[Database]
public class Person
{
  public IEnumerable EquityPortfolio => 
    Db.SQL<Shares>(
      "SELECT s.Equity FROM Shares s WHERE s.Owner = ?", this);
}

[Database]
public class Company
{
  public IEnumerable ShareHolders => 
    Db.SQL<Shares>(
      "SELECT s.Owner FROM Shares s WHERE s.Equity = ?", this);
}

[Database]
public class Shares
{
  public Person Owner { get; set; }
  public Company Equity { get; set; }
  public Int64 Quantity { get; set; }
}
```

In the code above, all instances of the class `Shares` has one reference to the person who owns the shares and one reference to the company of which the shares are issued. All instances of the `Person` class have a collection of all companies \(`EquityPortfolio`\) of which the person has shares, and all instances of the `Company` class have a collection of all persons \(`ShareHolders`\) which are shareholders of the company.

