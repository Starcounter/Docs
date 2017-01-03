# Relations

Starcounter is a database that offers relational access, graph access, object oriented access and document access all rolled into one. We feel that core of the database builds well on the theories of Dr.Codd. We have chosen to premiere implicit primary keys and foreign keys as object references.

You can make a traditional SQL query like this (given that you have a PersonId property)

```SQL
/* Not Starcounter */
SELECT FirstName, LastName, Text FROM Person, Quote 
  WHERE Quote.PersonId = Person.PersonId
```

But, in Starcounter, you would rather use object types and paths and do

```SQL
/* Starcounter */
SELECT Person.FirstName, Person.LastName, Text FROM Quote
```
or

```SQL
/* Starcounter */
SELECT FistName, LastName, Text FROM Person, Quote 
  WHERE Quote.Person = Person
```
