# Using a Shared Data Model

In the previous steps, we've used a data model declared using `[Database]` classes directly in `Program.cs`. That's practical for prototyping but doesn't get us very far in terms of code and data reuse.

Starcounter has a unique ability to allow multiple apps to [work on the same data](/guides/mapping-and-blending/sharing-data/). We do this by putting the data model into a separate project and loading it as a Dynamic-link Library (DLL) in all apps where it is applicable.

Starcounter comes with a built-in data model called ["Simplified"](https://github.com/StarcounterApps/Simplified). Using it or inheriting from it allows you to integrate your apps with our sample apps, without extensive data mapping.

1. Go to <code>Solution Explorer -> HelloWorld -> References -> Add Reference... -> Extensions</code>, find <code>Simplified.Data.Model</code> and check the checkbox next to it.
2. Open `References -> Simplified.Data.Model` in the Solution Explorer, right-click and click on `properties`. Find the property `Copy Local` and change it to `False`.

The Simplified data model has six different "Rings", we will use the first two of these. We include them at the start of our `Program.cs` file.

<div class="code-name">Program.cs</div>

```cs
using System;
using Starcounter;
using Simplified.Ring1;
using Simplified.Ring2;
```

Now that we have these in our file, we can start referencing them. First, we want our classes `Expense` and `Person` to inherit from the classes `Something` and `Person` which are inside `Simplified`.

<div class="code-name">Program.cs</div><div class="code-name code-title">Add inheritence</div>

```cs
[Database]
public class Spender : Person
{
  // Spender fields
}

[Database]
public class Expense : Something
{
  // Expense fields
}
```

Notice here that we change the name of what was previously our `Person` to be `Spender` because the class we inherit from has the same name as our initial class. Due of this, we will need to change all our references of `Person` to be `Spender` instead.

<div class="code-name">Program.cs</div>

```cs
var anyone = Db.SQL<Spender>("SELECT s FROM Spender s").First;
if (anyone == null)
{
    new Spender()
    {
        FirstName = "John",
        LastName = "Doe"
    }
}
```

<div class="code-name">Program.cs</div>

```cs
return Db.Scope(() =>
{
    var person = Db.SQL<Spender>("SELECT s FROM Spender s").First;

    // The rest of the code block
}
```

<div class="code-name">PersonJson.json.cs</div>

```cs
partial class PersonJson : Json, IBound<Spender>
```

<div class="code-name">PersonJson.json.cs</div>

```cs
void Handle(Input.AddNewExpenseTrigger action)
{
  var expense = new Expense()
  {
      Spender = (Spender) this.Data,
      Amount = 1
  };
  AddExpense(expense);
}
```

Great! The only thing remaining to fully implement our simplified data model is to delete the overlapping properties we have. These properties are `FirstName` and `LastName` in `Spender`, and `Description` in `Expense`.

When you're done, your classes should look like this:

<div class="code-name">Program.cs</div>

```cs
[Database]
public class Spender : Person
{
    public QueryResultRows<Expense> Spendings => Db.SQL<Expense>("SELECT e FROM Expense e WHERE e.Spender = ?", this);
    public decimal CurrentBalance => Db.SQL<decimal>("SELECT SUM(e.Amount) FROM Expense e WHERE e.Spender = ?", this).First;
}

[Database]
public class Expense : Something
{
    public Spender Spender;
    public decimal Amount;
}
```
Look at that! From seven different properties down to four. That's a significant simplification without reducing the functionality of our application.

Since this changes the data model, it might be necessary to clear the database for it to work. That can be done at `http://localhost:8181/#/databases` by clicking `delete` below the database tab.

If you run the application now, you shouldn't see any visual difference compared to the previous step. Though, if you investigate the dataset with SQL, you will see that our classes have some extra properties that we have not defined. These extra properties have been inherited together with the properties we actually use.

Now, let us make our application prettier by adding some images.

If you get any errors, you can check your code against the [source code](https://github.com/StarcounterApps/HelloWorld/commit/57070572f8a2ca7d04d430061eb4c542a1197b3e).
