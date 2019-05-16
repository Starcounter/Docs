# JSON-by-example

## Introduction

JSON-by-example defines Typed JSON objects.

It works by providing a sample instance of JSON that transpiles into Typed JSON classes. You can find these generated classes in the `obj > x64 > Debug` or `obj > x64 > Release` directory of the project with the filename extension `json.g.cs`.

JSON-by-example is useful for these reasons:

* It can double directly as JSON mockups
* It can express trees of objects and arrays
* It's easy to specify default values

## Create JSON-by-example

To create a Typed JSON class, choose `New item`, or use Ctrl + Shift + A in Visual Studio and then select `Starcounter -> Starcounter Typed JSON`. The created file contains an empty JSON object which is the JSON-by-example.

One of the simplest JSON-by-example files look like this:

{% code-tabs %}
{% code-tabs-item title="PersonPage.json" %}
```javascript
{
    "FirstName": "",
    "LastName": ""
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

Here, we set the value to an empty string to declare the type.

You create an instance of the generated Typed JSON with a normal constructor call: `new PersonPage()`.

Accesing the properties of Typed JSON is the same as with any C\# object:

```csharp
var personPage = new PersonPage();
string name = personPage.FirstName; // Contains the value "", an empty string
```

## Default Values

It's simple to set default values in JSON-by-example. Building on the previous code example, it might look like this:

{% code-tabs %}
{% code-tabs-item title="PersonPage.json" %}
```javascript
{
    "FirstName": "Steven", 
    "LastName": "Smith"
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

By doing this, the JSON returned when creating a new `PersonPage` object will be `{"FirstName":"Steven","LastName":"Smith"}`:

```csharp
Handle.GET("/GetPerson", () =>
{
    return new PersonPage(); // {"FirstName":"Steven","LastName":"Smith"}
});
```

## Supported Datatypes

Typed JSON follows the specification of JSON, which means that objects, arrays and single values are all allowed. One difference is that when working with the C\#-object and numbers we have the possibility to specify a more exact type. What in JSON is `Number`, splits up in `Int64`, `Double` and `Decimal`.

The following is a list of the tokens in JSON and the equivalence in C\#:

| JSON | C\# |
| :--- | :--- |
| `{ }` | Object |
| `[ ]` | Array |
| `"value"` | String |
| `123` | Int64 |
| `true`/`false` | Boolean |
| `1.234` and `2E3` | Decimal |

To specify the type of a member in JSON-by-example, define it in the code-behind:

```javascript
{
  "Value": 2E3 // will parse as decimal by default.
}
```

```csharp
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

To mark a specific value as writable by the client, add a dollar sign \(`$`\) at the end of the property name, e.g.:

```javascript
{
   "FirstName$": "",
   "LastName$": ""
}
```

## Trigger Properties

Trigger properties is a common use for writable JSON properties. They notify the code-behind that a change has happened. Here's an example of a trigger property:

```javascript
{
    "FirstName": "",
    "LastName": "",
    "SaveTrigger$": 0
}
```

An incrementation in `SaveTrigger$`

## The HTML Property

In all the [sample apps](https://github.com/search?q=topic%3Aapp+org%3AStarcounter&type=Repositories), there is an "Html" property in every, or almost every, `.json` file. The value of this property contains the path to the corresponding HTML view which means that the middleware [HtmlFromJsonProvider](../network/middleware.md#htmlfromjsonprovider) can locate this HTML view and send it to the client. This allows the developer to return a Typed JSON object from a handler and still return the corresponding view as well.

