# Creating a browser page

Most web app tutorials would teach you here how to build a full blown HTML template, starting from:

```html
<!doctype html>
<html>
  <head></head>
  <body></body>
</html>
```

and everything in between. But not with Puppet web apps! A view-model can be represented with a simple HTML template that looks like:

{% raw %}
```html
<template>
  <template is="dom-bind">
    First name: <input type="text" value="{{model.FirstName$::input}}"/>
    Last name: <input type="text" value="{{model.LastName$::input}}"/>
    Description: <input type="text" value="{{item.Description$::input}}"/>
  </template>
</template>
```
{% endraw %}

Where's the doctype and the `<head>`, you'll ask? It needs to be created by some app, for sure, but there is a class of apps that creates it for you - it's called "launchers". A sample Launcher is available in Starcounter App Store.
