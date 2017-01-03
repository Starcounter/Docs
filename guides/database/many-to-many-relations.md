# Many-to-many relations

In a relational database you implement a many-to-many relation between two entities/tables as two one-to-many relations to an associative entity/table. In object oriented programming code, you typically implement a many-to-many relation between two entities/classes as one collection of object references in each of the two entities/classes.

Using Starcounter we recommend that you model a many-to-many relation as in a relational database, i.e. introducing a new associative entity/class, but also add properties (or methods) that returns collections of object references in the two original entities/classes. In this way you both support the relational model and the typical object oriented implementation at the same time.

In the code example below there is a many-to-many relation between the entities/classes <code>Person</code> and <code>Company</code> regarding shares of the company. To represent this many-to-many relation we introduce the associative entity/class <code>Shares</code>.

```cs
[Database]
public class Person {
  ...
  public IEnumerable EquityPortfolio {
    get {
      return Db.SQL<Shares>("select s.Equity from Shares s where s.Owner = ?", this);
    }
  }
}

[Database]
public class Company {
  ...
  public IEnumerable ShareHolders {
    get {
      return Db.SQL<Shares>("select s.Owner from Shares s where s.Equity = ?", this);
    }
  }
}

[Database]
public class Shares {
  public Person Owner;
  public Company Equity;
  public Int64 Quantity;
  ...
}
```

In the code above, all instances of the class <code>Shares</code> has one reference to the person who owns the shares and one reference to the company of which the shares are issued. All instances of the <code>Person</code> class have a collection of all companies (<code>EquityPortfolio</code>) of which the person has shares, and all instances of the <code>Company</code> class have a collection of all persons (<code>ShareHolders</code>) which are shareholders of the company.
