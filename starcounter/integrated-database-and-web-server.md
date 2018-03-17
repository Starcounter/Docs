# Integrated Database and Web Server

Let's deepen into technical details in order to understand how two layers, application and database, are physically merged into one.  
We can say that database and application server are represented by two parallel running processes. In the picture they are presented as ScDATA and ScCODE respectively. Inbound and outbound traffic towards those processes is initially handled by Gateway process.

![](http://starcounter.io/wp-content/uploads/2016/06/web-DB-explanation.gif)

Distinction of depicted processes:

1. **ScNETWORKGATEWAY** represents a Gateway for network traffic, delivering requests to execute and sending responses back \(read more [here](../guides/network/network-gateway.md)\);
2. **ScCODE** represents Codehost as a run-space for all the applications operating on the same Database;
3. **ScDATA** represents Database and manages database memory handling for Codehost activity.

Communication between processes is organized through the shared memory. This allows processes to efficiently exchange messages, preserving processes isolation, security and consistency.

## Codehost isolation

Gateway and Codehost processes operate on one shared memory segment, while Database and Codehost on another, meaning that Gateway has no direct access to Database process - it can only communicate with the Codehost.

This is done for multiple reasons:

* Failure/restart of the Codehost process does not affect the Database.
* Developers can iterate application versions and update those without Database process restart \(only Codehost is restarted and being reconnected to Database\);
* Database is isolated from networking process therefore it is impossible to affect the Database directly through Gateway.
* In the future, each application will have their own Codehost processes to ensure app independence and overall system stability.

