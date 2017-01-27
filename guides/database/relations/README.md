# Relations

Starcounter is a database that offers relational access, graph access, object oriented access and document access all rolled into one. We feel that core of the database builds well on the [theories of Dr.Codd](https://www.seas.upenn.edu/~zives/03f/cis550/codd.pdf). We have chosen to premiere implicit primary keys and foreign keys as object references.

You can make a traditional SQL query like this (given that you have a PersonId property):

```sql
SELECT FirstName, LastName, Text FROM Person, Quote WHERE Quote.PersonId = Person.PersonId
```

But, in Starcounter, you would rather use object types and paths and do:
```sql
SELECT Person.FirstName, Person.LastName, Text FROM Quote
```
or

```sql
SELECT FistName, LastName, Text FROM Person, Quote WHERE Quote.Person = Person
```