# Object identity and object references

In Starcounter, an object reference uses an implicit foreign key to an implicit primary key called `ObjectId` as well as allowing natural keys and public keys to exist and be joined on when needed.

## Object identity

Each object has a `ObjectNo` and `ObjectID` property that can be accessed using SQL or the extension methods `Object.GetObjectNo()` and `Object.GetObjectID()`.

`ObjectNo` is a unique `ulong` that can be treated as a primary key. The value becomes available when a database object is created, and is never changed.

`ObjectID` is a [URL Base64](https://en.wikipedia.org/wiki/Base64#URL_applications) string representation of `ObjectNo`. It's friendly for sending over the Internet. For example, `ObjectID` for `ObjectNo=40023` is `"JxX"`. `ObjectID` may contain `[A-Za-z0-9-_]` characters. Since string comparison in SQL is case insensitive, SQL queries using `ObjectID` may return more than one result.

## Primary keys and foreign keys

As each object already has a unique key in `ObjectNo`, we recommend adding other keys when you need them, such as public keys that are meant to be exposed to the end user. Objects should be referenced using an object reference rather than by a foreign key except if there is a clear need for natural key referencing.

## Retrieving from object identity

Objects are retrieved from identity with `Db.FromId`. It handles both `ObjectID` and `ObjectNo`.

A common use is in handlers like this where `Person` is a database object:

```csharp
Handle.GET("PersonList/{?}", (string objectId) =>
{
    return new PersonPage()
    {
        Data = Db.FromId<Person>(objectId)
    };
});
```

There are four overloads of `Db.FromId`:

```csharp
T Db.FromId<T>(string base64Id);
T Db.FromId<T>(ulong id);
IObjectView FromId(ulong id);
IObjectView FromId(string base64Id);
```

