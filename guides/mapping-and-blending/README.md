# Mapping and Blending

With Starcounter, it is super easy to build a single app. But the true value of your app is unleashed when it runs simultaneously with other apps. Multiple simple apps can be used to build complex solutions fast.

## Running multiple apps

As you start the apps, they are weaved to the database. At this point:

- the apps’ data is stored in the same database,
- the apps’ dynamic HTTP handlers are exposed at the same port,
- the apps’ static HTTP resources are exposed at the same port

## Blending

Blending is the way how you make the apps share the data on a shared screen. There is a server and client side blending.

On the server there is a [Blender.MapUri](/blending/README.md) which allows a single request (`Self.GET`) to trigger responses from multiple apps. The common key for the requests is an arbitrary string token, used as a key for all of the mapped URIs. This is used to build UI regions like menu, user sign in, launchpad icons.

On the client side there is a feature of a Launcher app that allows to rearrange the rendering of the HTML response. It makes the result of `Blender.MapUri` appear like a single app, even though it is composed from separate micro apps.

{% import "../../macros.html" as macros %}

{{ macros.tocGenerator(page.title, summary.parts[0].articles[3].articles[6].articles) }}
