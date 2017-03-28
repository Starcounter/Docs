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

### Adding Values

It is allowed to specify JSON-by-example that contains of a single primitive value, object or array. In C# all of these are handled in the same way. A TypedJson instance is created and values/properties are read and written in the same way both for single values and complex objects and arrays.

For setting and getting single values a set of predefined properties exists, both to handle the value and to check what type a TypedJson-instance is. If a value that does not correspond to the type of Json is used an exception will be raised. For example trying to read `DecimalValue` on  a jsonobject that holds a string will not work.

The following properties are used to get and set primitive values: `BooleanValue`, `DecimalValue`, `DoubleValue`, `IntegerValue`, `StringValue`

There are also properties to check in an efficient way what type a Json-instance is: `IsBoolean`, `IsDecimal`, `IsDouble`, `IsInteger`, `IsString`, `IsObject`, `IsArray`

#### Setting Single Primitive Values Example

To solidify the understanding of how setting single primitive values works, take a look at this example where a single primitive value is changed by first creating a JSON file with a single value and then creating an instance of it that is type checked and changed:

Single primitive values can be 

<div class="code-name">SingleValueJson.json</div>

```
"Default"
```

<div class="code-name">Program.cs</div>

```cs
public static void Main()
{
    Handle.GET("/GetSingleValue", () =>
    {
        var json = new SingleValueJson();
        bool isString = json.IsString; // True
        string stringValue = json.StringValue; // "Default"
        json.StringValue = "Changed";
        stringValue = json.StringValue; // "Changed"
        return json.ToJson(); // "Changed"
    });
}
```

#### Adding Values to Arrays Example

A single array containing integers. We add two items and print.

<div class="code-name">SingleArrayJson.json</div>

```json
[ 99 ]
```

<div class="code-name">Program.cs</div>

```cs
public static void Main()
{
    Handle.GET("/GetSingleArray", () =>
    {
        var json = new SingleArrayJson();

        json.Add().IntegerValue = 1; // Adding an item to the array.
        json.Add().IntegerValue = 2; // Adding another item.

        return json; // [1,2]
    });
}
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
