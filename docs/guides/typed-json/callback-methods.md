# Callback Methods

## Introduction

Typed JSON files can be affected by changes from the client or the server. This page describes callback methods for changes on the server side.

## OnData

`OnData` is the method that is called when the `Data` property is set, as described in the [data bindings section](data-bindings.md#binding-to-database-objects).

The `OnData` method is usually used to initialize the parts of the view-model that cannot be initialized by setting the `Data` property. Several examples of this can be found in the [KitchenSink repo](https://github.com/Starcounter/KitchenSink/blob/fad83975ec3b4ebf6201458ea228547e6756d507/src/KitchenSink/ChartPage.json.cs).

The basic structure for using `OnData` looks like this:

{% code title="PersonPage.json.cs" %}
```csharp
using Starcounter;

namespace MyApp
{
    partial class PersonPage : Json
    {
        protected override void OnData()
        {
            base.OnData();

            // Code to be run when the Data property is set
        }
    }
}
```
{% endcode %}

## HasChanged

And the same goes for `HasChanged` - that method is always called when there is a change on a server side. But this time it indicates a change of a value in JSON root class.

> To capture a change in a child subtree, it is efficient to define another partial class and use HasChanged method there.

This method implemented in the same way as `OnData` - all the declaration is happening in the code-behind file.  
Unlike `OnData` the method `HasChanged` is not that commonly used but only when there is a need for auto-committed database transactions every time data updates.  
There is a quick example on `HasChanged` usage:

```csharp
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
* `HasChanged` - triggered when the value is changed. 

