# Static file server

Starcounter comes with a built-in HTTP static file server intended for client-side files such as HTML, JavaScript and images. Each application can have several static resources directories that are creating one virtual resources hierarchy.

Static content is always cached in RAM, so you should not use it for video or other very large pieces of content. An external server should be used for such files. The cache is updated when you replace the files on disk.

## Directories added by convention
When your application start, Starcounter support exposing static resources by convention.

For example, having this:

```
C:\Users\JohnDoe\apps\app1>dir
app1.exe
wwwroot
```

and starting the application doing this

```
C:\Users\JohnDoe\apps\app1>star app1
```

will add **wwwroot** as a resource directory explicitly.

Please note the *working directory* is what will be used to qualify the full path, so if you are in some other directory starting the application, you will have to specify the directory explicitly. See below.

The same convention applies in Visual Studio. If you have a Starcounter application project and run it from there, the working directory of the project is used to find a **wwwroot** and if such folder exist, it will registered as a static resource directory.

## Specifying static resources directories

There are several ways to explicitly add static resource directories to your application.

### Working Directory

When creating a new Starcounter application project, the default value for the projects *working directory* will be set to the project root directory. Hence, adding a **wwwroot** directory to the project will automatically make that a resource directory by means of the conventions described above.

To be even more specific, you can specify resource directories in the "Command line arguments" under the `Project | Properties | Debug` tab.

Command line arguments:
```
--resourcedir=thirdpartyscripts
```

By doing this, Starcounter will **add** that directory as an additional resource directory, in addition to a possible **wwwroot** as described above. You can specify both a relative directory or a fully qualified one. If you specify a relative directory, its qualified by means of the working directory, just as convention-based directories are.

### Star.exe CLI

When starting your application using `star.exe`, the resources directory can be set for that application explicitly using parameter `--resourcedir`, for example:

```
star --resourcedir=C:\MyWebsite\Content c:\MyWebsite\PMail.exe
```

You can use fully qualified paths, or relative paths. Relative paths are resolved by using the directory from where `star.exe` is invoked (i.e. the working directory).

### Code

Resources directory can also be added programmatically in your code, by calling `AppsBootstrapper.AddStaticFileDirectory`. The first parameter is relative/absolute static resources directory and second optional parameter is a port on which files should be served, for example:

```cs
AppsBootstrapper.AddStaticFileDirectory("C:\\MyWebsite\\Content", 80);
```

## Specifying multiple resource directories
In both command-line mode aswell as in Visual Studio, we have shown how to specify a resource directory using the `--resourceDir` option. You can also use this option to specify multiple resource directories.

Specify multiple resource directories using ";" (semi-colon) as the separator, like this:

```bash
star --resourcedir=thirdpartyfiles;images;d:\mywebserver\htmlutils app.exe
```

Mixing relative and absolute paths work in harmony. Relative paths are resolved as has been described above.

## Constraints
You can specify only directories that exist. As a consequence, you can not specify paths including characters not supported by the underlying platform. For example, specifying this on Windows

```
star --resourcedir="foo>>>" myapp.exe
```

result in an error like this

> ScErrBadArguments (SCERR1001): One or more arguments was invalid. Parameter --resourcedir contains an illegal element: 'foo>>>', error: Illegal characters in path.

## Directory locking

Once any file in the resource directory is served using HTTP - the parent directory can not be removed or renamed. This happens due to file changes observing mechanisms locking it. To be able to rename/delete/move the resource directory - the codehost has to be stopped for that operation.
