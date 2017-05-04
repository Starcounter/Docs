# More on partials

Most of Web apps will divide its code to partials. We strongly favor use of native HTML `<template>` Element , and [`<imported-template>`](http://github.com/Juicy/imported-template). However we recommend the use of our own Custom Element [`<starcounter-inlude>`](https://github.com/Polyjuice/launcher-include), as it incorporates all features needed, simplifies your code, integrates nicely with advanced Starcounter features, and moves the responsibility for proper inclusion from you to us.

## Static HTML

You can naturally use static HTML files with just

```html
<starcounter-include
    partial='{"Html": "/path/to/file.html"}'></starcounter-include>
```

or good old [`<imported-template>`](http://github.com/Juicy/imported-template).

```html
<template is="imported-template" content="/path/to/file.html">
```
It will stamp template from `GET text/html /path/to/file.html`, and attach data model that was attached to the element.

## Dynamic partials

If you are using Starcounter JSON binding features, you can make your partials more dynamic.

```csharp
Handle.GET("/your/partial/url", () =>
{
    var json = new Json()
    {
        Html = "/path/to/partial.html"
    };
    // ...
    return json;
});
```
request it in parent Handler:

```csharp
Handle.GET("/parent/path/{?}", (String objectId) =>
{
    var json = new ParentJson()
    {
        Html = "/parent.html"
    };
    json.PlaceInViewModel = (Json)Self.GET("/your/partial/url");
    // ...
    return json;
});
```
Then use `starcounter-include` (_HTML insertion point_), to include it into HTML:

<div class="code-name">parent.html</div>

{% raw %}
```html
<starcounter-include partial="{{PlaceInViewModel}}"></starcounter-include>
```
{% endraw %}

or with plain JavaScript, without (Polymer's) two-way data binding with mustaches:
> <i class="fa fa-exclamation-circle  button-icon-left"></i> Note, that this is just a HTML Element, so it will work as well with your Angular, jQuery, or React magic.

```html
<starcounter-include id="partialInsertionPoint"></starcounter-include>
```

```javascript
var insertionPoint = document.querySelector("#partialInsertionPoint");
insertionPoint.partial = model.PlaceInViewModel;
```

It will stamp template from `GET text/html` for `Json.Html`, and attach `GET application/json` for `Json` node - `Json` data model.


> In fact all that data is usually already there, so there is no need to actual request, this just just conceptual description.

## Dynamic partials with Mixed Apps

If you run your [dynamic partials](#dynamic-partials) with [mixed apps](/guides/blending/) environment, Starcounter may attach other apps' responses that matches the concept from your partial.

Fortunately, you do not have to bother about this much, the C# code remains untouched, and Starcounter will handle it by itself. You only need to be aware that few more elements may get stamped from your `starcounter-include` (`imported-template`).

From technical point of view, Starcounter changes the Page node that is returned by `Self.GET`, so now it represents your page with other apps' pages joined to it. Therefore the `text/html` response would contain concatenated outputs from all apps, and the `application/json` view model will contain all view-models wraped within namespacing object. No worries, appropriate model scope would be attached to your HTML template, by `starcounter-include` (in fact by `import-template` inside).

### .. styled with Launcher or layout app

If your app is run in Starcounter environment together with a Launcher or a layout app, you can get even more. To solve the problem of too much output floating around on screen, Launcher provides a feature to arrange elements in `starcounter-include` nicely.

Each Launcher may decorate included template the way it wants, but it should behave like [Juicy/imported-template](https://github.com/Juicy/imported-template#imported-template)

You can read more on how does Launcher/Layout app may hijack and extend `<starcounter-include>` behavior at [include template](https://github.com/Polyjuice/Launcher/wiki/imported-template-in-Polyjuice).

## Partials without layout

If you want to forcefully disable styling, layout features but still include a dynamic partial, you can use pure [`<imported-template>`](http://github.com/Juicy/imported-template) Element.

 1. Create a partial as in [Dynamic partials](#dynamic-partials)
 2. Include it, using `imported-template` insertion point:

{% raw %}
 ```html
 <div>
     <template is="imported-template" model="{{PlaceInViewModel}}"
         content$="{{PlaceInViewModel.Html}}"></template>
 </div>
 ```
{% endraw %}

## Partials without any Starcounter UI mixing (mapping and merging)

If you are sure, yo do not want Starcounter mixing of other apps, you can simply return your partial without `Self.GET`:

```csharp
var partialJson = new Json()
{
    Html = "/path/to/partial.html"
};
// ...
json.PlaceInViewModel = partialJson;
```

and insert it into HTML with `<starcounter-include>` or `imported-template` (whichever suits you better)

{% raw %}
```html
<starcounter-include
    partial="{{PlaceInViewModel}}"></starcounter-include>
or
<div>
    <template is="include-template" model="{{PlaceInViewModel}}"
        content$="{{PlaceInViewModel.Html}}"></template>
</div>
```
{% endraw %}

## Fetching partials in my very own way

If you really know what you are doing, and from whatever reason adding `<imported-template>` to your DOM does not fit your requirements, you can achieve it by your very own JS code.
Just sent XHR for `GET text/html` for `PlaceInViewModel.Html`, and fetch `<template>` elements from there and attach `application/json` data from `PlaceInViewModel`.


## Styled partials in standalone app

If you want to use elements composition from a Launcher (or layout app), you can simply import it's `starcounter-include` implementation instead of default one.

For example, install [Polyjuice/launcher-include](https://github.com/Polyjuice/launcher-include)
```bash
$ bower install Polyjuice/launcher-include --save
```
then
```html
<link rel="import" href="/sys/starcounter-include/starcounter-include.html">
```
will point to your version of `starcounter-include` instead of version from Starcounter installation


> <i class="fa fa-rocket button-icon-left"></i> In future we plan to provide Bower-on-fly so you will be able to choose versions in runtime using Starcounter administrator.


-------

# For Launcher developers

You can change/extend behavior of default partials handling - `starcounter-include`.
Default Custom Element, that is provided with Starcounter installation is defined at [Polyjuice/starcounter-include](https://github.com/Polyjuice/starcounter-include), and served from `/sys/starcounter-include/starcounter-include.html`.
To change it you need to put you own version in the same location.

Feel free to overwrite it, but keep in mind that App developers expects [`imported-template`](https://github.com/Juicy/imported-template#imported-template) behavior.


For example, the [default Launcher's `starcounter-include`](https://github.com/Polyjuice/launcher-include) wraps [`<juicy-tile-grid>`](https://github.com/Juicy/juicy-tile-grid) around [Juicy/imported-template](https://github.com/Juicy/imported-template#imported-template).
