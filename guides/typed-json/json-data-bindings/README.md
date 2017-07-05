# Data Bindings

Properties declared in Typed JSON can be bound to either a property in the code-behind file or a CLR object that exposes one or more public properties. Properties that are bound will read and write the values directly to the underlying object with no need to manually transfer the values to the view-model.

## Default Bindings

### Binding to Database Objects

To bind a Typed JSON object to a database object, the `Data` property is used. 

Consider the following JSON file:

<div class="code-name">PersonPage.json</div>

```json
{
   "FirstName": "",
   "LastName": "",
}
```

To bind the Typed JSON object `PersonPage` defined above to a database class `Person`, the following code can be used:

<div class="code-name">Program.cs</div> 

```cs
using Starcounter;

namespace MyApp
{
    [Database]
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => FirstName + " " + LastName;
    }

    class Program
    {
        static void Main()
        {
            Db.Transact(() => // Add a new Person instance to the database
            {
                new Person()
                {
                    FirstName = "Steve",
                    LastName = "Smith"
                };
            });

            Handle.GET("/GetPerson", () =>
            {
                var person = Db.SQL<Person>("SELECT P FROM Person P").First; // Retrieve a database object from the database

                var json = new PersonPage() 
                {
                    Data = person // Bind the database object to the Typed JSON object
                };

                return json;
            });
        }
    }
}
```

The `PersonPage` object will now look like this: `{"FirstName":"Steve","LastName":"Smith","FullName":"Steve Smith"}`.

Starcounter recognizes that the properties in `PersonPage` and `Person` object have the same name and populates the values in the Typed JSON accordingly. This is the default way that bindings are created.

### Binding to Code-Behind Properties

In addition to binding to database objects, Typed JSON properties can also be bound to code-behind properties.

To accomplish what was demonstrated in the [previous example](#binding-to-database-objects) by using code-behind properties instead of database properties, the following code can be used:

<div class="code-name">PersonPage.json</div>

```json
{
   "FirstName": "Steven",
   "LastName": "Smith",
   "FullName": ""
}
```

<div class="code-name">PersonPage.json.cs</div>

```cs
using Starcounter;

namespace MyApp
{
    partial class PersonPage : Json
    {
        public string FullName => FirstName + " " + LastName;
    }
}
```

<div class="code-name">Program.cs</div>

```cs
using Starcounter;

namespace MyApp
{
    class Program
    {
        static void Main()
        {
            Handle.GET("/GetPerson", () =>
            {
                return new PersonPage(); // {"FirstName":"Steven","LastName":"Smith","FullName":"Steven Smith"}
            });
        }
    }
}
```

### Mixing Database and Code-Behind Bindings

When mixing database and code-behind bindings, the code-behind bindings take presidence. This means that if there is a database property and code-behind property with the same name, the code-behind property will be the one that's used. 

## Different type of bindings

There are different settings that can be specified on each property or JSON object. Auto, Bound, Unbound or Parent.

**Auto**
When the binding is specified as auto (which is the default setting) each property is matched against the code-behind and the data object (if any). If a code-property with the same name or with a name specified as binding is found the JSON property is treated as bound and getting and setting values to it will get and set the value on the underlying property.

If no matching property is found, the JSON property is treated as unbound and values will be stored in the JSON.

**Bound**
When a JSON property is specified as bound a compiler error or runtime error, depending on if the type to bind to is specified, is thrown. A JSON property declared as Bound **must** match a property in the code-behind or data object.

**Unbound**
An unbound JSON property will store the value in the JSON.

**Parent**
The same setting as specified on a parent is used, which will be one of the above (auto, bound, unbound).


## Rules When Bindings are Created
1. If a code-behind file exists, a property is searched for there.
2. If a property was not found in the code-behind or no code-behind exists, a property in the data object is searched for.
3. If no property was found in steps 1 and 2 and the binding is set to `Auto`, the property will be unbound. If binding was set to `Bound` an exception will be raised.

## Modify Bindings

There are different ways to modify bindings so that they do not bind the default way.

All binding modifications are done in a static contructor in the code-behind file. Like so:

<div class="code-name">PersonPage.json.cs</div>

```cs
using Starcounter;

namespace MyApp
{
    partial class PersonPage : Json
    {
        static PersonPage()
        {
            // Modifications go here
        }
    }
}
```

### Opting Out of Bindings

In some cases we want to make sure that a specific property is not bound. This can be achieved by setting the value of `Bind` to `null`;

For example:

<div class="code-name">PersonPage.json.cs</div>

```cs
partial class PersonPage : Json
{
    static PersonPage()
    {
        DefaultTemplate.FullName.Bind = null;
    }
}
```

Setting the `Bind` property to `null` in this case ensures that the Typed JSON property `FullName` is not bound to any database, or code-behind, property.

By applying this to the example in the [bindings to database objects section](#binding-to-database-objects), the resulting JSON would be `{"FirstName":"Steve","LastName":"Smith","FullName":""}`. Since the `FullName` property is not bound, it will not contain any value.

### Binding to Properties With Different Names

If a property should be bound to a property that has a different name than the property in the JSON, a binding value can be set.

For example, to bind the Typed JSON property `FirstName` to the database property `LastName` in the example in the [bindings to database objects section](#binding-to-database-objects) and vice versa to essentially switch the names around, the following code can be used:

```cs
public class PersonPage : Json
{
    static PersonPage()
    {
        DefaultTemplate.FirstName.Bind = "LastName";
        DefaultTemplate.LastName.Bind = "FirstName";
    }
}
```

The resulting JSON looks like this: `{"FirstName":"Smith","LastName":"Steve","FullName":"Steve Smith"}`. 

### Binding to Custom Properties in Code-Behind

Since it is possible to [bind to properties with different names](#binding-to-properties-with-different-names), it is also possible to bind to custom properties in the code-behind. For example:

```cs
public class PersonPage : Json
{
    static PersonPage()
    {
        DefaultTemplate.FullName.Bind = "CustomFullName";
    }

    public string CustomFullName => FirstName + FirstName + " " + LastName; 
}
```

The resulting JSON looks like this with the example in the [bindings to database objects section](#binding-to-database-objects): `{"FirstName":"Steve","LastName":"Smith","FullName":"SteveSteve Smith"}`.

#### Binding to a Deep Property

It is also possible to bind to deep properties by providing full path to the property. Here, the property `FriendName` is bound to the deep property `Friend.FirstName`.

<div class="code-name">PersonPage.json</div>

```json
{
  "FirstName": "",
  "FriendName": ""
}
```

<div class="code-name">PersonPage.json.cs</div>

```cs
using Starcounter;

namespace MyApp
{
    partial class PersonPage : Json
    {
        static PersonPage()
        {
            DefaultTemplate.FriendName.Bind = "Friend.FirstName";
        }
    }
}
```

<div class="code-name">Program.cs</div>

```cs
using Starcounter;

namespace MyApp
{
    [Database]
    public class Person
    {
        public string FirstName { get; set; }
        public Person Friend { get; set; }
    }

    class Program
    {
        static void Main()
        {
            Db.Transact(() =>
            {
                var friend = new Person() { FirstName = "Bilbo" };

                new Person()
                {
                    FirstName = "Steve",
                    Friend = friend
                };
            });

            Handle.GET("/GetPerson", () =>
            {
                Person person = Db.SQL<Person>("SELECT P FROM Person P WHERE P.FirstName = ?", "Steve").First;
                var json = new PersonPage();
                json.Data = person;
                return json;
            });
        }
    }
}
```

The resulting JSON from this example looks like this: `{"FirstName":"Steve","FriendName":"Bilbo"}`.

### Setting Type of Binding for Children
Typed JSON objects that contain children can specify how the type of bindings on its children. 

By setting the property `BindChildren`, each child that don't specify it's own binding, i.e. use the `Parent` option will have the binding decided by the parent.

Setting the value in the code-behind from the `BindingStrategy` enum:

```cs
partial class PersonPage : Json
{
    static PersonPage()
    {
        DefaultTemplate.BindChildren = BindingStrategy.Bound;
    }
}
```

The enum has the following values: "Auto", "Bound", and "Unbound".

**NOTE:** It is not allowed to set the value for this property to `BindingStrategy.UseParent`. An exception will be raised in this case.

## Specify Data Type

Denoting a specific type that is used as data-object is optional but comes with some benefits.

If the JSON object is static, that is all properties are known compile-time, you will get an error during compilation if bound properties are invalid, instead of getting a runtime error on first use. The Data property itself will also be typed to the correct type which means that you don't need to cast the data-object when using it.

### IBound

The JSON code-behind class has to implement `IBound<T>` to set custom data type.

```cs
[PersonJson_json]
public partial class PersonJson : Json, IBound<MyNamespace.Person>
```

**Note:** empty JSON objects do not have code-behind classes so it is not possible to declare a custom data type for them.


### IExplicitBound
`IExplicitBound` is an improved implementation of `IBound`. They are used the exact same way, though `IExplicitBound` allows more control over the bindings.

When using `IExplicitBound`, properties in JSON-by-example are expected to be bound. This allows the pinpointing of failed bindings which otherwise could go unnoticed. If the JSON-by-example looks like this:

```json
{
  "Name": "",
  "Age": 0,
  "Address": ""
}
```

And the database class looks like this:

```cs
public class Person
{
  public string Name { get; set; }
  public long Age { get; set; }
  public string Address { get; set; }
}
```

If the code-behind includes `IExplicitBound` like this:

```cs
public class PersonPage : Json, IExplicitBound<Person>
```

Then it will compile successfully.
If `public long Age` was removed, then the following error would be displayed: `'Person' does not contain a definition for 'Age'`. The reason for this is that `IExplicitBound` would look for a database property corresponding to `Age` and fail.

Since `IExplicitBound` expects all values to be bound to _something_, properties that are not intended to be bound have to be explicitly unbound. As noted above, it will not compile without this. A static constructor can be used in order to explicitly unbind these properties. This is how it would look:

```cs
static PersonPage()
{
    DefaultTemplate.Age.Bind = null;
}
```
Now, the code will compile successfully because it is explicitly described that the `Age` property will not be bound. This is further described in the section "Opt-out of Bindings".