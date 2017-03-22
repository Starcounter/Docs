# Transactions

The secret to the speed of Starcounter lies in the fact that all database objects _live_ in the database all the time rather than being moved to and from the database.

Starcounter is fully [ACID](http://en.wikipedia.org/wiki/ACID) compliant and consequently transactions are consistent, isolated and atomic. But if our database objects are in the database at all times, how do we prevent other users and transactions to see our unfinished changes?  Starcounter solves this using transactional memory, such that each transaction sees its private snapshot of the database. This means that code inside and outside a transaction scope will potentially read a different value for the same property.

This works even if the database is in the terabytes. This means that nobody sees what you are doing while you are in your own transactional scope. You can even use SQL to query your database snapshot within the transaction while outside transactions will not be able to see your changes until they are done.

{% import "../../macros.html" as macros %}

{{ macros.tocGenerator(page.title, summary.parts[0].articles[3].articles[2].articles) }}
