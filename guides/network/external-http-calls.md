# External HTTP Calls

## Introduction

Starcounter `Http` and `Node` classes represent a convenient way to communicate with other Starcounter instances as well as 3rd party HTTP servers. You can use `Http` and `Node` from within Starcounter programs or as a stand alone packages for generic .NET programs \(as a generic HTTP client\). When used in server Starcounter applications, `Http` and `Node` are suited for communicating **outside** the current codehost, for example, some Web-site or other codehosts. To communicate **within** current codehost, `Self` should be used.

## Why Not Use Existing HTTP APIs?

One reason why you would use `Http` and `Node` instead of using another HTTP client API is that `Http` and `Node` are really REST clients rather than HTTP clients. Another reason is their simplicity for REST style programming as they are normally used as a single line, non-verbose statement.

## How to Use HTTP

A convenient way to use synchronous or asynchronous HTTP calls is to use the `Http` class and its functions `GET`, `POST`, `PUT`, `PATCH` and `DELETE`. To use these calls synchronously makes using HTTP requests very similar to using regular function calls.

## How to Use Node

Instead of using static function calls, you can instantiate a representation of an external HTTP server. This is recommended when performance is critical as this method allows Node to avoid the overhead of parsing the host from the URL and ensures optimal connection reuse. Using the static `Http` class still reuses connections and caches Node instances internally, but it comes at a slight overhead and you run into the risk of the Node instances being disposed and recreated more times than necessary.

### Constructing a Node Instance

The Node constructor accepts the following parameters: the DNS host name of the server we are trying to communicate to, an optional port number, aggregation parameters and an optional receive timeout:

```csharp
Node(String hostName,
     UInt16 portNumber = 0,
     Int32 defaultReceiveTimeoutMs = 0,
     Boolean useAggregation = false,
     UInt16 aggrPortNumber = 0)
```

for example:

```csharp
Node localNode = new Node("www.starcounter.com");
Node localNode2 = new Node("127.0.0.1", 8080);
Node localNode3 = new Node("buildserver", 8080);
```

### Prerequisites

To use Node you need to reference the assembly `Starcounter.Rest` \(to use Request and Response add also a reference to _Starcounter.Internal_\)

**NOTE**: To use Node outside of Starcounter in 32-bit software you should reference the same libraries in the `32BitComponents` subfolder in pointed to by the environment variable `StarcounterBin`, since the 32-bit libraries are not placed in the Windows GAC.

**NOTE**: Node instances are thread unsafe. See _How Node works internally_ section for more details.

### Using Node Methods

Node supports most popular HTTP methods: GET, POST, PUT, DELETE. User can specify arbitrary HTTP method as well.

For example, one of the Node REST **asynchronous** GET call has the following signature:

```csharp
void GET(String uri,
         String customHeaders,
         Object userObject,
         Action<Response, Object> userDelegate,
         Int32 receiveTimeoutMs = 0)
```

where:

* **uri** - **relative** resource URI, e.g.: "/", "/index.html", "/chairs", etc.
* **customHeaders** - user custom headers separated by "\r\n", or null if no custom headers needed, e.g.: "MyHeaderName: value123\r\n", "MyHeaderName1: value123\r\nMyHeaderName2: value456\r\n".
* **userObject** - user object to be passed to delegate or `null` if not needed.
* **userDelegate** - user delegate to call once the HTTP response is obtained.
* **receiveTimeoutMs** - timeout on receive.

In case if Node method fails it will construct erroneous `Response` object with non-successful status code and error information in `Body`, and then call user delegate with that object.

If exception occurs within user delegate, its logged to Starcounter server log if `Node.ShouldLogErrors` is set to `True`, otherwise exception is simply re-thrown.

Another version of Node REST **synchronous** GET simply returns the HTTP response instead of calling user delegate:

```csharp
Response GET(String uri, String customHeaders, Int32 receiveTimeoutMs = 0)
```

Returned `Response` should never be null.

Other standard HTTP method calls \(PUT, POST, DELETE\) have an optional body parameter, for example:

```csharp
void PUT(String uri,
         String body,
         String customHeaders,
         Object userObject,
         Action<Response, Object> userDelegate,
         Int32 receiveTimeoutMs = 0)
```

which also has a corresponding version of function that returns HTTP response instead of delegate.

In order to user arbitrary user HTTP method you can call the following Node call:

```csharp
void CustomRESTRequest(String method,
       String uri,
       String body,
       String customHeaders,
       Object userObject,
       Action<Response, Object> userDelegate,
       Int32 receiveTimeoutMs = 0)
```

You can obtain some information about Node instance using the following methods:

* **UInt16 PortNumber** - port number for this Node instance.
* **String HostName** - host name for this Node instance.
* **Uri BaseAddress** - URI of this Node.

### More on User Delegate

User delegate described above accepts two parameters: received `Response` object and object supplied by user. When delegate exits `Node` checks if response should be returned to the original `Request`. User indicates that by setting the `Response` object property on original `Request` object, like in the following example:

```csharp
Handle.GET("/postponed", (Request req) =>
{
    Http.POST("http://www.mywebsite.com/echotest", "Here we go!", null, null, (Response resp, Object userObject) =>
    {
        // Modifying the response object by injecting some data.
        resp.Headers["MySuperHeader"] = "Here is my header value!";
        resp.Headers["Set-Cookie"] = resp.Headers["Set-Cookie"] + ";MySuperCookie=CookieValue";
        req.Response = resp;
    }); // "resp" object will be automatically sent when delegate exits.

    return HandlerStatus.Handled;
});
```

### Error Checking

Once the Node call is finished, user should check for status code of the HTTP response \(returned Response object should never be null\).

The code below demonstrates the redirection of root HTTP URI "/" to startup HTML page `http://www.mywebsite.com/index.html`:

```csharp
// Redirecting root to index.html.
GET("/", (Request req) =>
{
    // Doing another request to obtain static file response.
    Response resp = Http.GET("http://www.mywebsite.com/index.html");

    // Checking that response status is correct.
    if (!resp.IsSuccessStatusCode)
    {
        throw new Exception(@"REST call returned
            status code: " + resp.StatusCode);
    }
    // Returns this response to original request.
    return resp;
});
```

### How Node Works Internally

During the first request, Node instance creates a TCP connection with specified server. All subsequent synchronous calls on this Node instance will use this TCP connection. Asynchronous Node calls are using pool of connections. If the connection is dropped - its automatically recreated. If connection can not be re-established, special error Response object is created \(with `StatusCode` 503\). One connection\(socket\) is used for synchronous Node calls because data should arrive in order \(with multiple connections client data can arrive out-of-order\). The network connection resources are automatically cleaned up upon Node instance garbage collection.

## Setting Receive Timeout

User can specify receive timeout both in synchronous and asynchronous `Node` and `Http` modes. Timeout is specified in milliseconds as last parameter for each method \(GET, POST, PUT, etc\), for example:

```csharp
Response GET(String uri, Int32 receiveTimeoutMs = 0)
```

Receive timeout can also be specified on Node instance when constructing the Node \(or at any time using accessor `DefaultReceiveTimeoutMs`\), and its value will be used as default for all calls on this Node instance, unless receive timeout is overwritten for a particular call, as described above.

Default value 0 means infinite timeout.  
If timeout is reached and response is not received yet, the erroneous `Response` object is constructed as explained before.

