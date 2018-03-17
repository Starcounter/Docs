# Primitive arrays and single value types

## Introduction

You can create JSON-by-example that contains a primitive value, object, or array. In C\#, all of these are handled the same way.

## Type checking

 To check the type of a `Json`-instance, use one of these methods that are in the the `Starcounter.Advanced.XSON` namespace::

* `IsBoolean`
* `IsDecimal`
* `IsDouble`
* `IsInteger`
* `IsString`

{% code-tabs %}
{% code-tabs-item title="SimpleIntegerJson.json" %}
```javascript
123
```
{% endcode-tabs-item %}
{% endcode-tabs %}

```csharp
var json = new SimpleIntegerJson();
Console.WriteLine(json.IsInteger()); // => true
Console.WriteLine(json.IsString()); // => false
```

## Getting and setting single value types

 To get or set values, use these extension methods that are in the `Starcounter.Advanced.XSON` namespace:

| Value | Set | Get |
| :--- | :--- | :--- |
| Boolean | SetBooleanValue\(value\) | GetBooleanValue\(\) |
| Integer | SetIntegerValue\(value\) | GetIntegerValue\(\) |
| Decimal | SetDecimalValue\(value\) | GetDecimalValue\(\) |
| Double | SetDoubleValue\(value\) | GetDoubleValue\(\) |
| String | SetStringValue\(value\) | GetStringValue\(\) |

{% code-tabs %}
{% code-tabs-item title="SimpleStringJson.json" %}
```javascript
"simple string"
```
{% endcode-tabs-item %}
{% endcode-tabs %}

```csharp
var json = SimpleStringJson();
Console.WriteLine(json.GetStringValue()); // => simple string
json.SetStringValue("another string");
Console.WriteLine(json.GetStringValue()); // => another string
```

 Trying to get or set to values of a different type will throw `InvalidOperationException`:

{% code-tabs %}
{% code-tabs-item title="SimpleStringJson.json" %}
```javascript
"simple string"
```
{% endcode-tabs-item %}
{% endcode-tabs %}

```csharp
var json = SimpleStringJson();
Console.WriteLine(json.GetIntegerValue()); // => false
json.SetIntegerValue(123); // InvalidOperationException
```

## Getting and setting primitive arrays

  
Values are added to arrays with the `Add` method. To get the values of an array, use `ToJson`.

{% code-tabs %}
{% code-tabs-item title="SingleArrayJson.json" %}
```javascript
[ ]
```
{% endcode-tabs-item %}
{% endcode-tabs %}

```csharp
var json = new SingleArrayJson();

json.Add(1);
json.Add("foo");

Console.WriteLine(json.ToJson()); // => [1, "foo"]
```

In the example above, the array holds values of different types. To restrict the array to one type, add a value of the type you want in the JSON-by-example. This value will not be included in the resulting JSON.

{% code-tabs %}
{% code-tabs-item title="SingleArrayJson.json" %}
```javascript
[ 99 ]
```
{% endcode-tabs-item %}
{% endcode-tabs %}

```csharp
var json = new SingleArrayJson();

Console.WriteLine(json.ToJson()); // => []
json.Add(4);
json.Add(2);
Console.WriteLine(json.ToJson()); // => [4, 2]
json.Add("foo"); // InvalidOperationException
```

