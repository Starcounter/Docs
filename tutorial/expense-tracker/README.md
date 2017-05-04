# Expense Tracker

We will now turn our application into a simple expense tracker. This will allow us to practice using multiple object instances and relations.

With Starcounter, views and view-models can be broken up into parts and nested. This allows for increased composability and modularity. In this case, the two main concepts in this app, `Person` and `Expense`, will be built with separate view-models and views and then nested to create a coherent whole. Let's start by creating the appropriate files.

1. Add a new Starcounter "HTML template with dom-bind" in the HelloWorld folder together with `PersonJson.html`. Name it `ExpenseJson.html`.
2. Add a new Starcounter "Typed JSON with Code-behind" file to the HelloWorld project together with `PersonJson.json` and `PersonJson.json.cs`. Name it `ExpenseJson.json`.

![Expense Tracker file structure](/assets/Expense-file-structure.png)

## Define the Expenses

To define the expenses, three parts are needed:
1. A database class which can store the expenses
2. A view that can display the expenses
3. A view-model to bind the database class and the view

### Database Class

To keep it as simple as possible, the expense only needs to contain information about its cost or amount, a description of the expense, and a reference to the person that spent the expense. 

This is how it looks in code:

<div class="code-name">Program.cs</div>

```cs
[Database]
public class Expense
{
    public Person Spender;
    public string Description;
    public decimal Amount;
}
```

### View-Model

The information that we want to have in the view is the description and the amount, which should be writable from the view since we're building an expense tracker, so that's what we add to the view-model in addition to the HTML path.

This is the result:

<div class="code-name">ExpenseJson.json</div>

```json
{
  "Html": "/HelloWorld/ExpenseJson.html",
  "Description$": "",
  "Amount$": 0
}
```

### View

The view for the expense can be extremely simple. It will only contain two input fields that are bound to the properties in the view-model:

<div class="code-name">ExpenseJson.html</div>

{% raw %}
```html
<template>
    <template is="dom-bind">
        <input value="{{model.Description$::input}}">
        <input value="{{model.Amount$::input}}">
    </template>
</template>
```
{% endraw %}

## Nest Expenses Within a Person

Now that the concept of an expense is defined, it should be connected to the concept of a person so that a person owns multiple expenses. There are a couple of steps to follow to make that happen.
1. The `Person` class should be linked to its expenses. Right now, each expense is bound to a `Person`. To create a one-to-many relationship, the `Person` should also be linked to the expenses.
2. The view-models of the expenses of a specific `Person` object should be nested into the view-model `PersonJson`. This nesting makes it easy to display the expenses of a person. 
3. The views corresponding to the nested expense view-models should be stamped out inside the person view. 

### Extending the Person Class

To link each `Person` object with its `Expense` objects, we'll utilize the fact that each `Expense` has the field `Spender`. Thus, a SQL query can be made to find all the `Expense` objects where the `Spender` is the current `Person`. In addition to containing a reference to all its `Expense` objects, it might be useful for the `Person` class to contain a field for the aggregated `Amount` of the `Expense` objects it has. This can also be accomplished with a SQL query.

In code, it looks like this:

<div class="code-name">Program.cs</div>

```cs
[Database]
public class Person
{
    public string FirstName;
    public string LastName;
    public QueryResultRows<Expense> Expenses => Db.SQL<Expense>("SELECT e FROM Expense e WHERE e.Spender = ?", this);
    public decimal CurrentBalance => Db.SQL<decimal>("SELECT SUM(e.Amount) FROM Expense e WHERE e.Spender = ?", this).First;
}
```

### Extend the Person View-Model

As you can see above, we are using `NewExpenseTrigger$`, which we haven't defined yet. Let's go and fix that now.

<div class="code-name">PersonJson.json</div>

```json
"FullName": "",
"Expenses": [{}],
"NewExpenseTrigger$": 0,
"CurrentBalance": 0
```

`Expenses` is a list of the expenses of a Person and `CurrentBalance` will be the sum of of all these expenses.

`NewExpenseTrigger$` is a trigger property that allows us to add a new `Expenses` template to the view. We can implement its handler now.

### Nesting the View

Great! Now we just need to stack these views inside the `Person` view. We can do that easily using `dom-repeat`. While we are on it, we will also modify the headline and add a button to add new expenses.

<aside class="read-more">
    <a href="https://www.polymer-project.org/1.0/docs/devguide/templates">Read more about dom-repeat</a>
</aside>

<div class="code-name">PersonJson.html</div>

{% raw %}
```html
<h1>{{model.FullName}}'s expense list</h1>
.
.
.
<hr>
<template is="dom-repeat" items="{{model.Expenses}}">
    <div>
        <starcounter-include partial="{{item}}"></starcounter-include>
    </div>
</template>
<button value="{{model.NewExpenseTrigger$::click}}" onmousedown="++this.value">Add new expense</button>
<hr>
<h2>Current Balance: {{model.CurrentBalance}}</h2>
```
{% endraw %}

`starcounter-include` is an insertion point for another template. In this case it's representing the template in ExpenseJson so that we can keep our code separate.

## Create New Expenses

<div class="code-name">PersonJson.json.cs</div>

```cs
void  Handle(Input.AddNewExpenseTrigger action)
{
    var expense = new Expense()
    {
        Spender = this.Data as Person,
        Amount = 1
    };
}
```

## Result

Run the program and try to add expenses, change their cost, and see the current balance change in real time.

![part 5 gif](/assets/Part5resized.gif)

With every keystroke, the UI is updated almost instantly from the database. Starcounter's in-memory database makes this possible. There's no delay, everything simply happens at the moment the user interacts with the view.

If you get any errors, you can check your code against the [source code](https://github.com/StarcounterApps/HelloWorld/commit/b382363636c865dce8f1beb3c886e738cc630a08).
