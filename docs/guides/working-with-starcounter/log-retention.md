# Log files

### Usage

Starcounter persists user data on the disk in the form of transaction log files. This ensures data durability and allows to reload data on database restart. The disk can contain many log files. Each transaction log file is 256Mb in size. Starcounter writes transaction data to the current log file until it is full and then creates and switches to a new one. The format of transaction log files aims mainly to allow high throughput of transaction data rather than to save disk space or to reload the database quickly. To overcome it, optimized log files are introduced. Optimized log files are created in the background. It combines data from any number of transaction log files and stores it in a compact and read efficient format. Effectively, it’s the image of the database at a specific point of time. To reload database using a optimized log, Starcounter needs an optimized log file and all transaction log files that comes after it. Should more than one optimized log file exists on disk, Starcounter performs a database reload using the most recent one. Thus, all optimized log files except the most recent one are no longer required for database reload. As well as all transaction log files that were compacted. But they may be necessary for other tasks, such as data replication.

### Replication

Starcounter offers out-of-the-box log based replication facility. Here we only the relation between replication and transaction log. Transaction log files is the source information for replication. Replication processes transaction logs sequentially by sending appropriate transactions to the replica. Depending on replication performance and replication scenario, it may require more transaction log files to be in place in addition to those that required for database reload. An important note here is that a single transaction log file itself doesn’t provide enough information for replication. Logically, replication requires to reconstruct the entire database state up to the moment of transaction being replicated. To achieve it, replication requires the presence of a particular transaction log file that contains the required transaction and all the previous database states, either in the form of transaction or optimized log files.

### Finding the log files

The path to the folder where logs are placed is stored in database configuration file at: `<my documents>\Starcounter\Personal\Databases\[DatabaseName]\[DatabaseName].db.config` in the text node `/Database/Runtime/TransactionLogDirectory`. More information about this can be found in the page [Configuration Structure](configuration-structure.md).

### Default handling

Log files \(both optimized and transaction\) can be divided into two classes: active \(that are currently being written and required for database reload\) and historical. Proper keeping of active logs is the most crucial part as it requires permanently available disk space for continuous database operations and efficient hardware to maximize database throughput. On the other hand historical log files doesn’t have much impact on database operations, but storing as much historical log files as possible is invaluable to provide flawless replication as well as for database debugging in case of support issue.  
To handle it, Starcounter keeps logs in two storages:

* Active storage – disk space for active logs, where active transaction logs are written and optimized logs are created. Active storage is located in the root of `TransactionLogDirectory`
* Historical storage – disk space for historical logs, where we keep log files for any purpose to save up disk space in the main storage. Historical storage is located in subfolder “archive” of active storage. The Database Administrator is able to create this subfolder as a link to a different volume thus redirecting historical logs on another drive, presumable with more capacity but less throughput as an active storage drive.

Starcounter provides default log retention policy following which it moves logs from active to historical storage and finally wipes them out. Policy is enforced by running the command script: `%StarcounterBin%\on_new_log.cmd`. The script is launched automatically every time Starcounter starts writing a new transaction log file.  
The script performs the following tasks:  
1.    Creates an optimized log file based on completed transaction log files and places it in active storage.  
2.    Moves log files that have become historical to historical storage. All moved files are compacted using built-in ntfs compression.  
3.    Scans historical storage and removes log files that are older than specified by retention period setting.

Predefined retention period for historical logs is 30 days and is subject to change via editing of `%StarcounterBin%\on_new_log.cmd` file. To change it, find the line `set retention_period=30` and change 30 to the desired amount of days.

