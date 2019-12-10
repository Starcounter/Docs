# Database files

Starcounter database consists of the following items:

Name                                  | Number | Description                      | Required | Platform Specific
------------------------------------- | ------ | -------------------------------- | -------- | -----------------
`📁 archive`                          | 0 - 1  | Archived transaction log files.  | No       | No
`📜 DatabaseName.000000000000.log`    | 0 - ∞  | Transaction log file.            | Yes      | No
`📜 DatabaseName.000000000000.optlog` | 0 - ∞  | Compressed transaction log file. | Yes      | No
`📜 DatabaseName.cfg`                 | 1      | Database configuration file.     | Yes      | Yes
`📜 starcounter.000000000000.log`     | 0 - ∞  | Database events log.             | No       | No

- **Required**: specifies whether this item is a required database part; for example, when moving or backing up a database.
- **Platform Specific**: `No` - the file is platform independent; `Yes` - the file is platform specific and not cross-compatible between Windows / Linux versions of Starcounter.

***Note**: due to the `.cfg` file being platform specific, it is not currently possible to copy databases between Windows and Linux.*

# Transaction log files

The very first database transaction log file is created during the first database startup. A created, but never started database does not contain any transaction log files.

When database grows, Starcounter:

- Creates new transaction log files.
- Compresses existing transaction log files into `.optlog` files.
- Moves compressed transaction log files into the `archive` folder.

***Note**: due to the current limitations, it is important to start the database at least once before using it from multiple different processes.*

# Database configuration file

This file contains all required information about the database.

# Database events log

Starcounter logs database events into this file. Sample content:

```
20191118T090843 Debug DatabaseName Starcounter - Database load started.
20191118T090843 Debug DatabaseName Starcounter - Database load completed. 0 transactions recovered.
```

# Deleting database files while `scdata` is running

On Windows, it's not possible to delete database files while the `scdata` process is running. On Linux operating systems, however, it is, which may lead to potential issues. Consider the following scenario:

- A database is created at `/home/database/TestDatabase`.
- An `scdata` process is started for the database.
- The database files are deleted.
- New database is created in exactly the same location.
- An app tries to connect to the new database.
- Starcounter detects, that there is a running `scdata` process associated with this database path.
- The app connects to the existing `scdata` process, which still operates on the deleted files. The filesystem will keep those files around for the `scdata` process, as long as it's running.
- The app continues to run as expected, but the next time `scdata` is restarted, for example due to a system restart, the database files will be removed from the system completely, taking all the app's database data with them.
