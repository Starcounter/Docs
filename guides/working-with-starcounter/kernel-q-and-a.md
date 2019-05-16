# Kernel Questions and Answers

## Introduction

We often get questions about how the Starcounter kernel works. Here are the answers to the most common questions.

## Transactions

### What contributes to the transaction size?

Any data modification an app makes inside a transaction contributes to the transaction size. The contribution is relatively small for record insertion and removal. For updates, it grows linearly with the size of the fields being updated. If the modified data uses indexes, it will further contribute to the transaction size.

### How large should transactions be?

Generally, it's hard to say what transaction size or how many operations is feasible to make in a transaction. There are a couple of factors to keep in mind. Every transaction has some overhead. Due to this, larger transactions reduces the amount of overhead per change. On the other hand, large transactions may lead to inefficient resource usage of CPU cache and physical memory which can cause more cache misses or page faults from outgrown physical memory. Large transactions are also more likely to face conflict resolution. This increases inefficiency even more due to possible rollbacks. In practice, the optimal transaction size should be chosen empirically based on the nature of the transactions. For example, with lower conflict probablity, larger transactions can be used.

### What is the maximum size for a single transaction?

If a large transaction is inevitable, there are certain aspects the user should be aware of. While processing a transaction, the database engine uses several buffers that have hard limits. Once a limit is reached, the transaction will most likely fail with the error code `SCERRMEMORYLIMITREACHEDABORT`. If this limit cannot be reached due to inefficient system resources, the transactions will, in most cases, fail with either `SCERROUTOFMEMORY` or `SCERROUTOFMEMORYABORT`. There is no way for the app to determine if a particular transcation will fit under the limits based solely on the size of the user data. Although, these limits are quite high and thus hard to reach in realistic and practical scenarios. Most large transactions, like creating millions of small records or updating binary fields in serveral thousands of records where the total size of those fields is about 4GB, should fit under the limits.   
If there is a need for transactions that reach over these limits, the app should break these transactions into several smaller transactions and maintain its own compensation and isolation mechanism.

### What transactions are asynchronous?

Under the hood, all write transactions are asynchronous. It means that app is free to continue with other work while the database engine processes the commit. Asynchronous transactions are marked as completed once the data is secured on the disk. The host gives the user's app means to schedule an activity upon transaction completion. Db.Transact is not a special version of transaction, it is instead a wrapper on top of an asynchronous transaction that synchronously waits for transaction completion. The same applies to Transaction.Scope API.

### How does group commits work?

Due to the latency of external storage, securing every transaction independently would be very inefficient in terms of overall transaction rate. Therefore, the database engine aspires to do group writes to the transaction log to spread the latency among a number of transactions. This is done by collecting the transaction log entries until they exceed 64K in total or a small timeout expires. The timeout is about tens of milliseconds. These parameters cannot be tuned.

## Logging

### How does logging work?

A description of logging and anwers to questions like "what is log and logopt?", "when and how are logs converted into logopt?", and "what is the way to tweak the parameters of logging?" can be found on the [Log Retention page](log-retention.md).

### How is the transaction log data secured?

The database engine uses every available measure to assure that the transaction log data is actually secured on the media before it reports its success back to the app. It opens the transaction log file in the mode that informs the OS to instruct the drive not to use write-back cache. Eventually, it's always a matter of the system administrator ensuring this. See the following links for more relevant information:

* [Ensuring data reaches disk](https://lwn.net/Articles/457667/)
* [Flushing your performance down the drain, that is](https://blogs.msdn.microsoft.com/oldnewthing/20100909-00/?p=12913)
* [Dangerous setting is dangerous: This is why you shouldn’t turn off write cache buffer flushing](https://blogs.msdn.microsoft.com/oldnewthing/20130416-00/?p=4643)

It's worth noting that the transaction log will not be corrupted. For example, if some historical transaction was visible after a database startup, there is no chance that this transaction will disappear from the transaction log unless the media is physically corrupted. Though, it may happen that with inappropriate system configurations that some transaction that was reported as successful during the current session will not be visible upon recovery from an unexpected system failure.

### Can certain hardware increase the durability of the transaction log?

In Starcounter, there are no durability tradeoffs. so the hardware used does not impact the durability of the transaction log. As mentioned above, a transaction is reported as committed only when it actually has been written to the media.

### How is transaction log consistency maintained?

Transaction log consistency is maintained in two different ways:

1. The transaction log structure supports checksums and ignores half-written transactions during recovery. 
2. The fact that the drive physically writes data in chunks called sectors. Starcounter never performs writes that could overlap with a sector already has been written. Thus, there is no chance to corrupt transactions that are already in the transaction log.

## Purging

### How does purging work for in-memory data?

Starcounter can keep several versions of the same record in memory simultaneously to support transaction isolation. A removed record is purged in-memory when no transaction can see that record anymore. Starcounter determines when to run these purges. Apps cannot interfere in this process.

### How does purging work for the transaction log?

For the transaction log, log optimization can be considered to provide garbage collection. It works by only copying records that still exist in the database at the moment of optimization. Deleted records can be reached by analyzing the transaction log using a special API until that portion of the transaction log finally gets removed.

## RAM

### How is memory allocated?

Starcounter limits the maximum database size to the amount of installed physical memory, as long as it is over 16Gb. Starcounter does not preallocate all that memory, instead, it allocates it gradually in chunks on-demand. The size of the chunks are 1/64 of maximum database size. Thus, the minimum amount of preallocated memory is 256Mb.

Starcounter doesn't try to pin database memory to the physical memory. It instead relies on the virtual memory management of the OS. This may involve paging out part of the database memory. Paging out may become a performance problem if a system is short on physical memory. The database administrator should pay attention to the page fault ratio to diagnose such problems.

### What happens to the current database when the physical amount of RAM or the pagefile is changed?

| Change | Consequence |
| :--- | :--- |
| Increasing RAM | The page fault rate should decrease which will improve the overall performance. The database limit will grow. It's likely that the chunk size will increase, so the database preallocates more memory. More prealocated memory does not have any real drawback, since unused memory does not consume physical memory. It may also be possible to start more database instances than previously without poor performance originating from high page faults. |
| Decreasing RAM | The page fault rate will probably increase, decreasing the performance of your database. Additionally, starting existing databases may fail if they do not fit the new database limit. |
| Increasing pagefile | More applications may be started \(including starcounter hosted databases\). Whether starting new database makes sense depends on observed performance and reported page faults rate. |
| Decreasing pagefile | Out of memory errors may be hit if the applications \(including starcounter hosted databases\) demand more memory than was specified. |

## Other

### Are hot backups safe?

No, it is generally not safe to do backups on active databases unless certain procedures are followed. These procedures and more can be found in [Making Database Backups](run-starcounter-in-production.md#backup-and-failover).

### What happens when the power goes off?

As stated in the answer regarding securing transaction log data, none of the successfully committed transactions would be lost during a power outage.

### What is the expected startup time "on cold" from disk?

It involves many factors: storage performance, database size, data schema \(declared indexes\), CPU performance, activity from other apps, and more. The best way to figure out is by measuring it in the relevant environment.

