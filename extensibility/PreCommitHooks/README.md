# Overview
Illustrates a way to implement *pre-commit hooking* in an application using the Starcounter database.

## Usage
First copy sources from `..\Starcounter.Database.Hosting.Extensibility` to your application. We suggest you put them in a folder with the same name.

Then copy `MyTransactor.cs` and `PreCommitHookOptions.cs` to your application.

Now you can configure `PreCommitHookOptions` and register callbacks in your application, that will get invoked when objects of specified types is either inserted or updated.

## Sample application
Look at [`Program.cs`](./Program.cs) to see how its used.