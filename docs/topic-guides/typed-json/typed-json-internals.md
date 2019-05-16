# Typed JSON internals

## Introduction

Typed JSON consists of two parts, a template and an instance of the type `Json`. The template is an object that works as a schema which contains all properties and metadata for a Typed JSON object. This includes .Net types, names of the properties, data-binding and more.

## Templates and instances

| Template class | .Net type | Json type | Comment |
| :--- | :--- | :--- | :--- |
| TObject | Json | object | A single JSON object of type `Json` or subclass |
| TObjArr | Arr&lt;Json&gt; | array | An array of JSON-objects. Arr is a subclass of Json |
| TArray&lt;T&gt; | Arr&lt;T&gt; | array | An array of T, where T is `Json` or subclass |
| TBool | Bool | true, false | Value |
| TDecimal | Decimal | Number | Value |
| TDouble | Double | Number | Value |
| TLong | Int64 | Number | Value |
| TString | String | string | Value |

_Table of the available templates_

The templates control setting and getting values on the JSON instance. Templates have delegates that are created at runtime to get and set values. These delegates use .Net types, so there's no casting or boxing. This allows us to change how values are stored and retrieved without changing the code around it.

The following delegates exist on each template. They are not always instantiated though, for example, if a data binding is not used, the delegate for the bound value will be null.

**Unbound value \(UnboundGetter, UnboundSetter\)**  
Used internally. Gets and sets values stored on JSON. For basic JSON objects, the values will be stored in a .Net list directly on the instance.

**Bound value \(BoundGetter, BoundSetter\)**  
Used internally. Gets and sets values directly on the data object when data binding is used.

**Bound or unbound value \(Getter, Setter\)**  
Can be used in user code. Takes the most suitable bound or unbound delegate. Will always be available.

The delegates take the JSON instance as a parameter, and the setters an additional value parameter. In the example below, a method on the instance is called, but what it does is simply to take the template and call the delegate with the instance as a parameter.

`person.Set(tfullname, "John Doe")` will call `tfullname.Setter(person, "John Doe")`

### Example

 In this example, we create a template for a single JSON object, that describes a person with the properties `FullName` \(string\) and `Age` \(Int64\):

```csharp
using System;
using Starcounter;
using Starcounter.Templates;

class Program
{
    static void Main()
    {
        TObject schema = new TObject();
        TString tfullname = schema.Add<TString>("FullName");
        TLong tage = schema.Add<TLong>("Age");

        var person = new Json() { Template = schema };
        person.Set(tfullname, "John Doe");
        person.Set(tage, 33);

        Console.WriteLine(person.ToJson()); // => {"FullName":"John Doe","Age":33}
    }
}
```

## Comparing bound Typed JSON objects 

Whenever a data object is bound to a Typed JSON template, it checks whether the data object is the same as the previous data object. If they are, it checks any value differences and updates the JSON template accordingly. Otherwise, a new JSON instance will be created. Performance wise, only updating the value differences is faster. 

Reference or ID is used to determine if the existing and incoming data objects are the same. This works well when data objects are not coming from an external source. For example, if a data object is retrieved from a REST API and deserialized, it will always be considered a new object, even if the values are similar. Thus, a new JSON instance will always be created.

By overriding the default comparison, this can be solved. `SetDataObjectComparer` is an extension method on `TValue` that allows you to do that. It takes a `Func` with the parameters of the existing `Json` instance and the new data object and returns `true` if they're different and `false` otherwise. In this example, we override the default comparer by checking the value `SocialSecurityNum` to determine if it's a new object, in that way, a deserialized object would be classified the right way:

```csharp
using System;
using Starcounter;
using Starcounter.Templates;

[Database]
public class Person
{
    public string Name { get; set; }
    public long SocialSecurityNum { get; set; }
}

class Program
{
    static void Main()
    {
        Func<Json, object, bool> personComparer = 
            (Json instance, object obj) =>
        {
            var oldPerson = instance.Data as Person;
            var newPerson = obj as Person;

            if (oldPerson == null || newPerson == null)
                return false;

            return oldPerson.SocialSecurityNum == 
                   newPerson.SocialSecurityNum;
        };

        var schema = new TObject();
        schema.Add<TString>("Name");
        schema.Add<TLong>("SocialSecurityNum");

        schema.SetDataObjectComparer(personComparer);

        var personJson = new Json() { Template = schema };

        personJson.Data = Db.Transact(() =>
        {
            return new Person()
            {
                Name = "John",
                SocialSecurityNum = 188011201234
            };
        });
    }
}
```

## Populating Typed JSON from JSON

Typed JSON objects can be populated from JSON strings with the method `PopulateFromJson`. This is, for example, useful when receiving JSON from an external service. 

```javascript
  {
    "Name": ""
  }
```

```csharp
var person = new PersonPage();
person.PopulateFromJson("{ \"Name\": \"John\"}");
Console.WriteLine(person.ToJson()); // => {"Name":"John"}
```

The JSON has to be valid and the properties have to match the properties in the Typed JSON schema. Otherwise, Starcounter throws either `ScErrInvalidJsonForInput (SCERR14007)` or `ScErrJsonPropertyNotFound (SCERR14003)`.  
  
There's also an overload of `PopulateFromJson` that takes a byte array and a source size instead of a string.

   

