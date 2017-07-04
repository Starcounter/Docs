# Cast operation

In some path expressions you need to cast the type of a property/column.

Assume <code>Employee</code> is a subtype of <code>Person</code>, the property <code>Father</code> is of type <code>Person</code> and is defined in <code>Person</code>, the property <code>Manager</code> is of type <code>Employee</code> and is defined in <code>Employee</code>, and you want to select the manager of each person's father whenever such manager exists.

```cs
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

In this case the query below would be incorrect because <code>Father</code> is of type <code>Person</code> and this type have no property called <code>Manager</code>.

```sql
/* incorrect */
SELECT p.Father.Manager FROM Person p
```

However, if you cast <code>Father</code> to type <code>Employee</code> then you can continue the path expression with <code>Manager</code>. See example below.

```sql
/* correct */
SELECT CAST(p.Father AS Employee).Manager FROM Person p
```

If the object reference <code>Father</code> for some objects in the extent <code>Person</code> is not of type (or a subtype of) <code>Employee</code> then this object reference cannot be cast to <code>Employee</code>. In such cases the cast operation returns <code>null</code> and consequently the complete path expression also returns <code>null</code>.

Currently, the cast operation only supports casts between different types of database objects, which are of type <code>Entity</code>. The cast operation does not support type conversion between different value types.
