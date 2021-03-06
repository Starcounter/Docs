# Internal Self Calls

## Introduction

Starcounter provides an efficient way for REST communication within the codehost instance. Simply put, `Self` is used to call handlers that are registered using the `Handle` class inside the codehost. To communicate between different codehosts, `Http` should be used. `Self` communication does not use either networking or shared memory, so it is very efficient. It is represented by the `Self` class, which is similar to the `Http` interface.   
For example, the same HTTP methods are supported, as in `Http`. However, in comparison, `Self` calls are always synchronous, so asynchronous mode is not presented in it. Like for `Http`, the `Response` object is returned as a result of `Self` call. To conclude, `Self` is used ubiquitously in Starcounter as it is the core REST communication mechanism.

## Usage

Here are some examples of `Self` calls:

```csharp
Response resp = Self.GET("/MyHandler");
```

Templated `Self` can be used to specify what object type is expected in `Body` of the `Response` and gets it as a return value, for example:

```csharp
Json json = Self.GET<Json>("/MyApp/MyJsonObject/13235");
```

Here, an object of type `Json` is expected to be in the `Body`.

A specific JSON type can also be used:

```csharp
Master master = Self.GET<Master>("/emails");
```

Here is an example of expecting and obtaining the string `Body`:

```csharp
String myText = Self.GET<String>("/MyApp/MyTextDocument/54664");
```

or expecting a binary body:

```csharp
Byte[] myBinaryData = Self.GET<Byte[]>("/EncodedDocument/34563");
```

Note that if the actual response `Body` object returned in handler is of different type than expected - the conversion exception will be thrown.

## Getting Current Level in the Call Hierarchy

The hierarchy of `Self` calls can be quite deep and sometimes its needed to get the current call level. To achieve that there is a special thread static variable `Handle.CallLevel`. Every `Self` call the variable is incremented and then restored to current value on the way back.

