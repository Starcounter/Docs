# Query plan hints

The Starcounter SQL optimizer decides the execution plan of an SQL-query. If you want to hint the optimizer that you prefer some particular join order or that you prefer some particular indexes to be used, you can do that in the OPTION clause at the end of the SQL statement.

To specify a preferred join order to use, you write JOIN ORDER \(extent-alias-sequence\) in the OPTION clause. You do not need to specify the order of all included extents in the extent-alias-sequence, only the ones for which you have a preferred join order. See example below.

```sql
SELECT d.Name, e.LastName FROM Department d 
  JOIN Employee e ON e.Department = d 
  WHERE e.FirstName = 'Bob' 
  OPTION JOIN ORDER (e,d)
```

If it is not possible to execute a query in the join order specified in the OPTION clause, which can be the case for outer joins, then the optimizer choses the join order to use. If you specify several join order hints only the first one will be considered.

To specify a preferred index to use for a particular extent/table, you write  INDEX \(extent-alias index-name\) in the OPTION clause. If some specified index does not exist then the optimizer choses another index if there is one. See example below.

```sql
SELECT e.FirstName, e.LastName FROM Employee e 
  WHERE e.FirstName = 'John' AND e.LastName = 'Smith' 
  OPTION INDEX (e MyIndexOnLastName)
```

You can specify an index to use for each extent in the SQL-query as in example  below. If you specify more than one index hint for a particular extent only the first one will be considered.

```sql
SELECT e, m FROM Employee e JOIN Employee m ON e.Manager = m 
  WHERE e.FirstName = 'John' AND e.LastName = 'Smith'
  AND m.FirstName = 'David' AND m.LastName = 'King' 
  OPTION INDEX (e MyIndexOnLastName), INDEX(m MyIndexOnFirstName)
```

You can specify both one index hint for each extent and one join order hint in the OPTION clause of a query, which is exemplified in example below.

```sql
SELECT e, m FROM Employee e JOIN Employee m ON e.Manager = m 
  WHERE e.FirstName = 'John' AND e.LastName = 'Smith' 
  AND m.FirstName = 'David' AND m.LastName = 'King' 
  OPTION JOIN ORDER (e, m), INDEX (e MyIndexOnLastName), 
  INDEX (m MyIndexOnFirstName)
```

