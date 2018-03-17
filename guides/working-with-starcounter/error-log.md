# Error Log

## Introduction

The Starcounter **error log** contains detailed **information about warnings and errors**. Whenever an error occurs, and whenever a component of Starcounter needs to issue a warning or a notice, a detailed message with the relevant severity is written to the error log.

## How to Access the Error Log

### Using Administrator Web UI

You can access the error log via the Administrator Web UI. To view the error log on your machine:

1. Open a browser
2. Navigate to [http://localhost:8181/\#/server/log](http://localhost:8181/#/server/log).

### Using Command-Line Tools

There are two ways to view the content of the log using the command-line tools: via `staradmin.exe` and via `star.exe`.

### Using StarAdmin CLI

For greatest flexibility, use `staradmin.exe`. Type `staradmin list --max=50 log` to see the 50 most recent log entries.

By default, `staradmin list log` will show you the 25 latest logged entries with severity `Notice` and higher \(i.e. including `Warning` and `Error`, but not `Debug`\).

Examples. To see,

1. all entries \(`Notice` and up\), type `staradmin list --max=all log`.
2. the 100 latest entries, including `Debug`, type `staradmin log --max=100 debug`.
3. all entries \(`Notice` and up\), type `staradmin list --max=all log`.
4. the 100 latest entries, including `Debug`, type `staradmin list --max=100 log debug`.
5. all errors, type `staradmin list --max=all log errors`
6. the 20 latest errors and warnings, type `staradmin list --max=20 log warnings`.
7. the 50 latest entries, independent of their severity, logged by the "Starcounter" log source, type `staradmin --max=50 list log all Starcounter`.

Finally, entries displayed by `staradmin list log` can be filtered by a named database by applying the global `-d` option. For example, to see the 10 last entries from the `Starcounter.Host` source, logging from within the `default` database, type `staradmin -d=default list --max=10 log all Starcounter.Host`.

### Using star CLI

You can instruct `star.exe` to display log entries scoped to the operation star executes, for example starting an application. You do this using the new **--logs** option.

Typing `star --logs app.exe` will first start "app.exe" and then output all log entries that was written to the log from the time when the command was invoked. By default, logs with `Notice` and up is displayed. To see debug logs too, instead type `star --verbose --logs app.exe`.

### Viewing the Raw File

You can also see the direct content of the log by browsing the file content on your hard drive. To do this,

1. Locate the root server directory of your Starcounter installation. The default path for the personal server is: `C:\Users\[YourName]\Documents\Starcounter\Personal`.
   * In older versions of Starcounter, it was: `C:\Users\[YourName]\Documents\Starcounter\[Version]\Personal`
2. Open the `Logs` subdirectory.

The log data is in one or more files named with the convention `Starcounter.[nnnnnnnnnn].log` where \[nnnnnnnnnn\] is an opaque sequence number used by Starcounter. If you see several files, the one with the highest number contains the most recent entries.

## Writing to the Error Log

Not only Starcounter components can write to the log. Applications running in Starcounter can do so too. Writing to the log is done using the `LogSource` class, part of the `Starcounter.Logging` namespace.

```csharp
using Starcounter;
using Starcounter.Logging;

class Program
{
    static void Main()
    {
        new LogSource("PerSamuelsson").LogWarning("I dont do any good!");
    }
}
```

If you run the above application using `star app.cs`, viewing the logged entry can be done with the `staradmin list log` command, using `staradmin list log all PerSamuelsson`.

The `LogSource` class has support for logging errors \(critical, text, and from exceptions\), warnings and notices. Invoking any of the corresponding `LogSource` methods, such as `LogWarning` above, will assure the message ends up in the log.

### Diagnostic Logging

In addition to errors, warnings and notices, Starcounter also allows diagnostic logging using the `Debug` and `Trace` methods respectively. Logging using the `Debug` method will only be available in Starcounter versions built with the DEBUG configuration. Similarly, logging using the `Trace` method will only be available in Starcounter versions built with the TRACE configuration.

### Conditional Low-Level Trace Logging

Various Starcounter components also support low-level diagnostics in Starcounter TRACE builds by enabling _trace logging_. With trace logging turned on, trace messages emitted by the Starcounter runtime is routed to the log using the `Debug` severity. Trace logging is an experimental feature and should not be considered future compatible. It is driven by an environment variable, `SC_ENABLE_TRACE_LOGGING`. To set this flag and thereby effectively enable trace logging for a set of Starcounter components, make sure all Starcounter processes are stopped and apply the `--tracelogging` flag to `scservice.exe`.

```csharp
staradmin kill all
start scservice --tracelogging
```

