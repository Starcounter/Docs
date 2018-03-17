# Aggregates

## Introduction

Starcounter SQL supports five different aggregates: `AVG`, `SUM`, `COUNT`, `MAX`, and `MIN`. These can be used with grouping and conditions on groups with the `GROUP BY` and `HAVING` clauses.

## Example

```sql
SELECT AVG(e.Salary), MAX(e.Salary), MIN(e.Salary), e.Department
  FROM Example.Employee e
  GROUP BY e.Department
  HAVING SUM(e.Salary) > 20000
```

## Using Asterisk Shorthand With `COUNT`

The asterisk shorthand is treated as a literal in `COUNT`. Since `Db.SQL` doesn't support literals, using `COUNT(*)` in `Db.SQL` will throw  `ScErrUnsupportLiteral (SCERR7029)`.

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

