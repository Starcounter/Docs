# JSON-by-example

Starcounter lets you define JSON schemas by providing a sample instance of the JSON. From this example, Starcounter infers partial nested classes that can be extended using C\#. The values of the primitive properties are used as default values.

The advantages of JSON-by-example over regular C\# classes are mainly:

* They can double directly as a JSON mockup \(for instance in a web browser expecting such a JSON object\).
* They can express trees of objects and arrays
* Default values can easily be specified

To create a Typed JSON class, choose New item in Visual Studio and then choose Starcounter Typed JSON and name it **PersonMsg**.



```javascript
{
   "FirstName": "Jocke",
   "LastName": "Wester",
   "Quotes": [
        { "Text": "This is an example" }
   ]
}
```

JSON-by-example does not require that property names are inside double quotes \(they still get serialized and deserialized using double quoted properties\).

The above example will act as partial nested C\# classes supporting intelligence and type safe compilation.



```csharp
using Starcounter;

class Hello
{
   static void Main()
   {
      Handle.GET("/hello", () =>
      {
         var json = new PersonMsg()
         {
            FirstName = "Albert",
            LastName = "Einstein"
         };
         json.Quotes.Add().Text = "Makes things as simple as possible but not simpler.";
         json.Quotes.Add().Text = "The only real valuable thing is intuition.";
         return json;
      });         
   }
}
```

### Read-only and writable values

By default, all the values declared in JSON-by-example are read-only for the client. Any client-side change to a read-only property will result in an error.

To mark a specific value as writable by the client, add a dollar sign \(`$`\) at the end of the property name, e.g.:



```javascript
{
   "Html": "",
   "FirstName$": "",
   "LastName$": "",
   "Message": ""
}
```

