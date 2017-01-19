# Limitations

Starcounter supports a limited subset of SQL.

Only `CREATE INDEX` statement is supported among SQL DDL and `SELECT` statement - among SQL DML.

Operations related to other statements of DML can be performed through C# API, see [Data manipulation](/guides/database/data-manipulation.html). Some statements of DDL are supported through C# API (see [Creating database classes](/guides/database/creating-database-classes.html)) and some - by using command line tools.

## INSERT, UPDATE and DELETE

There are three data manipulation statements in SQL92: INSERT, UPDATE and DELETE. None of these statements are supported in Starcounter SQL. Instead you create, update and delete objects directly in your programming code. See [Data Manipulation](/guides/database/data-manipulation.html) for more information.

## Subqueries

Starcounter SQL does **not** support subqueries. In SQL92 you can write a subquery at any place in the `WHERE` clause of a `SELECT` statement where a value expression is allowed. Such statements may give rise to run time errors depending on the current data in the database.
