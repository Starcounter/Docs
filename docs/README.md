# Starcounter 3.0 - alpha `20190930`

Starcounter 3.0.0 Release Candidate is available for general access.

Download [`Starcounter.3.0.0-alpha-20190930.zip`](https://starcounter.io/Starcounter/Starcounter.3.0.0-alpha-20190930.zip) archive with all required NuGet packages.

Please make sure to read our [End User License Agreement for Starcounter Software](https://starcounter.com/wp-content/themes/starcounter-custom/assets/docs/Starcounter_EULA.pdf).

## Starcounter 3.0 alpha `20190930` main changes

- Starcounter database access is now provided with a [`Microsoft.Extensions.DependencyInjection`](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection/) service.
- `Starcounter.Star.Tool` is no longer required to manipulate and communicate with the database.
- It is now possible to deploy Starcounter applications with `dotnet publish` command and [run them in a Docker container](docker-sample.md).
- Overall performance has been significantly improved with multiple optimizations in the communication layer between application and the database.

### Dependency Injection (DI) and Starcounter

- Read more about Dependency Injection on the official Microsoft documentation page - [Dependency injection in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-3.0).
- Checkout [ASP.NET Core sample](dependency-injection.md) controller with Starcounter database access using Dependency Injection (DI).

## Requirements

- [Ubuntu 18.04.02 x64](https://ubuntu.com/download/desktop) or [Windows 10 Pro x64 Build 1903](https://www.microsoft.com/en-us/software-download/windows10).

  - [Windows Subsystem for Linux (WSL)](https://docs.microsoft.com/en-us/windows/wsl/install-win10) is also supported.

- [.NET Core 3.0.100](https://dotnet.microsoft.com/download/dotnet-core/3.0), SDK for development, runtime for production.
- Enough RAM to load database of targeted size.
- It's recommended to have at least two CPU cores.

**Note**: Due to the alpha state of this release we cannot provide any guarantees, but we monitor our [GitHub: Starcounter/Home](https://github.com/Starcounter/Home/issues) issue tracker and stand ready to assist with any potential issues.

## Installation

**Note**: This section assumes that you have required operating system and .NET Core `3.0.100` SDK installed.

### Binaries

- Create a folder for Starcounter binaries, for example `Starcounter.3.0.0-alpha-20190930`.
- Download [`Starcounter.3.0.0-alpha-20190930.zip`](https://starcounter.io/Starcounter/Starcounter.3.0.0-alpha-20190930.zip) into the folder.
- Unzip downloaded archive into the folder.

#### Windows 10

On Windows Starcounter requires x64 version of Visual C++ to be installed. Download and install it from the Microsoft website:

[The latest supported Visual C++ downloads](https://support.microsoft.com/en-us/help/2977003/the-latest-supported-visual-c-downloads).

#### Ubuntu 18.04 & 19.04

**Install prerequisites.**

```text
sudo apt-get install wget unzip
sudo apt-get install libaio1 libtinfo5
```

Starcounter relies on a specific version of [SWI-Prolog](https://wwu-pi.github.io/tutorials/lectures/lsp/010_install_swi_prolog.html).

```text
sudo add-apt-repository ppa:swi-prolog/stable
sudo apt-get update
sudo apt-get install swi-prolog-nox=7.\*
```

**Download and unpack Starcounter binaries.**

```text
cd $HOME
mkdir Starcounter.3.0.0-alpha-20190930
cd Starcounter.3.0.0-alpha-20190930
wget https://starcounter.io/Starcounter/Starcounter.3.0.0-alpha-20190930.zip
unzip Starcounter.3.0.0-alpha-20190930.zip
```

### Application

**Create an application folder and initialize a .NET Core console application.**

```text
mkdir StarcounterConsoleSample
cd StarcounterConsoleSample

dotnet new console
```

All the following commands shall be executed from the `StarcounterConsoleSample` folder.

**Setup NuGet to consume Starcounter packages feeds.**

Create `nuget.config` file and add required package sources:

- `local`, points to the Starcounter binaries folder.
- `Starcounter`, points to `https://www.myget.org/F/starcounter/api/v2`.

**NuGet tips**:

- Default `NuGet.config` file can be created with [`dotnet new nugetconfig`](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-new) command.
- For more information on how to work with NuGet configurations, see [this post](https://docs.microsoft.com/en-us/nuget/consume-packages/configuring-nuget-behavior) by Microsoft.

**End file should look similar to this**:

```markup
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="local" value="[Starcounter.3.0.0-alpha-20190930]" />
    <add key="Starcounter" value="https://www.myget.org/F/starcounter/api/v2" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
```

**Visual Studio Setup**:

Visual Studio requires manual NuGet package sources configuration. For this go to the `Tools → Options → NuGet Package Manager → Package Sources` menu then add `local` and `Starcounter` feeds.

**Note**: Replace the `[Starcounter.3.0.0-alpha-20190930]` value with actual path to the folder with unzipped Starcounter binaries.

**Add Starcounter.Database package reference**

```text
dotnet add package Starcounter.Database --version 3.0.0-*
```

**Add minimal Starcounter database access**

Replace content of the `Program.cs` file with the following:

```csharp
using System;
using System.Linq;
using Starcounter.Database;
using Microsoft.Extensions.DependencyInjection;

namespace StarcounterConsoleSample
{
    [Database]
    public abstract class Person
    {
        public abstract string Name { get; set; }
    }

    public class Program
    {
        public static void Main()
        {
            string connectionString =
                "Database=./.database/StarcounterConsoleSample;"
                + "OpenMode=CreateIfNotExists;"
                + "StartMode=StartIfNotRunning;"
                + "StopMode=IfWeStarted";

            // Here we create a service collection that
            // we add the Starcounter services to.
            // When we call BuildServiceProvider(), we get an instance
            // that we can use to fetch service instances,
            // for example ITransactor, which we then can use to
            // to make database transactions.
            using var services = new ServiceCollection()
                .AddStarcounter(connectionString)
                .BuildServiceProvider();

            // Here we fetch our ITransactor instance from the service provider.
            var transactor = services.GetRequiredService<ITransactor>();

            // And here we use it to make a database transaction.
            var name = transactor.Transact(db =>
            {
                // Here inside the transaction,
                // we can use the IDatabaseContext instance
                // to interact with the Starcounter database.

                // We can query it using SQL
                // (which returns an IEnumerable<Person>
                // that we can use with LINQ).
                var p = db.Sql<Person>
                (
                    "SELECT p FROM Person p WHERE Name = ?",
                    "Jane"
                ).FirstOrDefault();

                if (p == null)
                {
                    // We can insert new rows in the database using Insert().
                    p = db.Insert<Person>();

                    // We write to the database row
                    // using standard C# property accessors.
                    p.Name = "Jane";
                }

                // Let's return the name as result of the transaction.
                return p.Name;
            });

            // And let's print it in the console
            Console.WriteLine(name);
        }
    }
}
```

- [Read more about Starcounter database connection string](database-connection-string.md).
- [Read how to configure Starcounter database options](database-creation-options.md).

**For ASP.NET Core application**:

Update `Startup.cs` class with the following:

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace StarcounterMvcSample
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString =
                "Database=./.database/StarcounterMvcSample;"
                + "OpenMode=CreateIfNotExists;"
                + "StartMode=StartIfNotRunning;"
                + "StopMode=IfWeStarted";

            // This adds and configures Starcounter services to our application, for
            // example ITransactor, that allows us to create database transactions.
            services.AddStarcounter(connectionString);
        }
    }
}
```

**Note**: Starcounter works with Kestrel Web Server only. IIS and IIS Express are not yet supported.

## Running with [Visual Studio Code](https://code.visualstudio.com/Download)

- Open Visual Studio Code in the application folder (from command line: `code ./`).
- Restore dependencies Visual Studio Code asks for.
- Click `Ctrl + F5` to start the application.

Everything should run out of the box.

## Running with [Visual Studio 2019](https://visualstudio.microsoft.com/vs/)

- Update Visual Studio 2019 to the latest version using Visual Studio Installer.
- We checked version 16.3.0.
- Open `StarcounterConsoleSample.csproj` from Visual Studio.
- Click `Ctrl + F5` to start the application.

## Extra information

_Before asking questions or reporting issues, please read these few lines, and maybe you will find an answer for your question._

- Currently there is no database tooling available except the bare minimum of `dotnet star new` and `dotnet star start` commands.
- Starting from Starcounter 3.0.0 beta, all required packages will be uploaded to one of the popular providers, such as [NuGet.org](https://www.nuget.org/), [MyGet.org](https://www.myget.org/) or [GitHub Package Registry](https://github.com/features/package-registry).
- It is recommended to define all database classes and properties as `abstract` to reduce memory footprint when compared to `virtual`. Support for `virtual` properties might be removed in the future.
- Publishing application in a single file with [`dotnet publish /p:PublishSingleFile=true`](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-publish) is not yet supported.
