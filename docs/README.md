# Starcounter 3.0 - public preview

Starcounter 3.0.0 Alpha 20190701, codenamed Nova, is available for public preview.

Please make sure to read our [End User License Agreement for Starcounter Software](https://starcounter.com/wp-content/themes/starcounter-custom/assets/docs/Starcounter_EULA.pdf).

## Starcounter 3.0 main focus

- Support Linux & Windows operating systems.
- Provide seamless integration with standard .NET Core applications:
  - .NET Core Console applications.
  - .NET Core Web applications with [Kestrel web server](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel).
  - .NET Core Desktop applications.
- Support development experience with standard `dotnet` CLI tooling.

## Requirements

- [Ubuntu 18.04.02 x64](https://ubuntu.com/download/desktop) or [Windows 10 Pro x64 Build 1903](https://www.microsoft.com/en-us/software-download/windows10).
  - [Windows Subsystem for Linux (WSL)](https://docs.microsoft.com/en-us/windows/wsl/install-win10) is also supported.
- [.NET Core 3.0.100-preview6-012264 SDK](https://dotnet.microsoft.com/download/dotnet-core/3.0). Make sure to select the SDK download for your platform, and not the runtime download.
- Enough RAM to load database of targeted size.
- It's recommended to have at least two CPU cores.

**Note**: Due to the preview state of this release we cannot provide any guarantees, but we monitor our [GitHub: Starcounter/Home](https://github.com/Starcounter/Home/issues) issue tracker and stand ready to assist with any potential issues.

## Installation

**Note**: This section assumes that you have required operating system and .NET Core `3.0.100-preview6-012264` SDK installed.

### Binaries

- Create a folder for Starcounter binaries, for example `Starcounter.3.0.0-alpha-20190701`.
- Download [`Starcounter.3.0.0-alpha-20190701.zip`](https://starcounter.io/Starcounter/Starcounter.3.0.0-alpha-20190701.zip) into the folder.
- Unzip downloaded archive into the folder.

#### Ubuntu 18.04

##### Install prerequisites.

```
sudo apt-get install wget unzip
sudo apt-get install libaio1
```

Starcounter relies on a specific version of [SWI-Prolog](https://wwu-pi.github.io/tutorials/lectures/lsp/010_install_swi_prolog.html).

```
sudo add-apt-repository ppa:swi-prolog/stable
sudo apt-get update
sudo apt-get install swi-prolog-nox=7.\*
```

##### Download and unpack Starcounter binaries.

```
cd $HOME
mkdir Starcounter.3.0.0-alpha-20190701
cd Starcounter.3.0.0-alpha-20190701
wget https://starcounter.io/Starcounter/Starcounter.3.0.0-alpha-20190701.zip
unzip Starcounter.3.0.0-alpha-20190701.zip
```

### Application

##### Create an application folder and initialize a .NET Core console application.

```
mkdir StarcounterConsoleSample
cd StarcounterConsoleSample

dotnet new console
```

All the following commands shall be executed from the `StarcounterConsoleSample` folder.

##### Setup NuGet to consume Starcounter packages feeds.

Create `nuget.config` file and add required package sources:

- `local`, points to the Starcounter binaries folder.
- `Starcounter`, points to `https://www.myget.org/F/starcounter/api/v2`.

**NuGet tips**:

- Default `NuGet.config` file can be created with [`dotnet new nugetconfig`](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-new) command.
- For more information on how to work with NuGet configurations, see [this post](https://docs.microsoft.com/en-us/nuget/consume-packages/configuring-nuget-behavior) by Microsoft.

**End file should look similar to this**:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="local" value="[Starcounter.3.0.0-alpha-20190701]" />
    <add key="Starcounter" value="https://www.myget.org/F/starcounter/api/v2" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
```

**Note**: Replace the `[Starcounter.3.0.0-alpha-20190701]` value with actual path to the folder with unzipped Starcounter binaries.

##### Add `Starcounter.Nova.App` package reference

```
dotnet add package Starcounter.Nova.App --version 3.0.0-*
```

##### Add minimal Starcounter database access

Replace content of the `Program.cs` file with the following:

```cs
using System;
using System.Linq;
using Starcounter.Nova;
using Starcounter.Nova.Hosting;

namespace StarcounterConsoleSample
{
    [Database]
    public abstract class Person
    {
        public abstract string Name { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            using (var host = new AppHostBuilder().Build())
            {
                host.Start();

                string name = Db.Transact<string>(() =>
                {
                    var p = Db.SQL<Person>
                    (
                        "SELECT p FROM StarcounterConsoleSample.Person p WHERE Name = ?", 
                        "Jane"
                    ).FirstOrDefault();

                    if (p == null)
                    {
                        p = Db.Insert<Person>();
                        p.Name = "Jane";
                    }

                    return p.Name;
                });

                Console.WriteLine(name);
            }
        }
    }
}
```

**For ASP.NET Core application**:

Update `Startup.cs` class with the following:

```cs
using Starcounter.Nova.Hosting;
using Starcounter.Nova.Extensions.DependencyInjection;

namespace StarcounterMvcSample
{
    public class Startup
    {
        /* Some other code here. */
    
        public void ConfigureServices(IServiceCollection services)
        {
            /* Some other service declarations here. */

            services.AddStarcounter(new AppHostBuilder());
        }
    
        /* Some other code here. */
    }
}
```

**Note**: Starcounter works with Kestrel Web Server only. IIS and IIS Express are not yet supported.

##### Install Starcounter dotnet CLI tool and create database

```
dotnet new tool-manifest
dotnet tool install Starcounter.Star.Tool --version 3.0.0-*

dotnet star new
```

Now there will be a Starcounter database located in the `StarcounterConsoleSample/.stardata` folder.

##### Start database and application

Start the database.

```
dotnet star start
```

Run the application.

```
dotnet run
```

## Running with [Visual Studio Code](https://code.visualstudio.com/Download)

- Open Visual Studio Code in the application folder (from command line: `code ./`).
- Restore dependencies Visual Studio Code asks for.
- Click `Ctrl + F5` to start the application.

Everything should run out of the box.

## Running with [Visual Studio 2019](https://visualstudio.microsoft.com/vs/)

- Update Visual Studio 2019 to the latest version using Visual Studio Installer.
- We checked version 16.1.5 (as of July 3 2019).
- Open `Tools → Options` and enter "preview" in the search box.
- In `Environment → Preview Features` check `Use previews of the .NET Core SDK` and restart Visual Studio.
- Open `StarcounterConsoleSample.csproj` from Visual Studio.
- Set the debug path in `Project properties → Debug → Working directory` to the path of the project. E.g. `C:\StarcounterSamples\StarcounterConsoleSample`.
- Click `Ctrl + F5` to start the application.

It is also possible to set working directory in the `.csproj` file by adding the following:

```xml
<PropertyGroup>
  <RunWorkingDirectory>$(MSBuildProjectDirectory)</RunWorkingDirectory>
</PropertyGroup>
```

## Extra information

*Before asking questions or reporting issues, please read these few lines, and maybe you will find an answer for your question.*

- Currently there is no database tooling available except the bare minimum of `dotnet star new` and `dotnet star start` commands.
- Currently it is only possible to start the database manually.
- Currently it is only possible to store the database files in the default `.stardata` folder.
- Starting from Starcounter 3.0.0 beta, all required packages will be uploaded to one of the popular providers, such as [NuGet.org](https://www.nuget.org/), [MyGet.org](https://www.myget.org/) or [GitHub Package Registry](https://github.com/features/package-registry).
- Base namespace will be changed from `Starcounter.Nova` to just `Starcounter`.
- It is recommended to define all database classes and properties as `abstract` to reduce memory footprint when compared to `virtual`. Support for `virtual` properties might be removed in the future.
- Packing with [`dotnet pack`](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-pack) and publishing with [`dotnet publish`](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-publish) are not yet implemented for applications which use Starcounter database.
