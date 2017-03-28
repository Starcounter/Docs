# Typed JSON

There are three ways to create Typed JSON objects in Starcounter:

1. [JSON-by-example](/guides/typed-json/json-by-example) (in JSON file) - this is the preferred way
2. dynamic (in C# code, defined using dynamic properties)
3. dynamic (in C# code, defined using string as constructor parameter)

## JSON properties

Properties of the view-model have to be bound to (in dynamic JSON) Common Language Runtime or (in static JSON) code-behind file. In the section [JSON data binding](/guides/typed-json/json-data-bindings), data bindings are explained in-depth, and how to avoid manual value transfer into the view-model.

If a JSON object is static, it is beneficial to denote a specific type that is used as data-object. Otherwise, opposite to dynamic JSON objects, faulty static properties will result in compilation error instead of runtime. Moreover, it allows binding data properties to the correct types without manual involvement.

There are two ways of doing that:
1. Set a metadata inside the view-model
2. Use the `IBound` interface to tag the code-behind class

{% import "../../macros.html" as macros %}

{{ macros.tocGenerator(page.title, summary.parts[0].articles[3].articles[3].articles) }}
