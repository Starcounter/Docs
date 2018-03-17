# Comparing database objects

## Comparison in SQL queries

In SQL queries database objects can be compared either by the equals `=` operator or by the `ObjectNo` value. The direct comparison is always preferable.

```csharp
var products = Db.SQL<Product>("SELECT p FROM Product p WHERE p.Customer = ?", customer);
```

```csharp
var products = Db.SQL<Product>("SELECT p FROM Product p WHERE p.Customer.ObjectNo = ?", customer.GetObjectNo());
```

## Comparison between instances

Two instances of a database class can be compared either with the `Object.Equals` method or with the `ObjectNo` value. The `Object.Equals` method is the preferable way.

**Note:** the equals `==` operator and the `Object.ReferenceEquals` method will always return `false`.

```csharp
var firstProduct = new Product();
var secondProduct = new Product();
var anotherFirstProduct = DbHelper.FromID(p1.GetObjectNo());
bool equals;

equals = firstProduct.Equals(secondProduct); // false
equals = firstProduct.GetObjectNo() == secondProduct.GetObjectNo(); // false

equals = firstProduct.Equals(anotherFirstProduct); // true
equals = firstProduct.GetObjectNo() == anotherFirstProduct.GetObjectNo(); // true

equals = firstProduct == anotherFirstProduct; // false
equals = object.ReferenceEquals(firstProduct, anotherFirstProduct); // false
```

