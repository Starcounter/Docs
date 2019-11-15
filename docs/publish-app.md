# Publishing Starcounter .NET Core app 

Starcounter supports the following runtimes:

- [`win7-x64`](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog#windows-rids) - for Windows 7 and newer versions.
- [`linux-x64`](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog#linux-rids) - for Linux based operating systems.

Self contained and framework dependent modes are supported.

*Detailed specification of the [`dotnet publish`]((https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-publish?tabs=netcore21)) command is available at the Microsoft website.*

### Single file deployment

Unfortunately, due to [this .NET Core SDK](https://github.com/dotnet/sdk/issues/3510) issue it is not yet possible to publish Starcounter .NET Core apps as a single file.

This would not work:

```
dotnet publish -r win7-x64 -c Release /p:PublishSingleFile=true
```