# Starcounter database access API with Microsoft Dependency Injection services

Starting from this version, static Starcounter 2.x style `Db` class has been removed.

The current release introduces new _`SC` framework API_ based on dependency injection (DI), which has been for long time requested by the majority of our users.

In this release, database access and operations are provided via two main Starcounter services.

- `Starcounter.Nova.Hosting.ITransactor` - provides database transactions and data manipulation ([DML](https://en.wikipedia.org/wiki/Data_manipulation_language)) API.
- `Starcounter.Nova.Hosting.IDdlExecutor` - provides data definition ([DML](https://en.wikipedia.org/wiki/Data_definition_language)) API.

The purpose of the current release is to polish the new `SC` framework API and approve it with the customers. In the future public releases we plan to introduce a so-called _static API_ serving same purpose as `Db` class in Starcounter 2.x, yet free from known flaws and _strictly_ built on top of the new `SC` framework API.

## Why Dependency Injection (DI)?

One of the major disadvantages of the static classes usage is inability to test such code with unit tests.
With Dependency Injection it is possible to mock tested services and write unit tests for the application.

Furthermore, current approach reduces possibilities for invalid database access:

- Attempt to create a new database object without a transaction.
- Attempt to access a database object without a transaction.
- Attempt to execute a DDL statement within a transaction.

Read more about Dependency Injection - [Design Patterns Explained – Dependency Injection with Code Examples](https://stackify.com/dependency-injection/).

### Examples

#### Database DML & DDL operations

Previously `Db.SQL` method was responsible for all kind of SQL statements.
This led to a confusion, when developers tried to execute DDL statements within a transaction and access database without a transaction.

```cs
Db.Transact(() =>
{
    // This is a valid database access.
    // DML operations require a database transaction.
    Db.SQL("SELECT i FROM Item i");
    
    // This is an invalid database access.
    // DDL operations cannot be executed within a transaction.
    Db.SQL("CREATE INDEX IX_Item ON Item (Name)");
    
    // This is a valid database object creation.
    // DML operations require a database transaction.
    var newItem = Db.Insert<Item>();
    
    // This is a valid database object access.
    // DML operations require a database transaction.
    var existingItem = Db.Get<Item>(1);
    existingItem.Name = "Valid";
});

// This is an invalid database object creation.
// A database transaction is required to create a database object.
var newItem = Db.Insert<Item>();

// This is an invalid database object access.
// DML operations require a database transaction.
var existingItem = Db.Get<Item>(1);
existingItem.Name = "Invalid";
```

With introduction of the `ITransactor` and `IDdlExecutor` database services it is easier to follow the code flow and database interaction.
The `Insert` and `Get` database operations are only available within a transaction.

## ASP.NET Core `DbAccessController` sample

```cs
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Starcounter.Nova;
using Starcounter.Nova.Hosting;

namespace DiSample
{
    // Define database class
    [Database]
    public abstract class Item
    {
        public abstract int Id { get; set; }
        public abstract string Name { get; set; }
    }
}

namespace DiSample.Controllers
{
    public class DbAccessController : Controller
    {
        private readonly IDdlExecutor _ddlExecutor;
        private readonly ITransactor _transactor;

        // The DbAccessController class requires 
        // two services: IDdlExecutor & ITransactor.
        public DbAccessController(IDdlExecutor ddlExecutor, ITransactor transactor)
        {
            _ddlExecutor = ddlExecutor;
            _transactor = transactor;
        }

        // Perform Starcounter DML operations.
        // Select an Item with specified id.
        // Create one if there is no such item yet.
        // Return as JSON string.
        public string GetItem(int id)
        {
            string result = null;

            _transactor.Transact(db =>
            {
                Item item = db.Sql<Item>
                (
                    "SELECT i FROM DiSample.Item i WHERE i.Id = ?",
                    id
                ).FirstOrDefault();

                if (item == null)
                {
                    item = db.Insert<Item>();
                    item.Id = id;
                    item.Name = DateTime.Now.ToString();
                }

                result = JsonConvert.SerializeObject(item);
            });

            return result;
        }

        // Perform Starcounter DDL operation.
        public string CreateIndex()
        {
            _ddlExecutor.Execute("CREATE INDEX IX_DiSample_Item ON Item (Id)");

            return "Database index IX_DiSample_Item has been successfully created.";
        }

        // Perform Starcounter DDL operation.
        public string DropIndex()
        {
            _ddlExecutor.Execute("DROP INDEX IX_DiSample_Item ON Item");

            return "Database index IX_DiSample_Item has been successfully dropped.";
        }
    }
}
```
