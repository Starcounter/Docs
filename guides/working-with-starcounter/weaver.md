# Weaver

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

## How it works

Starcounter applications can be started in several ways:

* Using F5 in Visual Studio.
* Using `star.exe` from a command-prompt.
* From Starcounter Administrator.

Behind the scenes the actual bootstraping of the application is done via the Starcounter server. Part of this bootstrapping is weaving. The server hands the path to the compiled executable to the weaver, and the weaver processes it. If weaving succeeds, the transformed code is loaded in the Starcounter database. If it fails, the relevant error\(s\) are reported back to the agent that tried to start the executable \(e.g. Visual Studio\) and the start attempt is cancelled.

### Dependencies

When the weaver is given the path to the compiled executable, it checks that it can resolve and process all _dependencies_ of the executable. By default, the weaver consider the following files to be a dependency of the executable:

* all  `.dll` files in the same folder as the executable
* all .NET assembly references from the executable \(recursively\) that reference `Starcounter.dll` and is not considered part of the Starcounter installation \(i.e. Starcounter system files are ignored\).

The weaver will try process \(load and transform\) any file considered a dependency of the given executable.

### How to exclude a file from being processed

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

## Troubleshooting weaver failures

In some situations, weaving fails. Failures normally come in one of two categories:

1. The application contains some feature Starcounter does not support yet.
2. The application contains, or _references_ some code that contains, a reference that can not be resolved or properly analyzed.

The first category of errors are generally easier to resolve and the error information we can provide to you as a developer is often quite concise. As an example, if you define a **private** database class in an application targeting Starcounter 2.x, you'll get a clear message informing you this is not supported, and the identity of the class that was private. The way to resolve it is to make it public.

For the second category, or for _any_ error that does not include a specific error condition, detecting what is actually wrong can be harder. As an example, dependency resolution failures can occur deep in a long chain of dependencies, and hence not trivial to fully comprehend. The **resolution** is normally to **exclude** some file part of your application from being weaved/analyzed, as was described above in "How to exclude a file from being processed". But how should you know what file you need to exclude?

One way to diagnose any failing application is to **invoke the weaver in isolation**. This is done by executing the weaver executable from a command-line environment. It's in the PATH, so what you have to do is simply:

1. Open a command-prompt
2. Run `scweaver path/to/your/app.exe`.

The effect of that is that the weaver will run, trying to analyze and weave \(i.e. transform\) your application into a form Starcounter will use to load it into. Weaving in isolation like this **does not load** your application though, and the output produced will end up in a `.starcounter` directory next to your executable.

Let's look at an example. Notice the options we used to get the maximum of diagnostics: `--nocache --verbosity=diagnostic`.

```text
C:\Users\Per\Bogota\bin\Debug>scweaver --nocache --verbosity=diagnostic bogota.exe

=== Bootstrap diagnostics ===
Code weaver:
  Input directory: C:\Users\Per\Bogota\bin\Debug
  Output directory: C:\Users\Per\Bogota\bin\Debug\.starcounter
  Application file: bogota.exe
  Disable edition libraries: False
  Disable cache: True
  Weave only to cache: False
Weaver cache:
  Cache directory: C:\Users\Per\Bogota\bin\Debug\.starcounter\cache
  Disabled: True
  4 search directories:
  C:\Users\Per\Bogota\bin\Debug
  C:\Users\Per\Git\Starcounter
  C:\Users\Per\Git\Starcounter\EditionLibraries
  C:\Users\Per\Git\Starcounter\LibrariesWithDatabaseClasses
File exclusion policy:
  5 weaver.ignore files:
  Newtonsoft.Json.dll
  CommandLine.dll
  ImageGlue.dll
  CsvHelper.dll
  unins000.exe
File manager:
  Source directory: C:\Users\Per\Bogota\bin\Debug
  Target directory: C:\Users\Per\Bogota\bin\Debug\.starcounter
  12 input files considered.
  C:\Users\Per\Bogota\bin\Debug:
    Concepts.Ring8.Bogota.dll:
    ImageGlue.dll:
    Microsoft.VisualStudio.QualityTools.UnitTestFramework.dll:
    Newtonsoft.Json.dll:
    Bogota.Common.dll:
    bogota.exe:
  C:\Users\Per\Git\Starcounter\EditionLibraries:
    Simplified.Data.Model.dll:
    SocietyObjects.Concepts.Ring1.dll:
    SocietyObjects.Concepts.Ring2.dll:
    SocietyObjects.Concepts.Ring3.dll:
    SocietyObjects.Concepts.Ring4.dll:
  C:\Users\Per\Git\Starcounter\LibrariesWithDatabaseClasses:
    Starcounter.Extensions.dll:
======
Retieving files to weave.
Not reusing cached assembly "Concepts.Ring8.Bogota.dll": The cache was disabled.
Not processing assembly ImageGlue.dll: it's excluded by policy.
Not reusing cached assembly "Microsoft.VisualStudio.QualityTools.UnitTestFramework.dll": The cache was disabled.
Not processing assembly Newtonsoft.Json.dll: it's excluded by policy.
Not reusing cached assembly "Bogota.Common.dll": The cache was disabled.
Not reusing cached assembly "bogota.exe": The cache was disabled.
Not reusing cached assembly "Simplified.Data.Model.dll": The cache was disabled.
Not reusing cached assembly "SocietyObjects.Concepts.Ring1.dll": The cache was disabled.
Not reusing cached assembly "SocietyObjects.Concepts.Ring2.dll": The cache was disabled.
Not reusing cached assembly "SocietyObjects.Concepts.Ring3.dll": The cache was disabled.
Not reusing cached assembly "SocietyObjects.Concepts.Ring4.dll": The cache was disabled.
Not reusing cached assembly "Starcounter.Extensions.dll": The cache was disabled.
Retreived 10 files to weave.
Weaving C:\Users\Per\Bogota\bin\Debug\Concepts.Ring8.Bogota.dll.
Detected reference: mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089.
Not analyzing/weaving C:\WINDOWS\assembly\GAC_64\mscorlib\2.0.0.0__b77a5c561934e089\mscorlib.dll: not part of inclusion set.
Weaving C:\Users\Per\Git\Starcounter\EditionLibraries\SocietyObjects.Concepts.Ring1.dll.
Not analyzing/weaving C:\Users\Per\Git\Starcounter\Starcounter.dll: not part of inclusion set.
Not analyzing/weaving C:\Users\Per\Git\Starcounter\Starcounter.Internal.dll: not part of inclusion set.
Weaving C:\Users\Per\Git\Starcounter\EditionLibraries\SocietyObjects.Concepts.Ring2.dll.
Detected reference: system, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089.
Not analyzing/weaving C:\WINDOWS\Microsoft.Net\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll: not part of inclusion set.
Weaving C:\Users\Per\Git\Starcounter\EditionLibraries\SocietyObjects.Concepts.Ring4.dll.
Weaving C:\Users\Per\Git\Starcounter\EditionLibraries\SocietyObjects.Concepts.Ring3.dll.
Detected reference: newtonsoft.json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed.
Not analyzing/weaving C:\Users\Per\Bogota\bin\Debug\Newtonsoft.Json.dll: not part of inclusion set.
Detected reference: system.core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089.
Not analyzing/weaving C:\WINDOWS\Microsoft.Net\assembly\GAC_MSIL\System.Core\v4.0_4.0.0.0__b77a5c561934e089\System.Core.dll: not part of inclusion set.
Detected reference: microsoft.csharp, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a.
Not analyzing/weaving C:\WINDOWS\Microsoft.Net\assembly\GAC_MSIL\Microsoft.CSharp\v4.0_4.0.0.0__b03f5f7f11d50a3a\Microsoft.CSharp.dll: not part of inclusion set.
Weaving C:\Users\Per\Bogota\bin\Debug\Microsoft.VisualStudio.QualityTools.UnitTestFramework.dll.
Detected reference: system.data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089.
Not analyzing/weaving C:\WINDOWS\assembly\GAC_64\System.Data\2.0.0.0__b77a5c561934e089\System.Data.dll: not part of inclusion set.
Detected reference: system.web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a.
Not analyzing/weaving C:\WINDOWS\assembly\GAC_64\System.Web\2.0.0.0__b03f5f7f11d50a3a\System.Web.dll: not part of inclusion set.
Detected reference: system.configuration, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a.
Not analyzing/weaving C:\WINDOWS\assembly\GAC_MSIL\System.Configuration\2.0.0.0__b03f5f7f11d50a3a\System.Configuration.dll: not part of inclusion set.
Detected reference: system.web.services, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a.
Not analyzing/weaving C:\WINDOWS\assembly\GAC_MSIL\System.Web.Services\2.0.0.0__b03f5f7f11d50a3a\System.Web.Services.dll: not part of inclusion set.
Detected reference: microsoft.visualstudio.qualitytools.utfresources, Version=10.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a.
Not analyzing/weaving C:\WINDOWS\assembly\GAC_MSIL\Microsoft.VisualStudio.QualityTools.UTFResources\10.1.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.QualityTools.UTFResources.dll: not part of inclusion set.
Weaving C:\Users\Per\Bogota\bin\Debug\Bogota.Common.dll.
Detected reference: starcounter.xson, Version=2.0.0.0, Culture=neutral, PublicKeyToken=d2df1e81d0ca3abf.
Not analyzing/weaving C:\Users\Per\Git\Starcounter\Starcounter.XSON.dll: not part of inclusion set.
Detected reference: system.servicemodel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089.
Not analyzing/weaving C:\WINDOWS\Microsoft.Net\assembly\GAC_MSIL\System.ServiceModel\v4.0_4.0.0.0__b77a5c561934e089\System.ServiceModel.dll: not part of inclusion set.
Detected reference: system.xml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089.
Not analyzing/weaving C:\WINDOWS\Microsoft.Net\assembly\GAC_MSIL\System.Xml\v4.0_4.0.0.0__b77a5c561934e089\System.Xml.dll: not part of inclusion set.
Detected reference: system.drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a.
Not analyzing/weaving C:\WINDOWS\Microsoft.Net\assembly\GAC_MSIL\System.Drawing\v4.0_4.0.0.0__b03f5f7f11d50a3a\System.Drawing.dll: not part of inclusion set.
Detected reference: starcounter.logging, Version=2.0.0.0, Culture=neutral, PublicKeyToken=d2df1e81d0ca3abf.
Not analyzing/weaving C:\Users\Per\Git\Starcounter\Starcounter.Logging.dll: not part of inclusion set.
Detected reference: system.runtime.serialization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089.
Not analyzing/weaving C:\WINDOWS\Microsoft.Net\assembly\GAC_MSIL\System.Runtime.Serialization\v4.0_4.0.0.0__b77a5c561934e089\System.Runtime.Serialization.dll: not part of inclusion set.
Detected reference: imageglue, Version=7.4.0.0, Culture=neutral, PublicKeyToken=d78d2cea5dcfdc98.
Not analyzing/weaving C:\Users\Per\Bogota\bin\Debug\ImageGlue.dll: not part of inclusion set.
Detected reference: system.io.compression, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089.
Not analyzing/weaving C:\WINDOWS\Microsoft.Net\assembly\GAC_MSIL\System.IO.Compression\v4.0_4.0.0.0__b77a5c561934e089\System.IO.Compression.dll: not part of inclusion set.
Detected reference: system.io.compression.filesystem, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089.
Not analyzing/weaving C:\WINDOWS\Microsoft.Net\assembly\GAC_MSIL\System.IO.Compression.FileSystem\v4.0_4.0.0.0__b77a5c561934e089\System.IO.Compression.FileSystem.dll: not part of inclusion set.
Weaving C:\Users\Per\Bogota\bin\Debug\bogota.exe.
Weaving C:\Users\Per\Git\Starcounter\EditionLibraries\Simplified.Data.Model.dll.
Weaving C:\Users\Per\Git\Starcounter\LibrariesWithDatabaseClasses\Starcounter.Extensions.dll.
=== Finalization diagnostics ===
Code weaver:
  19 assemblies actually referenced:
  mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
  system, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
  newtonsoft.json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
  system.core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
  microsoft.csharp, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
  system.data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
  system.web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
  system.configuration, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
  system.web.services, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
  microsoft.visualstudio.qualitytools.utfresources, Version=10.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
  starcounter.xson, Version=2.0.0.0, Culture=neutral, PublicKeyToken=d2df1e81d0ca3abf
  system.servicemodel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
  system.xml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
  system.drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
  starcounter.logging, Version=2.0.0.0, Culture=neutral, PublicKeyToken=d2df1e81d0ca3abf
  system.runtime.serialization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
  imageglue, Version=7.4.0.0, Culture=neutral, PublicKeyToken=d78d2cea5dcfdc98
  system.io.compression, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
  system.io.compression.filesystem, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
  31 assemblies in weaver domain:
  C:\Users\Per\Bogota\bin\Debug:
    Concepts.Ring8.Bogota.dll:
    Microsoft.VisualStudio.QualityTools.UnitTestFramework.dll:
    Bogota.Common.dll:
    bogota.exe:
    Newtonsoft.Json.dll:
    ImageGlue.dll:
  C:\Users\Per\Git\Starcounter\EditionLibraries:
    Simplified.Data.Model.dll:
    SocietyObjects.Concepts.Ring1.dll:
    SocietyObjects.Concepts.Ring2.dll:
    SocietyObjects.Concepts.Ring3.dll:
    SocietyObjects.Concepts.Ring4.dll:
  C:\Users\Per\Git\Starcounter\LibrariesWithDatabaseClasses:
    Starcounter.Extensions.dll:
  C:\WINDOWS\assembly\GAC_64\mscorlib\2.0.0.0__b77a5c561934e089:
    mscorlib.dll:
  C:\Users\Per\Git\Starcounter:
    Starcounter.dll:
    Starcounter.Internal.dll:
    Starcounter.XSON.dll:
    Starcounter.Logging.dll:
  C:\WINDOWS\Microsoft.Net\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089:
    System.dll:
  C:\WINDOWS\Microsoft.Net\assembly\GAC_MSIL\System.Core\v4.0_4.0.0.0__b77a5c561934e089:
    System.Core.dll:
  C:\WINDOWS\Microsoft.Net\assembly\GAC_MSIL\Microsoft.CSharp\v4.0_4.0.0.0__b03f5f7f11d50a3a:
    Microsoft.CSharp.dll:
  C:\WINDOWS\assembly\GAC_64\System.Data\2.0.0.0__b77a5c561934e089:
    System.Data.dll:
  C:\WINDOWS\assembly\GAC_64\System.Web\2.0.0.0__b03f5f7f11d50a3a:
    System.Web.dll:
  C:\WINDOWS\assembly\GAC_MSIL\System.Configuration\2.0.0.0__b03f5f7f11d50a3a:
    System.Configuration.dll:
  C:\WINDOWS\assembly\GAC_MSIL\System.Web.Services\2.0.0.0__b03f5f7f11d50a3a:
    System.Web.Services.dll:
  C:\WINDOWS\assembly\GAC_MSIL\Microsoft.VisualStudio.QualityTools.UTFResources\10.1.0.0__b03f5f7f11d50a3a:
    Microsoft.VisualStudio.QualityTools.UTFResources.dll:
  C:\WINDOWS\Microsoft.Net\assembly\GAC_MSIL\System.ServiceModel\v4.0_4.0.0.0__b77a5c561934e089:
    System.ServiceModel.dll:
  C:\WINDOWS\Microsoft.Net\assembly\GAC_MSIL\System.Xml\v4.0_4.0.0.0__b77a5c561934e089:
    System.Xml.dll:
  C:\WINDOWS\Microsoft.Net\assembly\GAC_MSIL\System.Drawing\v4.0_4.0.0.0__b03f5f7f11d50a3a:
    System.Drawing.dll:
  C:\WINDOWS\Microsoft.Net\assembly\GAC_MSIL\System.Runtime.Serialization\v4.0_4.0.0.0__b77a5c561934e089:
    System.Runtime.Serialization.dll:
  C:\WINDOWS\Microsoft.Net\assembly\GAC_MSIL\System.IO.Compression\v4.0_4.0.0.0__b77a5c561934e089:
    System.IO.Compression.dll:
  C:\WINDOWS\Microsoft.Net\assembly\GAC_MSIL\System.IO.Compression.FileSystem\v4.0_4.0.0.0__b77a5c561934e089:
    System.IO.Compression.FileSystem.dll:
======
```

