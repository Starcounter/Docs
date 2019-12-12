# The `star` tool

//

**Notes on how to proceed with this doc:**

I have splitted the various commands into groups, we should just continue with this, detailing each command and group like I've begun to do.

//

The `star` tool is a cross-platform command-line tool, distributed as a stand-alone console application, used to manage Starcounter databases and applications. By using the `star` tool, you can – for example – create, modify, delete and query Starcounter databases, as well as import and export data from compatible 3:rd party data sources.

The easiest way to expore the features of the `star` tool is to use it, in particular with the `--help` option, which prints help information about available commands and options. In this article, we will provide a reference for the available commands and describe some common use cases.

## Distribution

The Windows and Linux versions of the tool can be downloaded from our [downloads page](). For Linux operating systems, make sure to also install the prerequisites listed at the [installation page](README.md#installation).

## Usage

The `star` tool is a console application with a command-line interface, and we interact with it using commands, options and arguments.

The global command pattern is:

```
star [options] [command]
```

Specific commands may have different sub-patterns, which are defined in their respective sections below. The option `--help` is defined for each command, and can be used to print details on available options and sub-commands.

### Commands

The available commands can be split into three categories:

- Database management commands, used for creating and starting databases.
- Information commands, e.g. printing all tables or listing all indexes of a given table.
- SQL commands, used for executing DML and DDL statements manipulating the database schema and data.

#### Database management

##### `new`

```
star new [options] <path>
```

The `new` command creates a new database at the absolute or relative path provided in the `<path>` argument. If the directory referenced by the path does not exist, it will be created. To create a new database at the path `C:\databases\mydb`, we run:

```
star new C:\databases\mydb
```

##### `start`

```
star start [options] <db>
```

The `start` command starts
