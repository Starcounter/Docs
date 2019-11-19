# Database files

Starcounter database consists of the following items:

| Name                                  | Number | Description                      | Required | Platform Specific |
|---------------------------------------|--------|----------------------------------|----------|-------------------|
| `📁 archive`                          | 0 - 1  | Archived transaction log files.  | No       | No                |
| `📜 DatabaseName.000000000000.log`    | 0 - ∞  | Transaction log file.            | Yes      | No                |
| `📜 DatabaseName.000000000000.optlog` | 0 - ∞  | Compressed transaction log file. | Yes      | No                |
| `📜 DatabaseName.cfg`                 | 1      | Database configuration file.     | Yes      | Yes               |
| `📜 starcounter.000000000000.log`     | 0 - ∞  | Database events log.             | No       | No                |

- **Required**: specifies whether this item is a required database part; for example, when moving or backing up a database.
- **Platform Specific**: `No` - the file is platform independent; `Yes` - the file is platform specific and not cross-compatible between Windows / Linux versions of Starcounter.

## Transaction log files

The very first database transaction log file is created during the first database startup.
A created, but never started database does not contain any transaction log files.

When database grows, Starcounter:

- Creates new transaction log files.
- Compresses existing transaction log files into `.optlog` files.
- Moves compressed transaction log files into the `archive` folder.

***Note**: due to the current limitations, it is important to start the database at least once before using it from multiple different processes.*

## Database configuration file

This file contains all quired information about the database.

## Database events log

Starcounter logs database events into this file. Sample content:

```
20191118T090843 Debug DatabaseName Starcounter - Database load started.
20191118T090843 Debug DatabaseName Starcounter - Database load completed. 0 transactions recovered.
```