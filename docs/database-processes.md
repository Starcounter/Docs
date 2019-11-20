# Database processes

## The `scdata` process

This is the main database process.

A database can be accessed by only one `scdata` process at a time.
Attempt to start two or more `scdata` processes for the same database will fail with an exception.

```
failed to lock storage
Unhandled exception. System.Exception: ScErrCantStartDatabase (SCERR10004): Attempting to start the database failed. Failed to start ConsoleApp. Data process reported ScErrDbAlreadyStarted (SCERR2108): A Starcounter database with the specified name is already started.
```

## The `scdblog` process

This process is responsible for writing, compressing, and archiving database transaction log files.
No manual interaction is required with this process.
