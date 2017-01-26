# SQL isolation between applications

Applications running in the same code-host are isolated on different levels:

- REST handlers and URIs namespace.
- SQL classes and objects.
- Static file resources.

The principle for SQL isolation is that database classes of one application should not be visible to database classes of another application running in the same code-host.

## Isolation example

For example, first application defines database class `App1Class` in its namespace:
```cs
// We are in first application.
namespace App1
{
    [Database]
    public class App1Class
    {
        public string App1ClassField;
    }
}
```
the same does the second application but in own namespace:

```cs
// We are in second application.
namespace App2
{
    [Database]
    public class App2Class
    {
        public string App2ClassField;
    }
}
```

First application now is able to access its own `App1Class` using full and short names:
```cs
// We are in first application.
var x = Db.SQL("SELECT c FROM App1.App1Class c").First;
var x2 = Db.SQL("SELECT c FROM App1Class c").First;
```
same does the second application with its own class `App2Class`.

However, the first application will not be able to retrieve classes from second application and vice versa:

```cs
// We are in first application.
// The following SQL query will throw an exception:
// "Failed to process query: SELECT c FROM App2Class
// c: Unknown class App2Class."
var x = Db.SQL("SELECT c FROM App2Class c").First;
```

Classes defined in private application references (for example, private libraries) are only accessible within the application that references them.

## Shared library

If first and second application are referencing the same library, for example "SharedDll", then both applications have access to classes and objects from this shared library, regardless which application created those objects:

```cs
// We are inside shared library.
namespace SharedDll
{
    [Database]
    public class SharedDllClass
    {
        public string SharedDllClassField;
    }
}
```

then first and second application are able to query the `SharedDllClass`:
```cs
// We are inside either first or second application.
var x = Db.SQL("SELECT c FROM SharedDllClass c").First;
var x2 = Db.SQL("SELECT c FROM SharedDll.SharedDllClass c").First;
```

Usage of shared libraries is a way for several applications to share the same class definitions. If you have several applications that are required to use same classes, you will need to create a shared library and move all common class definitions there. In rare cases whenever this is not possible and you still need to have several applications accessing each other classes, you can reference other applications from you "main" application, so only one application is started.

## SQL queries in Administrator

Currently Starcounter Administrator only supports SQL queries with fully namespaced class names. In the above example only the following queries are legitimate:
```cs
"SELECT c FROM App1.App1Class c"
"SELECT c FROM App2.App2Class c"
"SELECT c FROM SharedDll.SharedDllClass c"
```
