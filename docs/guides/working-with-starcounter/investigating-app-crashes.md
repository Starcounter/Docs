# Investigating App Crashes

## Introduction

Recall how Starcounter works: it loads your whole application inside Starcounter host process. Hence, if your application has a bug \(in its code, in one of 3-rd party components you use etc\), the bug will crash Starcounter process. This crash shall not corrupt any data, as the data manager works in a different process and the database is inherently designed to survive software and hardware failures. In case of exception, Starcounter uses "let it crash and die" principle: thus, we do not intercept the exception from user code into Starcounter host process, and let the process die in case of exception. This also means that Starcounter will not leave a record in its log in case of your application's exception. However, when your code crashes Starcounter host, you want to get max out of the crash to debug your app. In order to achieve that, you need to tune your OS in the way explained below.

## Collecting Crash Dumps

If your application crashed:

1. Find event with `event_id = 1000` in Windows application log reporting crash of `sccode.exe`. If the entry exists, you should enable Windows local crash dump collection.
2. To enable Windows local crash dump collection, create a registry key `HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\Windows Error Reporting\LocalDumps\@DumpType = 2` and reproduce a crash. To finetune crash dumps collection, follow this article from Microsoft: [Collecting User-Mode Dumps](https://msdn.microsoft.com/en-us/library/bb787181.aspx).
3. On the next time of crash, collect the dumps at `%LOCALAPPDATA%\CrashDumps`.

