# Timestamp on Object Creation

Sometimes, it may be useful to have a timestamp of when a database object is created. This can be done using an abstract class with a constructor that sets the timestamp. 

In code, it would look something like this:

```cs
[Database]
public abstract class BaseOfAllOtherClasses 
{
    public DateTime Inserted { get; set; }

    public BaseOfAllOtherClasses() 
    {
        Inserted = DateTime.Now;
    }
}

public class Foo : BaseOfAllOtherClasses {}
public class Bar : BaseOfAllOtherClasses {}
```

From this, it is possible to get when all the instances were created chronologically by using the following query:

```sql
SELECT b.Inserted FROM BaseOfAllOtherClasses e ORDER BY e.Inserted
```