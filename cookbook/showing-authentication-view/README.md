# Showing Authentication View

When your App requires authentication, you should invite the user to login when he tries to access your app features.
The `SignIn` App provides a signin form ready to be invoked from other `Starcounter` apps thanks to the `Blending` approche.
Here is the pattern for requesting authentication from your app:

When your app, decides that the view cannot be presented without authentication, it redirects to its own "unauthenticated" view. 
for exemple: /your-app/partial/unauthenticated?return_uri={?}

<div class="code-name">MainHandlers.cs</div>

```cs
{
	if (!IsSignedIn()) {
		return Self.GET("/your-app/partial/Unauthenticated?return_uri=" + getURL);
	}
}
```

the `getURL` is the Url requested in the original Get request.


And then define a new handler for the `Unauthenticated` partial

<div class="code-name">PartialHandlers.cs</div>

```cs
{
	Handle.GET("/your-app/partial/Unauthenticated?return_uri={?}", (string returnUri) => 
	{
		return new UnauthenticatedPage();
	});
}
```

That view is empty, it will only trigger the blending with signin form

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


If SignIn app is running, it shows its own view. That view is clever! Depending on `SignInFormAsFullPage` setting it will decide whether to display the login form right in this view, or redirect to a standalone page that only has the login form.
you can have other apps than SignIn that do something in this view, but they will be competing for attention.
After successful log in, SignIn app redirects back to the return_uri in your app, so you can present to originally requested view.
