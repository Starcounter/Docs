# JSON-by-example

Starcounter lets you define JSON schemas by providing a sample instance of the JSON. From this example, Starcounter infers partial nested classes that can be extended using C#. The values of the primitive properties are used as default values.  

The advantages of JSON-by-example over regular C# classes are mainly:

* They can double directly as a JSON mockup to send where that is expected, such as in a web browser
* They can express trees of objects and arrays
* Default values can easily be specified

## Create JSON-by-example

To create a Typed JSON class, choose `New item` in Visual Studio and then select `Starcounter Typed JSON`. The created file contains an empty JSON object. 

One of the simplest JSON-by-example files look like this:

<div class="code-name">Person.json</div>

```json
{
    "FirstName": "",
    "LastName": ""
}
```

Here, the value is set to an empty string as a way to set the type of `Name` to string. It is also possible to set a default value instead.

### Default Values

It is incredibly simple to set the default value in JSON-by-example. Building on the previous code example, it might look like this:

<div class="code-name">Person.json</div>

```json 
{
    "FirstName": "Steven", 
    "LastName": "Smith"
}
```

By doing this, the JSON returned when creating a new `Person` object will be `{"FirstName":"Steven","LastName":"Smith"}`:

<div class="code-name">Program.cs</div>

```cs
Handle.GET("/GetPerson", () =>
{
    return new Person(); // {"FirstName":"Steven","LastName":"Smith"}
});
```

### Instantiating JSON-by-example

Since JSON-by-example is turned into C# classes, instantiation works the same way as other C# objects.

With the JSON-by-example file above, it would be instantiated this way:

```cs
new Person()
{
    First = "Steven",
    LastName = "Smith"
}
```

The resulting JSON will look like this: `{"FirstName":"Steven","LastName":"Smith"}`.

#### Adding Values to Arrays

<div class="code-name">Program.cs</div>

```cs
Handle.GET("/Hello", () =>
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

The JSON returned from the above handler looks like this:

```json
{"FirstName":"Albert","LastName":"Einstein","Quotes":["Makes things as simple as possible but not simpler.","The only real valuable thing is intuition."]}
```

## Writable JSON Values

By default, all the values declared in JSON-by-example are read-only for the client. Any client-side change to a read-only property will result in an error.

To mark a specific value as writable by the client, add a dollar sign (`$`) at the end of the property name, e.g.:

```json
{
   "Html": "",
   "FirstName$": "",
   "LastName$": "",
   "Message": ""
}
```
