# Cancel and delete

You have probably noticed that in the current app you can only add new expenses. That means that the database and the list of expenses will continue to grow endlessly.

In this part, we will add a cancel and delete button which allows the user to either cancel a change that has not been committed or delete all the expenses of a person.

Similar to how the "Add new expense" button was implemented, the process to add this functionality requires three steps:  
1. Add trigger properties  
2. Add elements to increment the trigger properties  
3. Add event handlers

## Adding trigger properties

We start by adding our needed trigger properties for our future buttons to the `PersonJson.json`.

{% code-tabs %}
{% code-tabs-item title="PersonJson.json" %}
```javascript
{
  "Html": "/HelloWorld/PersonJson.html",
  "FirstName$": "",
  "LastName$": "",
  "SaveTrigger$": 0,
  "FullName": "",
  "Expenses": [{}],
  "NewExpenseTrigger$": 0,
  "CurrentBalance": 0,
  "CancelTrigger$": 0,
  "DeleteAllTrigger$": 0
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

## Add buttons to the view

Now, let's add the buttons that will increment these values in the same way that our save and new expense button does now.

Delete button:

{% code-tabs %}
{% code-tabs-item title="PersonJson.html" %}
```markup
<button value="{{model.DeleteAllTrigger$::click}}" onmousedown="++this.value">Delete all expenses</button>
```
{% endcode-tabs-item %}
{% endcode-tabs %}

Cancel button:

{% code-tabs %}
{% code-tabs-item title="PersonJson.html" %}
```markup
<button value="{{model.CancelTrigger$::click}}" onmousedown="++this.value">Cancel</button>
```
{% endcode-tabs-item %}
{% endcode-tabs %}

We'll place the delete button at the bottom of the page and the cancel button next to the save button.

## Create event handlers

The next step is to build handlers to react accordingly. We will also do that similar to the way we did with the other buttons.

{% code-tabs %}
{% code-tabs-item title="PersonJson.json.cs" %}
```csharp
void Handle(Input.CancelTrigger action)
{
    AttachedScope.Rollback();
}

void Handle(Input.DeleteAllTrigger action)
{
    Db.SQL("DELETE FROM Expense WHERE Spender = ?", this.Data);
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

`AttachedScope.Rollback()` simply rolls back the state of your application to where you last ran a `AttachedScope.Commit()`.

The `DeleteAllTrigger` handler deletes all the expenses for the current `Person` in the database and clears the `Expenses` property in the view-model

## Result

You can now run your application again and rollback any mistakes you make or delete all expenses.

![](../../.gitbook/assets/resizedpart6.gif)

