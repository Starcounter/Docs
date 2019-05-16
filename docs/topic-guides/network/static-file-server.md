# Static file server

## Introduction

Starcounter comes with a built-in HTTP static file server intended for client-side files such as HTML, JavaScript and images. Each application can have several static resources directories that are creating one virtual resources hierarchy.

Static content is always cached in RAM, so you should not use it for video or other very large pieces of content. An external server should be used for such files. The cache is updated when you replace the files on disk.

## Directories added by convention

When your application start, Starcounter support exposing static resources by convention.

For example, having this:

```text
C:\Users\JohnDoe\apps\app1>dir
app1.exe
wwwroot
```

and starting the application doing this

```text
C:\Users\JohnDoe\apps\app1>star app1
```

will add **wwwroot** as a resource directory explicitly.

Please note the _working directory_ is what will be used to qualify the full path, so if you are in some other directory starting the application, you will have to specify the directory explicitly. See below.

The same convention applies in Visual Studio. If you have a Starcounter application project and run it from there, the working directory of the project is used to find a **wwwroot** and if such folder exist, it will registered as a static resource directory.

## Application specific static files

Application specific static files should be put in a directory with the name of the application. For example, for the [People](https://github.com/Starcounter/People) app, the HTML, CSS, images, JavaScript and other files that are strictly specific to that app should be put in the directory `wwwroot/People`. This prevents namespace collisions between apps running together.

## External dependencies

External dependencies, such as fonts, Web Components and other files that might be used in more than one app should be put in a `sys` directory. For example, the [People](https://github.com/Starcounter/People) app puts the Polymer elements it uses in `wwwroot/sys`.

Starcounter has certain files that are served by default to the `sys` directory which makes them available to all apps. One example is Polymer; even if it's not put in the `sys` folder of individual apps, it's still available with the HTML import `<link rel="import" href="/sys/polymer/polymer.html">` because it's served by Starcounter.

With this approach, we ensure that HTML imports only load each dependency one time.

For further instructions on how to add external dependencies to Web Components, read [avoiding loading the same files multiple times](../blendable-web-apps/web-components.md#avoiding-loading-the-same-files-multiple-times) on the Web Components page.

## Specifying static resource directory

There are several ways to explicitly add static resource directories to your application.

### Working directory

When creating a new Starcounter application project, the default value for the projects _working directory_ will be set to the project root directory. Hence, adding a **wwwroot** directory to the project will automatically make that a resource directory by means of the conventions described above.

To be even more specific, you can specify resource directories in the "Command line arguments" under the `Project | Properties | Debug` tab.

Command line arguments:

```text
--resourcedir=thirdpartyscripts
```

By doing this, Starcounter will **add** that directory as an additional resource directory, in addition to a possible **wwwroot** as described above. You can specify both a relative directory or a fully qualified one. If you specify a relative directory, its qualified by means of the working directory, just as convention-based directories are.

### star CLI

When starting your application using `star.exe`, the resources directory can be set for that application explicitly using parameter `--resourcedir`, for example:

```text
star --resourcedir=C:\MyWebsite\Content c:\MyWebsite\PMail.exe
```

You can use fully qualified paths, or relative paths. Relative paths are resolved by using the directory from where `star.exe` is invoked \(i.e. the working directory\).

### Code

Resources directory can also be added programmatically in your code, by calling `AppsBootstrapper.AddStaticFileDirectory`. The first parameter is relative/absolute static resources directory and second optional parameter is a port on which files should be served, for example:

```csharp
AppsBootstrapper.AddStaticFileDirectory("C:\\MyWebsite\\Content", 80);
```

To use `AddStaticFileDirectory`, add the `Starcounter.Apps.JsonPatch` assembly as a reference in the project. By default, it's found at `C:\Program Files\Starcounter\Public Assemblies`.

### Specifying multiple resource directories

In both command-line mode aswell as in Visual Studio, we have shown how to specify a resource directory using the `--resourceDir` option. You can also use this option to specify multiple resource directories.

Specify multiple resource directories using ";" \(semi-colon\) as the separator, like this:

```bash
star --resourcedir=thirdpartyfiles;images;d:\mywebserver\htmlutils app.exe
```

Mixing relative and absolute paths work in harmony. Relative paths are resolved as has been described above.

## Constraints

You can specify only directories that exist. As a consequence, you can not specify paths including characters not supported by the underlying platform. For example, specifying this on Windows

```text
star --resourcedir="foo>>>" myapp.exe
```

result in an error like this

> ScErrBadArguments \(SCERR1001\): One or more arguments was invalid. Parameter --resourcedir contains an illegal element: 'foo&gt;&gt;&gt;', error: Illegal characters in path.

## Directory locking

Once any file in the resource directory is served using HTTP - the parent directory can not be removed or renamed. This happens due to file changes observing mechanisms locking it. To be able to rename/delete/move the resource directory - the codehost has to be stopped for that operation.

