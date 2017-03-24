# Populate JSON with database data

In an [earlier example](/guides/typed-json/json-by-example/), we populated the JSON object with arbitrary data. A common scenario is that you want to create a JSON object from database data or from some CLR object. This can of course be done by setting one property at the time and iterating over result sets manually. But you can also bind the JSON object to a CLR object.

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
