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

## Setting the Data property

By setting the `Data` property on the typed JSON object, you will bind each property in the JSON object tree to the properties with the same name in the database object. You can override this by suppling alternative binding using the `Bind` property in JSON document.

<div class="code-name">Program.cs</div>

```cs
using System.Collections;
using Starcounter;

class Hello
{
   static void Main()
   {
      Handle.GET("/hello/{?}", (string name) =>
      {
         Person person = Db.SQL<Person>("SELECT P FROM Person P WHERE FirstName=?", name).First;
         var json = new PersonPage();
         json.Data = person;
         return json;
      });         
   }
}

[Database]
public class Person
{
   public string FirstName;
   public string LastName;
}
```
