# Populate JSON with database data

In the [previous example](/guides/json/), we populated the JSON object with arbitrary data. A common scenario is that you want to create a JSON object from database data or from some CLR object. This can of course be done by setting one property at the time and iterating over result sets manually. But you can also bind the JSON object to a CLR object.

## Declaring the JSON document
CLR objects and JSON documents work in concert. To create a JSON document that represents a CLR object, declare the class name of the root database object in the `DataType` metadata property. Metadata properties can be declared in a JSON object literal in the `$` property. This will map the entire JSON tree to the database object hierarchy by matching the property names. The data now lives in database RAM rather than .NET RAM. This is done at compile time such that you get compilation errors if you misspell a property and performance is maintained.

<div class="code-name">PersonMsg.json</div>

```javascript
{
   $:{DataType:"Person"},

   FirstName:"Jocke",
   LastName:"Wester",
   Quotes: [
        { Text:"This is an example" }
   ],
   $Quotes:{DataType:"Quote"}
}
```

## Setting the Data property

By setting the ```Data``` property on the typed JSON object, you will bind each property in the JSON object tree to the properties with the same name in the database object. You can override this by suppling alternative binding using the ```Bind``` property in JSON document.

<div class="code-name">Program.cs</div>

```cs
using System.Collections;
using Starcounter;

class Hello {
   static void Main() {
      Handle.GET("/hello/{?}", ( string name ) => {
         Person p = Db.SQL<Person>("SELECT P FROM Person P WHERE FirstName=?", name ).First;
         var json = new PersonMsg();
         json.Data = p;
         return json;
      });         
   }
}
 
[Database]
public class Person {
   public string FirstName;
   public string LastName;
   public string FullName { get { return FirstName + " " + LastName; } }
   public IEnumerable Quotes { 
     get { 
       return Db.SQL<Quote>("SELECT Q FROM Quote Q WHERE Person=?", this ); 
     } 
   } 
 }

[Database]
public class Quote {
   public Person Person;
   public string Text;
}
```
