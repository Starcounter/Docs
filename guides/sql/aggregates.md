# Aggregates

The current version of Starcounter SQL does not officially support aggregates because the support of aggregates is not yet performance optimized.

However, there is an experimental support for aggregates in the SQL browser in Starcounter Administrator. It is also available in your code by using the method `Db.SQL`.

`Db.SQL` allows for aggregates with grouping \(`GROUP BY`\), conditions on groups \(`HAVING`\), and the standard set functions `AVG`, `SUM`, `COUNT`, `MAX` and `MIN`.

```sql
SELECT AVG(e.Salary), MAX(e.Salary), MIN(e.Salary), e.Department
  FROM Example.Employee e
  GROUP BY e.Department
  HAVING SUM(e.Salary) > 20000
```

## Using Asterisk Shorthand With `COUNT`

The asterisk shorthand is treated as a literal in `COUNT`. Since `Db.SQL` doesn't support literals, using `COUNT(*)` in `Db.SQL` will throw `ScErrUnsupportLiteral`:

```text
ScErrUnsupportLiteral (SCERR7029): Literals are not supported in the query. Method Starcounter.Db.SQL does not support queries with literals. Found literal is 1. Use variable and parameter instead.
```

There are three ways to work around this:

```csharp
// This throws SCERR7029
Db.SQL("SELECT COUNT(*) FROM Person").First();
// Using an identifier instead of * works
Db.SQL("SELECT COUNT(p) FROM Person p").First();
// Db.SlowSQL supports literals, so it can be used
Db.SlowSQL("SELECT COUNT(*) FROM Person").First();
// Linq can also give you the number of rows
Db.SQL("SELECT p FROM Person p").Count();
```

The first option of using an identifier to get the count best in most cases, both for versatility and performance.

