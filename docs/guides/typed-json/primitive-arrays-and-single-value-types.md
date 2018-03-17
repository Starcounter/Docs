# Primitive arrays and single value types

It is allowed to specify JSON-by-example that contains of a single primitive value, object or array. In C\# all of these are handled in the same way. A Typed JSON instance is created and values/properties are read and written in the same way both for single values and complex objects and arrays.

For setting and getting single values a set of predefined properties exists, both to handle the value and to check what type a Typed JSON-instance is. If a value that does not correspond to the type of JSON is used an exception will be raised. For example trying to read DecimalValue on  a jsonobject that holds a string will not work.

The following properties are used to get and set primitive values: BooleanValue, DecimalValue, DoubleValue, IntegerValue, StringValue

There are also properties to check in an efficient way what type a Json-instance is: IsBoolean, IsDecimal, IsDouble, IsInteger, IsString, IsObject, IsArray

Example 1, single primitive value

In the following example we create a JSON-by-example file containing of a single string value, then we create a new instance, checks that the type is string, print the default value, sets another value and print that one and finally prints the whole Typed JSON-object as JSON.







```csharp
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

Running this example \(project called SingleValueTest\) will print the following to the console:



Example 2, single array with integers

A single array containing integers. We add two items and print.







```csharp
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

Running this example \(project called SingleArrayTest\) will print the following to the console:



