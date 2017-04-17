# Responding with JSON

Typed JSON objects are serialized automatically to the `application/json` format when returned from a handler.

<div class="code-name">PersonPage.json</div>

```json
{
    "FirstName": "Bilbo",
    "LastName": "Baggins"
}
```

<div class="code-name">Program.cs</div>

```cs
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

## Setting Status Code and Description

The default HTTP status code for responses is 200 OK.

To change this, two methods are provided to the `Handle` class: `SetOutgoingStatusCode` and `SetOutgoingStatusDescription`.

In code, they look like this:

<div class="code-name">Program.cs</div>

```cs
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

It is also possible to change the status code and description by creating a `Response` object: 

<div class="code-name">PersonPage.json</div>

```json
{
    "FirstName": "Gandalf",
    "LastName": "Gray",
    "Quote": "You shall not pass!" 
}
```

<div class="code-name">Program.cs</div>

```cs
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

Note that the JSON needs to be explicitly parsed to a string using `ToJson` when attaching a Typed JSON object to the body of a `Response`.