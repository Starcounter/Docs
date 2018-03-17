# How to add HTML elements to the document head

## Goal

With [`PartialToStandaloneHtmlProvider`](../guides/network/middleware.md#partialtostandalonehtmlprovider), views are automatically wrapped with a `DOCTYPE`, `head` and `body`. Thus, it's not possible to add elements to the header the way you would with a normal HTML document. Instead, we'll have to add the elements to the `head` with JavaScript. In this guide, you'll learn how to add a `meta` tag to the head, but this method applies to any element.

## Steps

### 1. Locate the view that should have the element

For this guide, we'll use this view:

```markup
<link rel="import" href="/sys/polymer/polymer.html">

<template>
    <h1>Hello World</h1>
</template>
```

Whenever this view is viewed, there should be a `meta` element in the `head` with the author. To have an element in every view, perform the following step on the lowest-level page, often called the "master page". 

### 2. Add the code that appends the meta element

To add the element, we create a new element with JavaScript and add it to the `head`:

```markup
<link rel="import" href="/sys/polymer/polymer.html">

<template>
    <h1>Hello World</h1>
    <script>
        (function () {
            const meta = document.createElement("meta");
            meta.name = "author";
            meta.content = "John Doe";
            document.querySelector('head').appendChild(meta);
        })();
    </script>
</template>
```

The type of element can be changed by exchanging "meta" for anything else. The same applies to the name and content.

## Summary

With this, the `head` should have an element that looks like this:

```markup
<meta name="author" content="John Doe">
```

