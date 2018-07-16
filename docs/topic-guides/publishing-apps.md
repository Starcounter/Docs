# Publishing apps

## Introduction

If your app is blendable, it might be used in complex UI systems consisting of multiple apps coming from different vendors.

To make sure that apps play well together, we recommend that you follow certain conventions.

These conventions exist for two reasons:

1. to prevent conflicts between apps in the same database
2. to provide a common ground, expectable user experience

## Mapping file

Each app that have blendable views should have a `<AppName>.map.md` file which contains the list of them.

The file should be provided at the root of the App Warehouse package. If the app is open source, the file should be provided in the code repository, preferably at the root.

Every view must be explained in the file and preferably illustrated with a screenshot.

In the future, the information in this file is used for automated blending as well as for [testing purposes](https://github.com/Starcounter/Guidelines/issues/26).

Sample `Images.map.md` \([source](https://github.com/Starcounter/Images/blob/develop/Images.map.md)\):

```text
# Blendable views

## /images/partials/contents/{[Simplified.Ring1.Content](https://github.com/Starcounter/Simplified/blob/master/Ring1/Content.cs)}

Shows a simple page for `Content` preview, image or video. In case of unexisting
content, shows empty file preview image.

![screenshot](docs/screenshot-content.png)
```

## Entry point URI

Every blendable web app should have the entry point URI that follows the pattern `/<appname>`, e.g. `/uniformdocs` \(lowercase\).

When you access this URI, the app main screen should be presented.

If there is no main screen, show a splash screen saying that the app is running.

Sample `MainHandlers.cs` \([source](https://github.com/Starcounter/People/blob/94341b2dc62ad6637808313c367f986a417d349b/src/People/Api/MainHandlers.cs#L32-L35)\):

```csharp
Handle.GET("/people", () =>
{
    return Self.GET("/people/organizations");
});
```

## UI metadata

By convention, a blendable web app must provide a metadata JSON that responds to the `app-name` blending token.

System apps and navigation apps like [Launchpad](https://github.com/Starcounter/Launchpad) will display a link to the entry point URI using the app name from this metadata. They will also use the icon, if provided.

The response JSON can consist of the following properties:

| Property | Explanation |
| --- | --- |
| `name` | **Required.** Human readable app name. |
| `description` | Optional. Short \(single sentence\) description of the app. |
| `html` | Optional. URI to a view that contains the app icon. |

Sample `MainHandlers.cs` \([source](https://github.com/Starcounter/CssVariablesManager/blob/develop/src/CssVariablesManager/Api/MainHandlers.cs#L13)\):

```csharp
Handle.GET("/cssvariablesmanager/app-name", () => new AppName());
```

Sample `BlendingHooks.cs` \([source](https://github.com/Starcounter/CssVariablesManager/blob/4aad5186522a6297bc4ca7f713d91424220d6552/src/CssVariablesManager/Api/BlendingHooks.cs#L9)\):

```csharp
Blender.MapUri("/cssvariablesmanager/app-name", "app-name");
```

Sample `AppName.json` \([source](https://github.com/Starcounter/CssVariablesManager/blob/4aad5186522a6297bc4ca7f713d91424220d6552/src/CssVariablesManager/ViewModels/AppName.json)\):

```javascript
{
  "name": "CssVariablesManager",
  "description": "CssVariablesManager",
  "html": "/cssvariablesmanager/AppIcon.html"
}
```

### App icon

The icon is an image \(preferably inline SVG\) in the HTML file which path is provided as the `html` property in the `app-name` metadata.

Check out the [Icons](../how-to-guides/how-to-use-blendable-icons.md) cookbook to learn more about icons.

Sample `AppIcon.html` \([source](https://github.com/Starcounter/CssVariablesManager/blob/4aad5186522a6297bc4ca7f713d91424220d6552/src/CssVariablesManager/wwwroot/CssVariablesManager/AppIcon.html)\):

```markup
<template>
    <svg slot="cssvariablesmanager/app-icon" viewBox="0 0 185 185" version="1.1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink">
        <use xlink:href="/cssvariablesmanager/images/cssvariablesmanager.svg#css-file" />
    </svg>
    <template is="declarative-shadow-dom">
        <slot name="cssvariablesmanager/app-icon"></slot>
    </template>
</template>
```

## Required and sample data

The app should not create required nor sample data without the user's consent.

### Required data

If the app requires any data to operate \(for example a list of the world's countries in the `Country` table\), it should present a nagging information with a button that creates the data:

* on the top of the main page
* possibly in every view that needs the required data

### Sample data

If the app offers some nice-to-have sample data, it should offer a button that creates this data:

* on the main page, in a welcome message that is dismissible once \(not per user\)
* on the [Settings page](publishing-apps.md#settings-page)

## Settings page

If your app is configurable, it's good for to provide a user interface for changing the app settings.

By having a settings page mapped to the common token, your configuration UI can appear along settings pages from other apps. This is good because it gives the end user a single go-to place to configure all apps.

The common pattern is to have the settings page addressable by `/<appname>/settings`, blended to the token `settings`.

The settings page might contain a button to do a "Factory Reset" of the app \(restores required data\) or a "Sample data" button that populates the database with sample data.

Sample and prefab apps that have a settings page:

* [https://github.com/Starcounter/Products](https://github.com/Starcounter/Products)
* [https://github.com/Starcounter/SignIn](https://github.com/Starcounter/SignIn)

## Submitting to the Warehouse

To make your app discoverable by others, you're invited to publish it in the Warehouse.

Submitting apps to the Warehouse requires an invitation.

[Contact Starcounter](https://starcounter.com/about/) for more information.

