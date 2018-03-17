# Database

In Starcounter, classes are tables and class instances are rows. The database objects live in the database from the beginning. This means that they are not serialized to the database, they are _created_ in the database from the time you use the new operator. SQL queries will immediately see them. There is no concept of moving data to and from the database. This means that accessing a property on a database object \(e.g myPerson.FirstName\) reads the value from the database rather than from the normal .NET heap. This is possible as the data of the database lives in the RAM.  
Read more in [Creating database classes](creating-database-classes.md) and [Data manipulation](data-manipulation.md).

### Object identity and object references

Starcounter is a database that offers relational access, graph access, object oriented access and document access all rolled into one. We recommend using [object references](object-identity-and-object-references.md) \(implicit keys\) rather than primary keys and foreign keys \(explicit keys\) as object references.

### SQL

Each class marked with the `[Database]` attribute or inheriting a class marked with the `[Database]` attribute are available to the [SQL query language](querying-using-sql.md). There is no [ORM](https://en.wikipedia.org/wiki/Object-relational_mapping) mapping needed as classes and tables are one and the same in Starcounter.

### Relations

Using object references in your code instead of foreign keys, it is easier than ever to create [relations](relations.md) between objects.

### Inheritance

Starcounter allows any database object to [inherit from any other database object](inheritance.md).





