# Database creation options

**Note**: Starcounter 3.0.0 is currently in preview stage. This API might be changed in the future releases without backwards compatibility.

Starcounter database can be created with the following options configured.

| Name | Type | Required | Default Value | Description |
| :--- | :--- | :--- | :--- | :--- |
| `Collation` | `string` | True | `en-GB-CI-AS` | Database string values collation. |
| `FirstObjectId` | `ulong` | True | `1` | First object id value. |
| `LastObjectId` | `ulong` | True | `ulong.MaxValue` | Last object id value. |
| `LogFileSize` | `int` | True | `256` | Database transaction log file size in [MiB](https://en.wikipedia.org/wiki/Mebibyte). |

These values are available to configure via `Starcounter.Nova.Bluestar.DatabaseCreationOptions` class. Sample configuration:

```csharp
services.AddStarcounter("Database=./path/to/db")
    .Configure<Starcounter.Nova.Bluestar.DatabaseCreationOptions>(options =>
    {
        options.Collation = "en-GB-CI-AS";
        options.FirstObjectId = 1;
        options.LastObjectId = int.MaxValue;
        options.LogFileSize = 64;
    });
```

**\*Note**: any of these values is not possible to change after database creation.\*

## The `Collation` option

Currently Starcounter supports the following string collations:

* `en-GB-CI-AS`, English, Case Insensitive, Accent Sensitive.
* `sv-SE`, Swedish, Case Insensitive, Accent Insensitive.
* `nb-NO`, Norwegian, Case Insensitive, Accent Insensitive.
* `en-GB`, English, Case Insensitive, Accent Insensitive.
* `ru-RU`, Russian, Case Insensitive, Accent Insensitive.

## The `FirstObjectId` and `LastObjectId` options

There options are used to limit range of the object identifiers used by Starcounter to identify objects. Beware that Starcounter will throw an exception if there is no more free values left. Any id value is used only once, and not reused in the future, even if the original object was deleted.

## The `LogFileSize` option

Starcounter streams all database changes to the disk in form of a transaction log, once the transaction log file grows up to the specified size, it's compressed and a new file created. Any Starcounter database consists of at least two transaction log files, thus the minimal database size is `2 * (transaction log file size)`.

256 MiB is the optimal value for production, but it can be tweaked if needed.

* For testing purposes it might be beneficial to use smaller transaction log file size, such as 64 MiB.
* The transaction log file size can be increased to any desired size if it's needed to perform transaction log compression less frequently.

