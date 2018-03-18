# Identifiers

## Introduction

Identifiers are used to select classes and properties in queries.

An identifier of a class or property name in Starcounter SQL is a sequence of upper and lower case letters between A and Z, digits, and underscores where the first character can't be a digit.

Querying a database class or property that doesn't follow these restrictions will throw a `SqlException`:

```csharp
[Database]
public class Päron {}

Db.SQL("SELECT p FROM Päron p"); // SqlException
```

## Casing

Since Starcounter SQL is case insensitive, identifiers that are equal except for different casing are regarded to be the same identifier. Starcounter will throw `ScErrTypeNameDuplicate (SCERR4078)` if it finds two database classes where only the casing differs and `ScErrFieldsDifferInCaseOnly (SCERR4261)` if there are two fields or properties in a database where only the casing is different.

## Qualifying classes in different namespaces

Starcounter automatically creates a database schema from the class definitions in the application program code. Thus, you could have several classes with the same name but in different namespaces. As a consequence, in Starcounter SQL you qualify a class name by specifying its namespace, not by specifying a database name or schema name. See example query below.

```sql
SELECT m.MyProperty FROM MyCompany.MyApplication.MyModule.MyClass m
```

As long as the class name is unique you do not have to specify the namespace. See example query below. However, in SQL statements in programming code we strongly recommend you to qualify all class names so the SQL statements will be guaranteed to still be valid when you add new classes.

```sql
SELECT m.MyProperty FROM MyClass m
```

## Reserved words

If you have some identifier in your database schema that conflicts with some  
[reserved words](reserved-words.md) in Starcounter SQL, you can tell the SQL parser that the term should not be interpreted as the reserved word by putting it inside double quotes, as in query below.

```sql
SELECT n."Left", n."Right" FROM Node n
```



