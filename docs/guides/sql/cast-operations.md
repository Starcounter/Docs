# Cast operation

In some path expressions you need to cast the type of a property/column.

Assume Employee is a subtype of Person, the property Father is of type Person and is defined in Person, the property Manager is of type Employee and is defined in Employee, and you want to select the manager of each person's father whenever such manager exists.

```
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

In this case the query below would be incorrect because Father is of type Person and this type have no property called Manager.

```
/* incorrect */
SELECT p.Father.Manager FROM Person p
```

However, if you cast Father to type Employee then you can continue the path expression with Manager. See example below.

```
/* correct */
SELECT CAST(p.Father AS Employee).Manager FROM Person p
```

If the object reference Father for some objects in the extent Person is not of type \(or a subtype of\) Employee then this object reference cannot be cast to Employee. In such cases the cast operation returns null and consequently the complete path expression also returns null.

Currently, the cast operation only supports casts between different types of database objects, which are of type Entity. The cast operation does not support type conversion between different value types.

