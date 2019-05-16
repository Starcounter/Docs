# UDP sockets

UDP is a connection-less protocol with unreliable delivery.

> Learn more about UDP on Wikipedia

Incoming UDP datagrams with maximum size of 30000 bytes are supported \(user code can send datagrams up to 65000 bytes\). In order to receive UDP datagrams, user needs to register a handler using `Handle.Udp`:

```
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

```
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

```
Handle.Udp(8787, (IPAddress clientIp, UInt16 clientPort, Byte[] datagram) =>
{    
    // One can update any resources associated with "clientIp" and "clientPort".
    UdpSocket.Send(clientIp, clientPort, 8787, datagram);
});
```

