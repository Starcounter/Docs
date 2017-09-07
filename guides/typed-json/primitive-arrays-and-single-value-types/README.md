# Primitive Arrays and Single Value Types 

You can create JSON-by-example that contains a primitive value, object, or array. In C#, all of these are handled the same way.

## Type Checking

To check the type of a `Json`-instance, use one of these methods that are in the the `Starcounter.Advanced.XSON` namespace:
* `IsBoolean`
* `IsDecimal`
* `IsDouble`
* `IsInteger`
* `IsString`

<div class="code-name">SimpleIntegerJson.json</div>

```json
123
```

```cs
var json = new SimpleIntegerJson();
Debug.WriteLine(json.IsInteger()); // => true
Debug.WriteLine(json.IsString()); // => false
```

## Getting and Setting Single Value Types

To get or set values, use these extension methods that are in the `Starcounter.Advanced.XSON` namespace:

| Value | Set | Get |
| ----- | ----------- | -------- |
| Boolean | GetBooleanValue() | SetBooleanValue(value)|
| Integer | GetIntegerValue() | SetIntegerValue(value)|
| Decimal | GetDecimalValue() | SetDecimalValue(value)|
| Double  | GetDoubleValue() | SetDoubleValue(value)|
| String  | GetStringValue() | SetStringValue(value)|

<div class="code-name">SimpleStringJson.json</div>

```json
"simple string"
```

```cs
var json = SimpleStringJson();
Console.WriteLine(json.GetStringValue()); // => simple string
json.SetStringValue("another string");
Console.WriteLine(json.GetStringValue()); // => another string
```

Trying to get or set to values of a different type will throw `InvalidOperationException`:

<div class="code-name">SimpleStringJson.json</div>

```json
"simple string"
```

```cs
var json = SimpleStringJson();
Console.WriteLine(json.GetIntegerValue()); // => false
json.SetIntegerValue(123); // InvalidOperationException
```

## Getting and Setting Primitive Arrays

Values are added to arrays with the `Add` method. To get the values of an array, use `ToJson`.

<div class="code-name">SingleArrayJson.json</div>

```json
[ ]
```

```cs
var json = new SingleArrayJson();

json.Add(1);
json.Add("foo");

Console.WriteLine(json.ToJson()); // [1, "foo"]
```

In the example above, the array holds values of different types. To restrict the array to one type, add a value of the type you want in the JSON-by-example. This value will not be included in the resulting JSON.

<div class="code-name">SingleArrayJson.json</div>

```json
[ 99 ]
```

```cs
var json = new SingleArrayJson();

Console.WriteLine(json.ToJson()); // => []
json.Add(4);
json.Add(2);
Console.WriteLine(json.ToJson()); // => [4, 2]
json.Add("foo"); // InvalidOperationException
```
