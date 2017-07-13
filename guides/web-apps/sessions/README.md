# Sessions

Sessions are used to retain the client state in your application. A session is represented by an instance of a `Session` class. Session is created on current Starcounter scheduler and should be operated only on that scheduler. That's why one should never store session objects statically (same as one shouldn't store SQL enumerators statically) or use session objects in multithreaded/asynchronous programming. In order to save session and utilize it later please use `Session.SessionId` described below.

Sessions and JSON objects can be connected: on JSON object session can be accessed using `Session` property, and on `Session` object, JSON object can be accessed through `Data` property.

New session is created by calling constructor on `Session` class. Current session (can be accessed by calling static `Session.Current` property) is automatically set during lifetime of the user handler execution. Whenever session object is set on JSON object using `Session` property (or JSON object is set on session using `Data` property), the `Session.Current` is automatically being set to that object.

Here are some examples of creating a new session (property `UseSessionCookie` is described below). All examples will produce the same result:

```cs
var json = new Json();

// Creating a new session

Session.Current = new Session()
{
    Data = json,
    UseSessionCookie = true
};

// Above is the same as

json.Session = new Session()
{
    UseSessionCookie = true
};

// Above is the same as

json.Session = new Session()
{
    UseSessionCookie = true,
    Data = json
};
```

## Useful Session Properties

Session creation time (UTC) can be retrieved from the `Created` property.
Session last active (a receive or send occurred on a session) time (UTC) is fetched from the `LastActive` property.

## Sessions Timeout

Inactive sessions (that do not receive or send anything) are automatically timed out and destroyed. Default sessions timeout (with 1 minute resolution) is set in database config: `DefaultSessionTimeoutMinutes`. Default timeout is 20 minutes. Each session can be assigned an individual timeout, in minutes, by setting `TimeoutMinutes` property.

## Session Destruction Callback

User can specify an event that should be called whenever session is destroyed. Session destruction can occur, for example, when inactive session is timed out, or when session `Destroy` method is called explicitly. User specified destroy events can be added using `Session.AddDestroyDelegate` on a specific session.

Session can be checked for being active by using `IsAlive` method.

## Operating on Multiple Sessions

One can store sessions by obtaining session ID string (`Session.SessionId`). Session strings can be grouped using desired principles, for example when one wants to broadcasts different messages on different session groups. When the session represented by the string should be used, one should call `Session.ScheduleTask(String sessionId, Action<Session, string> task, Boolean waitForCompletion = false)`. This procedure takes care of executing action that uses session on the scheduler where the session was originally created. This procedure underneath uses `Scheduling.ScheduleTask` thereby it can be executed from arbitrary .NET thread. If flag `waitForCompletion` is set to try, the action is scheduled and calling thread waits for its completion.

There is a variation of `Session.ScheduleTask` that takes care of sessions grouped by some principle: `Session.ScheduleTask(IEnumerable<String> sessionIds, Action<Session, string> task)`. Use it if you want to operate on a group of sessions, like in the following chat application example:

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

## Sessions and JSON Objects

Starcounter allows you to keep JSON objects on the server as resources without storing them in the database.

This is very useful when you want to keep server side session data such as server side view models.
JSON object can be attached to session by assigning `Data` property on `Session` object.

<div class="code-name">PersonView.json</div>

```javascript
{
   FirstName: "",
   LastName: "",
   Message: ""
}
```

<div class="code-name">Program.cs</div>

```cs
using Starcounter;

class Program  
{
  static void Main()
  {
    Handle.POST("/persons", () =>
    {
      var personView = new PersonView();

      Session.Data = personView;
      return personView;
    });
  }
}
```
When responding to a request, Starcounter will check if `Session.Data` is `null`. If not, Starcounter will create a resource with an unique URI to represent a session. In this case, the URI might be `/__default/D11C498A1A5F64ABD0000010`.

## Session Determination

Starcounter Gateway uses one of the following ways to determine the session that should be used for calling user handler.

* `Location` + `X-Referer` or `Referer` headers:
Using HTTP protocol, when creating a new session, the response will contain the `Location` HTTP header with  the value of a newly created session. Client code later can extract this session value from the received `Location` header and use the session in subsequent requests, using either of the specified ways. Often `X-Referer` or `Referer` request headers are used to specify the session value.

* Automatic session creation when using WebSockets:
When using WebSockets protocol, the session is automatically created (unless its already it was already on the socket). The session is kept for this WebSocket connection until it closes or session is explicitly destroyed. Session can also be changed during lifetime of WebSocket by setting `Session.Current` property.

* Session as handler URI parameter:
Session value can be specified as one of URI parameters when defining a handler, for example:

```cs
Handle.GET("/usesession/{?}", (Session session, Request request) =>
{
    // Implementation
});
```

* Session Cookie:

`Session` object has a property called `UseSessionCookie` indicating that a Web browser's cookie `ScSessionCookie` should be used for transferring the session value. Client's Web browser can store and automatically send the `ScSessionCookie` cookie containing the session. Whenever `UseSessionCookie` is set, the `Location` header that is specified in the first case is not used.

The priorities for session determination are the following (latter has higher priority than previous):
session on socket, `Referer` header, session cookie, `X-Referer` header, session URI parameter.

## Interacting With a Server-Side JSON Objects

The newly created JSON object is automatically made available to the client using the GET method. I.e. if the response to the `PersonView` object in the example above returns a location such as `Location: /__default/D11C498A1A5F64ABD0000010`, you can access this resource by doing a `GET /__default/D11C498A1A5F64ABD0000010`. If the resource have not been accessed for a configurable period of time, it will be removed from the server.

More important, however is the built in support for the [HTTP PATCH method](http://tools.ietf.org/html/rfc5789) and [JSON-Patch](http://tools.ietf.org/html/rfc6902). This allows Starcounter to communicate using delta updates rather than sending complete JSON representations of the entire resource.

## Session Options

The `Session` constructor has an overload that takes the enum `SessionOptions`. This enum has five options:

|Option|Explanation|
|---|---|
|`Default`| Is the default behavior of `Session`, declaring `new Session(SessionOptions.Default)` is the same as using the default constructor. |
|`IncludeSchema`| Was added for Starcounter 1.x and does not serve a purpose anymore. Is the same as using the default constructor.  |
|`PatchVersioning`| Enables operational transformation with Puppet/Palindrom. Thus, `PatchVersioning` is required for communication with Puppet/Palindrom.  |
|`StrictPatchRejection`| Throws an error instead of rejecting changes in two cases: (1) when an incoming patch tries to access an object or item in an array that is no longer valid and (2) when the client sends a patch with a different format than expected. |
|`IncludeNamespaces`|Enables namespacing of Typed JSON responses. This is the default behavior and is thus the same as using the default constructor.|
