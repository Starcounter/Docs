# Typed JSON

## Summary

Typed JSON are C# classes that are serializeable to JSON. The classes make up the view-model in Starcounter.

[JSON-by-example](/guides/typed-json/json-by-example) defines it, the [Code-Behind](/guides/typed-json/code-behind) extends it, and it's [bound]((/guides/typed-json/json-data-bindings) to database data. 

## Content

This section describes how to use Typed JSON.

* [JSON-by-example](/guides/typed-json/json-by-example) and [Code-Behind](/guides/typed-json/code-behind) describes how to define the Typed JSON and extend it to allow for interactivity.

* [JSON Data Bindings](/guides/typed-json/json-data-bindings) explains how to initially add database data to the Typed JSON objects and then bind Typed JSON objects to database classes.

* [Callback Methods](/guides/typed-json/callback-methods) describes the use of callback methods for certain actions on the Typed JSON objects.

* [Responding With JSON](/guides/typed-json/responding-with-json) and [Accepting JSON in Requests](/guides/typed-json/accepting-JSON-in-requests) describes how to send and receive Typed JSON objects using HTTP.

* [Primitive Arrays and Single Value Types](/guides/typed-json/primitive-arrays-and-single-value-types) and [Typed JSON Internals](/guides/typed-json/typed-json-internals) covers topics that, in most cases, are not practically applicable but can still be useful in some niche cases.