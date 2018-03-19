# URL Aliases and Redirects

## Introduction

With HTTP handlers, it is possible to express URL transformations such as aliases and redirects.

## Alias

An alias is a `GET` handle that returns the result of an `Self.GET` call.

For web apps, the URL displayed in the browser address bar does not get changed to the destination value.

Example:

```csharp
Handle.GET("/pmail", () =>
{
  return Self.GET("/pmail/inbox");
});
```

Starcounter gateway has support for aliases which is described in Network Gateway section.

## Redirect

An HTTP redirect is a server response that guides the client to access the resource using a different URL. It is achieved by an appropriate HTTP status code and value of the `Location` header.

For web apps, the URL displayed in the browser address bar gets changed to the destination value.

Example:

```csharp
Handle.GET("/villains/anakin-skywalker", () =>
{
  var resp = new Response()
  {
    StatusCode = 302,
    StatusDescription = "Moved Permanently"
  };
  resp.Headers["Location"] = "/villains/darth-vader";
  return resp;
});
```

### API for Redirection

To redirect your app's URL handler to another handler, call `GET /sc/redirect/{PortNumber}{FromUri};{ToUri}` on system port \(by default 8181\).

For example, to create redirection of `/` to `/myapp` on port 8080, the following handler should be called: `GET /sc/redirect/8080/;/myapp` on system port \(by default 8181\).

To create redirection on the startup of your application, you may call `Http.GET(8181, "/sc/redirect/8080/;/myapp");`. Keep in mind, that any other app can overwrite this redirect because there can only be one app handling the host root.

## Uri Aliasing

### URI Aliasing in Network Gateway

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

### API for URI Aliasing

Starcounter allows adding, modifying, listing, and deleting URI aliases using the following REST API \(should be called on system port \(by default 8181\)\):

* Adding and modifying a specific URI alias is done using `PUT /sc/alias`. The URI alias info should be in HTTP body with the following format:

```javascript
{
  "HttpMethod": "",
  "FromUri": "",
  "ToUri": "",
  "Port": 0
}
```

for example, `{"HttpMethod":"GET","FromUri":"/","ToUri":"/launcher","Port":8080}`

* Listing all URI aliases is done using `GET /sc/alias`:

```javascript
{  
  "Items":[  
    {  
      "HttpMethod": "GET",
      "FromUri": "/",
      "ToUri": "/launcher",
      "Port": 8080
    },
    {  
      "HttpMethod": "GET",
      "FromUri": "/myapp",
      "ToUri": "/myappalias",
      "Port": 8080
    }
  ]
}
```

Retrieving info about specific URI alias is done using `GET /sc/alias/{?}/{?}/{?}` where parameters are `string httpMethod, long dbport, string fromUri`.  
For example, `http://127.0.0.1:8181/sc/alias/GET/8080//SomeFromUri`

Deleting specific URI alias is done using `DELETE /sc/alias/{?}/{?}/{?}` where parameters are `string httpMethod, long dbport, string fromUri`.  
For example, `http://127.0.0.1:8181/sc/alias/GET/8080//SomeFromUri`

{% hint style="info" %}
In comparison with redirects, URI aliasing API adds entries to network gateway configuration and preserved upon gateway/code-host restart.
{% endhint %}

