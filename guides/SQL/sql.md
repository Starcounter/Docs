# SQL

Starcounter SQL follows the established standard SQL92 (ANSI INCITS 135-1992, R1998) to support easy data exchange with other databases and external tools. Currently we only support the query part of SQL92 which consists of the `SELECT` statement. See [Limitations](/guides/sql/limitations/).

## Calling SQL

You can write SQL queries synchronously integrated with your programming code using a static method `Db.SQL`. See [Querying using SQL](/guides/database/querying-using-sql/).

You can execute queries in the interactive SQL browser of [Starcounter Administrator](/guides/tools/administrator/).

## Object extensions to SQL

Starcounter SQL contains some extensions to the SQL92 standard to better deal with objects, since the standard SQL only supports relational databases. For these extensions we follow the Object Data Standard ODMG 3.0 (ISBN 1-55860-647-4). The object extensions in Starcounter SQL are:

- object references,
- [path expressions](/guides/sql/path-expressions/).

In traditional SQL you can only refer to tables, columns, rows and fields of values. The concept of an "object" is represented by a row in a table. You refer to an "object" by values on its primary key which are some specified columns.

In Starcounter SQL you can refer to an object itself. For example, in query below the identifier <code>e</code> is an object reference.

```sql
SELECT e FROM Employee e
```

In a traditional SQL database, to get data from more than one type of object (table/class) you have to do a "join" of a number of tables. For example, query below gives you the names of the employees and the names of the departments where they work.

```sql
SELECT e.FirstName, d.Name FROM Employee e JOIN Department d ON e.DepartmentId = d.Id
```

In Starcounter SQL there is a more convenient way to get the same result by instead using a path expression as in query below. In that way, you can from one type of object (<code>Employee</code>) reach another type of object (<code>Department</code>) by object reference.

```sql
SELECT e.FirstName, e.Department.Name FROM Employee e
```

In object oriented programming the extent of a class is all object instances of that class. An extent of object instances corresponds to a table of rows in a relational database. Thus, <code>Employee</code> in the example queries above can either be regarded as the extent of the class <code>Employee</code> or as the table <code>Employee</code>.

## Performance notes

We want to point out that the current version of Starcounter SQL is not completely
optimized, which means we give no general guarantees regarding performance. However, for most queries when the right indexes are defined the Starcounter SQL gives you the extreme performance of the Starcounter database.
