# JSON-by-example

Starcounter lets you define JSON schemas by providing a sample instance of the JSON. From this example, Starcounter infers partial nested classes that can be extended using C#. The values of the primitive properties are used as default values.  

The advantages of JSON-by-example over regular C# classes are mainly:

* They can double directly as a JSON mockup (for instance in a web browser expecting such a JSON object).
* They can express trees of objects and arrays
* Default values can easily be specified

To create a Typed JSON class, choose `New item` in Visual Studio and then choose `Starcounter Typed JSON`. In this example, the file will be named `PersonMessage`.

<div class="code-name">PersonMessage.json</div>

```json
{
   "FirstName": "",
   "LastName": "",
   "Quotes": [ ]
}
```

The above example will act as partial nested C# classes supporting intelligence and type safe compilation.

<div class="code-name">Program.cs</div>

```cs
Handle.GET("/hello", () =>
{
    var json = new PersonMessage()
    {
        FirstName = "Albert",
        LastName = "Einstein"
    };
    json.Quotes.Add().StringValue = "Makes things as simple as possible but not simpler.";
    json.Quotes.Add().StringValue = "The only real valuable thing is intuition.";
    return json;
});         
```

The JSON returned from the above handler look like this:

```json
{"FirstName":"Albert","LastName":"Einstein","Quotes":["Makes things as simple as possible but not simpler.","The only real valuable thing is intuition."]}
```

## Read-only and writable values

By default, all the values declared in JSON-by-example are read-only for the client. Any client-side change to a read-only property will result in an error.

To mark a specific value as writable by the client, add a dollar sign (`$`) at the end of the property name, e.g.:

<div class="code-name">PersonView.json</div>

```json
{
   "Html": "",
   "FirstName$": "",
   "LastName$": "",
   "Message": ""
}
```
