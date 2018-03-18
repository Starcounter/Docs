# StarDump CLI

## Introduction

StarDump CLI is a command-line interface that unloads and reloads database data. It's found in `C:\Program Files\Starcounter\StarDump`.  

The data is unloaded into `sqlite3` files that can be examined or [modified](using-unloadreload-to-modify-database-schema.md#modify-the-schema-in-the-database) with [DB Browser for SQLite](http://sqlitebrowser.org/).

{% hint style="info" %}
All examples on this page assume that you have `C:\Program Files\Starcounter\StarDump` as an environment variable.
{% endhint %}

The general format for the StarDump CLI looks like this:

```bash
stardump [command] [options]
```

There are two commands, `unload` and `reload`, and both of them have their own options.

## Unload Database

The `unload` command takes a Starcounter database and creates a `sqlite3` file with the data. Running `stardump unload` without any options will unload the `default` database to `%TEMP%\stardump-<database>-<date>.sqlite3`. To specify the unload further, these options can be used:

| Name | Type | Default value | Description |
| :--- | :--- | :--- | :--- |
| `-db`, `--database` | `string` | `default` | Database name to unload |
| `-f`, `--file` | `string` | `%TEMP%\stardump-<database>-<date>.sqlite3` | Output file path with file name |
| `-b`, `--buffersize` | `int` | `500` | Number of rows in a single `INSERT`operation |
| `-scp`, `--skipcolumnprefixes` | `string[]` | `__` | Column prefixes to skip |
| `-stp`, `--skiptableprefixes` | `string[]` |  | Table prefixes to skip |
| `-st`, `--skiptables` | `string[]` |  | Table names to skip |
| `-ut`, `--unloadtables` | `string[]` |  | Table names to unload |
| `-V`, `--verbose` | `int` | `2` | Verbose mode, higher number prints more \[Error:0, Warning:1, Notice:2, Info:3, Debug:4\] |

The option values are case sensitive. Also,  `unloadtables`disables `skiptableprefixes` and `unloadtables` if they are used together. `string[]` values should be space or comma separated.

Example of unload:

```text
stardump unload --database default --file C:\Temp\default.sqlite3
```

The targeted database can either be running or stopped when it's unloaded. If the database is running, StarDump creates a snapshot of the database when the unload starts so that no changes made during the unload are included. 

You can use `stardump unload --help` to get more information about the available options.

## Reload Database



The `reload` command takes a `sqlite3` file and reloads it into a Starcounter database. The basic structure of a reload looks like this:

```text
stardump reload --database [DatabaseName] --file [FilePath]
```

The `--database` and `--file` option always have to be specified. 

These are all options for the `reload` command:

| Name | Type | Description |
| :--- | :--- | :--- |
| `-db`, `--database` | `string` | Database name to reload into |
| `-f`, `--file` | `string` | Path to `sqlite3` file to reload |
| `-fr`, `--forcereload` | none | Force reload even if the database already contains data. The user has to take care of object ID uniqueness. |
| `-offset`, `--insertobjectnooffset` | `long` | `ObjectNo` offset in the target database |
| `-reassign`, `--reassignobjectno` | none | Sets the `ObjectNo` offset to the highest`ObjectNo` in the target database `-noschema`, `--skipschema` |
| `-noschema, --skipschema` | none | If flag is set, then schema creation and validation will be skipped.   In `--skipschema` mode, StarDump reloads data of matching tables and columns between existing Starcounter database and reloading dump. |
| `-V`, `--verbose` | `int` | Verbose mode, higher number prints more \[Error:0, Warning:1, Notice:2, Info:3, Debug:4\] |

None of these are have a default value except for `--verbose` which is set to `2`.

If `--forcereload` is not used, the database should be dropped and created prior to reload to ensure that the database is empty:

```text
staradmin -d=default delete --force db
staradmin -d=default new db DefaultUserHttpPort=8080
stardump reload --database default --file C:\Temp\default.sqlite3
```

You can use `stardump reload--help` to get more information about the available options.

