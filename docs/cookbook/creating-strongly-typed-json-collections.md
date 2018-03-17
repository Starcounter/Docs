# How to create typed JSON collections

## Goal

Create a statically Typed JSON array containing other Typed JSON objects.

## Steps

### 1. Create a Typed JSON class to store in the array

For this example, we'll create a simple Typed JSON class `Person`. The array that we will later create will be typed to this class. For your application, this can be any Typed JSON class. 

{% code-tabs %}
{% code-tabs-item title="Person.json" %}
```javascript
{
  "Name": ""
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

{% code-tabs %}
{% code-tabs-item title="Person.json.cs" %}
```csharp
using Starcounter;

namespace TypedJSONCollectionSample 
{
    partial class Person : Json 
    {
    }
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

### 2. Create the Typed JSON class with the array

The Typed JSON class that with the array that will hold the `Person` objects only needs one property: an array of objects:

{% code-tabs %}
{% code-tabs-item title="PersonCollection.json" %}
```javascript
{
  "People": [{}]
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

{% code-tabs %}
{% code-tabs-item title="PersonCollection.json.cs" %}
```csharp
using Starcounter;

namespace TypedJSONCollectionSample 
{
    partial class PersonCollection : Json 
    {
    }
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

### 3. Make the array typed

JSON arrays are not typed by default. To make them typed, we'll specify the instance type of the array. This is done in a static constructor in the code-behind:

{% code-tabs %}
{% code-tabs-item title="PersonCollection.json.cs" %}
```csharp
using Starcounter;

namespace TypedJSONCollectionSample 
{
    partial class PersonCollection : Json 
    {
      static PersonCollection() 
      {
        DefaultTemplate.People.ElementType.InstanceType = typeof(Person);
      }
    }
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

## Summary

With this setup, it's possible to add instances of the `Person` class to the `PersonCollection` and know that the array only contains `Person` objects:

{% code-tabs %}
{% code-tabs-item title="Program.cs" %}
```csharp
static void Main() 
{
  var alice = new Person() { Name = "Alice" };
  var bob = new Person() { Name = "Bob" };

  var friends = new PersonCollection();
  friends.People.Add(alice);
  friends.People.Add(bob);

  Console.WriteLine(alice.ToJson()); // {"Name":"Alice"}
  Console.WriteLine(bob.ToJson()); // {"Name":"Bob"}
  Console.WriteLine(friends.ToJson()); // {"People":[{"Name":"Alice"},{"Name":"Bob"}]}
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

