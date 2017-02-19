# Primitive arrays and single value types

It is allowed to specify JSON-by-example that contains of a single primitive value, object or array. In C# all of these are handled in the same way. A TypedJson instance is created and values/properties are read and written in the same way both for single values and complex objects and arrays.

**NOTE:** Starting from Starcounter version 2.4.0.995 a number of properties for working with single value json have been obsoleted in favour of using extensionmethods. The extension can be found in namespace `Starcounter.Advanced.XSON`. For a list of obsoleted properties see the last section on this page.

#### TypedJSON with single value
Each value that can be used as a single value json (`Boolean`, `Integer`, `Decimal`, `Double` and `String`) has corresponding methods for getting, setting and check type in the extension class.

If a method is called on a json that is not of the correct type, for example trying to get a string value from a json that holds an integer, an exception will be thrown.

| Value | Extensionmethods |
| ----- | ---------------- |
| Boolean | GetBooleanValue(), SetBooleanValue(value), IsBoolean() |
| Integer | GetIntegerValue(), SetIntegerValue(value), IsInteger() |
| Decimal | GetDecimalValue(), SetDecimalValue(value), IsDecimal() |
| Double  | GetDoubleValue(), SetDoubleValue(value), IsDouble() |
| String  | GetStringValue(), SetStringValue(value), IsString() |

An example of using single value json can be found below (Example 1).

#### Single values in arrays
As declared in the first section of this page, a TypedJSON instance containing a single value is no different from more complex objects, which means that they can be used with arrays without any special handling.

An array can, exactly the same as for arrays with objects, either be typed to hold only one type (example 2) or the array can be untyped (example 3) which means that it can contain any type of json (objects, single values or other arrays).

The extension contains a set of methods to make it simpler to add a new instance to an array with a specific single value. Each supported value has an add method with the following signature: `Json array.Add(value);` where value is one of the supported values.

If these methods are called on a TypedJSON instance that either is not an array or cannot hold the specific value, an exception will be thrown.

<h4>Example 1, single primitive value</h4>

In the following example we create a JSON-by-example file containing of a single string value, then we create a new instance, checks that the type is string, print the default value, sets another value and print that one and finally prints the whole typedjson-object as json.

<div class="code-name">SingleValueJson.json</div>

<pre><code class="javascript">"Default"
</code></pre>

<div class="code-name">Program.cs</div>

```cs
using Starcounter.Advanced.XSON;

public static void Main()
{
    var json = new SingleValueJson();
    Console.WriteLine("Json is string: " + json.IsString());
    Console.WriteLine("Value is: " + json.GetStringValue());
    json.SetStringValue("Changed");
    Console.WriteLine("Value is: " + json.GetStringValue());
    Console.WriteLine("ToJSon: " + json.ToJson());
}
```

Running this example (project called SingleValueTest) will print the following to the console:

<pre><code>&gt; star.exe SingleValueTest.exe
SingleValueTest -&gt; default (started, default port 8080, admin 8181)
Json is string: true
Value is: Default
Value is: Changed
ToJSon: "Changed"
</code></pre>

<h4>Example 2, single array with integers</h4>

A single array containing integers. We add two items and print.

<div class="code-name">SingleArrayJson.json</div>

<pre><code class="javascript">[ 99 ]
</code></pre>

<div class="code-name">Program.cs</div>

```cs
using Starcounter.Advanced.XSON;

public static void Main()
{
    var json = new SingleArrayJson();

    json.Add(1); // Adding an item to the array.
    json.Add(2); // Adding another item.
	
    // The following line would throw an exception since the array is typed to hold only integers. 
    // json.Add("Incorrect"); 
    
    foreach (Json child in json)
    {
        Console.WriteLine(child.GetIntegerValue());
    }
    Console.WriteLine("ToJSon: " + json.ToJson());
}
```

Running this example (project called SingleArrayTest) will print the following to the console:

<pre><code>&gt; star.exe SingleArrayTest.exe
SingleArrayTest -&gt; default (started, default port 8080, admin 8181)
1
2
ToJSon: [1,2]
</code></pre>

<h4>Example 3, array for any type of json</h4>

An array that can contain any type of json. Both single value and more complex jsonobjects.

<div class="code-name">AnyArrayJson.json</div>

<pre><code class="javascript">[ ]
</code></pre>

<div class="code-name">Program.cs</div>

```cs
using Starcounter.Advanced.XSON;

public static void Main()
{
    var json = new AnyArrayJson();

    json.Add(19); // Adding integer.
    json.Add("Test"); // Adding string.
    json.Add(true); // Adding boolean

    foreach (Json child in json)
    {
        Console.WriteLine(child.ToJson());
    }
    Console.WriteLine("ToJSon: " + json.ToJson());
}
```

Running this example (project called AnyArrayTest) will print the following to the console:

<pre><code>&gt; star.exe AnyArrayTest.exe
AnyArrayTest -&gt; default (started, default port 8080, admin 8181)
19
"Test"
true
ToJSon: [19,"Test",true]
</code></pre>


#### Obsoleted properties in class Json
The following table contains the properties in class `Json` that have been marked obsolete and their corresponding method in the extension that should be used instead.

| Obsoleted property | Replacement extensionmethod/s |
| ------------ | ------------------ |
| IsBoolean | IsBoolean() |
| IsDecimal | IsDecimal() |
| IsDouble | IsDouble() |
| IsInteger | IsInteger() |
| IsObject | IsObject() |
| IsArray | IsArray() |
| BooleanValue | GetBoolean(), SetBoolean(value) |
| DecimalValue | GetDecimal(), SetDecimal(value) |
| DoubleValue | GetDoubleValue(), SetDoubleValue(value) |
| IntegerValue | GetIntegerValue(), SetIntegerValue(value) |
| StringValue | GetStringValue(), SetStringValue(value) |