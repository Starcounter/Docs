# Aggregates

The current version of Starcounter SQL does not officially support aggregates because the support of aggregates is not yet performance optimized.

However, there is an experimental support for aggregates in the SQL browser in Starcounter Administrator. It is also available in your code by using the method `Db.SQL`.

`Db.SQL` allows for aggregates with grouping (`GROUP BY`), conditions on groups (`HAVING`), and the standard set functions `AVG`, `SUM`, `COUNT`, `MAX` and `MIN`.

```sql
SELECT COUNT(*), AVG(e.Salary), MAX(e.Salary), MIN(e.Salary), e.Department
  FROM Example.Employee e
  GROUP BY e.Department
  HAVING SUM(e.Salary) > 20000
```