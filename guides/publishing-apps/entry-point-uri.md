# Entry point URI

Every blendable web app should have the entry point URI that follows the pattern `/<appname>`, e.g. `/kitchensink` \(lowercase\).

When you access this URI, the app main screen should be presented.

If there is no main screen, show a splash screen saying that the app is running.

Sample `MainHandlers.cs` \([source](https://github.com/StarcounterApps/People/blob/94341b2dc62ad6637808313c367f986a417d349b/src/People/Api/MainHandlers.cs#L32-L35)\):

```csharp
Handle.GET("/people", () =>
{
    return Self.GET("/people/organizations");
});
```

