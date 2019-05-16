# Code-behind

To provide interactivity in your JSON, you can bind code methods to JSON properties. This is done using `.json.cs` files, which are used as partial definitions for the Typed JSON class.

To create a Typed JSON class with code-behind, choose `New item` in Visual Studio and then choose `Starcounter Typed JSON with Code-behind`. For example, name it `PersonViewModel`. This creates two files:



```
{
}
```



```
using Starcounter;

namespace MyApp
{
    partial class PersonViewModel : Json
    {
    }
}
```

The class name `PersonViewModel` given in C\# is arbitrary and can be different than the file name. You use the class name to in other parts of the code, for example to create new instances \(`var json = new PersonViewModel()`\).

### Handling input events

Consider that you have a JSON property that can be edited from the client. For example:



```
{
  "FirstName$": ""
}
```

You might observe changes to this property using a Code-behind method `Handle`:



```
using Starcounter;

namespace MyApp
{
    partial class PersonViewModel : Json
    {
        void Handle(Input.FirstName input)
        {
            if (input.Value == "Albert")
            {
                Message = "You are not allowed to enter Albert. There can be only one.";
                input.Cancel();
            }
        }
    }
}
```

The `Handle` method gets called with a parameter of type `Input`. `Input` is the base class for events triggered by the client.

The `Input` class is auto generated per each JSON view-model. It provides the following properties and methods:

* property `Value` - contains the new value of the user input
* property `OldValue` - contains the current value of the user input
* property `ValueChanged` - boolean, true if the new value is different than the old value
* method `Cancel()` - reject the new value. It prevents the propagation of the new value to JSON as well as to the bound data object
* property `Cancelled` - boolean, true if the `Cancel()` method was called

### Refering to nested objects

JSON-by-example might contain a nested object. For example:



```
{
  "Name": {
     "FirstName$": "",
     "LastName$": ""
  },
  "FullName$": ""
}
```

You can provide code-behind for the root level and `Name`-level as two separate partial classes. For example:

```
using Starcounter;

namespace Nara {
    partial class PersonViewModel : Json
    {
        void Handle(Input.FullName input)
        {
            var words = input.Value.Split(' ');
            this.Name.FirstName = words[0];
            this.Name.LastName = words[1];
        }

        [PersonViewModel_json.Name]
        partial class PersonViewModelName : Json
        {
            void Handle(Input.FirstName input)
            {
                var parent = (PersonViewModel)this.Parent;
                parent.FullName = input.Value + " " + this.LastName;
            }

            void Handle(Input.LastName input)
            {
                var parent = (PersonViewModel)this.Parent;
                parent.FullName = this.FirstName + " " + input.Value;
            }
        }
    }
}
```

The attribute `[PersonViewModel_json.Name]` is used to hint what is the path in JSON-by-example that the partial class refers to.

As you might have noticed, accessing a child object from a parent object in code-behind is as simple as providing a path expression: `this.Name.FirstName = words[0]`. The child property \(`this.Name`\) is of known type \(`PersonViewModelName`\).

However, accessing a parent from a child requires casting \(`var parent = (PersonViewModel)this.Parent`\). This is because there might be various parents that employ this particular child. In general, using the `Parent` property is discouraged, because it **breaks the single-direction data flow**. Child should be controlled by the parent and not vice versa.

