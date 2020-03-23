# star tool

The `star` tool is a cross-platform command-line tool, distributed as a stand-alone console application, used to manage Starcounter databases and applications. By using the `star` tool, you can – for example – create, modify, delete and query Starcounter databases, as well as import and export data from compatible 3:rd party data sources.

The easiest way to explore the features of the `star` tool is to use it, in particular with the `--help` option, which prints help information about available commands and options. In this article, we will provide a reference for the available commands and describe some common use cases.

## Distribution

The Windows and Linux versions of the tool are downloaded together with the main [Starcounter binaries](./#installation). For Linux operating systems, make sure to also install the prerequisites listed at the [installation page](./#installation) and grant execution permissions \(`chmod -R u+x /path/to/star/tool/folder`\).

## Usage

The `star` tool is a console application with a command-line interface, and we interact with it using commands, options and arguments.

The global command pattern is:

```text
star [options] [command]
```

Specific commands may have different sub-patterns, which are defined in their respective sections below. The option `--help` is defined for each command, and can be used to print details on available options and sub-commands.

In examples below, Windows-style directory paths are used. On Linux operating systems, the `star` tool expects Linux-style paths.

### Commands

The available commands can be split into four categories:

* [Database management commands](star-tool.md#database-management), used for creating and starting databases.
* [Import and export commands](star-tool.md#import-and-export), used for importing and exporting data to or from SQLite database files.
* [Information commands](star-tool.md#information), e.g. printing all tables or listing all indexes of a given table.
* [SQL REPL](star-tool.md#sql-repl), used for executing DML and DDL statements manipulating the database schema and data.

#### Database management

**new**

```text
star new [options] <path>
```

The `new` command creates a new database at the absolute or relative path provided in the `<path>` argument. If the directory referenced by the path does not exist, it will be created. To create a new database at the path `C:\databases\mydb`, we run:

```text
star new C:\databases\mydb
```

**start**

```text
star start [options] <db>
```

The `start` command starts the database referenced by the absolute or relative path given in the `<db>` argument. To start an existing database located at `C:\databases\mydb`, we run:

```text
star start C:\databases\mydb
```

**clean**

```text
star clean <db>
```

The `clean` command cleans a database from deleted tables and columns.

```text
star clean C:\databases\mydb
```

#### Import and export

**reload**

```text
star reload [options] <db> <file>
```

The `reload` command takes data from an SQLite database file referenced by the absolute or relative path in the `<file>` argument and imports that data into the database referenced by the absolute or relative path given in the `<db>` argument. To load data from an SQLite database file named `myDatabase.sqlite3` located in `C:\databases` into an existing database located at `C:\databases\mydb`, we run:

```text
star reload C:\databases\mydb C:\databases\myDatabase.sqlite3
```

_Note: only files created with the `unload` command can be reloaded._

**unload**

```text
star unload [options] <db> <file>
```

The `unload` command selects the SQLite database file at the path given in the `<file>` argument, or creates one if it doesn't exists, and then takes data from a database referenced by the absolute or relative path given in the `<db>` argument and imports that data into the created SQLite database. To load data from an existing database located at `C:\databases\mydb` into a new SQLite database file named `myDatabase.sqlite3` at `C:\databases`, we run:

```text
star unload C:\databases\mydb C:\databases\myDatabase.sqlite3
```

#### Information

**list**

```text
star list [options] [command]
```

The `list` command is used together with one of the following sub-commands to list ranges of entities in a database:

**list table**

```text
star list table [options] <db>
```

The `list table` sub-command lists all tables in the database referenced by the absolute or relative path given in the `<db>` argument. The following options are available to configure the output of the command:

| Option | Short form | Description |
| :--- | :--- | :--- |
| `--user` | `-u` | Includes user tables in the output. |
| `--system` | `-s` | Includes system tables in the output. |
| `--format` | `-f` | Lets you configure the formatting of the output by providing one or more format options. |

Formatting options:

* `t` – Include the name of the table to list.
* `b` – Include the name of the base table \(if inherited\) of the table to list.

To list all tables of an existing database located at `C:\databases\mydb`, including only user tables and displaying both the name of the table and the name of its base table \(if any\), we run:

```text
star list table -u -f tb C:\databases\mydb
```

**list index**

```text
star list index [options] <db> <table>
```

The `list index` sub-command lists all indexes in a table, the name of which is specified by the `<table>` argument, in the database referenced by the absolute or relative path given in the `<db>` argument. The following options are available to configure the output of the commmand:

| Option | Short form | Description |
| :--- | :--- | :--- |
| `--user` | `-u` | Includes user indexes in the output. |
| `--system` | `-s` | Includes system indexes in the output. |
| `--inherited` | `-i` | Includes the indexes of base tables in the output. |
| `--format` | `-f` | Lets you configure the formatting of the output by providing one or more formatting options. |

Formatting options:

* `i` – Include the name of the index to list.
* `t` – Include the name of the table of the index.
* `u` – Include whether or not the index is unique.

To list all indexes of the table `MyApp.Superhero`, of an existing database located at `C:\databases\mydb`, including both user indexes and system indexes and both the name of the index and whether it's unique, we run:

```text
star list index -u -s -f iu C:\databases\mydb MyApp.Superhero
```

**info**

```text
star info [options] [command]
```

The `info` command is used together with one of the following sub-commands to print info about some specific entity in a database:

**info table**

```text
star info table [options] <db> <table>
```

The `info table` sub-command prints detailed information about a table, the name of which is specified in the `<table>` argument, in the database referenced by the absolute or relative path given in the `<db>` argument. The following options are available to configure the output of the command:

| Option | Short form | Description |
| :--- | :--- | :--- |
| `--user` | `-u` | Includes user columns in the output. |
| `--system` | `-s` | Includes system columns in the output. |
| `--inherited` | `-i` | Includes inherited columns in the output. |
| `--format` | `-f` | Lets you configure the formatting of the output by providing one or more formatting options. |

Formatting options:

* `c` – Include the name of the columns of the table to list.
* `t` – Include the datatype of the columns of the table to list.
* `n` – For each column, include whether it accepts null values in table cells.
* `i` – For each column, include whether it is inherited.
* `b` – Include the name of the table of the column.

To list all user columns of the `MyApp.Superhero` table of an existing database located at `C:\databases\mydb`, including inherited columns and listing only the names and the datatypes of columns, we run:

```text
star info table -u -i -f ct C:\databases\mydb MyApp.Superhero
```

**info index**

```text
star info index [options] <db> <index>
```

The `info index` sub-command prints detailed information about an index, the name of which is specified in the `<index>` argument, in the database referenced by the absolute or relative path given in the `<db>` argument. The following options are available to configure the output of the command:

| Option | Short form | Description |
| :--- | :--- | :--- |
| `--format` | `-f` | Lets you configure the formatting of the output by providing one or more formatting options. |

Formatting options:

* `c` – Include the name of the column that the index is registered on.
* `b` – Include the name of the table that holds the column that the index is registered on.

To list all indexes of the `MyApp.Superhero` table of an existing database located at `C:\databases\mydb`, including only the name of the column of the index, we run:

```text
star info index -f c C:\databases\mydb MyApp.Superhero
```

**SQL REPL**

**sql**

```text
star sql [options] <db>
```

The `star` tool includes a built-in SQL REPL \(read-eval-print loop\) component that can be used to execute DDL and DML statements on the database referenced by the absolute or relative path given in the `<db>` argument.

To enter the SQL REPL for an existing database located at `C:\databases\mydb`, we run:

```text
star sql C:\databases\mydb
```

For more information on how to write SQL queries to a Starcounter database, and supported DDL statements, see the [SQL section](sql.md) of the documentation.

