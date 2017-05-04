# Create a Real Time UI

To create a user interface (UI), we establish a [MVVM](https://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93viewmodel) pattern by adding a view-model and a corresponding view. The view-model will act as a representation of the view and as a midpoint between the view and the database.

## Setup the File Structure

Let's create the file structure to contain a view and a view-model. These steps should be followed to create the structure:

1. In the Solution Explorer in Visual Studio, right click on the project file named `HelloWorld` and add a folder with the name `wwwroot`.
2. Inside this folder, create another folder with the name `HelloWorld`, the same name as the project.
3. Add an HTML file, which will be the view, into this folder by right-clicking and then choosing `Add -> New Item... -> Starcounter -> Starcounter HTML template with dom-bind`. Name this file `PersonJson.html`.
4. Add a JSON file, which will be the view-model, into the root of the project. This can be added by going to `Add -> New Item... -> Starcounter -> Starcounter Typed JSON with Code-behind`. Doing this creates a `json` and `json.cs` file. Both of these should have the name PersonJson.

![file structure](/assets/file-structure.PNG)

With a solid file strucutre, we can continue by creating the view-model.

## Define the View-Model

In the JSON file, create three properties called `Html`, `FirstName`, and `LastName`. The values of the properties `FirstName` and `LastName` will be bound to the view _and_ the database. Because of that, it is crucial, for this example, that you name these properties the same as the properties that we have in the `Person` class. That will allow Starcounter to recognize that the `Person` object and the `PersonJson` view-model represents the same things and bind the values of their properties. Learn more about this in the [docs](/guides/typed-json/json-data-bindings).

The value of the `Html` property is the path to the view that the view-model should be connected to.

<div class="code-name">PersonJson.json</div>

```json
{
  "Html": "/HelloWorld/PersonJson.html",
  "FirstName": "",
  "LastName": ""
}
```

## Create the HTTP Handler

To get the view-model and the corresponding view to the client, an HTTP handler has to be created. This handler sets the specific database object that the view-model should be bound to, creates a [session](/guides/web-apps/sessions), and returns the view-model to the client.

Note that this handler will only return the simple JSON tree that we defined earlier and not any HTML, which is what we want to render. To solve this issue, [middleware](/guides/network/middleware) is used. For this app, and most other Starcounter apps, the `HtmlFromJsonProvider` and `PartialToStandaloneHtmlProvider` middleware is used. These affect the pipeline by catching the outgoing JSON tree, finding the HTML at the defined `Html` path, wrapping the HTML to form a complete HTML document, and forwarding it to the client. By doing this, a complete HTML document can be sent to the client, even if the only thing that's returned from the handler is a simple JSON tree. 

This is how the handler and middleware looks in code:

<div class="code-name">Program.cs</div>

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

{% raw %}

## Define the View

The view will initially be a simple interface displaying the `FirstName` and `LastName` of a `Person` instance.

Since the middleware wraps the HTML to create a complete HTML document, we only need to define an HTML fragment wrapped in the `template` element.

To create a two-way binding between the view and the view model, we choose to use Polymer. There are three parts needed to make the bindings work:
1. Import Polymer
2. Use `<template is="dom-bind">` to allow the use of bindings in a fragment
3. Bind the specific properties using the double bracket syntax like this: `{{model.FirstName}}`. `model` represents the JSON file, so `{{model.FirstName}}` is the `FirstName` value in `PersonJson.json`.

This is how it looks in code:

<div class="code-name">PersonJson.html</div>

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

{% endraw %}

## Result

Now, we have established a real-time model-view-view-model (MVVM) binding. The JSON, which is our view-model, is bound to the model (database) with no latency; our view, the HTML, is in turn bound to the JSON, which is synced in real time using WebSocket and HTTP. Polymer helps us display this instantaneously to the user.

Check out how it looks by starting the application with <kbd>F5</kbd> and go to `http://localhost:8080/HelloWorld` in your web browser.
![Screenshot part 2](/assets/part2.png)

It's impossible for us to see the immediate changes as there is no way for the user to change the info. Let us fix that by adding some interactivity!  

If you get any errors, you can check your code against the [source code](https://github.com/StarcounterApps/HelloWorld/commit/ce3e787313aacbd6d8f6d18956ab39e24befc452).
