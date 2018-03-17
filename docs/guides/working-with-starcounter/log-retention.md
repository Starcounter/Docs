# Log files

## Introduction

Starcounter uses transaction logging to ensure durability. Transaction logging means that database operations are written to a log file when they are committed and are thus made persistent. By persisting database operations, it's possible to recreate the state of the database by going back in the log.

## Types of log files

Starcounter has two types of log files: read-optimized and write-optimized. Write-optimized log files are the files that Starcounter writes to when a transaction is committed. These files handle high throughput. Write-optimized files have a maximum size of 256 Mb. Starcounter writes transaction data to the current write-optimized log file until it is full and then switches to a new one. Read-optimized log files are more compact and faster to read from than write-optimized log files. A read-optimized log file is the image of the database at a specific point in time. The max size of a read-optimized file depends on the size of the database. Read-optimized log files contain data from multiple write-optimized log files.

To reload the database, Starcounter needs a read-optimized log file and all write-optimized log files that comes after it. If more than one read-optimized log file exists on disk, Starcounter performs a database reload with the most recent one. Thus, only the most recent read-optimized log file is required for database reload.

## Replication

Starcounter can replicate a database from the log files. Replication processes write-optimized logs sequentially by sending appropriate transactions to the replica. Depending on the replication performance and replication scenario, more write-optimized log files than those required for database reload might be needed. A single write-optimized log file doesn’t have enough information for replication. Logically, replication reconstructs the entire database state up to the moment of the replicated transaction. Replication requires log files that contain the replicated transaction and all the previous database states, either in the form of write-optimized or read-optimized log files.The path to the log directory is stored in database configuration file at `<my documents>\Starcounter\Personal\Databases\[DatabaseName]\[DatabaseName].db.config` in the text-node `/Database/Runtime/TransactionLogDirectory`.

## Active and historical log files

Log files can either be active or historical. Active log files are those that are currently written to or required for database reload. Starcounter can't carry out any database operations when there's no disk space left for active log files. Starcounter stores active logs in the active storage at the root of the \`TransactionLogDirectory\`. The active storage holds at least two write-optimized log files since Starcounter ensures that there's always one empty write-optimized log file. If a transaction spans over two write-optimized log files, Starcounter will keep both in active storage since they are both required for database reload. Also, if the process of converting write-optimized files to read-optimized files lags behind, an arbitrary number of write-optimized files are kept in active storage since they are required for database reload until Starcounter creates a read-optimized log file.

Historical log files don't impact database operations, but storing as many historical log files as possible is useful for replication as well as for database debugging. Starcounter stores historical logs in the historical storage. Files are moved to the historical storage to save space in the active storage. The historical storage is in the “archive” subfolder of the active storage. The Database Administrator can create this subfolder as a link to a different volume and thus allow storing historical logs on another drive, such as one with larger capacity but less throughput than the active storage drive.

The Starcounter log retention policy moves logs from active to historical storage when they are no longer needed for database reload and then deletes them after some days. By default, Starcounter deletes historical logs after 30 days. To change the default retention period, edit the `retention_period=30` line of the `%StarcounterBin%\on_new_log.cmd` file. Changing 30 to something else will keep the historical logs for that set amount of days and then delete them. The `%StarcounterBin%\on_new_log.cmd` script enforces the log retention policy. It's run every time Starcounter starts writing to a new transaction file because the current write-optimized was full. The script performs these steps:

1.  Creates a read-optimized log file in the active storage from full write-optimized log files.
2. Moves historical log files to historical storage and compresses them with NTFS compression.
3. Scans historical storage and removes log files that are older than the specified retention period.

In the first step of creating a read-optimized log file, Starcounter performs a number of operations to make the read-optimized file more compact:

* Removes mutually cancelled transactions. For example, transactions that add an entry and then delete the same entry.
* Combines, when beneficial, multiple transactions into a single entry.
* Moves all `CREATE INDEX` transactions in front of data creation.

## Manually trigger read-optimized write

You can manually trigger Starcounter to write to a read-optimized file. This can be useful when creating a large index on a database that is mostly read-only or when dropping a large index. These are the steps to manually write to a read-optimized file:

* Go to the [server repository](configuration-structure.md#established-directories-on-installation).
* Go to `<server repository>\Data\<database name>\<database name>-<identification>`, such as `C:\Users\User\Documents\Starcounter\Personal\Data\Default\Default-20180305T082236020`.
* Open the `<DATABASE NAME>.on_new_log.log` file. This file only exists if you already have one read-optimized file.
* Copy the line that starts with `logopt -make`, it looks like this:

```text
logopt -make "sc://desktop-ku8vj0p/personal/default" "C:\Users\User\Documents\Starcounter\Personal\Logs" "DEFAULT" "C:\Users\User\Documents\Starcounter\Personal\Data\Default\Default-20180305T082236020" 65535   || set error_source=Failed to optimize log 
```

* Remove everything after the path to the database:

```text
logopt -make "sc://desktop-ku8vj0p/personal/default" "C:\Users\User\Documents\Starcounter\Personal\Logs" "DEFAULT" "C:\Users\User\Documents\Starcounter\Personal\Data\Default\Default-20180305T082236020"
```

* Execute the command. It does not output anything when it's done.
* Starcounter has now created a read-optimized log file from the write-optimized ones.

{% hint style="info" %}
You can manually trigger a read-optimized write even when the database is active.
{% endhint %}

