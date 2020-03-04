# Overview
Extensibility is about extending core features of the Starcounter database, using some suggested approaches available here as source code.

## Organisation
The [Starcounter.Database.Hosting.Extensibility](./Starcounter.Database.Hosting.Extensibility) folder contains sources that forms the foundation for the more specific samples.

### OnDelete
* [OnDelete](./OnDelete/README.md) suggest a way for any application to support an interface that can be applied to any database class and act as a callback when instances of that class is deleted from the database.

### Pre-commit hooks
* [PreCommitHooks](./PreCommitHooks/README.md) describe an approach where applications can register callbacks to be invoked when instances of a specified database type is either inserted or updated, just before a transaction is committed. 