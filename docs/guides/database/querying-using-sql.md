# Querying using SQL

There is no [ORM](http://en.wikipedia.org/wiki/Object-relational_mapping) mapping needed as classes and tables are one and the same in Starcounter.

SQL queries are executed using the `Db.SQL` function. If the SQL command is `SELECT`, the function returns `Starcounter.QueryResultRows<T> : IEnumerable<T>`, it otherwise returns `null`.

```
Db.SQL("SELECT p FROM Person p"); // --> QueryResultRows<Person>
Db.SQL("DELETE FROM Person"); // --> null
```

`T` in `QueryResultRows<T>` is the type of the object retrieved if the whole object is retrieved, otherwise, it's `Starcounter.Query.Execution.Row`.

```
Db.SQL("SELECT p FROM Person p"); // --> QueryResultRows<Person>
Db.SQL("SELECT p.Name FROM Person p"); // --> QueryResultRows<Query.Execution.Row>
```

We recommend avoiding `Starcounter.Query.Execution.Row` when possible and instead retrieve the whole object and filter out the needed properties with Linq.

```
Db.SQL("SELECT p.Name FROM Person p"); // Not recommended
Db.SQL("SELECT p FROM Person p").Select(p => new { p.Name }); // Recommended
```

To get the first object in the enumeration, you can use the property `First`. In addition to traditional SQL, Starcounter allows you to select objects in addition to primitive types such as strings and numbers. Also it allows you to use C\# style path expressions such as `person.FullName`.

When writing queries by using `Db.SQL`, keep in mind that there are [certain reserved words](../sql/reserved-words.md) that should be escaped. That is done by surrounding the reserved word in quotation marks.

See more about SQL in [Guides: SQL](../sql/).

```
using Starcounter;

namespace Querying
{
    //Define the database class
    [Database]
    public class Person
    {
        public string Name { get; set; }
    }

    class Program
    {
        static void Main()
        {
           //Add a Person to the database
           Db.Transact(() =>
           {
               new Person()
               {
                   Name = "John Doe"
               };
            });

            Handle.GET("/querying", () =>
            {
                //Query for the first Person in the enumerable and return its name
                var person = Db.SQL<Person>("SELECT p FROM Person p").First;
                return "<h1>" + person.Name + "</h1>";
             });
         }
     }
}
```

Run this application by pressing f5 and go to `localhost:8080/querying`. "John Doe" should be displayed in the browser.

**Disclaimer**  
This code is purely for example and should not be used for practical purposes because JavaScript can be stored in `person.Name` and cause security issues.

### Strings vs Linq

For the C\# savvy user, it might be surprising that Starcounter uses strings to execute SQL queries. The reason for this is performance. The SQL statement in the string will only be evaluated once. In subsequent calls to the same query, a precompiled SQL query will be executed. If we used Linq, we would have to evaluate more managed code each time the query was run. As the string uses the `?` wherever a dynamic parameter is used, it can easily be used as a key to find a reference to a single function containing the code to execute the query. This is important as the DBMS will not move any data to execute the query. The data is already in primary memory and will not be moved to the C\# heap as the C\# heap for the database object is replaced with the database image memory. Execution overhead that would normally be considered insignificant in a traditional database would easily affect the throughput if you want to maximise Starcounter performance.

