# Overview
Illustrates a way to support an `IDeleteAware` interface to be implemented by database classes, to recieve notifications when instances of the type is deleted.

## Usage
First copy sources from `..\Starcounter.Database.Hosting.Extensibility` to your application. We suggest you put them in a folder with the same name.

Then copy `MyTransactor.cs` and `IDeleteAware.cs` to your application.

Now you can implement `IDeleteAware.OnDelete` on your database types that need to take action when an instance of the type is deleted.

## Sample application
Look at [`Program.cs`](./Program.cs) to see how its used.