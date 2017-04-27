# Primitive arrays and single value types 

<section class="hero">This page contains material that should be considered advanced or non-essential and will not be used in the absolute majority of Starcounter applications.</section>

It is allowed to specify JSON-by-example that contains of a single primitive value, object or array. In C# all of these are handled in the same way. A Typed JSON instance is created and values/properties are read and written in the same way both for single values and complex objects and arrays.

For setting and getting single values a set of predefined properties exists, both to handle the value and to check what type a Typed JSON-instance is. If a value that does not correspond to the type of JSON is used an exception will be raised. For example trying to read <code>DecimalValue</code> on  a jsonobject that holds a string will not work.

The following properties are used to get and set primitive values: <code>BooleanValue</code>, <code>DecimalValue</code>, <code>DoubleValue</code>, <code>IntegerValue</code>, <code>StringValue</code>

There are also properties to check in an efficient way what type a Json-instance is: <code>IsBoolean</code>, <code>IsDecimal</code>, <code>IsDouble</code>, <code>IsInteger</code>, <code>IsString</code>, <code>IsObject</code>, <code>IsArray</code>

<h4>Example 1, single primitive value</h4>

In the following example we create a JSON-by-example file containing of a single string value, then we create a new instance, checks that the type is string, print the default value, sets another value and print that one and finally prints the whole Typed JSON-object as JSON.

<div class="code-name">SingleValueJson.json</div>

<pre><code class="javascript">"Default"
</code></pre>

<div class="code-name">Program.cs</div>

```cs
public static void Main()
{
    var json = new SingleValueJson();
    Console.WriteLine("Json is string: " + json.IsString);
    Console.WriteLine("Value is: " + json.StringValue);
    json.StringValue = "Changed";
    Console.WriteLine("Value is: " + json.StringValue);
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
public static void Main()
{
    var json = new SingleArrayJson();

    var item = json.Add(); // Adding an item to the array.
    item.IntegerValue = 1;
    var item2 = json.Add(); // Adding another item.
    item2.IntegerValue = 2;

    foreach (Json child in json)
    {
        Console.WriteLine(child.StringValue);
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