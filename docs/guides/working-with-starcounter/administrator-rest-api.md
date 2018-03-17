# Administrator REST API

## Introduction

The Starcounter server has a number of REST endpoints registered on port 8181 by default. The server can be started with the command `staradmin start server`. 

## Redirection

To redirect your app's URL handler to another handler, call `GET /sc/redirect/{PortNumber}{FromUri};{ToUri}` on system port \(by default 8181\).

For example, to create redirection of `/` to `/myapp` on port 8080, the following handler should be called: `GET /sc/redirect/8080/;/myapp` on system port \(by default 8181\).

To create redirection on the startup of your application, you may call `Http.GET(8181, "/sc/redirect/8080/;/myapp");`. Keep in mind, that any other app can overwrite this redirect because there can only be one app handling the host root.

## URI aliasing

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

## 

