# Routing, Middleware and Context

## Low level APIs in Starcounter

Starcounter itself provides a low-level API to route the incoming HTTP requests - `Handle.GET`. You can read about it in the ["Handling HTTP requests"](https://docs.starcounter.io/guides/network/handling-http-requests/) chapter. This is low-level API that is sometimes needed, but usually should be avoided.

## High level routing - new, recommended way

Please see [Authorization library](https://github.com/Starcounter/authorization#routing-middleware-and-context---concepts) for overview of high level routing. This is a new, convenient way to route your HTTP requests.