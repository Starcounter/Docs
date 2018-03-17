# Object identity and object references

Starcounter uses a merger between relational, object-oriented and graph database theories wherein an object reference uses an implicit foreign key to an implicit primary key called ObjectId as well as allowing natural keys and public keys to exist and be joined on when needed.

### Object identity

Each object has a `ObjectNo` and `ObjectID` property that can be accessed using SQL or the extension methods `Object.GetObjectNo()` and `Object.GetObjectID()`.

`ObjectNo` is a `ulong` \(unsigned 64-bit number\) value that can be treated as a primary key. The value is available as soon as an object is created with the `new` operator, and it is never changed. It is guaranteed to be unique within the entire database.

`ObjectID` is a [URL Base64](https://en.wikipedia.org/wiki/Base64#URL_applications) string representation of `ObjectNo`. It is friendly for sending over the Internet. For example, `ObjectID` for `ObjectNo=40023` is `"JxX"`. `ObjectID` may contain `[A-Za-z0-9-_]` characters. Since string comparison in SQL is case insensitive, SQL queries using `ObjectID` may in some cases return more than one result.

### Primary keys and foreign keys

As each object already has a unique key in ObjectNo, we recommend adding other keys solely when you need them, such as public keys that are meant to be exposed to the end user. Objects should be referenced using an object reference rather than by a foreign key except if there is a clear need for natural key referencing.

