# Sorting

Starcounter SQL supports sorting (<code>ORDER BY</code>) as exemplified in query below.

```sql
SELECT e.LastName, e.Salary
  FROM Employee e
  ORDER BY e.Salary DESC, e.LastName ASC
```

<strong>Performance note:</strong> When there is an index matching the sort specification in the <nobr><code>ORDER BY</code></nobr> clause then the result can be obtained without executing any sorting. We therefore for performance reasons strongly recommend that necessary indexes are declared when using <code>ORDER BY</code> clauses as exemplified in statement below.

```sql
CREATE INDEX EmployeeIndex ON Employee (Salary DESC, LastName ASC)
```

See more information on [index declaration](/guides/SQL/indexes).</strong>
