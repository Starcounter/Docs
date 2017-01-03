# Staradmin CLI

With **staradmin.exe**, users can interact with Starcounter from the command-line, running different management tasks. These tasks include stopping of running applications, killing Starcounter processes, unloading and reloading databases, and more.

## Basic usage

Running staradmin without arguments will display the usage message in the console. The general syntax of the staradmin tool is:

```nginx
staradmin [options] command [<command options>] [<parameters>]
```

The **options** section include options that apply either to the program itself or to the majority of the commands. Options are specified using a `--` prefix and include both flags and properties. As an example, the `--help` flag tells staradmin to write out the usage message, while the `--database=<name>` property allow a user to specify a specific database the upcoming command is to target. Shorthand `-d=<name>` has the same effect.

The **command** tells staradmin what is to be done. Commands are usually verbs. Common commands include `stop`, `list` and `help`.

Some commands support **command options**. As their name implies, these options are optional and specific for the given command. As an example, the `list` command support the `max=<n>` property, allowing the user to limit the set of entries in a displayed list to the value `<n>`.

Finally, most commands will require or at least support some **command parameters**. The parameter will usually describe a type of object the command should operate on; examples include `stop app <name>` and `help list`. The first one instruct staradmin to stop an *application* by name; the second that it should display help on the *list* command.

### Getting help

To see the latest help overview, run `staradmin --help`. 

To find extended help on a certain command or a known topic, issue `staradmin help <command|topic>`, for example `staradmin help stop` to see the help for the *stop* command.

## Console command

The **console** command shows the console output from applications

```nginx
staradmin console [<databases>]
```

Running `staradmin console` without parameters shows the console output from the default database. 

Running `staradmin -d=foo console` shows console output from the "foo" database.

Provide a space-separated list of database names as command parameters to show output from multiple databases. For example, `staradmin console foo` shows console output from the "foo" database. `staradmin console foo bar baz` shows console output from the "foo", "bar" and "baz" databases.

## Delete command

The **delete** command deletes various types of objects, e.g. databases. Usage:

```nginx
staradmin delete [--force] [--failmissing]
```
<strong>Example: delete database </strong> 

To create a user-specified database use 
```cmd 
C:\>staradmin --database=NewDbName delete db  
```
##### Command options

The delete command supports the `--force` flag. This flag tell staradmin you don't want to confirm the requested delete, which otherwise is the default in case you are deleting some sensitive artifact such as a database. Use this flag with care, there is no going back.

```nginx
staradmin -d=foo delete --force db
```

The `--failmissing` flag toogle how staradmin behaves when the artifact you want to delete is not found. By default, such case is treated as a successful operation. With this flag applied, `staradmin` will instead issue an error.

```nginx
staradmin -d=nonExisting delete --failmissing db

ScErrDatabaseNotFound (SCERR10002): ScErrDatabaseNotFound (SCERR10002):
A database with the specified name was not found.
```

##### Object types

The delete command supports the following type of objects to be deleted.

* **Databases**. Usage: `staradmin -d=default delete db`. Deletes a database.

## Kill command

The **kill** command kills processes relating to Starcounter. Usage:

```nginx
staradmin kill <target>
```

Use `all` as the command parameter to target killing all processes relating to Starcounter on the current machine. Use this option with care and make sure no mission-critical processes are running. 

## List command

The **list** command provides viewing of lists. It takes the general form:

```nginx
staradmin list <type>
```

where *type* will indicate the kind of list you want to see. To see a list of databases, use the **db** type; to see a list of all running applications, use **app**.

##### Command options

The list command supports the `max=<n>` property. By using this property, you tell staradmin not to list more entries than the value of `<n>`.

```nginx
staradmin list --max=10 app
```

*Lists running applications, limiting the result to a maximum of 10*

##### Object types

The list command supports the following type of objects to be listed.

* **Databases**. Usage:  `staradmin list db`. List all databases part of the current installation, even those that are not running.
* **Applications**. Usage:  `staradmin list apps`. List all applications currently running, including information on the database they are running in.
* **Logs**. Usage:  `staradmin list log`. Shows the content of the server error log. See more usage on the [Error log](/guides/tools/error-log/) page.

## New command

The **new** command allows creation of new artifacts. It takes the general form:

```nginx
staradmin new <type>
```

where *type* specifies the kind of artifact to create. To create a database, use the **db** type; for applications, use **app**.

<strong>Example: create database </strong> 

To create a user-specified database use 
```cmd 
C:\>staradmin --database=NewDbName new db
```
##### Object types

The new command allow the following type of artifacts to be created:

* **Databases**. Usage:  `staradmin -d=foo new db`. Creates a new database named "foo".
* **Applications**. Usage:  `staradmin new app`. Creates a new application source code file, normally "app.cs".

### New Command From 2.3
The new command works the same way in Starcounter 2.3 as 2.2 except for two changes:

1. Databases are created using the syntax `staradmin new db foo` instead of `staradmin -d=foo new db`.

2. All the available configuration options in the underlying REST JSON representation can be set from the command line. These are:

* Uri
* DataDirectory
* TempDirectory
* DefaultUserHttpPort
* FirstObjectID
* LastObjectID

The options are specified on the creation of the database using this syntax:
```nginx
staradmin new db DataDirectory=C:\Users\Per\Foo DefaultUserHttpPort=1234 Uri=http://example.com/api/databases/foo/configuration"
```
## Reload command

The **reload** command reloads data into a data source, usually a database. Usage:

```nginx
staradmin reload [source] [--file=<path>]
```
If no *source* is given, `db` is used as the default.

##### Object types

- **Databases**. Usage: `staradmin reload db`. Reloads a database.

##### Command options

The reload command supports the `--file=<path>` option. The filename is resolved to the same directory from which the command runs. If the file option is omitted, the default file is used.

```nginx
staradmin -d=bar reload db --file=data.sql
``` 

*Reloads the "data.sql" file into the "bar" database.*

## Start command

The **start** command is used to start processes. It takes the general form:

```nginx
staradmin start <type>
```

where *type* specifies what should be started. Use  `staradmin start db` to start the default database; use `staradmin start server` to start the Starcounter server.

<strong>Example: start database</strong> 

To start a user-specified database use
```cmd 
C:\>staradmin --database=UserDbName start db
```
To start the application with <code>.exe</code> extension on a specified database use:
```cmd
C:\"path to your application">star --database=newdb YourApplicationName.exe
```
To find out more about how to start and stop applications read <a href="http://starcounter.io/guides/tools/starting-and-stopping-apps/">this article</a>.
The console output does not go to the console, but directed to a server side memory buffer and it is possible to view it from the <a href="http://starcounter.io/guides/tools/administrator/">Administrator Web UI</a>.

**NOTE**: Starting databases or the server like this is not the normal scenario. Instead, Starcounter employs a design where processes are started on demand. When starting applications, using `star <app>` or from within Visual Studio, doing "Start" on a **Starcounter Application project**, these processes are started for you automatically (if not already running).

*Starting the "foo" database*
```nginx
staradmin -d=foo start db
Starting foo (started, code host PID: 6048)
```

##### Object types

The start command support starting the following types.

* **Databases**. Usage:  `staradmin start db`. Starting the default database, including all its support processes.
* **The server**. Usage: `staradmin start server`. Starts the Starcounter server.

## Stop command

The **stop** command is used to stop running processes or applications. It takes the general form:

```nginx
staradmin stop <type> [reference]
```

where *type* will indicate the type of the given reference. As an example, to stop an application with the name *foo*, use `staradmin stop app foo`. To stop a database named *bar*, use `staradmin -d=bar stop db`. To stop the default database, use `staradmin stop db`.

<strong>Example: stop database</strong> 

To start a user-specified database use
```cmd 
C:\>staradmin --database=UserDbName stop db
```

##### Object types

The stop command supports stopping the following types.

* **Databases**. Usage:  `staradmin stop db`. Stops the default database, including all its support processes, effectively freeing all database memory.
* **Applications**. Usage:  `staradmin stop app [name]`. Stops an application by name. The `-d` option can be used to tell staradmin in what database look for it; if not given, the default database is assumed.
* **Hosts**. Usage:  `staradmin stop host`. Stops the code host of the default database. Support processes stay resident, meaning that database memory is not freed.

## Unload command

The **unload** command unloads data from a data source, usually a database. Usage:

```nginx
staradmin unload [source] [--file=<path>]
```

If no *source* is given, `db` is used as the default.

##### Object types

- **Databases**. Usage: `staradmin unload db`. Unloads a database.

##### Command options

The unload command supports the `--file=<path>` option. The filename is resolved to the same directory from which the command runs. If the file option is omitted, the default file is used.

```nginx
staradmin -d=bar unload db --file=data.sql
``` 

*Unloads the "bar" database into the "data.sql" file.*

The `--allowPartial` option unloads the database, allowing the unload to be partial. Usage: `staradmin unload db --allowPartial`.

The `--shiftKey` option makes every key being unloaded increase with the given number. 

```nginx
staradmin unload db --shiftKey=99999
``` 

*Makes every key being unloaded increase by 99999.*