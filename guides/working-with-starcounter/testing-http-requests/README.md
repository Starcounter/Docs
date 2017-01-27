# Testing HTTP requests

Some code examples in this guide describe handling of HTTP requests such as GET, POST, PUT, DELETE or PATCH. While it is trivial to make a GET request (you don't need anything more than a web browser), it may be troublesome to create a request with the other HTTP methods.

The following guide shows 3 tools that make it easier: cURL, Postman and JavaScript (XMLHttpRequest).

## cURL

[cURL](http://curl.haxx.se/) is a command line tool, that comes preinstalled in many Linux distributions and is available for all platforms (inluding Windows and Mac). There are many guides about using cURL so it is not neccessary to repeat it all here. Perhaps it is the best to show the cURL usage by example:

Send a `GET` request with a header `Content-Type: application/json` to `http://localhost:8080/invoicedemo`, show response body:  

```
$ curl -X GET -H "Content-Type: application/json" http://localhost:8080/invoicedemo

{"_ver#c$":0, "_ver#s":0, "Html":"/InvoiceDemo/InvoicePage.html", "InvoiceNo":0, "Name$":"", "Total":0.0, "Items":[{"Description$":"", "Quantity$":1, "Price$":0.0, "Total":0.0}], "AddRow$":0, "Save$":0, "Cancel$":0}
```


Send a `PUT` request with a header `Content-Type: application/json` and body `{"FirstName":"Olle","LastName":"Svensson", "Age":49}` to `http://localhost:8080/hello/Olle`, show response body:  

```
$ curl -X PUT -H "Content-Type: application/json"
-d '{"FirstName":"Olle","LastName":"Svensson", "Age":49}'
http://localhost:8080/hello/Olle

Welcome Olle you are 49 years old.
```

Adding a `-v` parameter results in more information being displayed, including the request and response HTTP headers:

```
$ curl -v -X PUT -H "Content-Type: application/json"
-d '{"FirstName":"Olle","LastName":"Svensson", "Age":49}'
http://127.0.0.1:8080/hello/Olle

* About to connect() to 127.0.0.1 port 8080 (#0)
*   Trying 127.0.0.1...
* Adding handle: conn: 0x7fa969803000
* Adding handle: send: 0
* Adding handle: recv: 0
* Curl_addHandleToPipeline: length: 1
* - Conn 0 (0x7fa969803000) send_pipe: 1, recv_pipe: 0
* Connected to 127.0.0.1 (127.0.0.1) port 8080 (#0)
> PUT /hello/Olle HTTP/1.1
> User-Agent: curl/7.30.0
> Host: 127.0.0.1:8080
> Accept: */*
> Content-Type: application/json
> Content-Length: 52
>
* upload completely sent off: 52 out of 52 bytes
< HTTP/1.1 200 OK
* Server SC is not blacklisted
< Server: SC
< Cache-Control: no-cache
< Content-Length: 34
<
* Connection #0 to host 127.0.0.1 left intact
Welcome Olle you are 49 years old.
```

## Postman

[Postman](https://chrome.google.com/webstore/detail/postman/fhbjgbiflinjbdggehcddcbncdddomop) is a graphical tool that comes as a Google Chrome Extension. It saves the previously sent requests which makes it convenient to reuse them.

The below screenshot shows the same request (& response) as above, sent in Postman:

![postman](/assets/postman.png)

## JavaScript (XMLHttpRequest)

[XMLHttpRequest](https://developer.mozilla.org/en-US/docs/Web/API/XMLHttpRequest) is a Web API for performing HTTP requests available in all modern web browsers.

The following JavaScript code makes the same request as above using XMLHttpRequest:

```js
var xhr = new XMLHttpRequest();
xhr.addEventListener('load', function (event) {
  console.log("Repsonse", event.target.responseText);
});
xhr.open("PUT", "http://localhost:8080/hello/Olle", true);
xhr.setRequestHeader('Content-Type', 'application/json');
xhr.send('{"FirstName":"Olle","LastName":"Svensson", "Age":49}');
```

Note that the web browser will execute the above request only if it is called from a document that was loaded from the same domain or if the server implements [CORS](http://enable-cors.org/).

The server side code that handles this request can be found here: [Accepting JSON in requests](/guides/restful-web-apps/accepting-json-in-requests)
