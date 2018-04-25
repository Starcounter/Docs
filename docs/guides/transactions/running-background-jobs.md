# Running Background Jobs
 
## Introduction

This article describes useful patterns to run background jobs in Starcounter applications which require access to the database. In regards to the current implementation in Starcounter 2.x, it is important to keep in mind how transactions and deleting of objects work and thus avoid jobs that run for a long time, or possible forever.

When you perform reads from the database, an implicit read-only transaction is created for you carefully by Starcounter. I.e., `Main` of Starcounter application, delegates passed to `Scheduling.RunTask` and URI handling delegates are all wrapped into such an implicit read-only transaction. The transaction allows you to do reads from the database. If a job inside that transaction is running forever, deleted objects will only be marked as deleted but not removed from the database until the code-host is restarted or until the delegate is done and some other transaction performs a commit afterwards. This can have implications of space \(on disc\) needed for the image-files since they will continue to grow even if objects are deleted.

What it means practically is that spawning background tasks by running an infinite loop with a timer inside `Scheduling.RunTask`, URI-handling delegates and `Main` isn't a good idea. This is not only preventing object from purging and will steal computing resources, but is also considered a bad pattern of a background job in general. The correct pattern uses inversion of control, when a timer object invokes the action, and is explained below. The main point is to keep transaction scopes short.

## Long-running Threads

There is often a case when long running threads are needed, for example, timer jobs, statistics gathering, external information retrieval, status information update, etc. For this matter the following pattern is recommended: all long-running tasks should be inside standard .NET threads, not Starcounter-related. However, inside these threads, whenever Starcounter operation should be performed \(database access, operation on the session, etc.\) a special Starcounter task should be scheduled. It depends if you want to run the database operation and wait for the result or just schedule a database operation that will be performed as soon as scheduler grabs the task: thus the synchronous parameter in the `Scheduling.RunTask()` \(read below for more information\). So the general rule is: Starcounter schedulers are limited resources and thus should run only short tasks. If something long-running can be done outside Starcounter schedulers - it should run as a standard .NET tasks/threads and not occupy the scheduler. Below is more information on Starcounter schedulers and tasks.

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

## Using a Timer

Running a short-lived job using some timer. In this example the .Net class `System.Timers.Timer` is used.

The following sample will execute a job every minute and do needed database operations and then exit, until the timer trigger again.

The same scheduler is used in these samples \(scheduler 0\) for simplicity but a better solution might be to schedule jobs on all available schedulers.

```csharp
using System;
using System.Timers;
using Starcounter;

namespace TimerSample
{
    class Program
    {
        private static Timer timer; // keep it to avoid the timer being GC:ed

        static void Main()
        {
            timer = new Timer(60 * 1000); // 1 minute interval
            timer.AutoReset = true;
            timer.Elapsed += OnTimer;
            timer.Start();
        }

        static void OnTimer(object sender, ElapsedEventArgs e)
        {
            // Schedule a job on scheduler 0 without waiting for its completion.
            Scheduling.RunTask(() =>
            {
                Db.Transact(() =>
                {
                        // Access database.
                }, false, 0);
            });
        }
    }
}
```

## Running a Separate Non-Database Thread

Running a separate \(non-database\) thread that regularly schedules jobs that access database instead of using a timer will work as solution for the first problem, deleting and purging objects, but have another issue with shutting down the codehost. This is due to lack of event that usercode can listen to when codehost is terminating.

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

**Note:** using `thread.Start()` instead of `ThreadPool.QueueUserWorkItem` will lead to the following entries in the Starcounter log and the shutdown will take longer time.

```csharp
Thread foreverThread = new Thread(new ThreadStart(RunForever));
foreverThread.Start();
```

> 20150928T084607 Warning sc://chrhol-pc/personal Starcounter.Server - User code process takes longer than expected to exit. \(, PID=1271188, Database=default\)

And then, finally:

> 20150928T084622 Error sc://chrhol-pc/personal Starcounter.Server - ScErrCodeHostProcessNotExited \(SCERR10018\): When asked to shut down, the user code process agreed to shut down, but the process didn't exit gracefully in time. Killing it.. \(, PID=1271188, Database=default\)\r\nVersion: 2.0.0.0.\r\nHelp page: [https://github.com/Starcounter/Starcounter/wiki/SCERR10018](https://github.com/Starcounter/Starcounter/wiki/SCERR10018).



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
   at StarcounterApplication1.Program.<>c.<Main>b__0_1() in C:\Users\User\source\epos\StarcounterApplication1\StarcounterApplication1\Program.cs:line 15
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

## Task scheduling and async/await

Async/await programming in Starcounter is the same as in standard .NET. The only difference is that if there is a Starcounter-related code (SQL, transactions, etc.) that comes after the `await` statement - such code should be wrapped into a delegate given to `Scheduling.RunTask`. Below is an example demonstrating this. It uses delayed `Response`. Note also the use of `Db.TransactAsync` - its needed so that Starcounter scheduler is not creating additional threads.

```csharp
[Database]
public class Person {
    public String Name { get; set; }
}
class Program {

    static void Main() {

        Handle.GET("/hello/{?}", (Request req, Int32 id) => {

            // As we are on Starcounter thread already, we can create a database object.
            // Notice that we use `Db.TransactAsync` here. Read about it in a separate article.
            Db.TransactAsync(() => new Person() { Name = id.ToString() });

            // Now we run an async task that will return the response later.
            DoSomethingAsync(req, id);

            // Indicating that HTTP response will be returned later.
            return HandlerStatus.Handled;
        });
    }

    // Async call that does some await and performs database operations after.
    static async void DoSomethingAsync(Request req, Int32 id) {

        // Now using async Simulating some long operation using await statement.
        await Task.Delay(10000);

        // Since this is not a Starcounter thread now, so we need to schedule
        // a Starcounter task so we can do our database operations.
        await Scheduling.RunTask(() => {

            // Quering the person.
            var person = Db.SQL<Person>("SELECT p FROM Person p WHERE p.Name = ?", id.ToString()).FirstOrDefault();

            // Doing some database operations.
            // Notice that we use `Db.TransactAsync` here. Read about it in a separate article.
            Db.TransactAsync(() => person.Name += id);

            // Finally returning the response.
            Response resp = new Response() {
                Body = "Done with object " + id
            };

            req.SendResponse(resp);
        });
    }
}
```



