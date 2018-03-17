# Joins

In SQL92 there are five different types of joins, "inner join", "cross join", "left outer join", "right outer join" and "full outer join". Starcounter SQL supports all of them except "full outer join".

The default join type is "inner join" \(`[INNER] JOIN`\), so if you just write the reserved word `JOIN`, the parser will interpret it as an "inner join".

A "cross join" \(`[CROSS] JOIN`\) is an "inner join" without join condition, and thus can be regarded as a special case rather than a separate type of join.

The query \(1\) below is interpreted as a "cross join":

```sql
SELECT e, d FROM Employee e JOIN Department d
```

This query can equivalently be written as an explicit "cross join" \(2\):

```sql
SELECT e, d FROM Employee e CROSS JOIN Department d
```

The query \(3\) below is interpreted as an "inner join":

```sql
SELECT e, d FROM Employee e JOIN Department d ON e.Department = d
```

This query can equivalently be written as an explicit "inner join" \(4\):

```sql
SELECT e, d FROM Employee e INNER JOIN Department d ON e.Department = d
```

Note that can use object references in join conditions, as for example `e.Department = d` in the queries \(3\) and \(4\). In standard SQL you would have to write something like query \(5\) below:

```sql
SELECT e, d FROM Employee e INNER JOIN Department d ON e.DepartmentId = d.Id
```

For "left outer join" \(`LEFT [OUTER] JOIN`\) and "right outer join" \(`RIGHT [OUTER] JOIN`\) you may omit the reserved word `OUTER`.

The query \(6\) below will return all employees and their managers, including the employees that have no manager:

```sql
SELECT e1, e2 FROM Employee e1 LEFT JOIN Employee e2 ON e1.Manager = e2
```

This "left outer join" can equivalently be rewritten as a "right outer join" as in query \(7\) below:

```sql
SELECT e1, e2 FROM Employee e2 RIGHT JOIN Employee e1 ON e1.Manager = e2
```

