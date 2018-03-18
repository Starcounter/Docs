# Blendable web apps

## Introduction

Starcounter apps are small, individually functional web apps that can easily interoperate with other Starcounter apps. There are a couple of traits that belongs to these apps:

1. They [do one thing and do it well](https://en.wikipedia.org/wiki/Unix_philosophy#Do_One_Thing_and_Do_It_Well)
2. They can run simultaneously with other apps.
3. They can share data with other apps.
4. They can share screen with other apps.

## Blending

Blending is the concept of making several apps look as one without apps knowing about each other. This means that apps can be developed separately and still be used together without touching the source code. In practice, a developer can build an app that does one thing and does it well and then run that app with multiple other apps in different combinations and still make all the apps look as one. It allows reuse on a new level.

Blending is achieved with two different technologies: [Attaching](view-attaching.md) and [Composing](view-composing.md).

Attaching handles requests from the browser to allow multiple apps to respond to the same request without the apps being aware of it. This means that the browser will receive one response containing the combined responses of multiple apps.

When a response containing multiple responses is received by the browser, the results are stacked on top of each other - it still looks like they are from different apps, even if they come from the same response. Composition takes the response and composes the layout to fit to that particular set of apps. Thus, the layout can be adapted to look like there's only one app on the screen.

{% hint style="info" %}
The code examples in this section are built on modern web technologies that are supported in the current versions of Chrome, Firefox, Safari, Edge, and Opera. Other web browsers are currently not supported.
{% endhint %}

