# Blending

<section class="hero">From Starcounter 2.3.1/2.4, the Blender API described here replaces the previous OntologyMap and UriMapping API's. To view the old API's, go to an earlier version of the documentation with the drop down on the left side of the screen</section>

## Introduction

Blending visually combines partial responses from different apps. It merges JSON/HTML responses. The JSON merging happens during the serialization process. The browser requests HTML partials through a special HTML-merging URI, it then displays combined partials from the different apps. Blending happens outside of apps and should not affect how they are functioning, essentially, apps should neither depend on, nor expect, blending. Blending applies to the JSON that GET handlers return. Apps should not know/assume neither about blender presence, nor other apps presense.

Blending uses tokens. These tokens are either strings or classes. Handlers blended on the same token are called on internal `Self.GET` calls or external URI that matches one of the handlers.

The class `Blender` in the Starcounter namespace does the blending. The blender API does dynamic addition and removal of blended handlers during the lifetime of the app. Handlers are called once, even during chained calls of all the blended handlers.

## Blending token

The first parameter is always a mapped handler URI. If the token is a string, it's defined as the second parameter. If it's one or more classes, they are defined in the template or as a `Type` array parameter.

```cs
Blender.MapUri("/Products/settings", "settings");
Blender.MapUri<Product>("/Products/partials/product/{?}");
Blender.MapUri("/Products/menu", "menu");
```

An arbitrary number of classes are allowed as a blending token (up to 3 in template, more in array of class `Type`). Here are the `MapUri` signature examples (same exist for removing handler from blender): 

```cs
static void MapUri<T>(String uri);
static void MapUri<T1, T2, T3>(String uri);
static void MapUri(String uri, Type[] types);
```

## Dynamic addition and removal

It is possible to get a list of handlers and tokens that are in Blender and then remove some of them or add new ones.
```cs
Blender.UnmapUri("/app4/{?}", token2);
...
Blender.UnmapUri("/twoparams3/{?}/{?}", token);
...
Blender.UnmapUri<MyClass>("/app1/{?}");
```

To check if certain handler is in blender:
```cs
isInBlender = Blender.IsMapped("/noparam1", token3);
isInBlender = Blender.IsMapped<MyClass>("/noparam1");
```

To get a list of all blended handlers, tokens and all:
```cs
static Dictionary<String, Boolean> ListUris();
static Dictionary<String, Boolean> ListTokens();
static Dictionary<String, String[]> ListAll();
```

## Parameters in handlers for Blending

Blending handlers are allowed to have arbitrary amount of parameters. When there is at least one parameter, the blender convertion functions are used. First converter translates handler arguments to token arguments, while the second converter does the opposite. Both converters are taking and returning an array of strings:
```cs
Blender.MapUri("/twoparams1/{?}/{?}", token,
(String[] from) => {
   return from;
}, (String[] to) => {
   return to;
});
```

In the example above, converter passes handler parameters to token parameters, as is. There are signatures of `Blender.MapUri` with no converters, which means that parameters are simply passed through, like above.

Often it's needed to trigger blending on a specific URI. To achieve this, first converter should return non-null string array on certain parameters.

## Direction of calls when blending

Blender allows to specify the direction in which blended handlers would be called. This is needed to trigger blending in a certain direction: from handler or to handler. The direction is determined by the value that corresponding converter is returning: `null` converter or `null` string array returned in converter blocks the direction of the call. In case of zero parameters, there is a special `Blender.MapUri` override with corresponding boolean parameters to determine the allowed call directions.

```cs
Blender.MapUri("/twoparams1/{?}/{?}", myToken, null, (String[] to) => {
    return to;
});
Blender.MapUri("/noparam1", token3, true, false);
```
In the first example above, handler "/twoparams1/{?}/{?}" should not trigger blending chain of calls on token `myToken`, but it can be called when blending is triggered by other handlers on the same token.
In the second example above, handler "/noparam1" can trigger other handlers on the same token, but can't be triggered by them.

## Same handler - different tokens

It is allowed to add blending for the same handler but on different tokens.
```cs
Blender.MapUri("/app1/{?}", token1);
Blender.MapUri("/app2/{?}", token1);
Blender.MapUri("/app3/{?}", token1);
 
Blender.MapUri("/app1/{?}", token2);
Blender.MapUri("/app4/{?}", token2);
```

Here handler "/app1/{?}" participates in blending with `token1` and `token2` handlers.


## Client side blending

A launcher can implement *blending*. It is a feature of rearranging the rendering of the HTML response.

By default, the merged HTML response from multiple applications comes with the application starting order. Where the output of a first application finishes, the output of a second application begins. This is far from the desired situation.

Normally, you want to achieve a particular rendering order. You can use CSS for that. There are a myriad of ways: absolute positioning, flexbox and CSS Grid Layout. These solutions require applying arbitrary CSS on top of the running app, which is not flexible.

To replace this tedious process, a Launcher might implement a layout editor. Our reference Launcher has a layout editor that can be invoked by pressing CTRL+E or the paint icon. It's a generator for CSS Grid Layout with a shim that uses HTML `<table>` and Shadow DOM.