# Database connection string

The database connection string is a character string that is used to define and configure the connection between an application and a Starcounter database. It consists of a number of option assignments, separated by semicolons (see [sample](#connection-string-sample)).

## Connection string options

Name         | Type      | Required | Default value       | Description
:----------- | :-------- | :------- | :------------------ | :-----------------------------------------------------------------------
Database     | `string`  | Yes      | None                | A path string defining the database to use.
OpenMode     | `enum`    | No       | `CreateIfNotExists` | Specifies the database creation strategy.
StartMode    | `enum`    | No       | `StartIfNotRunning` | Specifies the database startup strategy.
StopMode     | `enum`    | No       | `IfWeStarted`       | Specifies the database shut down strategy.
ContextCount | `integer` | No       | `2` – `24`          | Specifies the number of database contexts allocated for this connection.

The options are further explored in the sections below.

### Database

The Database option is required in the connection string, and consists of an absolute or relative path to the directory where the Starcounter database files are located, or should be located if they do not yet exist.

Internally, a Starcounter database is identified by it's physical location on disk and the version of Starcounter that is trying to access it. For example, attempting to start a database located at `/home/databases/TestDatabase` with Starcounter version `3.0.0-00001` will lead to this behavior:

- A database identification is calculated as `Starcounter-3.0.0-00001-/home/databases/TestDatabase`.
- If there is a running `scdata` process associated with this database identification, it is reused. Oterwise we spin up a new `scdata` process. This will fail if there is another version of `scdata` process already running for this database.
- Communication is set up with the `scdata` process.

_For more information, see the article about [database processes](database-processes.md)._

**Notes:**

The database path string is **case sensitive**, even on operating systems like Windows that use case insensitive file paths. Attempting to establish multiple connections to the same database using different path casing, like `C:\Databases\TestDatabase` and `c:\databases\testdatabase`, will fail.

Starcounter identifies database by the provided path, not by its physical location. Attempt to establish multiple connections to the same database using different paths will fail.

### OpenMode

The OpenMode option defines how to interpret the database path on connection. It can have the following string values:

Value               | Description
:------------------ | :--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
`Open`              | Opens the database directory from the path specified in the Database option, if it exist. If the directory does not exist, or if there is no Starcounter database located there, an exception is thrown.
`CreateIfNotExists` | Opens the database directory from the path specified in the Database option, if it exist. If the directory does not exist, or if there is no Starcounter database located there, it will create the necessary directories and database files.

### StartMode

The StartMode option defines how to start and/or connect to the database process once the database files have been located and/or created.

Value               | Description
:------------------ | :---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
`RequireStarted`    | Expects the database process to already be running and will connect to it if it is, or throw an exception if it is not.
`StartIfNotRunning` | Connects to the database process if it is already running, or starts and connects to it if it is not.
`StartExclusive`    | Expects no database process to be already running, and starts and connects to a new process if that is the case. If a database process is already running, an exception is thrown.

### StopMode

The StopMode option defines whether to stop the database process once the connection to it is closed, for example when an application is terminated.

Value         | Description
:------------ | :-----------------------------------------------------------------------------------
`IfWeStarted` | Stops the database process if and only if it was started by the current application.
`Never`       | Leaves the database process running, regardless of how it was started.

**Note:** the `Never` option is not effective when the application process is forcibly killed, for example from a task manager or by `Ctrl + C`. In this case operating system will kill the corresponding database process as well.

### ContextCount

The ContextCount option defines the number of **database contexts** to use in the database connection.

"Database context", as used here, should not be confused with `IDatabaseContext` as used in the .NET API for transactions. In the context of database connections, database contexts refer to the entrypoints to the database from the applications' point of view. When an application thread is assigned a database context, it can read and write to the database.

Each Starcounter database has a maximum of 31 database contexts available for allocation between multiple application processes. By default, each Starcounter application tries to allocate twice as many contexts as CPU cores available, with a minimum of 2 and a maximum of 24.

The ideal configuration is to have a database context per CPU core. Having more database contexts than CPU cores might increase performance in certain concurrent database access workloads. Having fewer database contexts than CPU cores might decrease performance of concurrent database access during high concurrent load.

Use `ContextCount` option to manually adjust the number of database contexts to occupy in this connection.

## Example

```text
Database=./.database/Sample;OpenMode=CreateIfNotExists;StartMode=StartIfNotRunning;StopMode=IfWeStarted;ContextCount=10
```

This string defines the Database option using a relative path to a database `Sample`, located in the `.database` directory of the current directory. If such a directory does not exist at the time of connection, or if it does not contain a Starcounter database, the OpenMode option of `CreateIfNotExists` ensures that it is created. If the database process is not running already, the StartMode option of `StartIfNotRunning` ensures that it should be started. And when the app disconnects from the database, it will stop the database process if it was started by this app, since the StopMode option is defined as `IfWeStarted`. Lastly, we set the number of occupied database contexts in this connection to 10.
