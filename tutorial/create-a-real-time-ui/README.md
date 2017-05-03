# Create a Real Time UI

To create a user interface (UI), we establish a [MVVM](https://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93viewmodel) pattern by adding a view-model and a corresponding view. The view-model will act as a representation of the view and as a midpoint between the view and the database.

## Setup the File Structure

Let's create the file structure to contain a view and a view-model. These steps should be followed to create the structure:

1. In the Solution Explorer in Visual Studio, right click on the project file named `HelloWorld` and add a folder with the name `wwwroot`.
2. Inside this folder, create another folder with the name `HelloWorld`, the same name as the project.
3. Add an HTML file, which will be the view, into this folder by right-clicking and then choosing `Add -> New Item... -> Starcounter -> Starcounter HTML template with dom-bind`. Name this file `PersonJson.html`.
4. Add a JSON file, which will be the view-model, into the root of the project. This can be added by going to `Add -> New Item... -> Starcounter -> Starcounter Typed JSON with Code-behind`. Doing this creates a `json` and `json.cs` file. Both of these should have the name PersonJson.

![file structure](/assets/file-structure.PNG)

With a solid file strucutre, we can continue by creating the view.

## Define the View

The view will initially be a simple interface displaying the `FirstName` and `LastName` of a `Person` instance.

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

## Create the HTTP Handler

Go to `Program.cs` and type in the following code inside the `Main()` method.

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

The handler `/HelloWorld` binds a `Person` database object to the view-model and attaches a [session](/guides/web-apps/sessions) to be able to have real-time synching using [WebSocket](/guides/network/websocket/). It then returns that view-model.

[`Application.Current.Use(new HtmlFromJsonProvider())`](/guides/network/middleware/#htmlfromjsonprovider) intercepts the view-model, looks for the `Html` property and sends the document found at that path to the web browser.

[`Application.Current.Use(new PartialToStandaloneHtmlProvider());`](/guides/network/middleware/#partialtostandalonehtmlprovider) wraps the HTML template coming from `HtmlFromJsonProvider` to form a complete HTML document.

With this structure, a complete HTML document can be sent to the client, even if the only thing that's returned from the handler is a simple JSON tree. 

## Result

We have established a real-time model-view-view-model (MVVM) binding. The JSON, which is our view-model, is bound to the model (database) with no latency; our view, the HTML, is in turn bound to the JSON, which is synced in real time using WebSocket and HTTP. Polymer helps us display this instantaneously to the user.

Check out how it looks by starting the application with <kbd>F5</kbd> and go to `http://localhost:8080/HelloWorld` in your web browser.
![Screenshot part 2](/assets/part2.png)

It's impossible for us to see the immediate changes as there is no way for the user to change the info. Let us fix that by adding some interactivity!  

If you get any errors, you can check your code against the [source code](https://github.com/StarcounterApps/HelloWorld/commit/9bc0f676fe1f986965aa2cf01c2f290759166d35).
