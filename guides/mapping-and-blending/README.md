# Mapping and Blending

With Starcounter, it is super easy to build a single app. But the true value of your app is unleashed when it runs simultaneously with other apps. Multiple simple apps can be used to build complex solutions fast.

## What is an app?

A Starcounter app adheres the following principles:

1. Does one thing and does it well.

2. Can run simultaneously with other apps.

3. Can share data with other apps.

4. Can share screen with other apps.

On the following pages we will examine how to put these principles to practice.

## Running multiple apps

As you start the apps, they are weaved to the database. At this point:

- the apps’ data is stored in the same database,
- the apps’ dynamic HTTP handlers are exposed at the same port,
- the apps’ static HTTP resources are exposed at the same port

## Mapping

Mapping is how you make the apps share the data on a shared screen.

Starcounter implements 2 distinct mapping APIs:

- [UriMapping.Map](/guides/mapping-and-blending/uri-mapping). Allows a single request (`Self.GET`) to trigger responses from multiple apps. The common key for the requests is an arbitrary URI (string), used as a key for all of the mapped URIs. This is used to build UI regions like menu, user sign in, launchpad icons.

- [UriMapping.OntologyMap](/guides/mapping-and-blending/ontology-mapping). Allows a single request (`Self.GET`) to trigger responses from multiple apps. The common key for requests is the fully qualified name of a database table (string). This is used to compose a view for a specific type/object (like person form, map for address, etc.).

## Blending

[Blending](/guides/mapping-and-blending/blending) is a feature of a Launcher app that allows to rearrange the rendering of the HTML response. It is the client-side feature that makes the result of `UriMapping.Map` and `UriMapping.OntologyMap` appear like a single app, even though it is composed from separate micro apps.

{% import "../../macros.html" as macros %}

{{ macros.tocGenerator(page.title, summary.parts[0].articles[3].articles[7].articles) }}
