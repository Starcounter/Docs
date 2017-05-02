# Create a Real Time UI

To create a user interface (UI) we need to add a view-model and a corresponding view. The view-model will act as a representation of the view and as a midpoint between the view and the database.

## Setup the File Structure

Let's create our file structure that contains our view (HTML) and view-model (JSON). If you follow these steps you will have the right files to build the UI:

1. In the Solution Explorer in Visual Studio, right click on the project file named `HelloWorld` and add a folder with the name `wwwroot`.
2. Inside this folder, you create another folder with the name `HelloWorld` the same name as the project.
3. Add an HTML file into this folder by right-clicking and then choosing `Add -> New Item... -> Starcounter -> Starcounter HTML template with dom-bind`. Name this file `PersonJson.html`.
4. Add a JSON file into the root of your project. You can add this by going to `Add -> New Item... -> Starcounter -> Starcounter Typed JSON with Code-behind`. This will create a `.json` and `.json.cs` file. Both of these should have the name PersonJson.

![file structure](/assets/file-structure.PNG)

Now that we have a solid file structure we can continue by creating our view.

## Define the View

Our view will be, for now, a simple interface displaying the `FirstName` and `LastName` of our `Person` instance.

To accomplish this we will use the `label` and `strong` tags. It's not essential that you use exactly these elements. `p`, `span`, and many others would work just as well. To make Polymer's template engine work, we also have to import it into our file with the `rel="import"`.

<div class="code-name">PersonJson.html</div><div class="code-name code-title">Create view</div>

{% raw %}
```html
<link rel="import" href="/sys/polymer/polymer.html"/>
<template>
    <template is="dom-bind">
        <fieldset>
            <label>First name:</label>
            <strong>{{model.FirstName}}</strong>
        </fieldset>

        <fieldset>
            <label>Last name:</label>
            <strong>{{model.LastName}}</strong>
        </fieldset>
    </template>
</template>
```

The double curly bracket syntax is a way of denoting [two-way bindings in Polymer](https://www.polymer-project.org/1.0/docs/devguide/data-binding). `model` represents our JSON file, so `{{model.FirstName}}` is the `FirstName` value in `PersonJson.json`.
{% endraw %}

## Define the View-Model

In the JSON file, create three properties called `Html`, `FirstName`, and `LastName`. The values of these properties will be the values that are bound to the Polymer bindings that we just created _and_ the database. It is therefore crucial, for this example, that you name these keys the same as the properties that we have in our Person class, otherwise they will not bind properly. Learn more about JSON binding in the [docs](/guides/typed-json/json-data-bindings).

<div class="code-name">PersonJson.json</div><div class="code-name code-title">Set JSON</div>

```json
{
  "Html": "/HelloWorld/PersonJson.html",
  "FirstName": "",
  "LastName": ""
}
```

## Establish the Handler

Go to `Program.cs` and type in the following code inside the `Main()` method. This code adds the correct information to our previously empty JSON file and creates a new [session](/guides/web-apps/sessions).

<div class="code-name">Program.cs</div><div class="code-name code-title">Bind JSON</div>

```cs
Application.Current.Use(new HtmlFromJsonProvider());
Application.Current.Use(new PartialToStandaloneHtmlProvider());

Handle.GET("/HelloWorld", () =>
{
    var person = Db.SQL<Person>("SELECT p FROM Person p").First;
    var json = new PersonJson
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
```
[`Application.Current.Use(new HtmlFromJsonProvider())`](/guides/network/middleware/#htmlfromjsonprovider) looks in the JSON file for the `Html` property and sends the document found at that path to the web browser.

[`Application.Current.Use(new PartialToStandaloneHtmlProvider());`](/guides/network/middleware/#partialtostandalonehtmlprovider) wraps the HTML template to form a complete HTML document.

## Result

We have established a real-time model-view-view-model (MVVM) binding. The JSON, which is our view-model, is bound to the model (database) with no latency; our view, the HTML, is in turn bound to the JSON, which is synced in real time using WebSocket and HTTP. Polymer helps us display this instantaneously to the user.

Check out how it looks by starting the application with <kbd>F5</kbd> and go to `http://localhost:8080/HelloWorld` in your web browser.
![Screenshot part 2](/assets/part2.png)

It's impossible for us to see the immediate changes as there is no way for the user to change the info. Let us fix that by adding some interactivity!  

If you get any errors, you can check your code against the [source code](https://github.com/StarcounterApps/HelloWorld/commit/9bc0f676fe1f986965aa2cf01c2f290759166d35).
