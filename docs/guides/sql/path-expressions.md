# Path expressions

In standard SQL92 you can only refer to columns \(properties\) of the tables \(extents\) which are specified in the FROM clause of the SELECT statement. In contrast to that, in Starcounter SQL you can refer to any property/column of an object in any extent/table as long as there is a path from an extent/table specified in the FROM clause to that property/column.

A path expression is an arbitrary long sequence of identifiers separated by points. The first identifier in the sequence should be an alias to an extent/table defined in the FROM clause of the SELECT statement. The following identifiers  except the last one should have object references as return values. The last identifier may return any supported datatype. See example below.

```sql
SELECT e.Manager.Department.Location.Street FROM Employee e
```

Note that if some identifier in a path expression will return null then the complete path expression will also return null.

In fact, you do not have to qualify \(start with an extent/table alias\) a path expression. You are allowed to omit the qualifications of the path expressions \(column references\) as in the   query \(2\) below.

```sql
SELECT Manager.Department.Location.Street, Name FROM Employee JOIN Department ON DepartmentId = Id
```

However, you are only allowed to omit the qualifications as long as the names of the  properties/columns are unique, and this may change over time. Therefore we strongly recommend that when you write SQL statements in program code you always qualify all path expressions so your code will hold for future modifications of the database schema.

You are also allowed to use a wildcard \(\*\) to select all properties/columns of an extent/table as in the queries below.

```sql
SELECT * FROM Employee
SELECT e.* FROM Employee e
```

Note that the above queries return all properties/columns of the Employee objects, while the below query returns references to the Employee objects themselves.

```sql
SELECT e FROM Employee e
```

