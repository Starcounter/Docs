# Database connection string

The database connection string is a character string that is used to define and configure the connection between a Starcounter application and its database. It consists of a number of option assignments, separated by semicolons (see [sample](#connection-string-sample)).

## Connection String options

Name         | Type     | Required | Default Value       | Description
:----------- | :------- | :------- | :------------------ | :-------------------------------------------------------------------
Database     | `string` | Yes      | None                | Absolute or relative database directory path.
OpenMode     | `enum`   | No       | `CreateIfNotExists` | Specifies database creation strategy.
StartMode    | `enum`   | No       | `StartIfNotRunning` | Specifies database startup strategy.
StopMode     | `enum`   | No       | `IfWeStarted`       | Specifies database shut down strategy.
ContextCount | `int`    | No       | 2 - 24              | Specifies number of database contexts allocated for this connection.

### The `Database` value

Starcounter database is identified by it's physical location and Starcounter version which attempts to access it.

For example, attempting to start a database located at `/home/databases/TestDatabase` with Starcounter version `3.0.0-00001` will do the following:

- Calculate database identification: `Starcounter-3.0.0-00001-/home/databases/TestDatabase`.
- Check if there is a running `scdata` process associated with this database identification.
- If no: spin up a new `scdata` process. This will fail if there is another version of `scdata` process already running for this database.
- Communicate with the `scdata` process.

_[Read more about Starcounter database processes](database-processes.md)._

**Notes:**

Database path is case sensitive even on the case insensitive operating systems (Windows). Attempt to establish multiple connections to the same database using different path casing, like `C:\Databases\TestDatabase` and `c:\databases\testdatabase`, will fail.

Starcounter identifies database by the provided path, not by its physical location. Attempt to establish multiple connections to the same database using different paths will fail.

#### Windows

It is not possible to delete database files while `scdata` process is running.

#### Linux

It is possible to delete database files while `scdata` is running, which may lead to a trap.

Consider the following scenario:

- A database is created at `/home/database/TestDatabase`.
- An `scdata` process is started for the database.
- The database files are deleted.
- New database is created in exactly the same location.
- An app tries to connect to the new database.
- Starcounter detects, that there is a running `scdata` process associated with this database path.
- The app connects to the existing `scdata` process, which still operates on the deleted files.

### The `DatabaseOpenMode` enumeration values

- `Open` - Opens the database if it exist.
- `CreateIfNotExists` - Opens the database if it exist, and creates one if it doesn't.

### The `DatabaseStartMode` enumeration values

- `RequireStarted` - Expects database processes to be running, and won't attempt starting them.
- `StartIfNotRunning` - Connects if database processes are already running, and start them first if they are not.
- `StartExclusive` - Expects no database processes to be running, and start them if they are not. Refuse to connect if database processes are already running.

### The `DatabaseStopMode` enumeration values

- `IfWeStarted` - Stops database processes if they where started by the current application.
- `Never` - Leaves database processes running, even after the application shuts down and even if the application started them.

_Note: the `Never` option is not effective when the application process is forcibly killed with task manager or `Ctrl + C`. In this case operating system will kill corresponding Starcounter processes as well._

### The `ContextCount` option

Starcounter has maximum of `31` database contexts available for allocation between multiple processes. By default, Starcounter application tries to allocate double as many contexts as CPU cores available, but minimum `2` and maximum `24`.

The ideal configuration is to have a database context per CPU core. Having more database contexts than CPU cores might increase performance in certain concurrent database access workloads. Having less database contexts than CPU cores might decrease performance of concurrent database access during high concurrent load.

Use `ContextCount` option to manually adjust database contexts allocation.

## Connection String sample

```text
Database=./.database/Sample;OpenMode=CreateIfNotExists;StartMode=StartIfNotRunning;StopMode=IfWeStarted;ContextCount=10
```
