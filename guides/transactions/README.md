# Transactions

## Introduction

Starcounter uses transactions to ensure [ACID](http://en.wikipedia.org/wiki/ACID) compliance. This page describes how it works in theory, while [Short-Running Transactions](short-running-transactions.md) and [Long Running Transactions](long-running-transactions.md) explains how to use transactions.

## Achieving ACID Compliance

### Atomicity

As defined on [Wikipedia](https://en.wikipedia.org/wiki/Atomicity_%28database_systems%29), an atomic transaction "is an indivisible and irreducible series of database operations such that either all occur, or nothing occurs." Starcounter ensures atomicity by wrapping changes of one transaction within a transaction scope. The changes commit simultaneously at the end of the scope. If something interrupts the transaction before the end of the scope is reached, none of the changes will commit to the database.

### Consistency

A consistent DBMS ensures that all the data written to the database follow the defined contraints of the database. In Starcounter, this is solved by raising exceptions when an "illegal" action is carried out within a transaction, such as commiting non-unique values to a field that requires unique values. The exception will in turn make the transaction roll back so that none of the changes are applied.

### Isolation

To make transaction isolated, Starcounter uses [snapshot isolation](https://en.wikipedia.org/wiki/Snapshot_isolation). This means that when a transaction initializes, it takes a snapshot of the database and stores it in a transactional memory. This means that every transaction sees its own snapshot of the database. For example, a SQL query that executes before a transaction commits will not be able to see the changes made by the transaction because the changes are isolated to that transaction's snapshot of the database. This works no matter how large the database is.

### Durability

Durability ensures that commited transactions will survive permanently. Starcounter solves this by writing transactions to a transaction log after commits. Find more information about this at the [log retention](../working-with-starcounter/log-retention.md) page.

### Concurrency Control

When many users write to the database at the same time, the database engine must ensure that the data keeps consistent using atomicity and isolation. For example, if an account reads 100 and you want to update it to 110 and another transaction is simultaneously reading a 100 and wants to update it to 120. Should the result be 110, 120 or 130? To resolve this, the transaction must be able to handle conflicts. The easiest way to do this is to use locking. If you want your database engine to serve large numbers of users and transactions, locking is slow and expensive and can lead to [deadlocks](http://en.wikipedia.org/wiki/Deadlock). Locking is efficient when you almost expect a conflict, i.e. when the probability is high that you will have a conflict. The slow nature of locking is that it always consumes time, even if there is no conflict. Another word for locking is 'pessimistic concurrency control'. A more efficient way of providing concurrency is to use 'optimistic concurrency control'. As the name implies, you don't expect a conflict, but you will still handle it. The concurrency control in Starcounter is **optimistic concurrency control**. It makes the assumption that conflicts between transactions are unlikely. The database can allow transactions to execute without locking the modified objects. If a conflict occurs, Starcounter will restart the transaction until it commits, or 100 times. For long-running transactions, the developer has to implement the retry functionality.

