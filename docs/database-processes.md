# Database processes

## The `scdata` process

This is the main database process.

A database can be accessed by only one `scdata` process at a time. Attempt to start two or more `scdata` processes for the same database will fail with an exception.

```
failed to lock storage
Unhandled exception. System.Exception: ScErrCantStartDatabase (SCERR10004): Attempting to start the database failed. Failed to start ConsoleApp. Data process reported ScErrDbAlreadyStarted (SCERR2108): A Starcounter database with the specified name is already started.
```

Starcounter tooling or management components control `scdata` processes and their lifetimes. Under normal circumstances no manual interaction with the `scdata` process shall be required from Starcounter end users.

### Deciding if a database process is started

When receiving a request to to start a database, Starcounter starts the corresponding database process if it's not already running. A client, such as an application asking to connect to Starcounter, specifies the path to the database directory, and Starcounter combines this with an internal, opaque database engine version number to determine if the process is running.

Let's look at two examples:

An application connects to Starcounter:

- During startup, the application requests a connection to the database `mydb`, located in the root folder. This is indicated by the `Database=./mydb` segment in the connection string.
- Starcounter checks the internal version of Starcounter binaries referenced by the **application**, e.g. "3.2.1".
- Starcounter combines the two above values to form a unique database identity, and uses that to check if the database process is running.

Starcounter tool starts a database:

- The Starcounter command line interface (CLI) is used to start the database `mydb`. This can be done with the command `dotnet star start ./mydb`.
- Starcounter checks the internal version of Starcounter binaries referenced by the **tool**, e.g. "3.2.1".
- Starcounter combines the two above values to form a unique database identity, and uses that to check if the database process is running.

If two requests come to start the same database (same path), but _versions mismatch_, the second request will fail, saying that the database is already started (but with another, incompatible version of the Starcounter binaries).

The easiest way to assure compatibility is to use the same version of the tool as the version you use for your application.

When a database fails to start, and you suspect that a versioning mismatch may be the problem, you can check the version used. The procedure is similar for both Starcounter tooling and any application referencing `Starcounter.Database`.

1. Find the folder from where the tool or application execute, e.g. `bin\Release`.
2. There should be a `runtimes\%OS%\native` subfolder in there, where `%OS` would be the platform you are on.
3. Check the content of the `version.inc` file in that folder.

## The `scdblog` process

This process is responsible for writing, compressing, and archiving database transaction log files. No manual interaction is required with this process.
