# SQL Queries

## Data Manipulation Language \(DML\) Queries

Starcounter 3.0 has the same SQL Query Processor as Starcounter 2.3.2. Please refer to the [original documentation](https://docs.starcounter.io/v/2.3.2/guides/sql) for the full specification.

### Arbitrary database access

It is possible to perform arbitrary database access using `Starcounter.Database.ISqlResult`, `Starcounter.Database.ISqlResultRow`, and `Starcounter.Database.ISqlResultColumn` interfaces.

The following example demonstrates how to execute an arbitrary SQL query and retrieve its result.

```cs
transactor.Transact(db =>
{
    // Retrieving SQL query result in an arbitrary form.
    ISqlResult<ISqlResultRow> result = db.Sql<ISqlResultRow>("SELECT p.FirstName, p.LastName FROM Person p");

    // Retrieving list of columns in the SQL result.
    IReadOnlyList<ISqlResultColumn> columns = result.Columns;

    // Iterating over each row in the SQL query result.
    foreach (ISqlResultRow row in result)
    {
        // Iterating over each column in the SQL query result.
        for (int i = 0; i < columns.Count; i++)
        {
            ISqlResultColumn col = columns[i];

            // Printing the column's value of the current row.
            Console.WriteLine($"{col.Name}: {row.GetValue(i)}");
        }

        Console.WriteLine();
    }
});
```

## Data Definition Language \(DDL\) Queries

As of right now the following DDL statements are supported:

### Create / drop database index

```sql
CREATE INDEX IX_IndexName ON TableName (ColumnName)
```

```sql
DROP INDEX IX_IndexName ON TableName
```

### Create / drop database table

```sql
CREATE TABLE TableName
(
    BooleanColumn boolean,
    DecimalColumn decimal,
    DoubleColumn double,
    FloatColumn, float,
    IntColumn int,
    UIntColumn uint,
    TextColumn text
)
```

```sql
DROP TABLE TableName
```

#### Starcounter database vs .NET CLR data types

There is no one to one match between Starcounter database data types and .NET CLR data types.

Use the following table to translate .NET CLR data type names into Starcounter database SQL data type names.

| .NET CLR Data Type | Starcounter database data type |
| :--- | :--- |
| `Boolean` | `boolean` |
| `Byte` | `int` |
| `Byte[]` | `binary` |
| `DateTime` | `uint` |
| `Decimal` | `decimal` |
| `Double` | `double` |
| `Float` | `float` |
| `Int16` | `int` |
| `Int32` | `int` |
| `Int64` | `int` |
| `SByte` | `int` |
| `String` | `text` |
| `UInt16` | `uint` |
| `UInt32` | `uint` |
| `UInt64` | `uint` |

### Create / drop database table column

```sql
ALTER TABLE TableName
DROP COLUMN ColumnName
```

```sql
ALTER TABLE TableName
ADD ColumnName (TextColumn text)
```
