# SQL Isolation Between Applications

Applications running in the same code-host are isolated on different levels:

- REST handlers and URIs namespace.
- SQL classes and objects.
- Static file resources.

The principle for SQL isolation is that the database classes of one application should not be visible to the database classes of another application running in the same code-host.

## Isolation Example

For example, the first application defines a database class `App1Class` in its namespace:

<div class="code-name">App1</div>

```cs
namespace App1
{
    [Database]
    public class App1Class
    {
        public string App1ClassField;
    }
}
```

In the second application, the class `App2Class` is defined:

<div class="code-name">App2</div>

```cs
namespace App2
{
    [Database]
    public class App2Class
    {
        public string App2ClassField;
    }
}
```

The first application is now able to access its own `App1Class` using full and short names:

<div class="code-name">App1</div>

```cs
var result = Db.SQL("SELECT c FROM App1.App1Class c").First;
result = Db.SQL("SELECT c FROM App1Class c").First;
```

The same appplies to the second application with the class `App2Class`.

However, the first application will not be able to retrieve classes from the second application and vice versa. For example, the following code will throw an exception `Unknown class App2Class`:

<div class="code-name">App1</div>

```cs
var result = Db.SQL("SELECT c FROM App2Class c").First;
```

Classes defined in private application references, such as private libraries, are only accessible within the application that references them.

## Shared Library

If the first and second application are referencing the same library, for example "SharedDll", then both applications have access to classes and objects from this shared library, no matter which application created those objects:

<div class="code-name">SharedDll</div>

```cs
namespace SharedDll
{
    [Database]
    public class SharedDllClass
    {
        public string SharedDllClassField;
    }
}
```

With this, both applications are able to query the `SharedDllClass`:

```cs
var x = Db.SQL("SELECT c FROM SharedDllClass c").First;
var x2 = Db.SQL("SELECT c FROM SharedDll.SharedDllClass c").First;
```

The usa of shared libraries is a way for several applications to share the same class definitions. If you have several applications that are required to use the same classes, you will need to create a shared library and move all common class definitions there. In rare cases whenever this is not possible and you still need to have several applications accessing each other classes, you can reference other applications from you "main" application, so only one application is started.

## SQL Queries in the Administrator

Currently, the Starcounter Administrator only supports SQL queries with fully namespaced class names. In the above example, only the following queries are legitimate:
```cs
"SELECT c FROM App1.App1Class c"
"SELECT c FROM App2.App2Class c"
"SELECT c FROM SharedDll.SharedDllClass c"
```
