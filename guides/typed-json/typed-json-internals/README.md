# Typed JSON Internals

## Templates and Instances
Typed JSON consists of two parts, a template and an instance of the type `Json`. The template is an object that works as a schema which contains all properties and metadata for a Typed JSON object. This includes .Net types, names of the properties, databinding and more.

| Template class | .Net type | Json type | Comment
| -------------- | --------- | --------- | --------
| TObject        | Json | object | A single JSON object of type `Json` or subclass
| TObjArr        | Arr<Json> | array | An array of JSON-objects. Arr is a subclass of Json
| TArray<T> | Arr<T> | array | An array of T, where T is `Json` or subclass
| TBool          | Bool      | true, false | Value
| TDecimal       | Decimal   | Number | Value
| TDouble        | Double    | Number | Value
| TLong          | Int64     | Number | Value
| TString        | String    | string | Value

*Table of the available templates*

The templates control setting and getting values on the JSON instance. Templates have delegates that are created at runtime to get and set values. These delegates use .Net types, so there's no casting or boxing. This allows us to change how values are stored and retrieved without changing the code around it.

The following delegates exist on each template. They are not always instantiated though, for example, if a data binding is not used, the delegate for the bound value will be null.

**Unbound value (UnboundGetter, UnboundSetter)**
Used internally. Gets and sets values stored on JSON. For basic JSON objects, the values will be stored in a .Net list directly on the instance.

**Bound value (BoundGetter, BoundSetter)**
Used internally. Gets and sets values directly on the data object when data binding is used.

**Bound or unbound value (Getter, Setter)**
Can be used in user code. Takes the most suitable bound or unbound delegate. Will always be available.

The delegates take the JSON instance as a parameter, and the setters an additional value parameter. In the example below, a method on the instance is called, but what it does is simply to take the template and call the delegate with the instance as  a parameter.

`person.Set(tfullname, "John Doe")` will call `tfullname.Setter(person, "John Doe")`

## Example
In this example, we create a template for a single JSON object, that describes a person with the properties `FullName` (string) and `Age` (Int64):

```cs
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
