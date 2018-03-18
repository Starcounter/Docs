# TCP and UDP sockets

## Introduction

Starcounter supports both TCP and UDP sockets.

## TCP sockets

TCP sockets in user code are represented by objects of the class `TcpSocket`. User code can define a handler for retrieving data and detecting disconnects on a socket by using `Handle.Tcp` method:

```csharp
static void Handle.Tcp(UInt16 port, Action<TcpSocket tcpSocket, Byte[] data> handler);
```

* `port`: local port on which incoming TCP connections should be accepted.
* `tcpSocket`: active socket object. This object can be used to send data, disconnect socket, etc. When incoming `data` is `null` it means that socket was disconnected.
* `data`: incoming data on TCP socket.

`TcpSocket` class has the following notable methods:

* `UInt64 ToUInt64()`: identifier that represents the TCP socket, normally used for storage in user code.
* `void Disconnect()`: disconnects an active TCP socket.
* `void Send(Byte[] data)`: sends given data on active TCP socket.
* `Boolean IsDead()`: checks if TCP socket object is still active.
* `static TcpSocket Current`: currently active TCP socket for this scheduler.

NOTE: When doing operations on the same TcpSocket but from different Starcounter schedulers - the order in which operations \(like `Send`\) will actually be performed is not guaranteed to be the same as the order in which they were initiated.

Once the TCP socket object is returned, user can fetch the ID representing this socket \(ToUInt64\(\)\), and of course, perform data sends and disconnect.

In the following example we process TCP sockects on port 8585 and send echo data back. The example shows how socket ID can be used to associate socket with resources, for example objects in database:

```csharp
Handle.Tcp(8585, (TcpSocket tcpSocket, Byte[] incomingData) =>
{
    UInt64 socketId = tcpSocket.ToUInt64();
    // Checking if we have socket disconnect here.
    if (null == incomingData)
    {
        // One can use "socketId" to dispose resources associated with this socket.
        return;
    }

    // One can check if there are any resources associated with "socketId" and otherwise create them.
    // Db.SQL("...");

    // Sending the echo back.
    tcpSocket.Send(incomingData);

  // Or even more data, if its needed: tcpSocket.Send(someMoreData);
});
```

## UDP sockets

Incoming UDP datagrams with maximum size of 30000 bytes are supported \(user code can send datagrams up to 65000 bytes\). In order to receive UDP datagrams, user needs to register a handler using `Handle.Udp`:

```csharp
void Udp(UInt16 port,
         Action<IPAddress clientAddr,
         UInt16 clientPort,
         Byte[] datagram> handler)
```

* `port`: local port on which to listen for datagrams.
* `clientAddr`: represents client's IP address.
* `clientPort`: holds client port number.
* `datagram`: incoming UDP datagram.

In order to send a UDP datagram from user code, static method `UdpSocket.Send` is used:

```csharp
static void Send(IPAddress ipTo,
                 UInt16 portTo,
                 UInt16 portFrom,
                 Byte[] datagram)
```

* `ipTo`: destination IP address.
* `portTo`: destination port.
* `portFrom`: local port from which UDP datagram should be sent \(usually same port on which `Handle.Udp` was called\).
* `datagram`: the actual UDP datagram that should be sent.

In the following example we receive \(on port 8787\) and send the same echo UDP datagram back to the client:

```csharp
Handle.Udp(8787, (IPAddress clientIp, UInt16 clientPort, Byte[] datagram) =>
{    
    // One can update any resources associated with "clientIp" and "clientPort".
    UdpSocket.Send(clientIp, clientPort, 8787, datagram);
});
```

