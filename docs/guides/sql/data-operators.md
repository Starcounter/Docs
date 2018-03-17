# Data operators

## Introduction

In Starcounter SQL, the most common operators on data are implemented.

## Arithmetic operators

The standard arithmetic operators, plus \(+ x\), minus \(- x\), addition \(x + y\), subtraction \(x - y\), multiplication \(x \* y\) and division \(x / y\), are supported for all numerical types:

```sql
SELECT (e.Salary * 12) / 365 FROM Employee e
```

{% hint style="info" %}
For the expected datatypes of an arithmetic operation, see [Data Types]().
{% endhint %}

## String concatenation

String concatenation \(x \|\| y\) is supported. See example below.

```sql
SELECT e.FirstName || ' ' || e.LastName FROM Employee e
```

