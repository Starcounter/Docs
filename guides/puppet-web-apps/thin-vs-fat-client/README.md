# Thin vs fat client

Starcounter's MVVM pattern keeps all the application logic on the server, right where the data is. Only the reflection of current application state is being sent to the thin client. The client can be implemented using web standards and rendered in a web browser. With that approach, you can morph the UI without reloading the whole page, leading to smallest download sizes possible.

## Why not fat client

Most of today's single page applications (SPAs) implement a fat client (a.k.a. thick client) architecture. They are built with frameworks such as AngularJS, Ember, React or Polymer and work by performing Ajax calls to a stateless server over a REST API.

Thick client is a satisfactory architecture when there is a slow server that needs to be scaled out to achieve decent performance. On the drawbacks side, it leads to duplicate application logic and huge amounts of glue code just to handle the REST API on the client and the server. Maintaining it is a challenge from the cost, time to market and security perspective. That's a thing you want to avoid.

## Thin client and security

Keeping your logic away from the client removes a whole attack vector. If a malicious user manipulates the client code, you run no risk of him overriding the boundaries of your application. The application is defined on the server and the client merely controls the widgets used and the visual appearance of you application. This pattern creates safe applications where client code manipulation is not a security concern.

## Thin client and latency

Imagine having a server with 0 ms execution time and a network with 0 ms latency. With instant response times, you would use the server as the only source of truth for the application logic, go home early and keep the UX designer happy.

In fact, Starcounter offers <1 ms execution time for a typical request. Network latency is usually less than 30ms within Europe, 45 ms within US and 90 ms from London to New York. 

You cannot fight with these numbers, because they come from the laws of physics. What you can do, however, is choose how much code and data needs to be transferred over the wire. 