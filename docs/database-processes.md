# Database processes

## The `scdata` process

This is the main database process.

A database can be accessed by only one `scdata` process at a time.
Attempt to start two or more `scdata` processes for the same database will fail with an exception.

## Database identification

Starcounter database is identified by it's physical location and Starcounter version which attempts to access it.

For example, attempting to start a database located at `/home/databases/TestDatabase` with Starcounter version `3.0.0-00001` do the following:

- Calculate database identification: `Starcounter-3.0.0-00001-/home/databases/TestDatabase`.
- Check if there is a running `scdata` process associated with this database identification.
- If no: spin up a new `scdata` process. This will fail if there is another version of `scdata` process is already running for this database.
- Communicate with the `scdata` process.

**Notes:**

Database path is case sensitive even on the case insensitive operating systems (Windows).
Attempt to establish multiple connections to the same database using different path casing `C:\Databases\TestDatabase` and `c:\databases\testdatabase` will fail.

Starcounter identifies database by the provided path, not by its physical location.
Attempt to establish multiple connections to the same database using different paths will fail.

### Windows

It is not possible to delete database files while `scdata` process is running.

### Linux

It is possible to delete database files while `scdata` is running, which may lead to a trap.

Consider the following scenario:

- A database is created at `/home/database/TestDatabase`.
- An `scdata` process is started for the database.
- The database files are deleted.
- New database is created in exactly the same location.
- An app tries to connect to the new database.
- Starcounter detects, that there is a running `scdata` process associated with this database path.
- The app connects to the existing `scdata` process, which still operates on the deleted files.

## The `scdblog` process

This process is responsible for writing, compressing, and archiving database transaction log files.
No manual interaction is required with this process.