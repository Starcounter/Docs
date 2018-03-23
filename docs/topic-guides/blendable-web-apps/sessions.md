# Sessions

## Introduction

Sessions are used to retain the state in your app. In Starcounter, the state is represented by a view-model, which is technically an instance of the `Json` class. A session is represented by an instance of the `Session` class. The combination of a session and a view-model enables client-server communication using JSON Patch.

## Session instance

A new session is created by calling the constructor on `Session` class. To set the new session as the active session, assign it to the static property `Session.Current`:

```csharp
var session = new Session();
Session.Current = session;
```

Sessions in Starcounter are tied to individual browser tabs, this is different from frameworks like ASP.NET or Zend where sessions store data from all the browser tabs where an app is open. This has two consequences for developers:

1. When multiple apps are running, only one of the apps has to create the `Session` and set the active session.
2. To avoid overriding a session created in another app, check if `Session.Current` is `null`before creating a new session.

`Session.Ensure` can be used to simplify `Session` creation. It checks if `Session.Current` exists and that is has the options `PatchVersioning` and `IncludeNamespaces` as described in the [session flags](sessions.md#session-flags) section. If so, it returns that `Session`. Otherwise, it creates a new `Session`, sets it as `Session.Current` and returns it.

`Session.Ensure` will never return `null`, so property access like this is safe:

```csharp
DateTime createdDate = Session.Ensure().Created;
```

Once you have the session, you can attach state to it. The `session` contains a storage where any number of `Json` instances can be kept, using a string as key. This storage is separated per app, so each running app has it own section and can only access it's own state.

```csharp
var state1 = new Json();
var state2 = new Json();

var session = Session.Ensure();
session.Store["state1"] = state1;
session.Store["state2"] = state2;
...
state1 = session.Store["state1"]
```

{% hint style="info" %}
The storage on the session is _server-side_ only. Nothing stored using `Session.Store` will be exposed to a client. This is a change in behavior from the  obsoleted `Session.Data`. See the next section on how to attach a `Json` to be used for client-server communication
{% endhint %}

## Client-server synchronization with JSON Patch

One additional feature when using `Session`, besides keeping state on the server, is that it can enable client-server synchronization using [JSON Patch](http://tools.ietf.org/html/rfc6902). In short, it allows the client and server to send deltas instead of the full view-model.

When this is used, the client can send HTTP requests using the `PATCH` verb \([HTTP PATCH method](http://tools.ietf.org/html/rfc5789)\) or use WebSocket to send and receive patches.

To enable this, the session needs to know which `Json` instance should be considered the root view-model. If the [PartialToStandalone middleware](https://github.com/Starcounter/Docs/blob/2ffb62f69111b62d73cc9493ef5f190250aa8729/guides/network/middleware#partialtostandalonehtmlprovider) is used, the root view-model will be automatically assigned to the session based on the `Json`instance returned from a handler.

To specify which `Json` instance to use as the root, use the `SetClientRoot` extension method from the `Starcounter.XSON.Advanced` namespace. It's called on a `Session` object with a `Json` instance as the argument.

The whole root view-model can be obtained on the client using the HTTP `GET` verb with an URL that is sent in the `Location` header for the response of the request that created the session. The location contains a specific identifier for the session and view-model that is calculated for each session to be non-deterministic.

## Session properties

The `Session` object exposes a few useful properties, including:

| Property | Explanation |
| :--- | :--- |
| `Created` | Session creation time \(UTC\). |
| `LastActive` | Session last active \(a receive or send occurred on a session\) time \(UTC\) |

## Sessions timeout

Inactive sessions that do not receive or send anything are automatically timed out and destroyed. Default sessions timeout, with 1 minute resolution, is set in database config: `DefaultSessionTimeoutMinutes`. Default timeout is 20 minutes. Each session can be assigned an individual timeout, in minutes, by setting `TimeoutMinutes` property.

## Session destruction callback

User can specify an event that should be called whenever session is destroyed. Session destruction can occur, for example, when inactive session is timed out, or when session `Destroy` method is called explicitly. User specified destroy events can be added using `Session.AddDestroyDelegate` on a specific session.

Session can be checked for being active by using `IsAlive` method.

## Operating on Multiple Sessions

Session is created on current Starcounter scheduler and should be operated only on that scheduler. That's why one should never store session objects statically \(same as one shouldn't store SQL enumerators statically\) or use session objects in multithreaded/asynchronous programming. In order to save session and utilize it later please use `Session.SessionId`described below.

One can store sessions by obtaining session ID string \(`Session.SessionId`\). Session strings can be grouped using desired principles, for example when one wants to broadcasts different messages on different session groups. When the session represented by the string should be used, one should call `Session.RunTask(String sessionId, Session.SessionTask task)`. This procedure takes care of executing action that uses session on the scheduler where the session was originally created. This procedure underneath uses `Scheduling.RunTask` thereby it can be executed from arbitrary .NET thread.

There is a variation of `Session.RunTask` that takes care of sessions grouped by some principle: `Session.RunTask(IEnumerable<String> sessionIds, Session.SessionTask task)`. 

To schedule tasks on all active sessions, then `Session.RunTaskForAll` should be used \(note that it runs on all active sessions and if you only need to update few - use `Session.RunTask`\).

## Transmission of the session identity

Current session is determined and set automatically before user handler is called.

Starcounter Gateway uses one of the following ways to determine the session that should be used for calling user handler.

* `Location` + `X-Referer` or `Referer` headers: Using HTTP protocol, when creating a new session, the response will contain the `Location` HTTP header with the value of a newly created session. Client code later can extract this session value from the received `Location` header and use the session in subsequent requests, using either of the specified ways. Often `X-Referer` or `Referer` request headers are used to specify the session value.
* Session as handler URI parameter: Session value can be specified as one of URI parameters when defining a handler, for example:

```csharp
Handle.GET("/usesession/{?}", (Session session, Request request) =>
{
    // Implementation
});
```

* Session Cookie: Add a header on outgoing response by setting property `UseSessionHeader` to `true` and optionally specify name of header with `SessionHeaderName` \(default `X-Location`\).

The priorities for session determination, for incoming requests, are the following \(latter has higher priority than previous\): session on socket, `Referer` header, `X-Referer` header, session URI parameter.

## Session flags

The `Session` constructor has an overload that takes the enum `Session.Flags`. The default flag is `PatchVersioning`. There are five flags:

| Option | Explanation |
| --- | --- |
| `None` | Overrides the default behavior of `new Session()` so that `PatchVersioning` is not used. |
| `IncludeSchema` | Was added for Starcounter 1.x and does not serve a purpose anymore. Is the same as `Session.Flags.None`. |
| `PatchVersioning` | Enables operational transformation with Palindrom. Thus, `PatchVersioning` is required for communication with Palindrom. Is set by default when declaring `new Session()`. |
| `StrictPatchRejection` | Throws an error instead of rejecting changes in two cases: \(1\) when an incoming patch tries to access an object or item in an array that is no longer valid and \(2\) when the client sends a patch with a different format than expected. |
| `IncludeNamespaces` | Enables namespacing of Typed JSON responses. Is the same as `Session.Flags.None` since it's the default behavior. |

## CalculatePatchAndPushOnWebSocket {#calculatepatchandpushonwebsocket}

JSON patches are calculated whenever there's an incoming request that asks for JSON patch. This means asynchronous changes to the view-model that are finished after the response is sent will not be included in the response patch.

For example, if a user clicks a button to send an order, the user expects to see a confirmation message when the order has been sent. Since the operation is scheduled on a separate thread that executes asynchronously to avoid blocking, the task to send the order will finish after Starcounter has sent the response. This means that the user will not see the status message until they send another patch that triggers a response that includes the status message.

```csharp
void Handle(Input.SendOrderTrigger _)
{
    Session.RunTask(Session.Current.SessionId, (session, id) =>
    {
        SendOrder(Order);
        Order.StatusMessage = "Done";
    }); // The user will not see the status message until they send another patch
}
```

To fix this, use `CalculatePatchAndPushOnWebSocket`. That lets the user see the status message immediately after the order has been sent. 

```csharp
void Handle(Input.SendOrderTrigger _)
{
    Session.RunTask(Session.Current.SessionId, (session, id) =>
    {
        SendOrder(Order);
        Order.StatusMessage = "Done";
        session.CalculatePatchAndPushOnWebSocket();
    });
}
```

When calling `CalculatePatchAndPushOnWebSocket`, Starcounter traverses the JSON tree and calculates the difference between the current and previous tree. It then sends patches to the client with the changes.

