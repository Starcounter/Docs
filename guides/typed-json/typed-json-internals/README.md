# Typed JSON Internals

Typed Json is a way to manipulate and work with json-documents in an object oriented manner, with considerations for performance and ease of use. It is fully compatible with the standard json specification (http://www.json.org/) and can be converted to and from different representations, for example to and from json as a utf8 string.

### Templates and instances
In its most basic form, typed json consists of two parts, a template and an instance of the type `Json`. The template is an object that works as a schema that contains all properties and metadata for a typed json object. This includes .Net types, names of the properties, databinding and more.

<!-- <p><font color='red'>TODO: picture </font></p> -->

| Template class | .Net type | Json type | Comment
| -------------- | --------- | --------- | --------
| TObject        | Json | object | A single json-object of type Json or subclass
| TObjArr        | Arr&lt;Json&gt; | array | An array of json-objects. Arr is a subclass of Json
| TArray&lt;T&gt; | Arr&lt;T&gt; | array | An array of T, where T is Json or subclass
| TBool          | Bool      | true, false | Value
| TDecimal       | Decimal   | Number | Value
| TDouble        | Double    | Number | Value
| TLong          | Int64     | Number | Value
| TString        | String    | string | Value

*Table of the different templates that are available*

The templates also controls setting and getting values on the json-instance. Each template have delegates that are created runtime, using Expression trees that are compiled to methods (http://msdn.microsoft.com/en-us/library/bb397951.aspx) to get and set values. Each delegate uses the correct .Net type directly so no casts or boxing of values will occur. This allow us to change the core behaviour on how values are stored and retrieved without changing the code around it and makes it easier to implement additional features.

The following delegates exists on each template. They are not always instantiated though, for example if databinding is not used the delegate for bound value will be null.

**Unbound value (UnboundGetter, UnboundSetter)**
Used internally. Gets and sets values stored on json. For basic jsonobjects the values will be stored in a .Net list directly on the instance.

**Bound value (BoundGetter, BoundSetter)**
Used internally. Gets and sets values directly on the dataobject when databinding is used (databinding will be covered later)

**Bound or unbound value (Getter, Setter)**
Can be used in usercode. Takes the most suitable bound or unbound delegate. Will always be available.

The delegates takes the json-instance as parameter, and the setters an additional value parameter. In the example below a method on the instance is called, but what it does is simply to take the template and call the delegate with the instance as parameter.

`person.Set(tfullname, "John Doe");` will call `tfullname.Setter(person, "John Doe");`


<!-- <p><font color='red'>TODO: picture</font></p> -->

### Example
In this example we create a template for a single json-object, that describes a person, containing the properties FullName (string) and Age (Int64):

```cs
using System;
using Starcounter;
using Starcounter.Templates;

namespace StarcounterApplication3
{
    class Program
    {
        static void Main()
        {
            TObject schema = new TObject();
            TString tfullname = schema.Add<TString>("FullName");
            TLong tage = schema.Add<TLong>("Age");

            Json person = new Json() { Template = schema };
            person.Set(tfullname, "John Doe");
            person.Set(tage, 33);

            Console.WriteLine(person.ToJson());
        }
    }
}
```
In the last line we convert the typedjson to a string-representation of the typedjson object. The output will be:
`{"FullName":"John Doe","Age":33}`

### Next
It's important to understand this basic structure of templates and instances. All other features for typed json, that will be described in later texts use this core implementation. For example Dynamic typed json will create the templates automatically when needed and JSON-by-example will parse a jsonfile and convert it to templates.
