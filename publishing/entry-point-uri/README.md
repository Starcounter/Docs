# Entry point URI

Every blendable web app should have the entry point URI that follows the pattern `/<appname>`, e.g. `/kitchensink` (lowercase).

This URI is automatically linked in Starcounter Administrator.

When you access this URI, the app main screen should be presented.

If there is no main screen, show a splash screen saying that the app is running.

Sample `MainHandlers.cs` ([source](https://github.com/StarcounterApps/CssVariablesManager/blob/4aad5186522a6297bc4ca7f713d91424220d6552/src/CssVariablesManager/Api/MainHandlers.cs#L15-L19)):

```cs
Handle.GET("/CssVariablesManager", () => {
    MasterPage master = this.GetMasterPageFromSession();
    master.CurrentPage = Self.GET("/CssVariablesManager/partials/collections");
    return master;
});
```
