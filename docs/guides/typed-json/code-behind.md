# Code-Behind

## Introduction

To create interactivity for the Typed JSON classes, code-behind classes can be added to extend existing Typed JSON. This is done using `.json.cs` files, which are partial definitions for Typed JSON classes.

## Create Code-Behind Files

To create a Typed JSON class with code-behind, choose `New item` in Visual Studio and then select `Starcounter Typed JSON with Code-behind`. By creating one of these with the filename "Person", two files will be created:

{% code title="PersonPage.json" %}
```javascript
{
}
```
{% endcode %}

{% code title="PersonPage.json.cs" %}
```csharp
using Starcounter;

namespace MyApp
{
    partial class PersonPage : Json
    {
    }
}
```
{% endcode %}

## Handling Input Events

Consider this JSON object with a property that is [writable](json-by-example.md#writable-json-values) from the client:

{% code title="PersonPage.json" %}
```javascript
{
  "FirstName$": ""
}
```
{% endcode %}

To observe changes to this property, the code-behind method `Handle` can be used:

{% code title="PersonPage.json.cs" %}
```csharp
partial class PersonPage : Json
{
    void Handle(Input.FirstName action)
    {
        if (action.Value == "Albert")
        {
            Message = "You are not allowed to enter Albert. There can be only one.";
            action.Cancel();
        }
    }
}
```
{% endcode %}

The `Handle` method gets called with a parameter of type `Input`. `Input` is the base class for events triggered by the client.

The `Input` class is auto generated per each JSON view-model. It provides the following properties and methods:

* property `Value` - contains the new value of the user input
* property `OldValue` - contains the current value of the user input
* property `ValueChanged` - boolean, true if the new value is different than the old value
* method `Cancel()` - reject the new value. It prevents the propagation of the new value to JSON as well as to the bound data object
* property `Cancelled` - boolean, true if the `Cancel()` method was called

To get many more examples of how interactivity is handled, take a look at the [KitchenSink repo](https://github.com/Starcounter/KitchenSink) where the most common UI patterns are demonstrated.

## Referring to Nested Objects

JSON-by-example might contain nested objects. For example:

{% code title="PersonPage.json" %}
```javascript
{
  "Name": {
     "FirstName$": "",
     "LastName$": ""
  },
  "FullName$": ""
}
```
{% endcode %}

Code-behind for the root level and `Name`-level can be provided as two separate partial classes. For example:

{% code title="PersonPage.json.cs" %}
```csharp
partial class PersonPage : Json
{
    void Handle(Input.FullName action)
    {
        var words = action.Value.Split(' ');
        this.Name.FirstName = words[0];
        this.Name.LastName = words[1];
    }

    [PersonPage_json.Name]
    partial class PersonPageName : Json
    {
        void Handle(Input.FirstName action)
        {
            var person = this.Parent as PersonPage;
            person.FullName = action.Value + " " + this.LastName;
        }

        void Handle(Input.LastName action)
        {
            var person = this.Parent as PersonPage;
            person.FullName = this.FirstName + " " + action.Value;
        }
    }
}
```
{% endcode %}

The attribute `[PersonPage_json.Name]` is used to hint what is the path in JSON-by-example that the partial class refers to.

As you might have noticed, accessing a child object from a parent object in code-behind is as simple as providing a path expression: `this.Name.FirstName = words[0]`. The child property \(`this.Name`\) is of known type \(`PersonPageName`\).

However, accessing a parent from a child requires casting \(`var person = this.Parent as PersonPage`\). This is because there might be various parents that employ this particular child. In general, using the `Parent` property is discouraged, because it breaks the single-direction data flow. Child should be controlled by the parent and not vice versa.

