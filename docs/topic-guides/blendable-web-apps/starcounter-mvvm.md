# Starcounter MVVM

## Introduction

Starcounter follows the model-view-viewmodel \(MVVM\) pattern. Although, it still has some distinct differences with other frameworks that use model-view-controller \(MVC\) or MVVM. This page describes some of these differences.

## Model

In most database systems, the back end tier is a database management system \(DBMS\) and the next tier is an application layer that reads and writes data to the DBMS. In Starcounter, these two tiers are merged.

The database image master uses RAM instead of disk. Without Starcounter, the computer resources needed to move data to and from the database image are greater than executing the business logic. With Starcounter, all data is already in RAM. The application layer and the database layer is the same. This way, development is simplified and performance is improved.

## View-model

Unlike most application frameworks, the view-model is mirrored between the client and the server. The server is the owner of the view-model and is the single source of truth. This means that the application logic can change the view-model locally on the server and the view on the client is updated accordingly.

While it might sound expensive to keep a copy of each client's state on the server, it actually is not. All data on the server, including all the database data is already in RAM so very little new RAM needs to be allocated to keep track of what is going on. After all, more data exists in the database than is currently on screen on the clients at any given moment for most business applications.

Starcounter view-models are [Typed JSON classes](../typed-json/) defined by the server. Although JSON is a text based notation format for object trees, the Typed JSON implementation is a dynamic runtime tree that allows you to add, remove and change objects.

### What is Typed JSON?

Typed JSON is a multipurpose vehicle. It supports the six data types of JSON \(object, array, boolean, number, string and null\). It typically has two uses. The first is to create and receive JSON messages as you would typically do in a REST style service or client. The other is to represent live view-models that can be bound to a user interface \(client\) or a controller \(server\).

### Schema-less or schema-full

Typed JSON allows dynamic creation of JSON trees. In addition, Typed JSON allows you to specify schemas. This can be done in two ways: with the Typed JSON API or by using JSON-by-example,  where the developer defines a sample JSON tree and Starcounter constructs the Types JSON schema from it. JSON-by-example gives you an intuitive way to view you schema and share it between the client and server.

### Data binding

In addition to allowing the developer to create object trees and assign properties one-by-one, Typed JSON also supports binding data with the `Data` property. In this way, you can assign database results to populate JSON. The data bindings are two-way bindings: changes to the data object is reflected in the Typed JSON and the changes to the Typed JSON is propagated to the data object.

Server-side view-models leverage the JSON capabilities of Starcounter. You can create a Typed JSON on the server that you send to the client **and** keep on the server. This means that the server can have direct access to the view-model from its controller logic. This is a faster and more secure way to deal with application logic as the client will only deal with view logic.

## View

Because view-models are shared between the client and server, it allows offsetting the view rendering to the client. In this way, you get better performance and as the number of users grow, so does the number of CPUs that renders the user interfaces.

When the view-model changes, the UI changes and when the user interacts with the UI, the model changes. In the simplest cases, this is often referred to as data based templating - you have some JSON data and you inject HTML into the DOM based on that data.

