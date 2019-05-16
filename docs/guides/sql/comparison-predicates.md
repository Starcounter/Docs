# Comparison predicates

The comparison predicates "equal" \(`x = y`\) and "not equal" \(`x <> y`\) are supported for all data types. See for example query below.

```
SELECT e FROM Employee e WHERE e.FirstName = 'Bob'
```

Note that all string comparisons are case insensitive. The query \(1\) is therefore equivalent to the query.

```
SELECT e FROM Employee e WHERE e.FirstName = 'bob'
```

The comparison predicates "less than" \(`x < y`\), "greater than" \(`x > y`\), "less than or equal" \(`x <= y`\) and "greater than or equal" \(`x >= y`\) are implemented for the data types `String`, `DateTime` and all numerical types. See example below.

```
SELECT e FROM Employee e WHERE e.LastName >= 'Smith'
```

Since a `DateTime` value represents a timestamp it is often necessary to compare it with a `DateTime` range. The query below returns all employees with a `HireDate` between `'2006-11-01 00:00:00.000'` and `'2006-11-01 23:59:59.999'`.

```
SELECT e.FirstName, e.HireDate FROM Employee e
  WHERE e.HireDate >= DATE '2006-11-01' AND e.HireDate < DATE '2006-11-02'
```

The comparison predicates "is null" \(`x IS NULL`\) and "is not null" \(`x IS NOT NULL`&gt;\) are implemented for all data types. See for example query below.

```
SELECT e FROM Employee e WHERE e.Manager IS NULL
```

The comparison predicate "like" \(`x LIKE y [ESCAPE z]`\) is implemented for the data type `String`. In the specified pattern \(`y`\) the underscore character \(`'_'`\) match any single character in the string, and the percent character \(`'%'`\) match any sequence \(possibly empty\) of characters in the string. See for example query below.

```
SELECT e FROM Employee e WHERE e.FirstName LIKE 'B_b%'
```

The optional third argument to the `LIKE` predicate is an "escape character", for use when a percent or underscore character is required in the pattern without its special meaning. This is exemplified in query below.

```
SELECT s FROM Share s WHERE s.Unit LIKE '\%' ESCAPE '\\'
```

### `IS` Operator

The comparison predicate `IS` checks if an object can be cast to a given type. It has similar semantics to [`Type.IsAssignableFrom`](https://msdn.microsoft.com/en-us/library/system.type.isassignablefrom.aspx). Consider, for example, the following code:

```
[Database]
public class Person
{
    public string FirstName { get; set; }
}

[Database]
public class Teenager : Person
{
}

[Database]
public class Child : Teenager
{
}

class Program
{
    static void Main()
    {
        Db.Transact(() =>
        {
            new Person { FirstName = "Bob" };
            new Teenager { FirstName = "Johnny" };
            new Child { FirstName = "Elsa" };
        });
    }
}
```

Here, the `IS` operator can be used to determine inheritance in the data model. For instance, to retrieve all instances that can be cast to `Teenager`, a query like this would be used:

```
SELECT p FROM Person p WHERE p IS Teenager
```

It returns the `Child` and `Teenager` instances "Johnny" and "Elsa" because they can both be cast as `Teenager`.

