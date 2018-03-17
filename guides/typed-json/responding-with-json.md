# Responding with JSON

## Introduction

Typed JSON objects are serialized automatically to the `application/json` format when returned from a handler.

## Example

{% code-tabs %}
{% code-tabs-item title="PersonPage.json" %}
```javascript
{
    "FirstName": "Bilbo",
    "LastName": "Baggins"
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

{% code-tabs %}
{% code-tabs-item title="PersonPage.json.cs" %}
```csharp
using Starcounter;

namespace MyApp
{
    class Program
    {
        static void Main()
        {
            Handle.GET("/GetPerson", () =>
            {
                return new PersonPage(); // {"FirstName":"Bilbo","LastName":"Baggins"}
            });
        }
    }
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

## Setting Status Code and Description

The default HTTP status code for responses is 200 OK.

To change this, two methods are provided to the `Handle` class: `SetOutgoingStatusCode` and `SetOutgoingStatusDescription`.

In code, they look like this:

{% code-tabs %}
{% code-tabs-item title="Program.cs" %}
```csharp
using Starcounter;

namespace MyApp
{
    class Program
    {
        static void Main()
        {
            Handle.GET("/NotFound", () => 
            {
                Handle.SetOutgoingStatusCode(404);
                Handle.SetOutgoingStatusDescription("Not Found");
                return "";
            });
        }
    }
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

It is also possible to change the status code and description by creating a `Response` object:

{% code-tabs %}
{% code-tabs-item title="PersonPage.json" %}
```javascript
{
    "FirstName": "Gandalf",
    "LastName": "Gray",
    "Quote": "You shall not pass!" 
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

{% code-tabs %}
{% code-tabs-item title="PersonPage.json.cs" %}
```csharp
using Starcounter;

namespace MyApp
{
    class Program
    {
        static void Main()
        {
            Handle.GET("/GetPerson", () =>
            {
                var json = new PersonPage();

                var response = new Response()
                {
                    StatusCode = 403,
                    StatusDescription = "Forbidden",
                    Body = json.ToJson()
                };

                return response; // Body: {"FirstName":"Gandalf","LastName":"Gray","Quote":"You shall not pass!"}
            });
        }
    }
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

The JSON needs to be explicitly parsed to a string using `ToJson` when attaching a Typed JSON object to the body of a `Response`.

