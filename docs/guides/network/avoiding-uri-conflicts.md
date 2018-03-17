# Avoiding URI conflicts

## Introduction

There are some HTTP "namespacing" precautions that you need to take to make your app run safely alongside other apps.

## Namespacing dynamic HTTP handlers

Your app should only create HTTP handlers \(using `Handle.GET`, `Handle.POST`, etc\) that begin with the app name. For example:

```csharp
Handle.GET("/myapp", ()
{
  var page = new MyAppHomePage();
  return page;
});
```

By default it is not enforced. You might enforce it by enabling a database configuration flag: [`EnforceURINamespaces`](../database/database-configuration.md).

## Namespacing static HTTP resources

The above principle should also be used for the static resources that are exposed from the `wwwroot` subdirectory in your app.

All the files that are unique to your app \(HTML templates, CSS stylesheets, custom JavaScript\) should be placed under `wwwroot/<appname>`.

For example a file saved as `wwwroot/myapp/style.css` will be exposed by the static HTTP server as [http://localhost:8080/myapp/style.css](http://localhost:8080/myapp/style.css). You don't have to worry that some other app will overwrite `style.css`, because it will use it's own directory for the file.

In contrast to the above, all the files that are NOT unique to your app \(shared libraries, Custom Elements HTML Imports, Bower components\) should be placed under `wwwroot/sys`. This allows apps to be sufficient yet allow for resource sharing.

For example, there might be two applications that use `<link rel="import" href="/sys/paper-input/paper-input.html">`. Both of the apps host the static resource `http://localhost:8080/sys/paper-input/paper-input.html`. When requesting this URL, you are served by the last app that was started. Because the URLs are the same, there is no conflict of two Web Components loaded simultaneously. More about that in [Using Web Components](../web-apps/introduction-to-web-components.md#using-web-components).

The next pages explain what's the benefit of all that - data sharing and screen sharing.

