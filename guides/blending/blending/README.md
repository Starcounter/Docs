# Blending

## Introduction

Blending is used to visually combine partial responses from several apps. JSON/HTML responses are merged. The actual JSON merging is done during serialization process, HTML partials are requested by the browser through special HTML-merging URI. The client (e.g. browser) then displays combined partials from several apps. Blending is done outside apps and should not affect how apps are functioning (apps should neither depend on, nor expext blending). Only GET handlers that return JSON are blended, not any response handlers. Apps should not know/assume neither about blender presence, nor other apps presense.

Blending is based on token: either a string or a class. Handlers blended on the same token will be called on internal `Self.GET` calls or external URI that matches one of the handlers.

Blending is represented by a class `Blender` in Starcounter namespace. There is a blender API to do dynamic (during the lifetime of application) addition and removal of blended handlers. During chained calls of all blended handlers, same handler can only be called once.

Note that from Starcounter 2.3.1, the `Blender` API replaces the previous `OntologyMap` and `UriMapping` API's. 

## Blending token

Both a string and class (or several classes) can be used as a blending token. First parameter is always a mapped handler URI, then token, if its defined as a string.
```cs
Blender.MapUri("/Products/settings", "settings");
Blender.MapUri<Product>("/Products/partials/product/{?}");
Blender.MapUri("/Products/menu", "menu");
```

Arbitrary amount of classes are allowed as a blending token (up to 3 in template, more in array of class `Type`). Here are the `MapUri` signature examples (same exist for removing handler from blender): 
```cs
static void MapUri<T>(String uri)
static void MapUri<T1, T2, T3>(String uri)
static void MapUri(String uri, Type[] types)
```

## Dynamic addition and removal

It is possible to get a list of handlers and tokens that are currently in Blender and then remove some of them or add new ones.
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

In the example above, converter simply passes handler parameters to token parameters, as is. There are signatures of `Blender.MapUri` with no converters, which means that parameters are simply passed through, like above.

Often its needed to trigger blending on a specific URI only. To achieve this, first converter should return non-null string array only on certain parameters.

## Direction of calls when blending

Blender allows to specify the direction in which blended handlers would be called. This is needed to trigger blending only in certain direction: from handler or to handler. The direction is determined by the value that corresponding converter is returning: `null` converter or `null` string array returned in converter blocks the direction of the call. In case of zero parameters, there is a special `Blender.MapUri` override with corresponding boolean parameters to determine the allowed call directions.

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

Normally, you want to achieve a particular rendering order. You can use CSS for that. There is myriad ways: absolute positioning, flexbox and CSS Grid Layout. All of these solutions require applying arbitrary CSS on top of the running app, which is not very flexible.

To replace this tedious process, a Launcher might implement a layout editor. Our reference Launcher has a layout editor that can be invoked by pressing CTRL+E or the paint icon. It's a generator for CSS Grid Layout with a shim that uses HTML `<table>` and Shadow DOM.