# Database

In Starcounter, classes are tables and class instances are rows. The database objects live in the database from the beginning. This means that they are not serialized to the database, they are _created_ in the database from the time you use the <code>new</code> operator. SQL queries will immediately see them. There is no concept of moving data to and from the database. This means that accessing a property on a database object (e.g <code>myPerson.FirstName</code>) reads the value from the database rather than from the normal .NET heap. This is possible as the data of the database lives in the RAM.
Read more in [Creating database classes](/guides/database/creating-database-classes) and [Data manipulation](/guides/database/data-manipulation).

## Object identity and object references
Starcounter is a database that offers relational access, graph access, object oriented access and document access all rolled into one. We recommend using [object references](/guides/database/object-identity-and-object-references/) (implicit keys) rather than primary keys and foreign keys (explicit keys) as object references.

## SQL
Each class marked with the `[Database]` attribute or inheriting a class marked with the `[Database]` attribute are available to the [SQL query language](/guides/database/querying-using-sql). There is no [ORM](https://en.wikipedia.org/wiki/Object-relational_mapping) mapping needed as classes and tables are one and the same in Starcounter.

## Relations
Using object references in your code instead of foreign keys, it is easier than ever to create [relations](/guides/database/relations) between objects.

## Inheritance
Starcounter allows any database object to [inherit from any other database object](/guides/database/inheritance).

{% import "../../macros.html" as macros %}

{{ macros.tocGenerator(page.title, summary.parts[0].articles[3].articles[0].articles) }}
