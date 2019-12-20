# Accepting JSON in Requests

## Introduction

Typed JSON objects can be used as a representation of the HTTP \(or other REST protocol\) request body payload. To accept a JSON object in a PUT message, simply declare a parameter with the type `Json`.

## Example

{% code title="PersonMsg.json" %}
```javascript
{
   "FirstName": "",
   "LastName": "",
   "Age": 0
}
```
{% endcode %}

{% code title="Program.cs" %}
```csharp
using Starcounter;

class Hello
{
   static void Main()
   {
      Handle.PUT("/hello/{?}", ( string name, PersonMsg message ) =>
      {
         return "Welcome " + name + " you are " + message.Age + " years old.";
      });         
   }
}
```
{% endcode %}

The parameter is not associated with the URI template, so the content of the body will be used to fill in the object.

Now you can call the above handler with a HTTP request, for example using `XMLHttpRequest` in a web browser or manually using cURL:

```text
$ curl -X PUT -H "Content-Type: application/json"
-d "{FirstName:"Olle",LastName:"Svensson", Age:49}"
http://localhost:8080/hello/Olle

Welcome Olle you are 49 years old.
```

