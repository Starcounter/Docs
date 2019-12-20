# Database Refactoring

## Introduction

When an application is started, Starcounter will analyze all database classes defined and synchronize their structure with the underlying database schema. This process is sometimes referred to as binding.

If the database has just been created, it contains no schema at all and the binding is simple - every database construct in the application \(such as a defined database class field\) will render a corresponding construct in the database schema.

If a database schema already exists, Starcounter must make sure that the application schema and the underlying database harmonize. If they don't, it will try to figure out how to make the two schemas match. This could involve adding, removing or changing already defined schema constructs. This process will be referred to as database refactoring.

## Rebuilding the Database

Starcounter supports rebuilding a database by using an unload-reload operation pair.

Unloading a database is supported via an API, though it is more often done using `staradmin unload db` in the command prompt. Unloading a database will fetch all data in the database and write out INSERT-statements to a file that, when reapplied, will insert the unloaded data inside a new, empty database. This file is sometimes referred to as a dump.

Reloading has similar, but reverse semantics. It will act on a file produced by a previous unload, a dump, and re-apply each INSERT-statement it contains.

Rebuilding a database is a requirement when the internal format of Starcounter data change between versions. It has historically been used as a mean to refactor a schema. It's not suited to do so after version 2.0.

To successfully unload all data in a database, each application that has originally defined the schema has to be loaded. Failing that will result in an error and the unload will fail. One way around this is to specify the unload to be partial. Similarly, to successfully reload a database, the schema defined at the time of the reload must match that of the dump, or the reload will fail.

Partial unloads allow unloading a database without forcing the application schema to match the database schema, i.e. apps and classes are allowed not to be loaded. Doing this from a Command Line Interface \(CLI\) can be achieved using `staradmin unload db --allowPartial`.

## Refactorings

If nothing else is stated, each refactoring has been done in isolation using a common procedure. We have tested the effects of two distinct cases:

1. Auto-migrations
2. Rebuild

Auto-migration test how Starcounter react when a refactored app is restarted. We have used this scheme:

1. Create a new, empty database
2. Build and run `Before.cs`
3. Replace content of `Before.cs` with that of `After.cs`
4. Re-run the application

Rebuilds are using a dump file captured before the refactoring that is applied to an empty database created after, with the refactored app running. We have used this scheme:

1. Unload the original database/app with `staradmin unload --file=data.sql`
2. Delete and recreate the database
3. Refactor the app and start it without any transactions being applied
4. Reload the dump with `staradmin reload --file=data.sql`.

### 1. Add a Field

_Add a database field to an existing database class_

#### Source code

{% tabs %}
{% tab title="Before.cs" %}
```csharp
using Starcounter;

namespace AddField
{
    [Database]
    public class Foo {}

    class Program
    {
        static void Main()
        {
            Db.Transact(() =>
            {
                new Foo();
            });
        }
    }
}
```
{% endtab %}

{% tab title="After.cs" %}
```csharp
using Starcounter;

namespace AddField
{
    [Database]
    public class Foo
    {
        public string Bar;
    }

    class Program
    {
        static void Main()
        {
            Db.Transact(() =>
            {
                new Foo();
            });
        }
    }
}
```
{% endtab %}
{% endtabs %}

#### Traits

* Auto-migration supported: true
* Rebuilt works: true

#### Dumps

{% tabs %}
{% tab title="Before" %}
```sql
Database dump. DO NOT EDIT!
INSERT INTO "Starcounter"."Raw"."AddField"."Foo"(__id)VALUES(object 1)
```
{% endtab %}

{% tab title="After" %}
```sql
Database dump. DO NOT EDIT!
INSERT INTO "Starcounter"."Raw"."AddField"."Foo"(__id,"Bar")VALUES(object 2,'Value'),(object 1,NULL)
```
{% endtab %}
{% endtabs %}

### 2. Add a Property

_Add a database property to an existing database class._

#### Source code

{% tabs %}
{% tab title="Before.cs" %}
```csharp
using Starcounter;

namespace AddProperty
{
    [Database]
    public class Foo {}

    class Program
    {
        static void Main()
        {
            Db.Transact(() =>
            {
                new Foo();
            });
        }
    }
}
```
{% endtab %}

{% tab title="After.cs" %}
```csharp
using Starcounter;

namespace AddProperty
{
    [Database]
    public class Foo
    {
        public string Bar { get; set; }
    }

    class Program
    {
        static void Main()
        {
            Db.Transact(() =>
            {
                new Foo();
            });
        }
    }
}
```
{% endtab %}
{% endtabs %}

#### Traits

* Auto-migration supported: true
* Rebuild works: true

#### Dumps

{% tabs %}
{% tab title="Before" %}
```sql
Database dump. DO NOT EDIT!
INSERT INTO "Starcounter"."Raw"."AddProperty"."Foo"(__id)VALUES(object 1)
```
{% endtab %}

{% tab title="After" %}
```sql
Database dump. DO NOT EDIT!
INSERT INTO "Starcounter"."Raw"."AddProperty"."Foo"(__id,"Bar")VALUES(object 2,'Value'),(object 1,NULL)
```
{% endtab %}
{% endtabs %}

### 3. Add a Class

_Add a database class when there is already a class present and instances of it._

#### Source code

{% tabs %}
{% tab title="Before.cs" %}
```csharp
using Starcounter;

namespace AddProperty
{
    [Database]
    public class Foo {}

    class Program
    {
        static void Main()
        {
            Db.Transact(() =>
            {
                new Foo();
            });
        }
    }
}
```
{% endtab %}

{% tab title="After.cs" %}
```csharp
using Starcounter;

namespace AddProperty
{
    [Database]
    public class Foo {}

    [Database]
    public class Bar {}

    class Program
    {
        static void Main()
        {
            Db.Transact(() =>
            {
                new Foo();
            });
            Db.Transact(() =>
            {
                new Bar();
            });
        }
    }
}
```
{% endtab %}
{% endtabs %}

#### Traits

* Auto-migration supported: true
* Rebuild works: true

#### Dumps

{% tabs %}
{% tab title="Before" %}
```sql
Database dump. DO NOT EDIT!
INSERT INTO "Starcounter"."Raw"."AddClass"."Foo"(__id)VALUES(object 1)
```
{% endtab %}

{% tab title="After" %}
```sql
Database dump. DO NOT EDIT!
INSERT INTO "Starcounter"."Raw"."AddClass"."Foo"(__id)VALUES(object 1),(object 2)
INSERT INTO "Starcounter"."Raw"."AddClass"."Bar"(__id)VALUES(object 3)
```
{% endtab %}
{% endtabs %}

### 4. Remove a Field

_Remove a database field from a class._

#### Source code

{% tabs %}
{% tab title="Before.cs" %}
```csharp
using Starcounter;

namespace RemoveField
{
    [Database]
    public class Foo
    {
        public string Bar;
    }

    class Program
    {
        static void Main()
        {
            Db.Transact(() =>
            {
                new Foo()
                {
                    Bar = "Value"
                };
            });
        }
    }
}
```
{% endtab %}

{% tab title="After.cs" %}
```csharp
using Starcounter;

namespace RemoveField
{
    [Database]
    public class Foo {}

    class Program
    {
        static void Main()
        {
            Db.Transact(() =>
            {
                new Foo();
            });
        }
    }
}
```
{% endtab %}
{% endtabs %}

#### Traits

* Auto-migration supported: true
* Rebuild works: **false** \(see remarks below\)

#### Dumps

{% tabs %}
{% tab title="Before" %}
```sql
Database dump. DO NOT EDIT!
INSERT INTO "Starcounter"."Raw"."RemoveField"."Foo"(__id,"Bar")VALUES(object 1,'Value')
```
{% endtab %}

{% tab title="After" %}
```
Not possible to achieve - see remarks below.
```
{% endtab %}
{% endtabs %}

#### Remarks

Reloading the database dump after removing the field will fail. Also, just doing an unload will fail if issued after the auto-migration have succeeded.

### 5. Remove a Property

_Remove a database property from a class._

#### Source code

{% tabs %}
{% tab title="Before.cs" %}
```csharp
using Starcounter;

namespace RemoveProperty
{
    [Database]
    public class Foo
    {
        public string Bar { get; set; }
    }

    class Program
    {
        static void Main()
        {
            Db.Transact(() =>
            {
                new Foo()
                {
                    Bar = "Value"
                };
            });
        }
    }
}
```
{% endtab %}

{% tab title="After.cs" %}
```csharp
using Starcounter;

namespace RemoveProperty
{
    [Database]
    public class Foo {}

    class Program
    {
        static void Main()
        {
            Db.Transact(() =>
            {
                new Foo();
            });
        }
    }
}
```
{% endtab %}
{% endtabs %}

#### Traits

* Auto-migration supported: true
* Rebuild works: **false** \(see remarks below\)

#### Dumps

{% tabs %}
{% tab title="Before" %}
```sql
Database dump. DO NOT EDIT!
INSERT INTO "Starcounter"."Raw"."RemoveProperty"."Foo"(__id,"Bar")VALUES(object 1,'Value')
```
{% endtab %}

{% tab title="After" %}
```
Not possible to achieve - see remarks below.
```
{% endtab %}
{% endtabs %}

#### Remarks

Reloading the database dump after removing the property will fail. Also, just doing an unload will fail if issued after the auto-migration have succeeded.

### 6. Remove a Class

_Remove a database class that was previously bound._

#### Source code

{% tabs %}
{% tab title="Before.cs" %}
```csharp
using Starcounter;

namespace RemoveClass
{
    [Database]
    public class Foo {}

    [Database]
    public class Bar
    {
        public string Value { get; set; }
    }

    class Program
    {
        static void Main()
        {
            Db.Transact(() =>
            {
                new Foo();
            });
            Db.Transact(() =>
            {
                new Bar()
                {
                    Value = "Value"
                };
            });
        }
    }
}
```
{% endtab %}

{% tab title="After.cs" %}
```csharp
using Starcounter;

namespace RemoveClass
{
    [Database]
    public class Foo {}

    class Program
    {
        static void Main()
        {
            Db.Transact(() =>
            {
                new Foo();
            });
        }
    }
}
```
{% endtab %}
{% endtabs %}

#### Traits

* Auto-migration supported: true
* Rebuild works: **true, with issues** \(see remarks below\)

#### Dumps

{% tabs %}
{% tab title="Before" %}
```sql
Database dump. DO NOT EDIT!
INSERT INTO "Starcounter"."Raw"."RemoveClass"."Foo"(__id)VALUES(object 1)
INSERT INTO "Starcounter"."Raw"."RemoveClass"."Bar"(__id,"Value")VALUES(object 2,'Value')
```
{% endtab %}

{% tab title="After" %}
```sql
Database dump. DO NOT EDIT!
INSERT INTO "Starcounter"."Raw"."RemoveClass"."Foo"(__id)VALUES(object 1),(object 3)
```
{% endtab %}
{% endtabs %}

#### Remarks

To successfully unload after the auto-migration has occurred, the unload has to be carried out with the `--allowPartial` flag, i.e. `staradmin unload --allowPartial`.

### 7. Rename a Field

_Rename a field in a database class._

#### Source code

{% tabs %}
{% tab title="Before.cs" %}
```csharp
using Starcounter;

namespace RenameField
{
    [Database]
    public class Foo
    {
        public string Bar;
    }

    class Program
    {
        static void Main()
        {
            Db.Transact(() =>
            {
                new Foo();
            });
        }
    }
}
```
{% endtab %}

{% tab title="After.cs" %}
```csharp
using Starcounter;

namespace RenameField
{
    [Database]
    public class Foo
    {
        public string BarRenamed;
    }

    class Program
    {
        static void Main()
        {
            Db.Transact(() =>
            {
                new Foo();
            });
        }
    }
}
```
{% endtab %}
{% endtabs %}

#### Remarks

Renaming a field equals a combined refactoring of removing a field and adding a field. Hence, everything that applies to those refactorings applies here.

### 8. Rename a Property

_Rename a property in a database class._

#### Source code

{% tabs %}
{% tab title="Before.cs" %}
```csharp
using Starcounter;

namespace RenameProperty
{
    [Database]
    public class Foo
    {
        public string Bar { get; set; }
    }

    class Program
    {
        static void Main()
        {
            Db.Transact(() =>
            {
                new Foo();
            });
        }
    }
}
```
{% endtab %}

{% tab title="After.cs" %}
```csharp
using Starcounter;

namespace RenameProperty
{
    [Database]
    public class Foo
    {
        public string BarRenamed { get; set; }
    }

    class Program
    {
        static void Main()
        {
            Db.Transact(() =>
            {
                new Foo();
            });
        }
    }
}
```
{% endtab %}
{% endtabs %}

#### Remarks

Renaming a property equals a combined refactoring of removing a property and adding a property. Hence, everything that applies to those refactorings applies here.

### 9. Rename a Class

_Rename a database class._

#### Source code

{% tabs %}
{% tab title="Before.cs" %}
```csharp
using Starcounter;

namespace RenameClass
{
    [Database]
    public class Foo {}

    class Program
    {
        static void Main()
        {
            Db.Transact(() =>
            {
                new Foo();
            });
        }
    }
}
```
{% endtab %}

{% tab title="After.cs" %}
```csharp
using Starcounter;

namespace RenameClass
{
    [Database]
    public class FooRenamed {}

    class Program
    {
        static void Main()
        {
            Db.Transact(() =>
            {
                new FooRenamed();
            });
        }
    }
}
```
{% endtab %}
{% endtabs %}

#### Remarks

Renaming a field equals a combined refactoring of removing a class and adding a class. Hence, everything that applies to those refactorings applies here.

