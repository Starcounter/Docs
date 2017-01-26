# Data operators

In Starcounter SQL only the most common operators on data are implemented.

The standard arithmetic operators, plus (<code>+ <var>x</var></code>), minus (<code>- <var>x</var></code>), addition (<code><var>x</var> + <var>y</var></code>), subtraction (<code><var>x</var> - <var>y</var></code>), multiplication (<code><var>x</var> * <var>y</var></code>) and division (<code><var>x</var> / <var>y</var></code>), are supported for all numerical types. See example below.

```sql
SELECT (e.Salary * 12) / 365 FROM Employee e
```

String concatenation (<code><var>x</var> || <var>y</var></code>) is supported. See example below.

```sql
SELECT e.FirstName || ' ' || e.LastName FROM Employee e
```

For the expected datatypes of an arithmetic operation, see [Datatypes](/guides/database/datatypes).
