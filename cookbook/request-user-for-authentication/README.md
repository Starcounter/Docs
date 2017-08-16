# Requesting a user to authenticate

To separate the concerns, your app should not directly deal with user authentication. Rather than that, it should have a view that blends in an authentication view from another app that deals with that.

Below instruction shows how to provide an authentication notice that blends in the authentication form from the [SignIn](https://github.com/StarcounterApps/SignIn) app.

When your app decides that the view cannot be presented without authentication, it redirects to its own "Unauthenticated" view. 
For example: `/your-app/partial/unauthenticated?return_uri={?}`

<div class="code-name">MainHandlers.cs</div>

```cs
if (!IsSignedIn()) {
	return Self.GET("/your-app/partial/unauthenticated?return_uri=" + getURI);
}
```


The `getURI` is the URI requested in the original GET request.

Then define a new handler for the "Unauthenticated" partial.

<div class="code-name">PartialHandlers.cs</div>

```cs
Handle.GET("/your-app/partial/unauthenticated?return_uri={?}", (string returnURI) => 
{
	return new UnauthenticatedPage();
});
```

The "unauthenticated" view model could be associated with a "Unautheticated.html" which defines a warning message about not being authenticated. The main purpose for this partial is to provide the mapping to the authentication view coming from the [SignIn](https://github.com/StarcounterApps/SignIn) app using a predifined token.

<div class="code-name">Unauthenticated.json</div>

```json
{
  "Html": "/people/viewmodels/Unauthenticated.html"
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

<div class="code-name">Unauthenticated.Html</div>

```html

<template>
	<template is="dom-bind">
		<div>
			<iron-icon icon="icons:warning"></iron-icon>
			<label>You need to be signed in to use People.</label>
		</div>
	</template>
</template>

```

It should be mapped to a token `userform-return`, which is understood in other apps

<div class="code-name">Mapping.cs</div>

```cs

	Blender.MapUri("/your-app/partial/Unauthenticated?return_uri={?}", "userform-return");

```


If [SignIn](https://github.com/StarcounterApps/SignIn) app is not running, the user will only get a message about not being authenticated.

This message should be defined inside "Unauthenticated.Html" file which has to be set as the `Html` property for "unauthenticated" partial view.

![When signin app is not running](/assets/Authentication-nosignin.png)


But if [SignIn](https://github.com/StarcounterApps/SignIn) app is running, it shows its own view blended with the "Unauthenticated" warning message from the "Unauthenticated.Html" file. 
![When signin app is running](/assets/signin-authentication.png)

That view is clever! Depending on `SignInFormAsFullPage` setting it will decide whether to display the login form right in this view, or redirect to a standalone page that only has the login form.

You can have other apps than [SignIn](https://github.com/StarcounterApps/SignIn) that do something in this view, but they will be competing for attention.

After successful log in, [SignIn](https://github.com/StarcounterApps/SignIn) app redirects back to the `return_uri` in your app, so you can present to originally requested view.
