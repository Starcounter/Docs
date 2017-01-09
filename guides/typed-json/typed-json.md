# Typed JSON

There are three ways to create Typed JSON objects in Starcounter:

1. [JSON-by-example](/guides/typed-json/json-by-example.html) (in JSON file) - this is the preferred way
2. dynamic (in C# code, defined using dynamic properties)
3. dynamic (in C# code, defined using string as constructor parameter)

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
| `1.234` | Decimal |
| `2E3` | Double |

**NOTE:** From version 2.3.0.4343 (and forward) the syntax for double and decimal types have changed. The default C# type will always be `Decimal` for values containing a decimal point or exponent.

To specify that a member in Json-by-example should be of type `Double` is done in the code-behind file.

*Foo.json*
```javascript
{
  "Value": 2E3 // will be parsed as decimal by default.
}
```

*Foo.json.cs*
```csharp
partial class Foo : Json {
    static void Foo(){
    	// Value should be of type double, not decimal.
        DefaultTemplate.Value.InstanceType = typeof(double);
    }
}
```

## JSON properties

Properties of the view-model have to be bound to (in dynamic JSON) Common Language Runtime or (in static JSON) code-behind file. In <a href="/guides/typed-json/json-data-binding.html">JSON data binding</a> we explain data bindings in-depth, and how to avoid manual value transfer into the view-model.

If JSON object is static it is beneficial to denote a specific type that is used as data-object. Otherwise, opposite to dynamic JSON objects, faulty static properties will result in compilation error instead of runtime. Moreover, it allows binding data properties to the correct types without manual involvement.
<a href="/guides/typed-json/json-data-binding.html">There are two ways of doing that:</a>
<ul>
1. Set a metadata inside the view-model
2. Use the <code>IBound interface</code> to tag the code-behind class</ul>
