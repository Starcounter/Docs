# First Interactive UI

In order to make our UI interactive, we first have to make the data in the view-model editable. In JSON we do this by adding a `$` at the end of our property. That allows us to edit these JSON values from the view. In addition to adding `$` to our already existing properties, we will add the property `"Save$"` which will act as a trigger between the view and the code-behind.

<aside class="read-more">
    <a href="/guides/typed-json/json-by-example.html">Learn more about JSON in Starcounter</a>
</aside>

<div class="code-name">PersonJson.json</div>
```json
{
  "Html": "/HelloWorld/PersonJson.html",
  "FirstName$": "",
  "LastName$": "",
  "Save$": 0
}
```

Next, we have to add some elements to our view. We will start by changing our previously static fields into input fields and adding a button which will allow us to confirm that input.

<div class="code-name">PersonJson.html</div>
```html
<fieldset>
    <label>First name:</label>
    <input value="{{model.FirstName$::input}}">
</fieldset>
<fieldset>
    <label>Last name:</label>
    <input value="{{model.LastName$::input}}">
</fieldset>
<button value="{{model.Save$::click}}" onmousedown="++this.value">Save</button>
```

The `::input` declaration sets up an event listener. It updates the JSON as you type in the HTML `input` element.

`onmousedown="++this.value"` increments the `Save$` value in our JSON. That change can then be registered in our code-behind and trigger a handler.

Let's write that handler!

<div class="code-name">PersonJson.json.cs</div>
```cs
void Handle(Input.Save action)
{
    Transaction.Commit();
}
```
`Input.Save action` makes the method run when a change is detected in the Save value. Note that we do not need to use a `$` here like in the JSON. The rule is that we use `$` for the view, and view-model, but not in anything we write in C#.

`Transaction.Commit()` simply commits the input to the database so that the data is accessible from other sessions.

With server-side view-models, you don't have to write a single line of "glue code" to update the view in HTML. Any change in the view-model made in C# will instantly be synced to the client using PuppetJs, which in turn automatically renders because of Polymer's data bindings. This saves you from creating single-purpose REST APIs, need for double validation of user input, and more.

<aside class="read-more">
    <a href="/guides/puppet-web-apps/starcounter-mvvm.html">Learn about Starcounter's MVVM</a>
</aside>

The last step is to modify our `Program.cs` file to create a long running transaction that will allow us to make changes to our database multiple times during our session. We do that by wrapping everything in our `Handle.GET` inside a `Db.Scope`.

<aside class="read-more">
    <a href="/guides/transactions/long-running-transactions.html">Read more about long-running transactions</a>
</aside>

<div class="code-name">Program.cs</div><div class="code-name code-title">Add Db.Scope</div>
```cs
Handle.GET("/HelloWorld", () =>
{
    return Db.Scope(() =>
    {
        var person = Db.SQL<Person>("SELECT p FROM Person p").First;
        var json = new PersonJson()
        {
            Data = person
        };
        if (Session.Current == null)
        {
            Session.Current = new Session(SessionOptions.PatchVersioning);
        }

        json.Session = Session.Current;
        return json;
    });
});
```
We now have a program where we can change our view-model in real time and then commit our changes to the database at our will.

<section class="see-yourself">Start the application with <kbd>F5</kbd> and go to `http://localhost:8080/HelloWorld` in your web browser. You should see two input boxes with their respective label and a button below.</section>

If you are an especially curious person, you can try to change the name and then take a look at the database again with SQL. Here's how it should work:

![part 3 gif](/assets/page3resized.gif)

Neat! Right? The next step is to display the name change in real time and let the code-behind calculate the full name.

If you get any errors, you can check your code against the [source code](https://github.com/StarcounterSamples/HelloWorld/commit/3a6ccfff12daa109a1afad335565493a8fd2fc9a).
