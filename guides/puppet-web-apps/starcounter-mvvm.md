# Starcounter MVVM

If you have used MVC or MVVM frameworks in database applications, it can be worth noting that Starcounter does things a little differently.

## Physical tiers

Most Starcounter applications have just two physical tiers. The client and the server. The client is typically a web browser, a PC or a smartphone or tablet.

## Logical tiers

There are typically three logical tiers in a Starcounter application. The domain model (business model), the view-model and the view. All three tiers are quite different from what you might be used to.

### Tier 1 - The domain model

In most database systems, the back end tier is normally a database management system (DBMS) and the next tier is a application layer that reads and writes data to the DBMS. In Starcounter, these two tiers are merged.

The database image master uses RAM instead of disk. The computer resources needed to move data to and from the database image are greater than actually executing the business logic. Because all data is already in RAM, the Starcounter application layer and the database layer is one and the same. In this way, programming becomes simplified and performance gets better.

### Tier 2 - The view-model

Unlike most database systems, the view-model is mirrored between the client and the server. The server is the owner of the view-model and is the single source of truth. This means that the application logic on the server can simply change the view-model locally and the view is updated accordingly.

While it might sound expensive to keep a copy of each clients view on the server, all data on the server, including all the database data is already in RAM so very little new RAM needs to be allocated to keep track of what is going on. After all, more data exists in the database than is currently on screen on the clients at any given moment for most business applications.

<aside class="read-more"><a href="/guides/typed-json.html">Read more about Typed JSON in Starcounter</a></aside>

Starcounter view-models are Typed JSONs defined by the server. Although JSON is merely a text based notation format for object trees, the Typed JSON implementation is a dynamic runtime tree that allows you to add, remove and change objects.

#### What is Typed JSON?

Typed JSONs are multipurpose vehicles. It supports the six data types of JSON (object, array, boolean, number, string and null). It typically has two uses. The first is to create and receive JSON messages as you would typically do in a REST style service or client. The other is to represent live view-models that can be bound to a user interface (client) or a controller (server).

#### Schema-less or schema-full

Typed JSON allows dynamic (expando-like) creation of JSON trees. In addition, Typed JSON allows you to specify schemas. This can be done in two ways. One way to create a schema is to use the Typed JSON API. Another more innovative way is to provide an example JSON file with a sample instance of a tree. This method is referred to as JSON-by-example and provides you with an intuitive way to view you schema and share it between nodes (clients and servers).

#### Data binding

In addition to allowing the developer to create object trees and assign properties one-by-one, Typed JSON also supports binding data using a special `Data` property. In this way, you can assign database results to populate JSON. The data binding is bi-directional (two-way) such that changes to the data object is reflected in the Typed JSON and the changes to the Typed JSON is propagated to the data object.

Server-side view-models leverage the JSON capabilities of Starcounter. You can create a Typed JSON on the server that you send to the client **and** keep on the server. This means that the server can have direct access to the view-model from its controller logic. This is a more performant and secure way to deal with application logic as the client is left only to deal with the view (presentation logic).

### Tier 3 - The view

Because view-models are shared between the client and server, view rendering is performed on the client. In this way, you get better performance and as the number of users grow, so does the number of CPUs that renders the user interfaces. <!--Also, it makes it easier to provide offline functionality as view-models can easily be created as client-only view-models if need be.-->

When the view-model changes, the UI changes and when the user interacts with the UI, the model changes. In the simplest cases, this is often referred to as data based templating. I.e. you have some JSON data and you inject HTML into the DOM based on that data.
