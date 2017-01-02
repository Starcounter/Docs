# Using a Shared Data Model

In the previous steps, we've used a data model declared using <code>[Database]</code> classes directly in <code>Program.cs</code>. That's practical for prototyping but doesn't get us very far in terms of code and data reuse.

Starcounter has a unique ability to allow multiple apps to work on the same data. We do this by putting the data model into a separate project and loading it as a Dynamic-link Library (DLL) in all apps where it is applicable.

Starcounter comes with a built-in data model called "Simplified". Using it or inheriting from it allows you to integrate your apps with our sample and prefab apps, without extensive data mapping.

1. Go to <code>Solution Explorer -> HelloWorld -> References -> Add Reference... -> Extensions</code>, find <code>Simplified.Data.Model</code> and check the checkbox next to it. 
2. Open <code>References -> Simplified.Data.Model</code> in the Solution Explorer, right-click and click on <code>properties</code>. Find the property <code>Copy Local</code> and change it to <code>False</code>.

The Simplified data model has six different "Rings", we will use the first two of these. We include them at the start of our <code>Program.cs</code> file.

<div class="code-name">Program.cs</div>
```cs
using System;
using Starcounter;
using Simplified.Ring1;
using Simplified.Ring2;
```

Now that we have these in our file, we can start referencing them. First, we want our classes <code>Expense</code> and <code>Person</code> to inherit from the classes <code>Something</code> and <code>Person</code> which are inside <code>Simplified</code>.

<div class="code-name">Program.cs</div><div class="code-name code-title">Add inheritence</div>
```cs
[Database]
public class Spender : Person
.
.
[Database]
public class Expense : Something
```

Notice here that we change the name of what was previously our <code>Person</code> to be <code>Spender</code> because the class we inherit from has the same name as our initial class. Due of this, we will need to change all our references of <code>Person</code> to be <code>Spender</code> instead.

<div class="code-name">Program.cs</div>
```
static void Main()
{
    var anyone = Db.SQL<Spender>("SELECT s FROM Spender s").First;
    if (anyone == null)
    {
        Db.Transact(() =>
        {
            new Spender()
.
.
        return Db.Scope(() =>
        {
            var person = Db.SQL<Spender>("SELECT s FROM Spender s").First;
```
<div class="code-name">PersonJson.json.cs</div>
```cs
partial class PersonJson : Json, IBound<Spender>
.
.
        var expense = new Expense()
        {
            Spender = (Spender) this.Data,
            Amount = 1
        };
```

Great! The only thing remaining to fully implement our simplified data model is to delete the overlapping properties we have. These properties are <code>FirstName</code> and <code>LastName</code> in <code>Spender</code>, and <code>Description</code> in <code>Expense</code>. 

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

If you run the application now, you shouldn't see any visual difference compared to the previous step. Though, if you investigate the dataset with SQL, you will see that our classes have some extra properties that we have not defined. These extra properties have been inherited together with the properties we actually use.

Now, let us make our application prettier by adding some images.

If you get any errors, you can check your code against the [source code](https://github.com/StarcounterSamples/HelloWorld/commit/a2d7bff038b595f716bbf497c1bac95603415bb4).