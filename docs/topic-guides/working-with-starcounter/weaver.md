# Weaver

## Introduction

The weaver** **is the component that transforms and recompiles user code before it is loaded in the database. The weaver lets developers to write plain, ordinary business-focused source code and transparently enjoy the power of the Starcounter database engine.

For example, the simplest transformation might look something like this:

{% code-tabs %}
{% code-tabs-item title="BeforeWeaving" %}
```csharp
[Database]
public class Person
{
  public string Name { get; set; }
}
```
{% endcode-tabs-item %}

{% code-tabs-item title="AfterWeaving" %}
```csharp
[Database]
public class Person
{
    private ulong _id;
    public string Name
    {
        get => DbState.GetString(_id, "Name");
        set => DbState.SetString(_id, "Name", value);
    }
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

The design of the weaver is opaque to developers by design. Just as you don't have to care about the inner transformation details of some high-level language feature, such as [C\# delegates](http://msdn.microsoft.com/en-us/library/ms173171.aspx) - where the compilers and the common language runtime \(CLR\) do a lot of behind-the-scenes processing to hide the complexity - the weaver also handles transformation of user code in the background and on-the-fly. As a developer, you don't have to see or care about this transformation.

## How it works

The default behavior of the weaver, is that when you compile a Starcounter application, the weaver takes the input assembly, weaves it, and replaces the initial input assembly with the weaved input assembly. It also analyzes all the assemblies referenced by the input assembly and weaves those that it's able to analyse, and skips the rest. That means that only those assemblies that contains code that needs weaving are weaved.

By weaving in compile-time, weaving will only happen when something in the assembly has changed or when the developer explicitly rebuilds the assembly. For example, create a new Starcounter Application or Starcounter Library. Build it. There's now a weaved assembly in`bin\Debug`. Build again - weaving will not happen since everything is up to date. Rebuild - weaving will happen again.

## Weaving from the command-line

If you want to have more control over the weaving, you can do so from the command line. For example, weaving as part of compiling can be disabled by setting the MSBuild property `RunCompileTimeWeaver` to false. Likewise, projects that are not created as a Starcounter Application or Starcounter Library are not weaved.

In those cases, you can weave by using the command `scweaver <assembly.extension>`, for example `scweaver mylib.dll` or `scweaver myapp.exe`. When weaving is accomplished the input assembly will be weaved.

Weaving an assembly that is already weaved will generate an error:

> ScErrAlreadyWeaved \(SCERR12040\): The given assembly is already weaved. Assembly "myapp.exe" is already weaved.

Thus, if you need to script weaving and assure an assembly is weaved, check either for the error code 0 or 12040.

## Excluding assemblies from weaving

Since the weaver is able to determine what assemblies to weave and which not to weave, you don't have to explicitly exclude assemblies from being weaved, unless in edge cases, such as if you want to optimize the build time of massive projects. This is different from how it worked in versions prior to 2.4 where the weaver would always weave everything unless explicitly asking the weaver not to.

To exclude assemblies from weaving,

1. Create a plain text file in the same directory as the executable
2. Name it `weaver.ignore`, without any other extension
3. Specify the name of each assembly on a single line in the file

{% code-tabs %}
{% code-tabs-item title="weaver.ignore" %}
```text
foo.dll
bar.dll
```
{% endcode-tabs-item %}
{% endcode-tabs %}

Regular expressions are allowed and are matched according to the following pattern:

```csharp
new Regex("^" + specification
    .Replace(".", "\\.")
    .Replace("?", ".")
    .Replace("*", ".*"), RegexOptions.IgnoreCase);
```

If you are using Visual Studio to build your application, you can add the `weaver.ignore` file to the same project that builds your executable, and instruct Visual Studio to copy it to the output directory on every build.

1. Right-click the project in the Solution Explorer window and select `Add...` and then `New Item...`, or alternatively select `PROJECT | Add New Item...` from the main menu.
2. In the `Add New Item` dialog window, select `C#` and `General` and then `Text File`.
3. Name the file `weaver.ignore` and click `OK`.
4. In the Solution Explorer, locate the `weaver.ignore` file. Right-click it and select `Properties`.
5. In the `Properties` dialog window, set the `Copy To Output Directory` property to `Copy if newer`.

The next time you build, the `weaver.ignore` file will end up next to your compiled executable and thereby found by the weaver the next time you run the application in Starcounter.

## Troubleshooting weaver failures

In some situations, weaving fails. Failures normally come in one of two categories:

1. The application contains some feature Starcounter does not support yet.
2. The application contains, or references some code that contains, a reference that can not be resolved or properly analyzed.

The first category of errors are generally easier to resolve and the error information we can provide to you as a developer is often quite concise. As an example, if you define a `private` database class in an application targeting Starcounter 2.x, you'll get a clear message informing you this is not supported, and the identity of the class that was private. The way to resolve it is to make it public.

For the second category, or for any error that does not include a specific error condition, detecting what is actually wrong can be harder. As an example, dependency resolution failures can occur deep in a long chain of dependencies, and hence not trivial to fully comprehend. The solution is normally to exclude** **some file part of your application from being weaved/analyzed, as was described above in [How to exclude a file from being processed](weaver.md#how-to-exclude-a-file-from-being-processed) But how should you know what file you need to exclude?

One way to diagnose any failing application is to invoke the weaver in isolation. Instructions for this is in the [weaving from the command line section](weaver.md#weaving-from-the-command-line).

We can get the full diagnostic by running `scweaver --nocache --verbosity=diagnostic [app.exe]`

## Exceptions

### ScErrUnhandledWeaverException \(SCERR6066\)

There's no obvious solution to this exception. Reading [Troubleshooting Weaver Failures](weaver.md#troubleshooting-weaver-failures) might be helpful. You can also take a look at [Starcounter/Home\#88](https://github.com/Starcounter/Home/issues/88), [Starcounter/Home\#87](https://github.com/Starcounter/Home/issues/87), and [Starcounter/Home\#166](https://github.com/Starcounter/Home/issues/166) for more information.

#### When nothing else works: running the restricted weaver
If nothing of the above resolve the issue, there is a last-resort workaround that might require you to restructure your code somewhat, but have the potential of solving some of the more advanced cases. This option is available starting from 2.4 and allow the weaver to run in a more restricted mode by giving the promise that no database classes are defined in the project failing to weave (they can instead be moved out to a separate assembly).

Let's look at a typical scenario and how to address it:

You have an assembly (application or library) that fail to weave, reporting `ScErrUnhandledWeaverException`. The most likely cause is that the weaver can't handle some dependency of that assembly, for example due to the fact that new, unsupported features are used, causing the weaver trouble when analyzing the dependency tree.

What can be done in this case is

1. If you have any database classes in your assembly, move these out to another (possibly new) library - one with a minimum set of those dependencies of your failing application.
2. Edit your project, adding a new property: `<DisallowDatabaseClasses>True</DisallowDatabaseClasses>`
3. Rebuild your project (causing weaving to run again, but in a restricted way).

With `<DisallowDatabaseClasses>True</DisallowDatabaseClasses>` set, you can't *declare* database classes in that project / assembly, but you can still *use* database classes from referenced assemblies, including instantiating database types, reading- and writing properties, querying, and so on.

To test this feature by running it from the command-line, use `scweave --disallowdatabaseclasses your_assembly.dll`
