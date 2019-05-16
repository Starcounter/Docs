# Proposed Project Structure

The Starcounter app project structure is inspired by [the structure of ASP.NET 5 projects](http://gunnarpeipman.com/2014/10/asp-net-5-new-structure-of-solutions-and-projects/). The difference is that ASP.NET has `Controllers`, `Models`, `Scripts`, and `Views` while Starcounter has `Api` handlers, JSON + HTML `viewmodels` and `database` classes.

This tree displays the structure of a Starcounter project:



### Namespaces

`Api`, `Database`, `Helpers`, and `ViewModels` folders may or may not have their own namespace.

A json view model class would look like this:

```
using Starcounter;

namespace StarcounterSample 
{
    public class MainPage : Json 
    {
    }
}
```

A helper file inside `Helpers` folder would look like this:

```
namespace StarcounterSample 
{
    public class StringsHelper 
    {
    }
}
```

A handler file inside `Api` folder would look like this:

```
using Starcounter;

namespace StarcounterSample 
{
    public class MainHandlers 
    {
        public void Register() 
        {
            Handle.GET("/StarcounterSample", () => new MainPage());
        }
    }
}
```

Custom sub-folders and sub-namespaces could be used if needed.

A view page inside sub-folder: `StarcounterSample/ViewModels/Launcher/AppMenuPage.json`.

```
using Starcounter;

namespace StarcounterSample.Launcher 
{
    public class AppMenuPage: Json 
    {
    }
}
```

### C\# files & classes

Name of `.cs` and `.json` files should be exactly the same as classes inside. Name of `.json` and `.json.cs` files should also match.

The correct structure for sign in page would be like this:

* `ViewModels/SignInPage.json`
* `ViewModels/SignInPage.json.cs`
* `class SignInPage : Json`

### Html page files

Html files which represent a json view model like `SignInPage.Html` should be named exactly the same as name of json file.

This is correct:

```
{
    "Html": "/GoogleSignIn/views/SignInPage.html"
}
```

And these are incorrect:

```
{
    "Html": "/GoogleSignIn/views/signinpage.html"
}
```

```
{
    "Html": "/GoogleSignIn/views/sign-in-page.html"
}
```

### Custom elements

The html files which do not represent any of json page should be stored inside `elements` folder. The name of a file should be exactly the same as name of the element inside.

### JavaScript libraries

Most of the Starcounter applications do not use custom JavaScript libraries outside of WebComponents.  
If you need a custom JavaScript library then it should be scoped to your application and saved inside a `js` folder.

### Startup `Program.cs` class

* In small applicaions, the startup class may register handlers and make some preparations.
* In big applications, the startup class should be minified and only call `Register` method of `/Api/` classes.

### Visual Studio files

Visual Studio solution \(`sln`\) and project \(`csproj`, `kproj`\) files should be checked in to the version control system.

User preference files \(`suo`\), which carry information about unloaded projects in a solution or breakpoints in a project, should not be checked in to the version control system.

