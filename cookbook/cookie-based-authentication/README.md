# Cookie-based authentication

In Starcounter you have as many sessions as there are browser windows or tabs opened with your app. The session object stores the state of a view-model for that particular window or tab. Each session is assigned a new, unique identifier. Session is not aware of concepts like "authentication" or "a returning user".

This is different from persistent user sessions known from other frameworks such as [ASP.NET Core](https://docs.asp.net/en/latest/fundamentals/app-state.html) or [PHP](http://php.net/manual/en/book.session.php). These frameworks use the term "session" when they mean remembering of returning users. That is the basis for cookie-based authentication.

How do you implement cookie-based authentication with Starcounter? You have to code it on your own using our HTTP low level API. Or if you want to keep it simple - run your app alongside another app that does just that in middleware.

[Sign In](https://github.com/StarcounterApps/SignIn) is a prefab app that authenticates a user using a username and a password. It generates an auth token and stores it in a cookie for use in future sessions.

## Internals of Sign In app

Let's take a deeper look at how Sign In app works. Essentially, this can be divided into three parts:

* Form for authentication input
* HTTP handler that sets the cookie
* Middleware that reads the cookie

## Form for authentication input

Sign In has a view-model that can be mapped to any app. It is a typical HTML form that asks for authentication input (username &amp; password).

Unlike typical Puppet-based view-models, the form is not sent using WebSocket. The form result is sent using `XmlHttpRequest` to a HTTP handler, because that's the only way to set a cookie.

This is how the form gets submitted to force using HTTP:

<div class="code-name">
<a href="https://github.com/StarcounterApps/SignIn/blob/master/src/SignIn/wwwroot/SignIn/elements/signin-element.html" target="_blank">signin-element.html</a></div>

```cs
var password = Sha1.hash(this.password);
var url = this.url + this.username + "/" + password + "/" + this.rememberMe;
this.set("password", "");
document.querySelector("puppet-client").network.changeState(url);
```

## HTTP handler that sets the cookie

When the user submits the form, a relevant HTTP handler tries to authenticate the user. In case of successful authentication, an auth token is generated and stored in the database:

<div class="code-name">
<a href="https://github.com/StarcounterApps/Simplified/blob/master/Ring3/User/SystemUser.Static.cs" target="_blank">Simplified/blob/master/Ring3/User/SystemUser.Static.cs</a></div>

```cs
var systemUser = Db.SQL<SystemUser>("SELECT o FROM Simplified.Ring3.SystemUser o WHERE o.Username = ?", Username).FirstOrDefault();

if (systemUser == null)
{
    return null;
}

GeneratePasswordHash(Username.ToLower(), Password, systemUser.PasswordSalt, out hashedPassword);

if (systemUser.Password != hashedPassword)
{
    return null;
}

SystemUserSession userSession = null;

Db.Transact(() =>
{
    var token = new SystemUserTokenKey();

    token.Created = token.LastUsed = DateTime.UtcNow;
    token.Token = CreateAuthToken(systemUser.Username);
    token.User = systemUser;

    userSession = AssureSystemUserSession(token);
});
```

Auth token is now sent to the user as a cookie in the HTTP response. Future HTTP requests from that user will include the auth token cookie:

<div class="code-name">
<a href="https://github.com/StarcounterApps/SignIn/blob/master/src/SignIn/Api/MainHandlers.cs" target="_blank">MainHandlers.cs</a></div>

```cs
var cookie = new Cookie()
{
    Name = AuthCookieName
};

if (Session != null && Session.Token != null)
{
    cookie.Value = Session.Token.Token;
}

if (Session == null)
{
    cookie.Expires = DateTime.Today;
}
else if (RememberMe)
{
    cookie.Expires = DateTime.Now.AddDays(rememberMeDays);
}
else
{
    cookie.Expires = DateTime.Now.AddDays(1);
}

Handle.AddOutgoingCookie(cookie.Name, cookie.GetFullValueString());
```

## Middleware that reads the cookie

Sign In app registers a request middleware. The middleware checks if the request contains a valid auth token cookie. In that case, we can immediately authenticate the user without asking for the credentials.

If the auth token is correct, the middleware creates a Session object and identifies the user. Any other app that handles this request will now see the user.

<div class="code-name">
<a href="https://github.com/StarcounterApps/SignIn/blob/master/src/SignIn/Api/MainHandlers.cs" target="_blank">MainHandlers.cs</a></div>

```cs
Application.Current.Use((Request request) =>
{
    Cookie cookie = GetSignInCookie();

    if (cookie != null)
    {
        Session.Current = Session.Current ?? new Session();

        var session = SystemUser.SignInSystemUser(cookie.Value);

        if (session != null)
        {
            RefreshAuthCookie(session);
        }
    }

    return null;
});
```

With this middleware, any app that handles this request will see that the session already exists. In case of our sample apps, the User Admin app shares the data model with the Sign In app. A simple lookup in the `SystemUserSession` table will get the relevant `SystemUser` object.

<div class="code-name">
<a href="https://github.com/StarcounterPrefabs/UserAdmin/blob/master/src/UserAdmin/Helpers/Helper.cs" target="_blank">UserAdmin/Helpers/Helper.cs</a></div>

```cs
var query = "SELECT o FROM Simplified.Ring5.SystemUserSession o WHERE o.SessionIdString = ?";
var userSession = Db.SQL<SystemUserSession>(query, Session.Current.SessionId).FirstOrDefault();

return userSession?.Token?.User;
```
