# Identifiers

An identifier \(class/table name, property/column name etc.\) in Starcounter SQL is a sequence of the following characters:

* lower case letters \[ a - z \],
* upper case letters \[ A - Z \],
* digits \[ 0 - 9 \] or
* underscore \[ \_ \],

where the first character may not be a digit \[ 0 - 9 \].

Note that Starcounter SQL is case insensitive.  This means that identifiers which are equal except for different cases of the letters are regarded to be the same identifier. Consequently you cannot have different identifiers in your database schema with the same name but different cases.

Starcounter automatically creates a database schema from the class definitions in the application program code. Thus you could have several classes/tables with the same name but in different namespaces. As a consequence, in Starcounter SQL you qualify a class/table name by specifying its namespace, not by specifying a database name or schema name. See example query below.

```sql
SELECT m.MyProperty FROM MyCompany.MyApplication.MyModule.MyClass m
```

As long as the class/table name is unique you do not have to specify the namespace. See example query below. However, in SQL statements in programming code we strongly recommend you to qualify all class/table names so the SQL statements will be guaranteed to still be valid when you add new classes/tables.

```sql
SELECT m.MyProperty FROM MyClass m
```

If you have some identifier in your database schema that conflicts with some  
[reserved words](reserved-words.md) in Starcounter SQL, you can tell the SQL parser that the term should not be interpreted as the reserved word by putting it inside double quotes, as in query below.

```sql
SELECT n."Left", n."Right" FROM Node n
```

