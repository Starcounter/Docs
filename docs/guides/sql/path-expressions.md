# Path Expressions

## Introduction

In Starcounter SQL you can refer to any property of a database classes as long as there is a path from the database class specified in the `FROM` clause to that property.

A path expression is an arbitrary long sequence of identifiers separated by dots. The first identifier in the sequence should be an alias to an database class defined in the `FROM` clause of the `SELECT` statement. The following identifiers except the last one should have object references as return values. The last identifier may return any supported datatype. For example:

```sql
SELECT e.Manager.Department.Location.Street FROM Employee e
```

If an identifier in a path expression returns `null` then the complete path expression will return `null`.

Path expression can also be used in `WHERE` and `ORDER BY` clauses:

```sql
SELECT e FROM Employee e WHERE e.Manager.Department.Name = 'sales'
SELECT e FROM Employee e ORDER BY e.Manager.Department.Profit
```

## Wildcards

You can use a wildcard \(\*\) to select all properties of a database class:

```sql
SELECT * FROM Employee
SELECT e.* FROM Employee e
```

The above queries return all the properties of the `Employee` objects, while the below query returns references to the `Employee` objects themselves.

```sql
SELECT e FROM Employee e
```

## Cast Operation

In some path expressions you need to cast the type of a property.

For example, consider a data model like this:

```csharp
[Database]
public class Person
{
  public Person Father { get; set; }
}

[Database]
public class Employee : Person
{
  public Employee Manager { get; set; }
}
```

 If you want to select the manager of each person's father whenever such manager exists, then this is incorrect  since `Father` is of type `Person` and `Person` has no `Manager` property:

{% code-tabs %}
{% code-tabs-item title="Incorrect" %}
```sql
SELECT p.Father.Manager FROM Person p
```
{% endcode-tabs-item %}
{% endcode-tabs %}

However, if you cast `Father` to type `Employee` then you can continue the path expression with `Manager`:

{% code-tabs %}
{% code-tabs-item title="Correct" %}
```sql
SELECT CAST(p.Father AS Employee).Manager FROM Person p
```
{% endcode-tabs-item %}
{% endcode-tabs %}

If the object reference `Father` for some objects in the extent `Person` is not of type, or subtype of `Employee` , then this object reference can't be cast to `Employee` and the operation returns `null`.

The cast operation only supports casts between different types of database objects and not between different value types.

