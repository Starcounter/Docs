# Comparing database objects

## Comparison in SQL queries

In SQL queries database objects can be compared either by the equals `=` operator or by the `ObjectNo` value. The direct comparison is always preferable.

```cs
var products = Db.SQL<Product>("SELECT p FROM Product p WHERE p.Customer = ?", customer);
```

```cs
var products = Db.SQL<Product>("SELECT p FROM Product p WHERE p.Customer.ObjectNo = ?", customer.GetObjectNo());
```

## Comparison between instances

Two instances of database class can be compared either by the `Object.Equals` method or by the `ObjectNo` value. The `Object.Equals` method is the preferable way.

**Note:** the equals `==` operator and the `Object.ReferenceEquals` method will always return `false`.

```cs
var p1 = new Product();
var p2 = new Product();
var anotherP1 = DbHelper.FromID(p1.GetObjectNo());
bool equals;

equals = p1.Equals(p2); // false
equals = p1.GetObjectNo() == p2.GetObjectNo(); // false

equals = p1.Equals(anotherP1); // true
equals = p1.GetObjectNo() == anotherP1.GetObjectNo(); // true

equals = p1 == anotherP1; // false
equals = object.ReferenceEquals(p1, anotherP1); // false
```