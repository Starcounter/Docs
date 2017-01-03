# Literals

A boolean literal can have one of the two values <em>true</em> and <em>false</em>, which are represented by the two reserved words <code>TRUE</code> and <code>FALSE</code>. See example below.

```sql
SELECT e FROM Employee e WHERE e.Commission = TRUE
```

There are three types of numerical literals <code>Int64</code>, <code>Decimal</code> and <code>Double</code>.

An <code>Int64</code> literal is described by its integer value, as in example below.

```sql
SELECT e FROM Employee e WHERE e.Salary = 5000
```

A <code>Decimal</code> literal is described by its numerical value including a decimal point, as in example below.

```sql
SELECT e FROM Employee e WHERE e.Salary = 5000.00
```

A <code>Double</code> literal is described by two numerical values, the <em>mantissa</em> and the <em>exponent</em>, separated by the character <code>E</code>. The mantissa may include a decimal point, but the exponent may not. See example below.

```sql
SELECT e FROM Employee e WHERE e.Salary = 5.0E3
```

A string literal is a sequence of characters beginning and ending with single quote characters. To represent a single quote character within a String literal, you write two consecutive single quote characters, as in example below.

```sql
SELECT p FROM Photo p WHERE p.Description = 'Smith''s family'
```

A date-time literal is either described by the reserved word <code>DATE</code> followed by a <code>String</code> literal of the form <code><var>yyyy</var>-<var>mm</var>-<var>dd</var></code>, the reserved word <code>TIME</code> followed by a <code>String</code> literal of the form <code><var>hh</var>:<var>mm</var>:<var>ss</var>[.<var>nnn</var>]</code> (the specification of milliseconds is optional), or the reserved word <code>TIMESTAMP</code> followed by a <code>String</code> literal of the form <code><var>yyyy</var>-<var>mm</var>-<var>dd</var> <var>hh</var>:<var>mm</var>:<var>ss</var>[.<var>nnn</var>]</code>. See examples below.

```sql
SELECT e FROM Employee e WHERE e.HireDate = DATE '2006-11-01'
SELECT e FROM Employee e WHERE e.HireDate = TIMESTAMP '2006-11-01 00:00:00'
```

Note that all date-time literals in fact are timestamps, which means that the date-time literal in the first query above does not represent the date <code>'2006-11-01'</code> but in fact the first millisecond of that date <code>'2006-11-01 00:00:00.000'</code>. Consequently, above examples are equivalent.

A binary literal is described by the reserved word <code>BINARY</code> and the binary value represented by a Hexadecimal string, as in example below.

```sql
SELECT d FROM Department d WHERE d.BinaryId = BINARY 'D91FA24E19FB065A'
```

Since Starcounter SQL supports object references, you also need a way to represent an object reference to a specific object, i.e. an object literal. Every object in a Starcounter database can be identified by its unique object-id-number. You describe an object literal by the reserved word <code>OBJECT</code> followed by the object's object-id-number, as in example below.

```sql
SELECT e FROM Employee e WHERE e = OBJECT 123
```

<strong>Performance note</strong>: In code, for performance reasons, are not allowed to use literals. Instead you should use [variables](/guides/database/variables/) and parameters with values.
