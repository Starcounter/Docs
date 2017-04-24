# Relations

Starcounter is a database that offers relational access, graph access, object oriented access, and document access all rolled into one. We feel that core of the database builds well on the [theories of Dr.Codd](https://www.seas.upenn.edu/~zives/03f/cis550/codd.pdf). We have chosen to premiere implicit primary keys and foreign keys as object references.

You can make a traditional SQL query like this (given that you have a PersonId property):

```sql
SELECT FirstName, LastName, Text FROM Person, Quote WHERE Quote.PersonId = Person.PersonId
```

But, in Starcounter, you would rather use object types and paths and do:
```sql
SELECT Person.FirstName, Person.LastName, Text FROM Quote
```
or

```sql
SELECT FistName, LastName, Text FROM Person, Quote WHERE Quote.Person = Person
```

## One-to-many relations

In a relational database you implement a one-to-many relation between two tables using a primary key of the first table and a foreign key of the second table. In object oriented programming code, you typically implement a one-to-many relation with a collection of object references in the first class and an object reference in the second class.

Using Starcounter we recommend that you model a one-to-many relation as in a relational database, i.e. an object reference in the second entity/class, but also add a property (or method) in the first class that returns a collection of object references. In this way you both support the relational model and the typical object oriented implementation at the same time.

In the code example below there is a one-to-many relation between the entities/classes `Department` and `Employee` regarding employment. This one-to-many relation is stored in the object references `Department` in the class `Employee`.

```cs
[Database]
public class Department
{
  ...
  public IEnumerable Employees
  {
    get { return Db.SQL<Employee>("select e from Employee e where e.Department = ?", this); }
  }
}

[Database]
public class Employee
{
  ...
  public Department Department;
}
```

In the code above, all instances of the `Employee` class has a `Department` reference to the department where they are employed and all instances of the `Department` class has a `Employees` collection of all employees of that particular department. However, within a `Department` object the collection is not stored internally in any data structure but instead is represented by an SQL query.

## Many-to-many relations

In a relational database you implement a many-to-many relation between two entities/tables as two one-to-many relations to an associative entity/table. In object oriented programming code, you typically implement a many-to-many relation between two entities/classes as one collection of object references in each of the two entities/classes.

Using Starcounter we recommend that you model a many-to-many relation as in a relational database, i.e. introducing a new associative entity/class, but also add properties (or methods) that returns collections of object references in the two original entities/classes. In this way you both support the relational model and the typical object oriented implementation at the same time.

In the code example below there is a many-to-many relation between the entities/classes `Person` and `Company` regarding shares of the company. To represent this many-to-many relation we introduce the associative entity/class `Shares`.

```cs
[Database]
public class Person
{
  ...
  public IEnumerable EquityPortfolio
  {
    get { return Db.SQL<Shares>("select s.Equity from Shares s where s.Owner = ?", this);}
  }
}

[Database]
public class Company
{
  ...
  public IEnumerable ShareHolders
  {
    get
    {
      return Db.SQL<Shares>("select s.Owner from Shares s where s.Equity = ?", this);
    }
  }
}

[Database]
public class Shares
{
  public Person Owner;
  public Company Equity;
  public Int64 Quantity;
  ...
}
```

In the code above, all instances of the class `Shares` has one reference to the person who owns the shares and one reference to the company of which the shares are issued. All instances of the `Person` class have a collection of all companies (`EquityPortfolio`) of which the person has shares, and all instances of the `Company` class have a collection of all persons (`ShareHolders`) which are shareholders of the company.
