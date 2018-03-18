# Data bindings

## Introduction

Properties declared in Typed JSON can be bound to either a property in the code-behind file or a CLR object that exposes one or more public properties. Properties that are bound will read and write the values directly to the underlying object with no need to manually transfer the values to the view-model.

## Default bindings

### Binding to database objects

To bind a Typed JSON object to a database object, the `Data` property is used.

Consider the following JSON file:

{% code-tabs %}
{% code-tabs-item title="PersonPage.json" %}
```javascript
{
   "FirstName": "",
   "LastName": "",
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

To bind the Typed JSON object `PersonPage` defined above to a database class `Person`, the following code can be used:

{% code-tabs %}
{% code-tabs-item title="Program.cs" %}
```csharp
using Starcounter;
using System.Linq;

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
            // Retrieve a database object from the database
            var person = Db.SQL<Person>("SELECT P FROM Person P")
                .First(); 

            return new PersonPage() 
            {
                // Bind the database object to the Typed JSON object
                Data = person 
            };
        });
    }
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

The `PersonPage` object will now look like this: `{"FirstName":"Steve","LastName":"Smith","FullName":"Steve Smith"}`.

Starcounter recognizes that the properties in `PersonPage` and `Person` object have the same name and populates the values in the Typed JSON accordingly. This is the default way that bindings are created.

### Binding to code-behind properties

In addition to binding to database objects, Typed JSON properties can also be bound to code-behind properties.

To accomplish what was demonstrated in the [previous example](../../#binding-to-database-objects) by using code-behind properties instead of database properties, the following code can be used:

{% code-tabs %}
{% code-tabs-item title="PersonPage.json" %}
```javascript
{
   "FirstName": "Steven",
   "LastName": "Smith",
   "FullName": ""
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

{% code-tabs %}
{% code-tabs-item title="PersonPage.json.cs" %}
```csharp
using Starcounter;

partial class PersonPage : Json
{
    public string FullName => FirstName + " " + LastName;
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

{% code-tabs %}
{% code-tabs-item title="Program.cs" %}
```csharp
using Starcounter;

class Program
{
    static void Main()
    {
        Handle.GET("/GetPerson", () =>
        {
            return new PersonPage(); // => {"FirstName":"Steven","LastName":"Smith","FullName":"Steven Smith"}
        });
    }
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

{% hint style="info" %}
If there's a database property and code-behind property with the same name, the code-behind property will be the one that's used for bindings.
{% endhint %}

## Different types of bindings

You can specify what type of binding each JSON property should have. The available bindings are: `Auto`, `Bound,` `Unbound`, or `Parent`. These exists as values of the `enum` `BindingStrategy`.

The binding are specified in a static constructor in the code-behind:

{% code-tabs %}
{% code-tabs-item title="PersonPage.json.cs" %}
```csharp
using Starcounter;
using Starcounter.Templates;

partial class PersonPage : Json
{
    static PersonPage()
    {
        DefaultTemplate.FirstName.BindingStrategy = 
            BindingStrategy.Unbound;
    }
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

### Auto

This is the binding described under [Default Bindings](data-bindings.md#default-bindings), it checks whether there's a property in the bound data object or code-behind with the same name and binds them if that's the case. No binding is made otherwise.

This is the default binding that will be used if no other binding is specified. 

### Bound

Bindings that are set to `Bound` must have a property in the code-behind or database object to bind to, otherwise,`ScErrCreateDataBindingForJson (SCERR14006)`is thrown:

{% code-tabs %}
{% code-tabs-item title="PersonPage.json" %}
```javascript
{
  "FirstName": "",
  "LastName": "",
  "FullName": ""
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

{% code-tabs %}
{% code-tabs-item title="PersonPage.json.cs" %}
```csharp
using Starcounter;
using Starcounter.Templates;

partial class PersonPage : Json
{
    static PersonPage()
    {
        DefaultTemplate.FullName.BindingStrategy = 
            BindingStrategy.Bound;
    }
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

{% code-tabs %}
{% code-tabs-item title="Program.cs" %}
```csharp
using System;
using Starcounter;

[Database]
public class Person
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}

class Program
{
    static void Main()
    {
        var person = Db.Transact(() => new Person
        {
            FirstName = "John",
            LastName = "Doe"
        });

        var personPage = new PersonPage() { Data = person };

        Console.WriteLine(personPage.ToJson()); // SCERR14006
    }
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

### Unbound

JSON properties with the `BindingStrategy` set to `Unbound` will not bind to any property, neither in the code-behind nor in the data object:

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

{% code-tabs %}
{% code-tabs-item title="PersonPage.json.cs" %}
```csharp
using Starcounter;
using Starcounter.Templates;

partial class PersonPage : Json
{
    public string LastName => "Doe";

    static PersonPage()
    {
        DefaultTemplate.LastName.BindingStrategy = 
            BindingStrategy.Unbound;
    }
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

{% code-tabs %}
{% code-tabs-item title="Program.cs" %}
```csharp
using System;
using Starcounter;

[Database]
public class Person
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}

class Program
{
    static void Main()
    {
        var person = Db.Transact(() => new Person
        {
            FirstName = "John",
            LastName = "Doe"
        });

        var personPage = new PersonPage() { Data = person };

        Console.WriteLine(personPage.ToJson()); // =>  {"FirstName":"John","LastName":""}
    }
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

### Parent

The same binding as specified on the parent is used.

### Binding types for children

Typed JSON objects that contain children can specify how the type of bindings on its children.

By setting the property `BindChildren`, each child that don't specify it's own binding with the `Parent` option will have the binding decided by the parent:

{% code-tabs %}
{% code-tabs-item title="PersonPage.json.cs" %}
```csharp
partial class PersonPage : Json
{
    static PersonPage()
    {
        DefaultTemplate.BindChildren = BindingStrategy.Bound;
    }
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

{% hint style="warning" %}
Setting `BindChildren` to `BindingStrategy.UseParent`is not allowed and will raise an exception.
{% endhint %}

## Modifying bindings

There are different ways to modify bindings so that they do not bind the default way.

All binding modifications are done in a static constructor in the code-behind file. Like so:

{% code-tabs %}
{% code-tabs-item title="PersonPage.json.cs" %}
```csharp
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
{% endcode-tabs-item %}
{% endcode-tabs %}

### Binding to properties with different names

If a property should be bound to a property that has a different name than the property in the JSON, a binding value can be set.

For example, to bind the Typed JSON property `FirstName` to the database property `LastName` in the example in the [bindings to database objects section](../../#binding-to-database-objects) and vice versa to essentially switch the names around, the following code can be used:

{% code-tabs %}
{% code-tabs-item title="PersonPage.json.cs" %}
```csharp
public class PersonPage : Json
{
    static PersonPage()
    {
        DefaultTemplate.FirstName.Bind = "LastName";
        DefaultTemplate.LastName.Bind = "FirstName";
    }
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

The resulting JSON looks like this: `{"FirstName":"Smith","LastName":"Steve","FullName":"Steve Smith"}`.

### Binding to custom properties in code-behind

Since it is possible to [bind to properties with different names](../../#binding-to-properties-with-different-names), it is also possible to bind to custom properties in the code-behind. For example:

{% code-tabs %}
{% code-tabs-item title="PersonPage.json.cs" %}
```csharp
public class PersonPage : Json
{
    static PersonPage()
    {
        DefaultTemplate.FullName.Bind = "CustomFullName";
    }

    public string CustomFullName => 
        FirstName + FirstName + " " + LastName; 
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

The resulting JSON looks like this with the example in the [bindings to database objects section](../../#binding-to-database-objects): `{"FirstName":"Steve","LastName":"Smith","FullName":"SteveSteve Smith"}`.

### Binding to a deep property

It is also possible to bind to deep properties by providing full path to the property. Here, the property `FriendName` is bound to the deep property `Friend.FirstName`.

{% code-tabs %}
{% code-tabs-item title="PersonPage.json" %}
```javascript
{
  "FirstName": "",
  "FriendName": ""
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

{% code-tabs %}
{% code-tabs-item title="PersonPage.json.cs" %}
```csharp
using Starcounter;

partial class PersonPage : Json
{
    static PersonPage()
    {
        DefaultTemplate.FriendName.Bind = "Friend.FirstName";
    }
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

{% code-tabs %}
{% code-tabs-item title="Program.cs" %}
```csharp
using Starcounter;
using System.Linq;

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
            var person = Db.SQL<Person>(
                "SELECT P FROM Person P WHERE P.FirstName = ?",
                "Steve")
                .FirstOrDefault();
            
            return new PersonPage() { Data = person };
        });
    }
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

The resulting JSON from this example looks like this: `{"FirstName":"Steve","FriendName":"Bilbo"}`.

## Specify data type

Denoting a specific type that is used as data-object is optional but comes with some benefits.

If the JSON object is static, that is all properties are known compile-time, you will get an error during compilation if bound properties are invalid, instead of getting a runtime error on first use. The Data property itself will also be typed to the correct type which means that you don't need to cast the data-object when using it.

### IBound

The JSON code-behind class has to implement `IBound<T>` to set custom data type.

{% code-tabs %}
{% code-tabs-item title="PersonPage.json.cs" %}
```csharp
[PersonJson_json]
public partial class PersonJson : Json, IBound<MyNamespace.Person>
```
{% endcode-tabs-item %}
{% endcode-tabs %}

{% hint style="info" %}
Empty JSON objects do not have code-behind classes so it is not possible to declare a custom data type for them
{% endhint %}

### IExplicitBound

`IExplicitBound` is an improved implementation of `IBound`. They are used the exact same way, though `IExplicitBound` allows more control over the bindings.

When using `IExplicitBound`, properties in JSON-by-example are expected to be bound. This allows the pinpointing of failed bindings which otherwise could go unnoticed. If the JSON-by-example looks like this:

{% code-tabs %}
{% code-tabs-item title="PersonPage.json" %}
```javascript
{
  "Name": "",
  "Age": 0,
  "Address": ""
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

And the database class looks like this:

{% code-tabs %}
{% code-tabs-item title="Person.cs" %}
```csharp
public class Person
{
  public string Name { get; set; }
  public long Age { get; set; }
  public string Address { get; set; }
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

If the code-behind includes `IExplicitBound` like this:

{% code-tabs %}
{% code-tabs-item title="PersonPage.json.cs" %}
```csharp
public class PersonPage : Json, IExplicitBound<Person>
```
{% endcode-tabs-item %}
{% endcode-tabs %}

Then it will compile successfully.  
If `public long Age` was removed, then the following error would be displayed: `'Person' does not contain a definition for 'Age'`. The reason for this is that `IExplicitBound` would look for a database property corresponding to `Age` and fail.

Since `IExplicitBound` expects all values to be bound to _something_, properties that are not intended to be bound have to be explicitly unbound. As noted above, it will not compile without this. A static constructor can be used in order to explicitly unbind these properties. This is how it would look:

{% code-tabs %}
{% code-tabs-item title="PersonPage.json.cs" %}
```csharp
static PersonPage()
{
    DefaultTemplate.Age.Bind = null;
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

Now, the code will compile successfully because it is explicitly described that the `Age` property will not be bound.

## Exceptions

### ScErrInvalidOperationDataOnEmptyJson \(SCERR14013\)

It's not possible to set the data object of an empty Typed JSON object. For example:

{% code-tabs %}
{% code-tabs-item title="PersonPage.json" %}
```javascript
{
  "Person": {},
  "Friends": [ {} ]
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

```csharp
var personPage = new PersonPage();

// Throws ScErrInvalidOperationDataOnEmptyJson
personPage.Person.Data = Db.SQL("SELECT p FROM Person p").First();

 // Throws ScErrInvalidOperationDataOnEmptyJson 
personPage.Friends.Data = Db.SQL("SELECT p.Friends FROM Person p");
```

These lines throw exceptions because it's not clear what should be done. Since there are no properties, there's nothing to bind to, and thus the assignment serves no purpose. Throwing an exceptions ensures that developers don't write code that they expect something from but in reality does nothing.

{% hint style="info" %}
In version 2.3.1 and earlier, Starcounter created a JSON schema based on the assigned data object if the Typed JSON object was empty. This suffered from performance problems and was hard to understand, thus, it now throws an exception instead.
{% endhint %}

