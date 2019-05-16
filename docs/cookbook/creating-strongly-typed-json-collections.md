# Creating Strongly Typed JSON Collections

Sometimes, it might be useful to declare JSON and a corresponding code-behind that makes it possible to programmatically create collections that, when serialized to JSON, become documents that are collections of other JSON objects.

In this example, it's described how to create a strongly typed collection of `Person` objects as created here:



```
{
  "Name": ""
}
```



```
using Starcounter;

namespace TypedJSONCollectionSample 
{
    partial class Person : Json 
    {
    }
}
```

To start creating a collection, add a new item using the `Starcounter Typed JSON with code-behind` template. In this example, it'll be named "PersonCollection".

To allow this JSON to contain a collection of other objects, the JSON-by-example has to look something like this:



```
{
  "People": [{}]
}
```

In the code-behind for the JSON object, make the collection strongly typed by using the following code:



```
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

With this setup, it's possible to add instances of the `Person` class to the `PersonCollection`:

```
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

