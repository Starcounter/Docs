# Referential Integrity and Constraints

A widely accepted <a href="http://databases.about.com/cs/administration/g/refintegrity.htm">definition</a> of referential integrity says that: "Referential integrity is a database concept that ensures that relationships between tables remain consistent. When one table has a foreign key to another table, the concept of referential integrity states that you may not add a record to the table that contains the foreign key unless there is a corresponding record in the linked table."

You can get a more in-depth explanation of this concept on <a href="https://en.wikipedia.org/wiki/Referential_integrity">Wikipedia</a>.

Starcounter does not, in the status quo, have complete support for referential integrity. Instead, referential integrity can be achieved using <a href="/guides/transactions/commit-hooks">commit hooks</a> which allow the developer to ensure that the correct corresponding item is deleted or added when removing or committing an item to the database.

These commit hooks should be implemented in a separate class and then registered when the application is started. Here's an example of that:

Let's say you have two DB classes: parent <code>Order</code> and child <code>OrderItem</code>:
```cs
[Database]
public class Order
{
    public string Customer;
    public DateTime Date;
}

[Database]
public class OrderItem
{
    public Order Order;
    public string Product;
    public int Quantity;
}
```

Here the commit hooks are declared in a separate class within a <code>Register</code> method:
```cs
public class Hooks
{
    public void Register()
    {
        Hook<Order>.BeforeDelete += (sender, entity) =>
        {
            Db.SlowSQL("DELETE FROM OrderItem oi WHERE oi.\"Order\" = ?", entity);
        };

        Hook<OrderItem>.CommitInsert += (sender, entity) =>
        {
            if (entity.Order == null)
            {
                throw new Exception("OrderItem.Order is not a null property");
            }
        };

        Hook<OrderItem>.CommitUpdate += (sender, entity) =>
        {
            if (entity.Order == null)
            {
                throw new Exception("OrderItem.Order is not a null property");
            }
        };
    }
}
```

In this code, we register the hooks by simply using `Hooks hooks = new Hooks();` and `hooks.Register();`.

```cs
public class Program {
    public void Main() {
        Hooks hooks = new Hooks();

        hooks.Register();

        // This transaction will pass without errors.
        Db.Transact(() =>
        {
            Order order = new Order()
            {
                Customer = "A customer",
                Date = DateTime.Now
            };

            OrderItem item = new OrderItem()
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
            OrderItem item = new OrderItem()
            {
                Product = "Starcounter license",
                Quantity = 10
            };
        });

        // This transaction will delete all Order and OrderItem entries.
        // The OrderItem entries will be deleted by the BeforeDelete commit hook on the Order class.
        Db.Transact(() =>
        {
            Db.SlowSQL("DELETE FROM \"Order\"");
        });
    }
}
```
There are essentially two things that are done here:

1. When making an `INSERT` or `UPDATE`, the commit hooks will check if the reference to the database object is set properly. If not, then it throws an exception and prevents the item from being updated or saved to the database.

2. Before deleting a parent entity it deletes the children of that entity.

### OnDelete

As an alternative to the `BeforeDelete` commit hook, you can use the Starcounter method `OnDelete`.

`OnDelete` works similar to the `OnData` and `HasChanged` callback methods that are explained <a href="/guides/typed-json/callback-methods">here</a>. It executes some code every time an instance of that class is deleted. To accomplish this you have to make use of the `IEntity` interface.

This is how it would look in code:
```cs
[Database]
public class Foo : IEntity
{
    public void OnDelete()
    {
        CalledWhenInstanceIsDeleted();
    }
}
```
So if we create a new instance of the class `Foo` and then delete it by calling `fooInstance.Delete()` we would run `CalledWhenInstanceIsDeleted()`.

## Constraints

Database constraints define certain requirements that a database has to comply with.

Starcounter does not have database constraints as a part of its schema definition. As a consequence of this, you cannot define constraints in Starcounter the same way you would do with most SQL databases.

One of the most common constraints is the unique constraint which states that values in specified columns must be unique for every row in the table. This constraint can be set in Starcounter, even though it's not a part of the schema definition, by using [indexes](/guides/SQL/indexes).
