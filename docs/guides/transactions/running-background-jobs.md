# Running Background Jobs
 
## Introduction

Code that is provded via Starcounter API (transactions, SQL, request/response handling, Web sockets handling, etc.) should execute in a special context. Such context is already given inside the `Main()` function, view-model handles and handlers (for example, `Handle.GET`). In other situations, like in separately created background .NET threads or tasks, to be able to execute Starcounter code, one should schedule it using `Scheduling.RunTask()`.

Example of running a background job and accessing database periodically:

```csharp
using System.Threading;
using Starcounter;

namespace StarcounterApplication4
{
    class Program
    {
        private static AutoResetEvent arEvent;

        static void Main()
        {
            arEvent = new AutoResetEvent(false);
            ThreadPool.QueueUserWorkItem(o => { RunForever(); });
        }

        static void RunForever()
        {
            while (true)
            {
                Scheduling.RunTask(() => // Schedule a job on scheduler 0
                {
                    Db.Transact(() =>
                    {
                            // Access database.
                    });
                    arEvent.Set(); // Signal job complete.
                });

                System.Threading.Thread.Sleep(1000);
                arEvent.WaitOne(); // Wait for the current job to finish
            }
        }
    }
}
```

## Basic Information About Scheduling

Each Starcounter scheduler has a queue of tasks that are supposed to be run on this scheduler. Tasks are picked from the queue and executed. To put a task in a queue, the `Scheduling.RunTask` should be used. When scheduling a task, you can specify the scheduler number, and if the thread should wait for the task to be picked by scheduler and completed. Here is the signature of the `Scheduling.RunTask`:

```csharp
Task RunTask(
    Action action,
    Byte schedulerId = StarcounterEnvironment.InvalidSchedulerId)
```

where:

* `Action action`: procedure to execute on scheduler.
* `Byte schedulerId = StarcounterEnvironment.InvalidSchedulerId`: optional parameter to select the scheduler, on which the `action` is going to run.

To make the `Action` execute synchronously, use the `Wait` method:

```csharp
Scheduling.RunTask(() => { }).Wait()
```

To determine if current thread is on scheduler call `StarcounterEnvironment.IsOnScheduler()`. To determine the amount of schedulers in your database call `StarcounterEnvironment.SchedulerCount`. To get current scheduler id call `StarcounterEnvironment.CurrentSchedulerId` \(in case if calling thread is not on Starcounter scheduler the value `StarcounterEnvironment.InvalidSchedulerId` is returned\).

## Task scheduling and async/await

Async/await programming in Starcounter is the same as in standard .NET. The only difference is that if there is a Starcounter-related code (SQL, transactions, etc.) that comes after the `await` statement - such code should be wrapped into a delegate given to `Scheduling.RunTask`. Below is an example demonstrating this. It uses delayed `Response`. Note also the use of `Db.TransactAsync` - it's needed so that the Starcounter scheduler does not create additional threads.

```csharp
[Database]
public class Person
{
    public String Id { get; set; }
}
class Program
{

    static void Main()
    {

        Handle.GET("/hello/{?}", (Request req, Int32 id) => {

            // As we are on Starcounter thread already, we can create a database object.
            // Notice that we use `Db.TransactAsync` here. Read about it in a separate article.
            Db.TransactAsync(() => new Person() { Id = id.ToString() });

            // Now we run an async task that will return the response later.
            DoSomethingAsync(req, id);

            // Indicating that HTTP response will be returned later.
            return HandlerStatus.Handled;
        });
    }

    // Async call that does some await and performs database operations after.
    static async void DoSomethingAsync(Request req, Int32 id)
    {

        // Now using async to simulate some long operation using await statement.
        await Task.Delay(10000);

        // Since this is not a Starcounter thread now, so we need to schedule
        // a Starcounter task so we can do our database operations.
        await Scheduling.RunTask(() => {

            // Querying the person.
            var person = Db.SQL<Person>("SELECT p FROM Person p WHERE p.Id = ?", id.ToString()).FirstOrDefault();

            // Doing some database operations.
            // Notice that we use `Db.TransactAsync` here. Read about it in a separate article.
            Db.TransactAsync(() => person.Id += id);

            // Finally returning the response.
            Response resp = new Response() {
                Body = "Done with object " + id
            };

            req.SendResponse(resp);
        });
    }
}
```

## Exceptions in scheduled tasks

Exceptions in scheduled tasks are logged to the [Administrator log](../working-with-starcounter/administrator-web-ui.md#log). For example, the following code will return "No exception":

```csharp
Handle.GET("/Hello", () =>
{
    try
    {
        Scheduling.RunTask(() => throw new Exception());
        return "No exception";
    }
    catch
    {
        return "Exception";
    }
});
```

This is logged to the console:

```text
System.Exception: Exception of type 'System.Exception' was thrown.
   at StarcounterApplication1.Program.<>c.<Main>b__0_1() in C:\Users\User\source\
epos\StarcounterApplication1\StarcounterApplication1\Program.cs:line 15
   at Starcounter.DbSession.<>c__DisplayClass5_0.<RunAsync>b__0() in C:\TeamCity\BuildAgent\work\sc-11226\Level1\src\Starcounter\DbSession.cs:line 216
HResult=-2146233088
```

If you wait for the `Task`, the exception will be brought to the waiting thread and logged. For example, the following code will return "Exception" and the same exception as above will be logged:

```csharp
Handle.GET("/Hello", () =>
{
    try
    {
        Scheduling.RunTask(() => throw new Exception()).Wait();
        return "No exception";
    }
    catch
    {
        return "Exception";
    }
});
```

