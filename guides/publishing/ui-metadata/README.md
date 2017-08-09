## UI metadata

By convention, a blendable web app must provide a metadata JSON that responds to the `app-name` blending token.

System apps and navigation apps like [Launchpad](https://github.com/StarcounterApps/Launchpad) will display a link to the entry point URI using the app name from this metadata. They will also use the icon, if provided.

The response JSON can consist of the following properties:

|Property|Explanation|
|---|---|
|`name`| **Required.** Human readable app name. |
|`description`| Optional. Short (single sentence) description of the app. |
|`html`| Optional. URI to a view that contains the app icon. |

Sample `MainHandlers.cs` ([source](https://github.com/StarcounterApps/CssVariablesManager/blob/develop/src/CssVariablesManager/Api/MainHandlers.cs#L13)):

```cs
Handle.GET("/cssvariablesmanager/app-name", () => new AppName());
```

Sample `BlendingHooks.cs` ([source](https://github.com/StarcounterApps/CssVariablesManager/blob/4aad5186522a6297bc4ca7f713d91424220d6552/src/CssVariablesManager/Api/BlendingHooks.cs#L9)):

```cs
Blender.MapUri("/cssvariablesmanager/app-name", "app-name");
```

Sample `AppName.json` ([source](https://github.com/StarcounterApps/CssVariablesManager/blob/4aad5186522a6297bc4ca7f713d91424220d6552/src/CssVariablesManager/ViewModels/AppName.json)):

```json
{
  "name": "CssVariablesManager",
  "description": "CssVariablesManager",
  "html": "/CssVariablesManager/AppIcon.html"
}
```

## App Icon

The icon is an image (preferably inline SVG) in the HTML file which path is provided as the `html` property in the `app-name` metadata.

Check out the [Icons](../../cookbook/icons/) cookbook to learn more about icons.

Sample `AppIcon.html` ([source](https://github.com/StarcounterApps/CssVariablesManager/blob/4aad5186522a6297bc4ca7f713d91424220d6552/src/CssVariablesManager/wwwroot/CssVariablesManager/AppIcon.html)):

```html
<template>
    <svg slot="CssVariablesManager/app-icon" viewBox="0 0 185 185" version="1.1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink">
        <use xlink:href="/CssVariablesManager/images/cssvariablesmanager.svg#css-file" />
    </svg>
</template>
```
