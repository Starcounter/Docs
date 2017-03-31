# Callback Methods

Typically there are two kind of events, happening in typed JSON files.
There might be events coming from the client side and events, happening on the server side of the app.
In this article we will deepen into the methods of Starcounter JS triggered by the actions on the server side.

## OnData

Typed JSON objects have properties, which exist as a playground for your actions.
Therefore, once you update a `DataType` mandatory property internally on Typed JSON object their own `OnData` method will be triggered, indicating property initialization / update. Moreover when a new data object is set, `OnData` method can implement the update for other linked properties in the [code-behind file]("/guides/typed-json/code-behind") of the view-model.

The result of `OnData` functionality is that after connecting a database object to the view-model method will refresh the view every time you set the new property. No more copying and setting values through added functionality. Use case of the method can be studied from our explicit Tutorial.

## HasChanged

And the same goes for `HasChanged` - that method is always called when there is a change on a server side. But this time it indicates a change of a value in JSON root class.

> To capture a change in a child subtree, it is efficient to define another partial class and use HasChanged method there.

This method implemented in the same way as `OnData` - all the declaration is happening in the code-behind file.
Unlike `OnData` the method `HasChanged` is not that commonly used but only when there is a need for auto-committed database transactions every time data updates.
There is a quick example on `HasChanged` usage:

```cs
using Starcounter;
using Starcounter.Templates;

namespace ModelChangeEventTestProject
{
    partial class Page2 : Json
    {
        protected override void HasChanged(TValue property)
        {
            base.HasChanged(property);
        }
    }

    [Page2_json.Property2]
    partial class Page2Property2 : Json
    {
        protected override void HasChanged(TValue property)
        {
            base.HasChanged(property);
        }
    }
}  
```

Just to sum up methods purposes:

* `OnData` - triggered when the data property is changed.
* `Haschaned` - triggered when the value is changed. 
