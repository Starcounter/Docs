# Populate JSON with database data

As demonstrated [earlier](/guides/typed-json/json-by-example/#default-values), Typed JSON can be can be created with default values that is set in the JSON-by-example. This might be useful in some cases. However, in most applications, it is much more valuable to populate the Typed JSON with database data. The simplest way to do this is by using the `Data` property of the Typed JSON object.

Consider the following JSON file:

<div class="code-name">PersonPage.json</div>

```json
{
   "FirstName": "",
   "LastName": "",
}
```

The goal is to populate this JSON with data from the database class `Person` that looks like this:

```cs
[Database]
public class Person
{
   public string FirstName;
   public string LastName;
}
```

To accomplish this, the following code can be used:

```cs
var person = Db.SQL<Person>("SELECT P FROM Person P").First;
var json = new PersonPage();
json.Data = person;    
```

The `PersonPage` object will now look like this: `{"FirstName":"Steve","LastName":"Smith"}`.

Starcounter recognizes that the properties `PersonPage` and `Person` object have the same name and populates the values in the Typed JSON accordingly.
