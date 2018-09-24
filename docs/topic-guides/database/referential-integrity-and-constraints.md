# Referential integrity and constraints

## Introduction

Starcounter does not have complete support for referential integrity or constraints. 

Referential integrity can instead be achieved using commit hooks which allow the developer to ensure that the correct corresponding item is deleted or added when removing or committing an item to the database.

## Referential integrity with commit hooks

Commit hooks should be implemented in a separate class and then registered when the application is started. Here's an example of that:

Let's say you have two DB classes: parent `Order` and child `OrderItem`:

```csharp
[Database]
public class Order
{
    public string Customer { get; set; }
    public DateTime Date { get; set; }
}

[Database]
public class OrderItem
{
    public Order Order { get; set; }
    public string Product { get; set; }
    public int Quantity { get; set; }
}
```

Here the commit hooks are declared in a separate class in a `Register` method:

```csharp
public class Hooks
{
    public void Register()
    {
        Hook<Order>.BeforeDelete += (sender, entity) =>
        {
            Db.SQL($"DELETE FROM {typeof(OrderItem)} oi WHERE oi.\"Order\" = ?", entity);
        };

        Hook<OrderItem>.CommitInsert += (sender, entity) =>
        {
            if (entity.Order == null)
            {
                throw new Exception(
                    "OrderItem.Order is not a null property");
            }
        };

        Hook<OrderItem>.CommitUpdate += (sender, entity) =>
        {
            if (entity.Order == null)
            {
                throw new Exception(
                    "OrderItem.Order is not a null property");
            }
        };
    }
}
```

In this code, we register the hooks by using `var hooks = new Hooks();` and `hooks.Register();`.

```csharp
public class Program 
{
    public void Main() 
    {
        Hooks hooks = new Hooks();

        hooks.Register();

        // This transaction will pass without errors.
        Db.Transact(() =>
        {
            var order = new Order()
            {
                Customer = "A customer",
                Date = DateTime.Now
            };

            var item = new OrderItem()
            {
                Order = Order,
                Product = "Starcounter license",
                Quantity = 10       
            };
        });

        // This transaction will fail with an exception
        // since the Order property of the OrderItem is NULL.
        Db.Transact(() =>
        {
            var item = new OrderItem()
            {
                Product = "Starcounter license",
                Quantity = 10
            };
        });

        // This transaction will delete all Order and OrderItem entries.
        // The OrderItem entries will be deleted by the BeforeDelete commit hook on the Order class.
        Db.Transact(() =>
        {
            Db.SQL($"DELETE FROM {typeof(Order).Namespace}.\"Order\"");
        });
    }
}
```

Two things that are done here:

1. When making an `INSERT` or `UPDATE`, the commit hooks will check if the reference to the database object is set properly. If not, then it throws an exception and prevents the item from being updated or saved to the database.
2. Before deleting a parent entity it deletes the children of that entity.

### OnDelete

As an alternative to the `BeforeDelete` commit hook, you can use the Starcounter method `OnDelete`.

`OnDelete` works similar to the `OnData` and `HasChanged` callback methods that are explained on the [callback methods page](../typed-json/callback-methods.md). It executes some code every time an instance of that class is deleted.

This is how it looks in code:

```csharp
[Database]
public class Foo
{
    public void OnDelete()
    {
        CalledWhenInstanceIsDeleted();
    }
}
```

If we create a new instance of the class `Foo` and then delete it by calling `fooInstance.Delete()` we would run `CalledWhenInstanceIsDeleted()`.

## Constraints

Database constraints define certain requirements that a database has to comply with.

Starcounter does not have database constraints as a part of its schema definition. As a consequence of this, you cannot define constraints in Starcounter the same way you would do with most SQL databases.

One of the most common constraints is the unique constraint which states that values in specified columns must be unique for every row in the table. This constraint can be set in Starcounter, even though it's not a part of the schema definition, by using [indexes](../sql/indexes.md).

