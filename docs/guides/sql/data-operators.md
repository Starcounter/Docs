# Data operators

In Starcounter SQL only the most common operators on data are implemented.

The standard arithmetic operators, plus \(+ x\), minus \(- x\), addition \(x + y\), subtraction \(x - y\), multiplication \(x \* y\) and division \(x / y\), are supported for all numerical types. See example below.

```
SELECT (e.Salary * 12) / 365 FROM Employee e
```

String concatenation \(x \|\| y\) is supported. See example below.

```
SELECT e.FirstName || ' ' || e.LastName FROM Employee e
```

For the expected datatypes of an arithmetic operation, see [Datatypes](../database/datatypes.md).

