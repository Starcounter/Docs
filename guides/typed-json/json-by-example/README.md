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

## Supported datatypes

Typed JSON follows the specification of JSON, which means that objects, arrays and single values are all allowed. One difference is that when working with the C#-object and numbers we have the possibility to specify a more exact type. So what in JSON is `Number` we split up in `Int64`, `Double` and `Decimal`.

The following is a list of the tokens in JSON and the equivalence in C#:

| JSON | C# |
|----------------|---------|
| `{ }` | Object |
| `[ ]` | Array |
| `"value"` | String |
| `123` | Int64 |
| `true`/`false` | Boolean |
| `1.234` and `2E3` | Decimal |

To specify that a member in Json-by-example should be of type `Double` is done in the code-behind file.

<div class="code-name">Foo.json</div>

```js
{
  "Value": 2E3 // will be parsed as decimal by default.
}
```

<div class="code-name">Foo.json.cs</div>

```cs
partial class Foo : Json
{
    static void Foo()
    {
    	// Value should be of type double, not decimal.
        DefaultTemplate.Value.InstanceType = typeof(double);
    }
}
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
