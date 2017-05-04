# Expense Tracker

We will now turn our application into a simple expense tracker.

With Starcounter, views and view-models can be broken up into parts and [nested](/guides/web-apps/html-views/#using-partials). This allows for increased composability and modularity. In this case, the two main concepts in this app, `Person` and `Expense`, will be built with separate view-models and views and then nested to create a coherent whole. Let's start by creating the appropriate files.

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

### Extending the View-Model

With the fields `Expenses` and `CurrentBalance` in place, we can bind them to the view-model in order to use them in the view.

<div class="code-name">PersonJson.json</div>

```json
{
  "Html": "/HelloWorld/PersonJson.html",
  "FirstName$": "",
  "LastName$": "",
  "SaveTrigger$": 0,
  "FullName": "",
  "Expenses": [{}],
  "CurrentBalance": 0
}
```

When binding the view-model with fields of database classes, the types have to be specified. `Expenses` is defined as an array of objects, that is not quite enough. We also need to set what type of object it should contain. This can be done in the code-behind for `PersonJson` by creating a constructor that specifies the type to be `ExpenseJson`. This is how it looks in code:  

<div class="code-name">PersonJson.json.cs</div>

```cs
static PersonJson()
{
    DefaultTemplate.Expenses.ElementType.InstanceType = typeof(ExpenseJson);
}
```

### Nesting the View

With the expenses in the view-model, they are easy to include in the view. We simply need to loop over and stamp them out on the page. This can be done with the custom HTML template element `dom-repeat`. It is essentially equivalent to a C# `foreach` loop. 

The elements that it should loop over are the expenses. For each expense it should stamp out the view for that particular expense. `starcounter-include` is a custom element that helps with that by acting as an insertion point.

In addition to this, we'll also want to display the current balance and change the headline to reflect the actual purpose of the page:

<div class="code-name">PersonJson.html</div>

{% raw %}
```html
<template>
    <template is="dom-bind">
        <h1>{{model.FullName}}'s expense list</h1>

        <fieldset>
            <label>First name:</label>
            <input value="{{model.FirstName$::input}}">
        </fieldset>

        <fieldset>
            <label>Last name:</label>
            <input value="{{model.LastName$::input}}">
        </fieldset>

        <button value="{{model.SaveTrigger$::click}}" onmousedown="++this.value">Save</button>

        <hr>

        <template is="dom-repeat" items="{{model.Expenses}}">
            <starcounter-include partial="{{item}}"></starcounter-include>
        </template>

        <hr>

        <h2>Current Balance: {{model.CurrentBalance}}</h2>
    </template>
</template>
```
{% endraw %}

## Create New Expenses

If you start the application now, you will not see any expenses because no expenses have been added to the database yet. Implementing the functionality to add new expenses is quite simple. Three things are needed:
1. A trigger in the view-model to signal when to add an expense
2. A button to activate the trigger
3. A handler to add the new expense

### Add Trigger Property

The trigger property will look almost identical to the save trigger:

<div class="code-name">PersonJson.json</div>

```json
{
  "Html": "/HelloWorld/PersonJson.html",
  "FirstName$": "",
  "LastName$": "",
  "SaveTrigger$": 0,
  "FullName": "",
  "Expenses": [{}],
  "NewExpenseTrigger$": 0,
  "CurrentBalance": 0
}
```

### Add Button

The button to create new expenses simply have to increment the trigger we just defined. It should look like this:

{% raw %}
```html
<button value="{{model.NewExpenseTrigger$::click}}" onmousedown="++this.value">Add new expense</button>
```
{% endraw %}

This button should be added below the list of expenses.

### Implement the Handler

To act on the trigger, we'll create a handler in the code-behind. Since the view-model is bound to the `Person` object that holds a reference to a collection of `Expense` objects, we only need to add an `Expense` object to the database and it will immidiately be synched to the view-model and placed in the view.

<div class="code-name">PersonJson.json.cs</div>

```cs
void Handle(Input.NewExpenseTrigger action)
{
    new Expense()
    {
        Spender = this.Data as Person,
        Amount = 1
    };
}
```

`This.Data` is the current database object, which in this case is the `Person` that added a new expense.

## Result

Run the program and try to add expenses, change their cost, and see the current balance change in real time.

![part 5 gif](/assets/Part5resized.gif)

With every keystroke, the UI is updated almost instantly from the database. Starcounter's in-memory database makes this possible. There's no delay, everything simply happens at the moment the user interacts with the view.

If you get any errors, you can check your code against the [source code](https://github.com/StarcounterApps/HelloWorld/commit/a5dcf4af93621d2c6c006f440eb7f628976544b1).
