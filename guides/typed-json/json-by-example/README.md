# JSON-by-example

To define the Typed JSON objects, JSON-by-example is used. 

JSON-by-example works by providing a sample instance of JSON that is transpiled into Typed JSON classes. For the curious, these classes can be found in the `obj > x64 > Debug` or `obj > x64 > Release` directory of the project with the filename extension `.json.g.cs`.

JSON-by-example is useful for a number of reasons:

* It can double directly as JSON mockups
* It can express trees of objects and arrays
* Default values can easily be specified

## Create JSON-by-example

To create a Typed JSON class, choose `New item`, or use <key>Ctrl + Shift + A</key> in Visual Studio and then select `Starcounter -> Starcounter Typed JSON`. The created file contains an empty JSON object which is the JSON-by-example. 

One of the simplest JSON-by-example files look like this:

<div class="code-name">PersonPage.json</div>

```json
{
    "FirstName": "",
    "LastName": ""
}
```

Here, the value is set to an empty string to declare the type as a string.

An instance of the Typed JSON created from this JSON-by-example is created the same an instance would be created for any other class: `new PersonPage()`.

The value of the properties in the Typed JSON object can be accessed in the ordinary way:

```cs
var personPage = new PersonPage();
string name = personPage.FirstName; // Contains the value "", an empty string
```

### Default Values

It is incredibly simple to set default values in JSON-by-example. Building on the previous code example, it might look like this:

<div class="code-name">PersonPage.json</div>

```json 
{
    "FirstName": "Steven", 
    "LastName": "Smith"
}
```

By doing this, the JSON returned when creating a new `PersonPage` object will be `{"FirstName":"Steven","LastName":"Smith"}`:

<div class="code-name">Program.cs</div>

```cs
Handle.GET("/GetPerson", () =>
{
    return new PersonPage(); // {"FirstName":"Steven","LastName":"Smith"}
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
    static Foo()
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
   "FirstName$": "",
   "LastName$": ""
}
```

### Trigger Properties

A common use of writable JSON values is trigger properties. These properties carry no specific meaning, they are simply there to notify the [code-behind](/guides/typed-json/code-behind/) that a change has happened. Here's an example of a trigger property:

```json
{
    "FirstName": "",
    "LastName": "",
    "SaveTrigger$": 0
}
```

The value of this trigger would be incremented from the client which allows the code-behind to react accordingly.

## The HTML property

In all the [sample apps](https://github.com/StarcounterApps/), there is an "Html" property in every, or almost every, `.json` file. The value of this property contains the path to the corresponding HTML view which means that the middleware [HtmlFromJsonProvider](/guides/network/middleware/#htmlfromjsonprovider) can locate this HTML view and send it to the client. This allows the developer to only return a Typed JSON object from a handler and still return the corresponding view as well. 