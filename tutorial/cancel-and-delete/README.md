# Cancel and Delete

You have probably noticed that in the current app you can only add new expenses. That means that the database and the list of expenses will continue to grow endlessly. We can't accept that.

In this part, we will add a cancel and delete button which allows the user to either cancel a change that has not been committed or delete all the expenses of a person.

We start by adding our needed trigger properties for our future buttons to the `PersonJson.json`.

<div class="code-name">PersonJson.json</div>

```json
"CurrentBalance": 0,
"CancelTrigger$": 0,
"DeleteAllTrigger$": 0
```

No surprises here.

Now, let's add the buttons that will increment these values in the same way that our save button does now.

<div class="code-name">PersonJson.html</div>

{% raw %}
```html
<button value="{{model.SaveTrigger$::click}}" onmousedown="++this.value">Save</button>
<button value="{{model.CancelTrigger$::click}}" onmousedown="++this.value">Cancel</button>
.
.
<h2>Current Balance: {{model.CurrentBalance}}</h2>
<button value="{{model.DeleteAllTrigger$::click}}" onmousedown="++this.value">Delete all expenses</button>
```
{% endraw %}


We now expect the values `DeleteAllTrigger$` and `CancelTrigger$` to be incremented on click.

The next step is to build handlers to react accordingly. We will also do that similar to the way we did with the `Save` button.

<div class="code-name">Person.json.cs</div>

```cs
void Handle(Input.CancelTrigger action)
{
    Transaction.Rollback();
    RefreshExpenses(this.Data.Spendings);
}

void Handle(Input.AddNewExpenseTrigger action)
.
.
void Handle(Input.DeleteAllTrigger action)
{
    Db.SlowSQL("DELETE FROM Expense WHERE Spender = ?", this.Data);
    this.Expenses.Clear();
}
```

`Transaction.Rollback()` simply rolls back the state of your application to where you last ran a `Transaction.Commit()`.

`this.Data.Spendings` is every `Expense` of the current `Person`. With the code we currently have, this will not work. To fix it we simply add the [`IBound<Person>`](/guides/typed-json/json-data-bindings/#specify-data-type) to the `PersonJson` partial class. This creates a binding between the view-model and `Person` database class which allows us to use its property `Spendings`.

<div class="code-name">PersonJson.json.cs</div>

```cs
partial class PersonJson : Json, IBound<Person>
```

The `DeleteAllTrigger` handler deletes all the expenses for the current `Person` in the database and clears the `Expenses` property in its JSON file.

`RefreshExpenses` in the `CancelTrigger` handler is a method that updates the `Expenses` of the `Person`. It should look like this:

<div class="code-name">PersonJson.json.cs</div>

```cs
public void RefreshExpenses(IEnumerable<Expense> expenses)
{
    this.Expenses.Clear();
    foreach (Expense expense in expenses)
    {
        AddExpense(expense);
    }
}
```
This method should also be called in the initial `GET` request to make sure that the expenses are loaded in properly and displayed after refreshing the page. This is how it should be implemented to protect against potential `null` errors:

<div class="code-name">Program.cs</div>

```cs
if (person.Spendings != null)
{
    json.RefreshExpenses(person.Spendings);
}
```

You can now run your application again and rollback any mistakes you make or delete all expenses.

![part 6 gif](/assets/resizedpart6.gif)

If you get any errors, you can check your code against the [source code](https://github.com/StarcounterApps/HelloWorld/commit/7491b94014938bd9a1a0af6591bd754fc3e3b1e1).
