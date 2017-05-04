# Sorting

Starcounter SQL supports sorting with `ORDER BY` as exemplified in query below.

```sql
SELECT e.LastName, e.Salary FROM Employee e ORDER BY e.Salary DESC, e.LastName ASC
```

**Performance note:** When there is an index matching the sort specification in the `ORDER BY` clause then the result can be obtained without executing any sorting. We therefore for performance reasons strongly recommend that necessary indexes are declared when using `ORDER BY` clauses as exemplified in statement below.

```sql
CREATE INDEX EmployeeIndex ON Employee (Salary DESC, LastName ASC)
```

See more information on [index declaration](/guides/SQL/indexes).
