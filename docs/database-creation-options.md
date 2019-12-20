# Database creation options

When creating a Starcounter database, we can provide the following options to configure it.

| Name | Type | Required | Default Value | Description |
| :--- | :--- | :--- | :--- | :--- |
| `Collation` | `string` | Yes | `en-GB-CI-AS` | The collation to use for string values in the database. |
| `FirstObjectId` | `ulong` | Yes | `1` | The first [object id](https://github.com/Starcounter/Docs/tree/0b0e3679194d671f9b0fa4bfafeb9f5f4aefa817/docs/database-classes/README.md#database-object-identity) \(oid\) value. |
| `LastObjectId` | `ulong` | Yes | `ulong.MaxValue` | The last [object id](https://github.com/Starcounter/Docs/tree/0b0e3679194d671f9b0fa4bfafeb9f5f4aefa817/docs/database-classes/README.md#database-object-identity) \(oid\) value. |
| `LogFileSize` | `int` | Yes | `256` | Database transaction log file size in [MiB](https://en.wikipedia.org/wiki/Mebibyte). |

These options are available to configure via the `Starcounter.Database.Bluestar.DatabaseCreationOptions` class.

Example:

```csharp
var services = new ServiceCollection();

services.AddStarcounter("Database=./path/to/db")
    .Configure<Starcounter.Database.Bluestar.DatabaseCreationOptions>(options =>
    {
        options.Collation = "en-GB-CI-AS";
        options.FirstObjectId = 1;
        options.LastObjectId = int.MaxValue;
        options.LogFileSize = 64;
    });
```

**\*Note**: These options are unchangeable after the database has been created.\*

## `Collation`

The `Collation` option defines the collation to use for string values in the database. Starcounter supports the following string collations:

* `en-GB-CI-AS`, English, case insensitive, accent sensitive.
* `sv-SE`, Swedish, case insensitive, accent insensitive.
* `nb-NO`, Norwegian, case insensitive, accent insensitive.
* `en-GB`, English, case insensitive, accent insensitive.
* `ru-RU`, Russian, case insensitive, accent insensitive.

## `FirstObjectId` and `LastObjectId`

These options are used to limit range of the [object identifiers](https://github.com/Starcounter/Docs/tree/0b0e3679194d671f9b0fa4bfafeb9f5f4aefa817/docs/database-classes/README.md#database-object-identity) \(oid's\) used by Starcounter. Every time a new database object is created, a unique oid will be assigned to it. If there is no more unique numbers in the range, an exception will be thrown.

## The `LogFileSize` option

Starcounter continuously writes database changes to the transaction log on disk. Once the transaction log file grows to the size specified by `LogFileSize`, it's compressed and a new file created. Any Starcounter database consists of at least two transaction log files.

256 MiB is the optimal value for production, but it can be tweaked if needed.

* For testing purposes it might be beneficial to use smaller transaction log file size, such as 64 MiB.
* The transaction log file size can be increased to any desired size to make transaction log compression happen less frequently.

