# Star CLI


## Introduction

The `star` command line interface \(CLI\) handles tasks in a database. This page covers the most important commands. The rest of the commands can be found with `star --help` or `star --helpextended`.

## Starcounter version

The version of the current Starcounter installation is displayed with `star --version` or the shorthand `star -v`.

```bash
> star --version
Version=2.3.1.7018
```

## Starting apps

Apps are started with `star [app]` by specifying the path to the `exe` file. For example:

```bash
~\Apps\HelloWorld> star .\src\HelloWorld\bin\Debug\HelloWorld
```

### Set resource directory

When starting an app, Starcounter will fail to find the application resource directory \(`wwwroot`\) if the current working directory is not the root of the project or the same directory as the resource directory. This will, for apps that use the Starcounter web stack presented in [Blendable Web Apps](../blendable-web-apps/), throw, `ArgumentOutOfRangeException` when opening the app.

To solve this, specify the resource directory with the `--resourcedir` option:

```bash
~\Apps\HelloWorld\src\HelloWorld\bin\Debug> star --resourcedir=../../wwwroot HelloWorld
HelloWorld -> default (started, default port 8080, admin 8181)
```

Read more about this on the page [Static File Server](../network/static-file-server.md).

### Specify database

By default, apps are started in the `default` database. To start apps in another database, use the `--database` option:

```bash
> staradmin new db myDatabase
Created (Name=myDatabase)
> star --database=myDatabase HelloWorld
HelloWorld -> mydatabase (started, default port 8080, admin 8181)
```

The shorthand notation is `-d`: `star -d=myDatabase HelloWorld`.

### Change app name

To start an app with another name than the existing one, use the `--name` option:

```bash
> star --name=HelloUniverse HelloWorld
HelloUniverse -> default (started, default port 8080, admin 8181)
```

### Passing custom arguments

You can pass custom arguments when starting apps with `star`:

```bash
> star app.exe argument1 argument2
```

```csharp
class Program 
{
  static void Main(string[] args) 
  {
    Console.WriteLine(args.Length); // => 2
    Console.WriteLine(args[0]); // => argument1
  }
}
```

Custom arguments starting with hyphens will throw `ScErrBadCommandLineSyntax (SCERR1031)`. To prevent this, prefix the argument with any other character:

```text
> star app.exe \-argument1
```

Arguments enclosed in quotation marks will be evaluated as one argument.

The arguments can also be passed from Visual Studio as described on [Starting and stopping apps](starting-and-stopping-apps.md#specifying-options-in-visual-studio).

## Stopping apps

Apps are stopped with the `--stop` option:

```bash
> star HelloWorld
HelloWorld -> default (started, default port 8080, admin 8181)
> star --stop HelloWorld
HelloWorld <- default (stopped)
```

Read [Starting and Stopping Apps](starting-and-stopping-apps.md) for more information.

