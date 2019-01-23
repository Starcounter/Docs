# Running Background Jobs
 
## Introduction

Starcounter maintains its own thread pool. Most of Starcounter API may be called only from a thread from the Starcounter thread pool. Among such APIs are: methods from Starcounter.Db class, methods from Starcounter.Request class etc. Safer to regard all APIs from Starcounter namespace as required to be called on a Starcounter thread, unless explicitly stated otherwise. Starcounter assures that an application `Main`, typed JSON event handlers and HTTP-handlers registered with the `Handle` API are called on a Starcounter thread. In other situations, like your code is running on separately created .Net threads or on .Net thread pool, you have to schedule a task on Starcounter thread pool using `Scheduling.RunTask` in order to be able to use Starcounter API. You can call `Scheduling.RunTask` while on a .NET or Starcounter thread. `StarcounterEnvironment.IsOnScheduler` is another method that can be called on any thread, it tells you if the current thread is a Starcounter thread.

Example of running a background job and accessing database:

```csharp
using System.Threading;
using Starcounter;

namespace StarcounterSampleApp
{
    class Program
    {
        static void Main()
        {
            ThreadPool.QueueUserWorkItem(o => { RunForever(); });
        }

        static void RunForever()
        {
            while (true)
            {
                Scheduling.RunTask(async () =>
                {
                    await Db.TransactAsync(() =>
                    {
                        // Access database.
                    });
                }).Wait();
            }
        }
    }
}
```

## Basic Information About Scheduling

Tasks scheduled by `Scheduling.RunTask` as well as ready-to-call handlers make a queue of ready tasks. Then Starcounter picks tasks and runs them on a free thread from the thread pool. By default Starcounter thread pool hosts number of threads equal to number of CPU cores. It allows to maximize throughput by avoiding unnecessary context switches. But what if user code happens to block on IO or synchronization? Starcounter is able to detect such condition and launch an additional thread, so that queue processing doesn't stall. To maximize throughput, don't rely on thread pool scaling. Better to avoid blocking calls and use async IO whenever possible. This means that Starcounter tasks, scheduled by `Scheduling.RunTask` should be as light as possible, and execute only Starcounter related code (like performing access to database). All non-related to Starcounter code should preferrably be moved outside the `Scheduling.RunTask` procedure, as explained below in the example.
To put a task in a queue, the `Scheduling.RunTask` should be used:

```csharp
Task RunTask(Action action)
Task RunTask(Func<Task> action)
```

where `action` is a procedure to execute on a Starcounter thread. Task being returned is a regular .Net task object which can be used in any appropriate context as any regular .Net task.

## Task scheduling and async/await

Starcounter currently doesn't provide specialized .Net synchronization context. As a consequence if you're programming asynchronous delegate and use async/await in your code, the continuation code after await gets scheduled on a regular .Net thread pool thread, not on a Starcounter thread. It means that Starcounter API can't be directly used in a continuation. Instead, one should manually schedule continuation with `Scheduling.RunTask`. Below is an example demonstrating this. It uses delayed `Response`. Note also the use of `Db.TransactAsync` - it's needed because `Db.Transact` is also a blocking call that can increase number of threads in Starcounter thread pool.

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
    static async void DoSomethingAsync(Request req, Int32 id) {

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
        });

        // Now in continuation we need to perform some other Starcounter operations
        // therefore we need to use `Scheduling.RunTask` again.
        await Scheduling.RunTask(() => {

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

Unhandled exceptions in scheduled tasks are logged to the [Administrator log](../working-with-starcounter/administrator-web-ui.md#log). Besides of this fact, exception handling is identical to that of a regular .Net task.
