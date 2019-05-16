# Network gateway

## Introduction

The network gateway is a key network communication component of Starcounter. The `scnetworkgateway.exe` process, which is separated from the database, represents the network gateway. It handles all external communications with the code-host, as well as communication between different code-hosts on one machine.

The gateway process talks to the codehost process \(sccode\) through shared memory. The same way codehost process is also separated from database process \(scdata\) and talks to it through shared memory. There is no direct connection between gateway and scdata, only through sccode. Network gateway parses network traffic, and prepares "messages" that are delivered to sccode through shared memory and executed there. The responses from sccode go back through the same shared memory to gateway, which sends them outside.

Detailed network gateway configuration and statistics can be retrieved using `GET /gw/stats` on system port \(by default 8181\).

## Configuration

Network gateway is configured in `scnetworkgateway.xml`, which is located in the server repository \(e.g User/Documents/Starcounter/Personal\).

To apply changes in this file, you must either:

* restart Starcounter, or:
* call `GET /gw/updateconf` on system port \(by default 8181\). If errors occur while applying the new changes, the previous configuration is kept.

Here are some notable configuration options:

### WorkersNumber

Is the number of gateway worker threads. Normally this value should be 1-2. On high loads, this value can be increased.

### MaxConnectionsPerWorker

Is the maximum number of connection per each gateway worker. Connections are equally distributed between workers.

### MaximumReceiveContentLength

Is the maximum size of incoming HTTP body/content, in bytes. Requests with bigger bodies are rejected with `413 Request Entity Too Large` and closure of TCP connection.

### InactiveConnectionTimeout

Is the inactive HTTP connections life time in seconds. Inactive connections are those on which send/receive are not performed.

## Big size uploads

Since the network gateway supports limited size uploads, the user has to write a custom big-data uploader for both HTTP and WebSocket protocols.

## Reverse proxy functionality

Starcounter gateway provides basic reverse proxy functionality to other port \(either on codehost or in gateway\) based on HTTP Host header in requests. Reverse proxies are defined in `ReverseProxies` section of gateway configuration. Here is an example of reverse proxy that redirects all incoming HTTP requests on port 80, with Host header equals "www.example1.sc", to the port 8080:

```markup
<ReverseProxy>
  <DestinationPort>8080</DestinationPort>
  <StarcounterProxyPort>80</StarcounterProxyPort>
  <MatchingHost>www.example1.sc</MatchingHost>
</ReverseProxy>
```

* **DestinationPort**: destination port.
* **StarcounterProxyPort**: Starcounter gateway port on which requests that should be proxied are received.
* **MatchingHost**: HTTP Host header value to filter out requests to be processed by this proxy.

In the example above when HTTP request with `Host` header `www.example1.sc` comes on port 80 its automatically transferred to handlers on port 8080.

## URI aliasing

The classical example is to alias root URI to some other URI in your application. URI aliases are configured in `scnetworkgateway.xml` in your server directory.  
The following section is an excerpt from gateway configuration:

```markup
<UriAliases>
    <UriAlias>
        <HttpMethod>GET</HttpMethod>
        <FromUri>/</FromUri>
        <ToUri>/index.html</ToUri>
        <Port>8181</Port>
    </UriAlias>

    <UriAlias>
        <HttpMethod>GET</HttpMethod>
        <FromUri>/</FromUri>
        <ToUri>/launcher</ToUri>
        <Port>8080</Port>
    </UriAlias>
</UriAliases>
```

* **HttpMethod**: for which HTTP method the alias is created.
* **FromUri**: URI that should be aliased.
* **ToUri**: URI to which the aliased URI should be changed.
* **Port**: port on which this rule should apply.

In the example above we have declared two URI aliases:  
1. On port 8181 from URI "/" to "/index.html". This is often used to display application initial "index" page when user just specifies the DNS name of the Web-server in the browser.  
2. On port 8080 from URI "/" to "/launcher". Used to display a dynamic launcher page for the same purposes.

