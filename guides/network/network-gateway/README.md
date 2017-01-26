# Network Gateway

Network gateway is a key network communication component of Starcounter. It is represented by `scnetworkgateway.exe` process (separated from the database). All external communications with codehost (as well as communication between different codehosts on one machine) are handled by network gateway.

The gateway process talks to the codehost process (sccode) through shared memory. The same way codehost process is also separated from database process (scdata) and talks to it through shared memory. There is no direct connection between gateway and scdata, only through sccode. Network gateway parses network traffic, and prepares "messages" that are delivered to sccode through shared memory and executed there. The responses from sccode go back through the same shared memory to gateway, which sends them outside.

Detailed network gateway configuration and statistics can be retrieved using `GET /gw/stats` on system port (by default 8181).

## Configuration

Network gateway is configured in `scnetworkgateway.xml`, which is located in your server installation directory. Here are some notable configuration options:

* **WorkersNumber**: number of gateway worker threads. Normally this value should be 1-2. On very high load this value can be increased, if needed.

* **MaxConnectionsPerWorker**: maximum number of connection per each gateway worker. Connections are distributed equally between workers.

* **MaximumReceiveContentLength**: maximum size of incoming HTTP body/content, in bytes. Requests with bigger bodies are rejected with `413 Request Entity Too Large` and closure of TCP connection.

* **InactiveConnectionTimeout**: inactive HTTP connections life time in seconds. Inactive connections are those on which send/receive are not performed.

## Big size uploads

Size the network gateway supports only limited size uploads, user has to write a custom big-data uploader for both HTTP and WebSocket protocols.

## Reverse proxy functionality

Starcounter gateway provides basic reverse proxy functionality based on HTTP Host header in requests. Reverse proxies are defined in `ReverseProxies` section of gateway configuration. Here is an example of reverse proxy that redirects all incoming HTTP requests on port 80, with Host header equals "www.example1.sc", to service on localhost and port 8080:
```
<ReverseProxy>
  <DestinationIP>127.0.0.1</DestinationIP>
  <DestinationPort>8080</DestinationPort>
  <StarcounterProxyPort>80</StarcounterProxyPort>
  <MatchingHost>www.example1.sc</MatchingHost>
</ReverseProxy>

```

* **DestinationIP**: IP address of destination server, to which proxied requests are redirected.
or
* **DestinationDNS**: DNS name of destination server, to which proxied requests are redirected.
* **DestinationPort**: port address of destination service.
* **StarcounterProxyPort**: Starcounter gateway port on which requests that should be proxied are received.
* **MatchingHost**: HTTP Host header value to filter out requests to be processed by this proxy.

In the example above when HTTP request with `Host` header `www.example1.sc` comes on port 80 its automatically transferred to port 8080 on localhost.

You can see current configuration for reverse proxies by calling `GET /gw/stats` on system port.

To apply current gateway configuration (URI aliases and reverse proxy configuration) from `scnetworkgateway.xml` one should call `GET /gw/updateconf` on system port. If errors occur applying new changes - the previous configuration is kept.

