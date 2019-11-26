# Database processes

## The `scdata` process

This is the main database process.

A database can be accessed by only one `scdata` process at a time.
Attempt to start two or more `scdata` processes for the same database will fail with an exception.

```
failed to lock storage
Unhandled exception. System.Exception: ScErrCantStartDatabase (SCERR10004): Attempting to start the database failed. Failed to start ConsoleApp. Data process reported ScErrDbAlreadyStarted (SCERR2108): A Starcounter database with the specified name is already started.
```

Starcounter tooling or management components control `scdata` processes and their lifetime. Under normal circumstances no manual interaction with the `scdata` process shall be required from Starcounter end users.

### Deciding if a database process is started

When a request to start a database comes, Starcounter starts corresponding database process if it's not running. A client, such as an application asking to connect to Starcounter, specifies the path to the database directory, and Starcounter combines this with an internal, opaque database engine version number to determine if the process is running.

Let's look at two examples:

An application connects to Starcounter:
* Application asks to connect to `./mydb`, i.e `Database=./mydb`.
* Starcounter checks the internal version of Starcounter binaries referenced by the **application**, e.g. "3.2.1".
* Starcounter combines the two above values to form a unique database identity, and use that to check if the database process is running.

Starcounter tool starts a database:
* Tooling is used to start the database i.e. `dotnet start start ./mydb`.
* Starcounter checks the internal version of Starcounter binaries referenced by the **tool**, e.g. "3.2.1".
* Starcounter combines the two above values to form a unique database identity, and use that to check if the database process is running.

If two requests come to start the same database (same path), but _versions mismatch_, the second request will fail, saying that the database is already started (but with another, incompatible version of Starcounter binaries). 

The easiest way to assure compatibility is use the same version of tooling and your application.

When a database fails to start, and you suspect that a versioning mismatch may be the problem, you can check the version used. The procedure is similar for both Starcounter tooling and any application referencing `Starcounter.Database`.

1. Find the folder from where the tool or application execute, e.g. `bin\Release`.
2. There should be a `runtimes\%OS%\native` subfolder in there, where `%OS` would be the platform you are on.
3. Check the content of the `version.inc` file in that folder.  

## The `scdblog` process

This process is responsible for writing, compressing, and archiving database transaction log files.
No manual interaction is required with this process.
