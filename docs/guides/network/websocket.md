# WebSocket

WebSocket is a web technology providing full-duplex communications channels over a single TCP connection.

> Learn more about WebSocket on Wikipedia

Starcounter WebSocket implementation is based on RFC 6455.

WebSocket connection upgrade can be made inside an ordinary HTTP handler by calling `SendUpgrade` method on received HTTP `Request` object, for example:

```csharp
Handle.GET(8080, "/wstest", (Request req) =>
{
    // Checking if its a WebSocket upgrade request.
    if (req.WebSocketUpgrade)
    {
        // Setting some headers and cookies on response for WebSockets upgrade.
        List<String> myCookies = new List<String>
        {
            "MyCookie1=123", "MyCookie2=456"
        };

        var myHeaders = new Dictionary<String, String>()
        {
            { "MyHeader", "MyHeaderData" }
        };

        // Performing upgrade and getting WebSocket object
        // (SendUpgrade call implicitly sends an HTTP response confirming
        // WebSocket upgrade, so another response can't be returned
        // in this handler).
        WebSocket ws = req.SendUpgrade("echotestws", myCookies, myHeaders);

        // Immediately sending a message on the obtained WebSocket.
        ws.Send("Hello WebSocket!");

        // Sending another message on the WebSocket.
        ws.Send("Hello again WebSocket!");

        // Indicating that response on the original request was already
        // sent (during SendUpgrade call).
        return HandlerStatus.Handled;
    }

    // We only support WebSockets upgrades in this HTTP handler
    // and not other ordinary HTTP requests.
    return new Response()
    {
        StatusCode = 500,
        StatusDescription = "WebSocket upgrade on " + req.Uri + " was not approved."
    };
});
```

Once the `SendUpgrade` is called and WebSocket object is available, it can immediately be used to perform sends and other operations allowed on WebSocket.

WebSocket can be identified as a one UInt64 integer, that user can store in database and by some other means. Whenever needed, user can restore WebSocket object using the ID and perform operations on that object.  
To obtain a WebSocket ID, call `WebSocket.ToUInt64()`, in opposite, to create a WebSocket object just pass the UInt64 ID to WebSocket constructor:

```csharp
WebSocket ws = new WebSocket(savedWsId);
ws.Send("My server message!");
```

### Details of the SendUpgrade call

When the `SendUpgrade` method is called, the approving WebSocket upgrade HTTP Response is sent immediately. To be able to get the WebSocket ID before calling `SendUpgrade`, call the `UInt64 Request.GetWebSocketId()`.

The WebSocket `SendUpgrade` method on Request has the following signature:

```csharp
WebSocket SendUpgrade(
    String groupName,
    List<String> cookies = null,
    Dictionary<String, String> headers = null,
    IAppsSession session = null)
```

where:

* `groupName`: string identifying WebSocket "group" on which subsequent events for this WebSocket should arrive.
* `cookies`: Cookies that should be set on returned WebSocket upgrade HTTP response.
* `headers`: Custom headers that should be set on returned WebSocket upgrade HTTP response.
* `session`: a session object that is attached to this WebSocket in subsequent events.

Once the WebSocket object is returned, user can fetch the ID representing this WebSocket \(`ToUInt64()`\), and of course, perform data sends and disconnect.

### WebSocket group name and disconnect handlers

To register a specific WebSocket group to handle WebSocket data receiving events, the `Handle.WebSocket` method should be used, for example:

```csharp
Handle.WebSocket("echotestws", (String s, WebSocket ws) =>
{
    Console.WriteLine("Received on WebSocket: " + s);
    WebSocket.Current.Send("Here is the server push!");
    ws.Send("Here is the server push!"); // Does the same as previous line.   
});

Handle.WebSocket("echotestws", (Byte[] s, WebSocket ws) =>
{
    ws.Send(s);
});
```

As the name suggests, the group name is used to group together certain types of WebSockets, for example, chat messages, log events, game objects updates, etc.

Note that if arriving frame type is `Text` and the only registered handler is for `Binary` then WebSocket connection will be closed with a type: `WS_CLOSE_CANT_ACCEPT_DATA`. Same applies for vice-versa scenario.

To handle WebSocket disconnect event, the `Handle.WebSocketDisconnect` method should be used, for example:

```csharp
Handle.WebSocketDisconnect("echotestws", (WebSocket ws) =>
{
    // Handle resources associated with WebSocket ws.
});
```

Disconnect event is triggered when underlying socket connection is closed, regardless the reason for it \(e.g. normal WebSocket closure, connection drop, etc\). Note that WebSockets have no inactive timeout disconnect.

### WebSocket object

The returned `WebSocket` object contains the following notable methods:

Sending string data on WebSocket:

```csharp
void Send(String data,
  Boolean isText = true,
  Response.ConnectionFlags connFlags = Response.ConnectionFlags.NoSpecialFlags)
```

Sending binary data on WebSocket:

```csharp
void Send(Byte[] data,
  Boolean isText = false,
  Response.ConnectionFlags connFlags = Response.ConnectionFlags.NoSpecialFlags)
```

Using `isText` parameter you can specify if your WebSocket data should be sent as `Text` or `Binary` frame \(see RFC6455\)

`Response.ConnectionFlags` parameter is used to manipulate corresponding WebSocket connection: for example, if you want to close the connection with client or send data and then close the connection. Please see section about handling responses for more information on `Response.ConnectionFlags`.

NOTE: When doing operations on the same WebSocket but from different Starcounter schedulers - the order in which operations \(like `Send`\) will actually be performed is not guaranteed to be the same as the order in which they were initiated.

Disconnecting an active WebSocket \(with an error message and/or close code according to RFC6455\):

```csharp
void Disconnect(String message = null,
  WebSocketCloseCodes code = WebSocketCloseCodes.WS_CLOSE_NORMAL)
```

Other following methods and properties are available:

* `UInt64 ToUInt64()`: identifier that represents the WebSocket, normally used for storage in user code.
* `IAppsSession Session`: session object that was initially attached on WebSocket if any.
* `static WebSocket Current`: static object identifying currently active WebSocket.
* `enum WebSocketCloseCodes`: list of available WebSocket disconnect codes, according to RFC6455.
* `Boolean IsDead()`: checks if WebSocket is already disconnected or invalid.

### Operating on multiple WebSockets

Sometimes its needed to perform the some "action" on a group of WebSocket connections, for example, to unicast some common data. To accomplish this, user can create a WebSocket object from saved UInt64 ID and perform operations with that object \(send/disconnect\). The only requirement is that operating thread should be attached to Starcounter scheduler. The following code snippet demonstrates broadcasting of previously saved WebSockets every second from arbitrary .NET thread:

```csharp
[Database]
public class WebSocketId
{
    // WebSocket identifier.
    public UInt64 Id;

    // Number of messages sent on this WebSocket.
    public UInt32 NumBroadcasted;
}

...
WebSocketSessionsTimer = new Timer((state) =>
{
    // Getting sessions for current scheduler.
    Scheduling.ScheduleTask(() =>
    {
        Db.Transact(() =>
        {
            // Going through all active WebSockets.
            var result = Db.SQL("SELECT w FROM WebSocketId w");
            foreach (WebSocketId wsDb in result)
            {
                WebSocket ws = new WebSocket(wsDb.Id);
                // Checking if ready to disconnect
                // after certain amount of sends.
                if (wsDb.NumBroadcasted == 100)
                {
                    // Disconnecting this WebSocket.
                    ws.Disconnect();
                    wsDb.Delete();

                    // Proceeding to next active WebSocket.
                    continue;
                }
                String sendMsg = "Broadcasting id: " + wsDb.Id;
                // Sending message to this WebSocket.
                ws.Send(sendMsg);
                wsDb.NumBroadcasted++;
            }
        });
    });
}, null, 1000, 1000);
```

### Additional information

`Session` object contains the reference to the last active WebSocket: `ActiveWebsocket`, which is associated with this session. `ActiveWebsocket` however can be `null` when session has no WebSocket attached to it.

