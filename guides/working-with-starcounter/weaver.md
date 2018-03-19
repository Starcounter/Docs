# Weaver

## Introduction

The **weaver** \(also **code weaver** or **enhancer**\) is the component that **transforms and recompiles user code** before it is loaded in the database. The weaver allows developers to write plain, ordinary business-focused source code in their language of choice and transparently enjoy the power of the Starcounter database engine.

For example, the simplest transformation might look something like this:

**Before**:

```csharp
[Database]
public class Person
{
  public string Name { get; set; }
}
```

**After**:

```csharp
[Database]
public class Person
{
  public string Name
  {
    get { return Db.ReadString(123); }
    set { return Db.WriteString(123, value); }
  }
}
```

One of the design criteria of the weaver was to make it opaque to developers. Just as you don't have to care about the inner transformation details of some high-level language feature, such as [C\# delegates](http://msdn.microsoft.com/en-us/library/ms173171.aspx) - where the compilers and the common language runtime \(CLR\) do a lot of behind-the-scenes processing to hide the complexity - the weaver also handles transformation of user code in the background and on-the-fly. As a developer, you should generally never have to see or care about this transformation.

## How it Works

Starcounter applications can be started in several ways:

* Using F5 in Visual Studio.
* Using `star.exe` from a command-prompt.
* From Starcounter Administrator.

Behind the scenes the actual bootstraping of the application is done via the Starcounter server. Part of this bootstrapping is weaving. The server hands the path to the compiled executable to the weaver, and the weaver processes it. If weaving succeeds, the transformed code is loaded in the Starcounter database. If it fails, the relevant error\(s\) are reported back to the agent that tried to start the executable \(e.g. Visual Studio\) and the start attempt is cancelled.

## Dependencies

When the weaver is given the path to the compiled executable, it checks that it can resolve and process all _dependencies_ of the executable. By default, the weaver consider the following files to be a dependency of the executable:

* all  `.dll` files in the same folder as the executable
* all .NET assembly references from the executable \(recursively\) that reference `Starcounter.dll` and is not considered part of the Starcounter installation \(i.e. Starcounter system files are ignored\).

The weaver will try process \(load and transform\) any file considered a dependency of the given executable.

## How to Exclude a File From Being Processed

In certain cases, you might want to exclude files from being processed by the weaver. If, for example,

* you have a native library deployed in the same directory as the executable
* you have a strong-named assembly in the same directory as the executable
* you have a file that you know doesn't need any transformation - either because it does not use any database classes or it does not even reference Starcounter - and you want the weaver to perform faster.

To instruct the weaver to exclude files,

1. Create a plain text file in the same directory as your executable.
2. Name it `weaver.ignore` \(without any other extension\)
3. Specify the name of each file on a single line in the file.

A simple `weaver.ignore` file:

```text
foo.dll
bar.dll
```

Regular expressions are allowed and is matched according to the following pattern:

```csharp
new Regex("^" + specification.Replace(".", "\\.").Replace("?", ".").
Replace("*", ".*"), RegexOptions.IgnoreCase);
```

If you are using Visual Studio to build your application, you can add the `weaver.ignore` file to the same project that builds your executable, and instruct Visual Studio to copy it to the output directory on every build.

1. Right-click the project in the **Solution Explorer** window and select **Add...** and then **New Item...**, or alternatively select **PROJECT \| Add New Item...** from the main menu.
2. In the **Add New Item** dialog window, select **C\#** and **General** and then **Text File**.
3. Name the file `weaver.ignore` and click **OK**.
4. In the **Solution Explorer**, locate the `weaver.ignore` file. Right-click it and select **Properties**.
5. In the **Properties** dialog window, set the **Copy To Output Directory** property to **Copy if newer**.

The next time you build, the `weaver.ignore` file will end up next to your compiled executable and thereby found by the weaver the next time you run the application in Starcounter.

## Troubleshooting Weaver Failures

In some situations, weaving fails. Failures normally come in one of two categories:

1. The application contains some feature Starcounter does not support yet.
2. The application contains, or _references_ some code that contains, a reference that can not be resolved or properly analyzed.

The first category of errors are generally easier to resolve and the error information we can provide to you as a developer is often quite concise. As an example, if you define a **private** database class in an application targeting Starcounter 2.x, you'll get a clear message informing you this is not supported, and the identity of the class that was private. The way to resolve it is to make it public.

For the second category, or for _any_ error that does not include a specific error condition, detecting what is actually wrong can be harder. As an example, dependency resolution failures can occur deep in a long chain of dependencies, and hence not trivial to fully comprehend. The **resolution** is normally to **exclude** some file part of your application from being weaved/analyzed, as was described above in [How to exclude a file from being processed](weaver.md#how-to-exclude-a-file-from-being-processed) But how should you know what file you need to exclude?

One way to diagnose any failing application is to **invoke the weaver in isolation**. This is done by executing the weaver executable from a command-line environment. It's in the PATH, so what you have to do is simply:

1. Open a command-prompt
2. Run `scweaver path/to/your/app.exe`.

The effect of that is that the weaver will run, trying to analyze and weave \(i.e. transform\) your application into a form Starcounter will use to load it into. Weaving in isolation like this **does not load** your application though, and the output produced will end up in a `.starcounter` directory next to your executable.

We can get the full diagnostic by running `scweaver --nocache --verbosity=diagnostic [app.exe]`

## Exceptions

### ScErrWeaverFailedStrongNameAsm \(SCERR2143\)

To solve this exception, try to follow these two steps:

1. Set `weaver.ignore` file "Copy to output directory" to "Copy always".
2. Make sure there are no blank lines at the the beginning or end of `weaver.ignore`.

More details can be found in [Starcounter/Home\#31](https://github.com/Starcounter/Home/issues/31).

### ScErrUnhandledWeaverException \(SCERR6066\)

There's no obvious solution to this exception. Reading [Troubleshooting Weaver Failures](weaver.md#troubleshooting-weaver-failures) might be helpful. You can also take a look at [Starcounter/Home\#88](https://github.com/Starcounter/Home/issues/88), [Starcounter/Home\#87](https://github.com/Starcounter/Home/issues/87), and [Starcounter/Home\#166](https://github.com/Starcounter/Home/issues/166) for more information.

