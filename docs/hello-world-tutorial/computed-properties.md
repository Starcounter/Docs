# Computed Properties

Starcounter makes it easy to use computed properties in the data model. Computing values on the fly is often as fast as accessing cached data and brings additional benefits. It allows you to save memory and always be sure that you get the current value.

Let us compute the `FullName` of a person from their `FirstName` and `LastName` and display it with minimal delay.

## Preparing the View-Model

To synchronize the computed property between view and code-behind, simply add the `FullName` property to the view-model.

{% code-tabs %}
{% code-tabs-item title="PersonJson.json" %}
```javascript
{
  "Html": "/HelloWorld/PersonJson.html",
  "FirstName$": "",
  "LastName$": "",
  "SaveTrigger$": 0,
  "FullName": ""
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

Notice that we don't have to make `FullName` writable because we will modify it from the code-behind and not the view.

## Compute in Code-Behind

There are two ways to implement computed properties, in the code-behind or in the database class. For this tutorial, it'll be done in the code-behind.

The `FullName` property can be calculated by concatenating `FirstName` and `LastName`.

{% code-tabs %}
{% code-tabs-item title="PersonJson.json.cs" %}
```csharp
partial class PersonJson : Json
{
    public string FullName => $"{FirstName} {LastName}";

    void Handle(Input.SaveTrigger action)
    {
        Transaction.Commit();
    }
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

This property will now be bound to the property with the same name in the view-model and always be up to date.

## Display the Computed Property

To display this computed property, we just have to add it to to the view. This is done the same way as earlier; by using a Polymer binding:

{% code-tabs %}
{% code-tabs-item title="PersonJson.html" %}
```markup
<template>
    <template is="dom-bind">
        <h1>Hey, {{model.FullName}}!</h1>

        <fieldset>
            <label>First name:</label>
            <input value="{{model.FirstName$::input}}">
        </fieldset>

        <fieldset>
            <label>Last name:</label>
            <input value="{{model.LastName$::input}}">
        </fieldset>

        <button value="{{model.SaveTrigger$::click}}" onmousedown="++this.value">Save</button>
    </template>
</template>
```
{% endcode-tabs-item %}
{% endcode-tabs %}

## Result

Start the application and see how the computed property is calculated keystroke by keystroke and displayed instantly:



![](../.gitbook/assets/part4resized.gif)



The next step is to practice working on multiple object instances and relations by turning the app into a simple expense tracker.

If you get any errors, you can check your code against the [source code](https://github.com/Starcounter/HelloWorld/commit/69cfcb0bd2dedf268b4d97fcb24cab4da3f40190).

