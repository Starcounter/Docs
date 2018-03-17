# Primitive Arrays and Single Value Types

## Introduction

You can create JSON-by-example that contains a primitive value, object, or array. In C\#, all of these are handled the same way.

## Type Checking

 To check the type of a `Json`-instance, use one of these properties:

* `IsBoolean`
* `IsDecimal`
* `IsDouble`
* `IsInteger`
* `IsString`
* `IsObject`
* `IsArray`

```javascript
123
```

```csharp
var json = new SimpleIntegerJson();
Debug.WriteLine(json.IsInteger); // => true
Debug.WriteLine(json.IsString); // => false
```

## Getting and Setting Single Value Types

 To get or set values, use one of these properties:

* `BooleanValue`
* `DecimalValue`
* `DoubleValue`
* `IntegerValue`
* `StringValue`

```javascript
"simple string"
```

```csharp
var json = SimpleStringJson();
Console.WriteLine(json.StringValue); // => simple string
json.StringValue = "another string";
Console.WriteLine(json.StringValue); // => another string
```

 Trying to get or set to values of a different type will throw `InvalidOperationException`:

```javascript
"simple string"
```

```csharp
var json = SimpleStringJson();
Console.WriteLine(json.IsInteger); // => false
json.IntegerValue = 123; // InvalidOperationException
```

## Getting and Setting Primitive Arrays

 Values are added to arrays with the `Add` method. To get the values of an array, use `ToJson`.

```javascript
[ ]
```

```csharp
var json = new SingleArrayJson();

json.Add().IntegerValue = 1;
json.Add().StringValue = "foo";

Console.WriteLine(json.ToJson()); // [1, "foo"]
```

In the example above, the array holds values of different types. To restrict the array to one type, add a value of the type you want in the JSON-by-example. This value will not be included in the resulting JSON.

```javascript
[ 99 ]
```

```csharp
var json = new SingleArrayJson();

Console.WriteLine(json.ToJson()); // => []
json.Add().IntegerValue = 4;
json.Add().IntegerValue = 2;
Console.WriteLine(json.ToJson()); // => [4, 2]
json.Add().StringValue = "foo"; // InvalidOperationException
```

 Adding strings can be further simplified with an overload of `Add`:

```csharp
var json = EmptyArrayJson();
json.Add("foo");
Console.WriteLine(json.ToJson()); // => ["foo"]
```

