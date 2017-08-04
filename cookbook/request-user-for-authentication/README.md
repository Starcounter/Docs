# Requesting a user to authenticate

To separate the concerns, your app should not directly deal with user authentication. Rather than that, it should have a view that blends in an authentication view from another app that deals with that.

Below instruction shows how to provide an authentication notice that blends in the authentication form from the [SignIn](https://github.com/StarcounterApps/SignIn) app.

When your app decides that the view cannot be presented without authentication, it redirects to its own "Unauthenticated" view. 
For example: `/your-app/partial/unauthenticated?return_uri={?}`

<div class="code-name">MainHandlers.cs</div>

```cs
	if (!IsSignedIn()) {
		return Self.GET("/your-app/partial/Unauthenticated?return_uri=" + getURI);
	}
```


The `getURI` is the Uri requested in the original GET request.

Then define a new handler for the "Unauthenticated" partial.

<div class="code-name">PartialHandlers.cs</div>

```cs
	Handle.GET("/your-app/partial/Unauthenticated?return_uri={?}", (string returnUri) => 
	{
		return new UnauthenticatedPage();
	});
```

The "unauthenticated" view can be empty (no JSON properties). Its only purpose is to map it with a token to the authentication view coming from the [SignIn](https://github.com/StarcounterApps/SignIn) app.

<div class="code-name">Unauthenticated.json</div>

```json
{
}
```

```
<div class="code-name">Unauthenticated.json.cs</div>

```cs
using Starcounter;

namespace Your-App 
{
    partial class Unauthenticated : Json 
    {
    }
}
```

It should be mapped to a token `userform-return`, which is understood in other apps

<div class="code-name">Mapping.cs</div>

```cs
{
	Blender.MapUri("/your-app/partial/Unauthenticated?return_uri={?}", "userform-return");
}
```


If [SignIn](https://github.com/StarcounterApps/SignIn) app is not running, the user will only get a message about not being authenticated.

This message would come from the "Unauthenticated.Html".

![When signin app is not running](/assets/Authentication-nosignin.png)


But if [SignIn](https://github.com/StarcounterApps/SignIn) app is running, it shows its own view. 
![When signin app is running](/assets/signin-authentication.png)

That view is clever! Depending on `SignInFormAsFullPage` setting it will decide whether to display the login form right in this view, or redirect to a standalone page that only has the login form.

You can have other apps than [SignIn](https://github.com/StarcounterApps/SignIn) that do something in this view, but they will be competing for attention.

After successful log in, [SignIn](https://github.com/StarcounterApps/SignIn) app redirects back to the `return_uri` in your app, so you can present to originally requested view.
