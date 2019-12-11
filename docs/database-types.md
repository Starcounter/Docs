# Database types

Starcounter supports most of the .NET CLR primitive types.

## List of supported .NET CLR types

- `Boolean`
- `Byte`
- `DateTime` – [limitations](#datetime)
- `Decimal` – [limitations](#decimal)
- `Double`
- `Int16`
- `Int32`
- `Int64`
- `SByte`
- `Single`
- `String` – [limitations](#string)
- `UInt16`
- `UInt32`
- `UInt64`

### DateTime

Starcounter saves and returns all `DateTime` values in UTC.

```csharp
[Database]
public abstract class Item
{
    public abstract DateTime Value { get; set; }

    public void TestDateTimeValues()
    {
        var v = new DateTime(DateTime.Now.Ticks, DateTimeKind.Local);

        // The value and the kind are converted into UTC when the CLR type
        // is converted to a database type, which happens here:
        this.Value = v;

        var kindAfter = this.Value.Kind; // System.DateTimeKind.Utc
    }
}
```

Starcounter does not yet support the `DateTimeOffset` data type.

### Decimal

The `Decimal` values are stored as a 64-bit integer and has a precision of six decimals and a range between `-4398046511103.999999` and `4398046511104.999999`. Trying to store a `Decimal` value with higher precision or outside of the specified range will result in the following exception: `ScErrCLRDecToX6DecRangeError (SCERR4246)`. In those cases, `Double` can be used if the data loss is acceptable.

### String

The string data type can store data up to 1 MiB of encoded text. Thus, all strings with a length of less than 270600 will fit into the string data type. Strings with more than 270600 characters might fit depending on string content.

Starcounter does not yet support non-nullable strings.

## Binary

The `Binary` data type is a Starcounter-defined datatype used to store binary data in the database. The maximum size of a binary field is 1 MiB.

Starcounter only supports nullable binary properties.

```csharp
public abstract class Item
{
    // This defines a nullable binary property.
    // It is supported by Starcounter.
    public abstract Binary? NonNullBinary { get; set; }

    // This defines a not nullable binary property.
    // It is not yet supported by Starcounter.
    public abstract Binary NonNullBinary { get; set; }
}
```
