# Inheritance

Starcounter allows any database object to inherit from any other database object.

```cs
[Database]
public class Customer
{
   public string Name;
}

public class PrivateCustomer : Customer
{
   public string Gender;
}

public class CorporateCustomer : Customer
{
   public string VatNumber;
}
```

The `Database` attribute is inherited from base- to subclasses. Hence, any class that directly or indirectly inherits a class with the `Database` attribute implicitly becomes a database class too. In the above example, both `PrivateCustomer` and `CorporateCustomer` become database classes due to them inheriting `Customer`.

The table `Customer` will contain all `PrivateCustomers` and all `CorporateCustomers`. So if there are a private customer called "Goldman, Carl" and a corporate customer called "Goldman Sachs", the result of `SELECT C FROM Customer c` will contain both of them.

### Base classes

A base class contains all instances of all derived classes in addition to the instances with the its own exact type.
```sql
SELECT C FROM Customer C WHERE Name LIKE 'Goldman%'
```
Returns ```[ { Name:"Goldman Sachs" }, { Name:"Goldman, Carl" } ]```

### Derived classes
```sql
SELECT C FROM PrivateCustomer C WHERE Name LIKE 'Goldman%'
```
Returns ```[{ Name:"Goldman, Carl", Gender:"Male" }]```

```sql
SELECT C FROM CorporateCustomer C WHERE Name LIKE 'Goldman%'
```
Returns ```[{ Name:"Goldman Sachs", VatNumber:"1234" } ]```
