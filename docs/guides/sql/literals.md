# Literals

**Disclaimer**: For performance reasons, it is strongly discouraged to use literals in code. Instead, use [variables](../database/variables.md).

A boolean literal can have one of the two values true and false, which are represented by the two reserved words `TRUE` and `FALSE`. See example below.

```
SELECT e FROM Employee e WHERE e.Commission = TRUE
```

There are three types of numerical literals `Int64`, `Decimal` and `Double`.

An `Int64` literal is described by its integer value, as in example below.

```
SELECT e FROM Employee e WHERE e.Salary = 5000
```

A `Decimal` literal is described by its numerical value including a decimal point, as in example below.

```
SELECT e FROM Employee e WHERE e.Salary = 5000.00
```

A `Double` literal is described by two numerical values, the mantissa and the exponent, separated by the character `E`. The mantissa may include a decimal point, but the exponent may not. See example below.

```
SELECT e FROM Employee e WHERE e.Salary = 5.0E3
```

A string literal is a sequence of characters beginning and ending with single quote characters. To represent a single quote character within a String literal, you write two consecutive single quote characters, as in example below.

```
SELECT p FROM Photo p WHERE p.Description = 'Smith''s family'
```

A date-time literal is either described by the reserved word `DATE` followed by a `String` literal of the form `yyyy-mm-dd`, the reserved word `TIME` followed by a `String` literal of the form `hh:mm:ss[.nnn]` \(the specification of milliseconds is optional\), or the reserved word `TIMESTAMP` followed by a `String` literal of the form `yyyy-mm-dd hh:mm:ss[.nnn]`. See examples below.

```
SELECT e FROM Employee e WHERE e.HireDate = DATE '2006-11-01'
SELECT e FROM Employee e WHERE e.HireDate = TIMESTAMP '2006-11-01 00:00:00'
```

Note that all date-time literals in fact are timestamps, which means that the date-time literal in the first query above does not represent the date `'2006-11-01'` but in fact the first millisecond of that date `'2006-11-01 00:00:00.000'`. Consequently, above examples are equivalent.

A binary literal is described by the reserved word `BINARY` and the binary value represented by a Hexadecimal string, as in example below.

```
SELECT d FROM Department d WHERE d.BinaryId = BINARY 'D91FA24E19FB065A'
```

Since Starcounter SQL supports object references, you also need a way to represent an object reference to a specific object, i.e. an object literal. Every object in a Starcounter database can be identified by its unique object-id-number. You describe an object literal by the reserved word `OBJECT` followed by the object's object-id-number, as in example below.

```
SELECT e FROM Employee e WHERE e = OBJECT 123
```

