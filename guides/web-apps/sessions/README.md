# Sessions

Sessions are used to retain the state in your app. A session is represented by an instance of a `Session` class. Together with an instance of a `Json` object it can be used to enable client-server communication and synchronization using JSON patch.


## Session Instance

A new session is created by calling the constructor on `Session` class. To set the new session as active session, it's necessary to assign it to the static property `Session.Current`.

When multiple apps are running, only one of them needs to create the `Session` object and set is as active, because the session identifies the browser tab, not only your app in that tab. Therefore, you always need to check before creating a new session if `Session.Current` is `null`.

> **Note**: Each browser tab is a separate state of the UI. Therefore, each tab is tied to its own session. This makes it totally different from the session concept in frameworks like ASP.NET or Zend, where a session stores data from all the browser tabs.

Starting with Starcounter 2.3.1.6839 there is a static method, `Session.Ensure()`, that can be used as simplification of the pattern described above that does this check and makes sure that a session with options enabled for patch-versioning and namespaces is set (if not already set) as current and returned. This method can be used everywhere where a session should be created if `Session.Current` is `null`.

```csharp
DateTime createdDate = Session.Ensure().Created; // Session.Ensure() will never be null.
```

Once you have the session, you have the possibility to attach state to it. In Starcounter, the state is represented by a `Json` instance (also called a view-model). The `session` contains a storage where any number of `Json` instances can be kept, using a string as key. This storage is separated per app, so each running app has it own section and can only access it's own state.

> **Note**: From Starcounter 2.3.1.6839, `Session.Data` have been obsoleted and replaced with `Session.Store`. Also `Json.Session` is obsoleted. Session should be obtained by using `Session.Current` or `Session.Ensure()`.

```csharp
Json state1 = new Json();
Json state2 = new Json();

Session session = Session.Ensure();
session.Store["state1"] = state1;
session.Store["state2"] = state2;
...
state1 = session.Store["state1"]
```

> **Note**: The storage on the session is *server-side* only. Nothing stored using `Session.Store` will be exposed to a client. This is a change in behaviour of using the old obsoleted `Session.Data`. See the next section on how to attach a `Json` to be used for client-server communication

## Client-server synchronization using JSON Patch

One additional feature when using `Session`, besides keeping state on the server,  is that it can enable client-server synchronization using [JSON Patch](http://tools.ietf.org/html/rfc6902). In short it allows the client and server to send deltas instead of the full viewmodel.

When this is used, the client can send http requests using the `PATCH` verb ([HTTP PATCH method](http://tools.ietf.org/html/rfc5789)) or use websocket to send and receive patches.

To enable this, the session needs to know which `Json` instance should be considered the root viewmodel. If the [PartialToStandalone middleware](../../network/middleware#partialtostandalonehtmlprovider) is used, the root viewmodel will be automatically assigned to the session based on the `Json` instance returned from a handler.

There is also a way to manually specify which `Json` instance to use as root, `session.SetClientRoot(json)`. This functionality is included as an extension method instead of directly in the session class. The extension-method can be found in the `Starcounter.XSON.Advanced` namespace.


> **Note**: From Starcounter 2.3.1.6839 the handling of determining root viewmodel on session have changed. `Session.PublicViewmodel` and `Session.Data` have been obsoleted and should no longer be used to set client root. Instead use the information in the section above.

The whole root viewmodel can be obtained on the client using HTTP GET verb with a url that is sent in the `Location` header for the response of the request that created the session. The location contains a specific identifier for the session and viewmodel that is calculated for each session to be non-deterministic.


## Session Properties

The `Session` object exposes a few useful properties, including:

|Property|Explanation|
|---|---|
|`Created`| Session creation time (UTC). |
|`LastActive`| Session last active (a receive or send occurred on a session) time (UTC)|

## Sessions Timeout

Inactive sessions (that do not receive or send anything) are automatically timed out and destroyed. Default sessions timeout (with 1 minute resolution) is set in database config: `DefaultSessionTimeoutMinutes`. Default timeout is 20 minutes. Each session can be assigned an individual timeout, in minutes, by setting `TimeoutMinutes` property.

## Session Destruction Callback

User can specify an event that should be called whenever session is destroyed. Session destruction can occur, for example, when inactive session is timed out, or when session `Destroy` method is called explicitly. User specified destroy events can be added using `Session.AddDestroyDelegate` on a specific session.

Session can be checked for being active by using `IsAlive` method.

## Operating on Multiple Sessions

Session is created on current Starcounter scheduler and should be operated only on that scheduler. That's why one should never store session objects statically (same as one shouldn't store SQL enumerators statically) or use session objects in multithreaded/asynchronous programming. In order to save session and utilize it later please use `Session.SessionId` described below.

One can store sessions by obtaining session ID string (`Session.SessionId`). Session strings can be grouped using desired principles, for example when one wants to broadcasts different messages on different session groups. When the session represented by the string should be used, one should call `Session.ScheduleTask(String sessionId, Action<Session, string> task, Boolean waitForCompletion = false)`. This procedure takes care of executing action that uses session on the scheduler where the session was originally created. This procedure underneath uses `Scheduling.ScheduleTask` thereby it can be executed from arbitrary .NET thread. If flag `waitForCompletion` is set to try, the action is scheduled and calling thread waits for its completion.

There is a variation of `Session.ScheduleTask` that takes care of sessions grouped by some principle: `Session.ScheduleTask(IEnumerable<String> sessionIds, Action<Session, string> task)`. Use it if you want to operate on a group of sessions, like in the following chat app example:

```cs
[Database]
public class SavedSession
{
   public string SessionId { get; set; }
   public string GroupName { get; set; }
}

Session.ScheduleTask(Db.SQL<SavedSession>("SELECT s FROM GroupedSession s WHERE s.GroupName = ?", myGroupName).Select(x => x.SessionId).ToList(), (Session session, string sessionId) =>
{
  var master = session.Data as MasterPage;

  if (master != null && master.CurrentPage is ChatGroupPage)
  {
    ChatGroupPage page = (ChatGroupPage)master.CurrentPage;

    if (page.Data.Equals(this.Data))
    {
      if (page.ChatMessagePages.Count >= maxMsgs)
      {
      	page.ChatMessagePages.RemoveAt(0);
      }
    page.ChatMessagePages.Add(Self.GET<Json>("/chatter/partials/chatmessages/" + ChatMessageKey));
    session.CalculatePatchAndPushOnWebSocket();
    }
  }
});
```

To schedule tasks on all active sessions, then `Session.ForAll` should be used (note that it runs on all active sessions and if you only need to update few - use `Session.ScheduleTask`).

## Advanced: Transmission of the session identity

Current session is determined and set automatically before user handler is called.

Starcounter Gateway uses one of the following ways to determine the session that should be used for calling user handler.

* `Location` + `X-Referer` or `Referer` headers:
Using HTTP protocol, when creating a new session, the response will contain the `Location` HTTP header with  the value of a newly created session. Client code later can extract this session value from the received `Location` header and use the session in subsequent requests, using either of the specified ways. Often `X-Referer` or `Referer` request headers are used to specify the session value.

* Automatic session creation when using WebSockets:
When using WebSockets protocol, the session is automatically created (unless it's already it was already on the socket). The session is kept for this WebSocket connection until it closes or session is explicitly destroyed. Session can also be changed during lifetime of WebSocket by setting `Session.Current` property.

* Session as handler URI parameter:
Session value can be specified as one of URI parameters when defining a handler, for example:

```cs
Handle.GET("/usesession/{?}", (Session session, Request request) =>
{
    // Implementation
});
```

* Session Cookie:

Use of automatic session cookie and the property `UseSessionCookie` have been obsoleted. Instead enable adding a header on outgoing response by setting property `UseSessionHeader` to `true` and optionally specify name of header with `SessionHeaderName` (default `X-Location`).

The priorities for session determination, for incoming requests, are the following (latter has higher priority than previous):
session on socket, `Referer` header, `X-Referer` header, session URI parameter.

## Session Options

The `Session` constructor has an overload that takes the enum `SessionOptions`. This enum has five options:

|Option|Explanation|
|---|---|
|`Default`| Is the default behavior of `Session`, declaring `new Session(SessionOptions.Default)` is the same as using the default constructor. |
|`IncludeSchema`| Was added for Starcounter 1.x and does not serve a purpose anymore. Is the same as using the default constructor.  |
|`PatchVersioning`| Enables operational transformation with Palindrom. Thus, `PatchVersioning` is required for communication with Palindrom.  |
|`StrictPatchRejection`| Throws an error instead of rejecting changes in two cases: (1) when an incoming patch tries to access an object or item in an array that is no longer valid and (2) when the client sends a patch with a different format than expected. |
|`IncludeNamespaces`|Enables namespacing of Typed JSON responses. This is the default behavior and is thus the same as using the default constructor.|


