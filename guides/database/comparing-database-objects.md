# Comparing Database Objects

## Introduction {#comparison-in-sql-queries}

Database object can either be compared in SQL `WHERE` clauses or in programming code. 

## Comparison in SQL Queries {#comparison-in-sql-queries}

In SQL queries database objects can be compared either by the equals `=` operator or by the `ObjectNo` value. The direct comparison is always preferable.

```csharp
var products = Db.SQL<Product>(
    "SELECT p FROM Product p WHERE p.Customer = ?", customer);
```

```csharp
var products = Db.SQL<Product>(
    "SELECT p FROM Product p WHERE p.Customer.ObjectNo = ?",
    customer.GetObjectNo());
```

## Comparison Between Instances {#comparison-between-instances}

Two instances of a database class can be compared either with the `Object.Equals` method or with the `ObjectNo` value. The `Object.Equals` method is the preferable way.

```csharp
var firstProduct = new Product();
var secondProduct = new Product();

var anotherFirstProduct = Db.FromId(firstProduct.GetObjectNo());

firstProduct.Equals(secondProduct); // false
firstProduct.GetObjectNo() == secondProduct.GetObjectNo(); // false
firstProduct.Equals(anotherFirstProduct); // true
firstProduct.GetObjectNo() == anotherFirstProduct.GetObjectNo(); // true
```

{% hint style="warning" %}
The equals `==` operator and the `Object.ReferenceEquals` method will always return `false` when comparing database objects.
{% endhint %}

