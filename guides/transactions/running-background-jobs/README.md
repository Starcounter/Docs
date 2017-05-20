# Running background jobs

This article describes useful patterns to run background jobs in Starcounter applications which require access to the database. In regards to the current implementation in Starcounter 2.x, it is important to keep in mind how transactions and deleting of objects work and thus avoid jobs that run for a long time, or possible forever.

When you perform reads from the database, an implicit read-only transaction is created for you carefully by Starcounter. I.e., ```Main``` of Starcounter application, delegates passed to ```Scheduling.ScheduleTask``` and URI handling delegates are all wrapped into such an implicit read-only transaction. The transaction allows you to do reads from the database. If a job inside that transaction is running forever, deleted objects will only be marked as deleted but not removed from the database until the codehost is restarted or until the delegate is done and some other transaction performs a commit afterwards. This can have implications of space (on disc) needed for the image-files since they will continue to grow even if objects are deleted.

What it means practically is that spawning background tasks by running an infinite loop with a timer inside ```Scheduling.ScheduleTask```, URI-handling delegates and ```Main``` isn't a good idea. This is not only preventing object from purging and will steal computing resources, but is also considered a bad pattern of a background job in general. The correct pattern uses inversion of control, when a timer object invokes the action, and is explained below. The main point is to keep transaction scopes short.

## Long-running tasks/threads and Starcounter

There is often a case when long running threads are needed, for example, timer jobs, statistics gathering, external information retrieval, status information update, etc. For this matter the following pattern is recommended: all long-running tasks should be inside standard .NET threads, not Starcounter-related. However, inside these threads, whenever Starcounter operation should be performed (database access, operation on the session, etc.) a special Starcounter task should be scheduled. It depends if you want to run the database operation and wait for the result or just schedule a database operation that will be performed as soon as scheduler grabs the task: thus the synchronous parameter in the `Scheduling.ScheduleTask()` (read below for more information). So the general rule is: Starcounter schedulers are limited resources and thus should run only short tasks. If something long-running can be done outside Starcounter schedulers - it should run as a standard .NET tasks/threads and not occupy the scheduler. Below is more information on Starcounter schedulers and tasks.

## Basic information about scheduling in Starcounter

Each Starcounter scheduler has a queue of tasks that are supposed to be run on this scheduler. Tasks are picked from the queue and executed. To put a task in a queue, the `Scheduling.ScheduleTask` should be used. When scheduling a task, you can specify the scheduler number, and if the thread should wait for the task to be picked by scheduler and completed. Here is the signature of the `Scheduling.ScheduleTask`:

```csharp
void ScheduleTask(
	Action action,
	Boolean waitForCompletion = false,
	Byte schedulerId = StarcounterEnvironment.InvalidSchedulerId)
```
where:
* `Action action`: procedure to execute on scheduler.
* `Boolean waitForCompletion = false`: optional flag to wait for the `action` to complete synchronously.
* `Byte schedulerId = StarcounterEnvironment.InvalidSchedulerId`: optional parameter to select the scheduler, on which the `action` is going to run. This parameter is for advanced usage.

To determine if current thread is on scheduler call `StarcounterEnvironment.IsOnScheduler()`. To determine the amount of schedulers in your database call `StarcounterEnvironment.SchedulerCount`. To get current scheduler id call `StarcounterEnvironment.CurrentSchedulerId` (in case if calling thread is not on Starcounter scheduler the value `StarcounterEnvironment.InvalidSchedulerId` is returned).

*NOTE*: `DbSession` interface is still usable but not obsolete.

## Using a timer

Running a short-lived job using some timer. In this example the .Net class `System.Timers.Timer` is used.

The following sample will execute a job every minute and do needed database operations and then exit, until the timer trigger again.

The same scheduler is used in these samples (scheduler 0) for simplicity but a better solution might be to schedule jobs on all available schedulers.

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
			Scheduling.ScheduleTask(() =>
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

## Running a separate (non-database) thread

Running a separate (non-database) thread that regularly schedules jobs that access database instead of using a timer will work as solution for the first problem, deleting and purging objects, but have another issue with shutting down the codehost. This is due to lack of event that usercode can listen to when codehost is terminating.

```cs
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
				Scheduling.ScheduleTask(() => // Schedule a job on scheduler 0
				{
					Db.Transact(() =>
					{
							// Access database.
					});
					arEvent.Set(); // Signal job complete.
				}, false);

				System.Threading.Thread.Sleep(1000);
				arEvent.WaitOne(); // Wait for the current job to finish
			}
		}
	}
}
```

**Note:** using `thread.Start()` instead of `ThreadPool.QueueUserWorkItem` will lead to the following entries in the Starcounter log and the shutdown will take longer time.

```cs
Thread foreverThread = new Thread(new ThreadStart(RunForever));
foreverThread.Start();
```

> 20150928T084607 Warning sc://chrhol-pc/personal Starcounter.Server - User code process takes longer than expected to exit. (, PID=1271188, Database=default)

And then, finally:

> 20150928T084622 Error sc://chrhol-pc/personal Starcounter.Server - ScErrCodeHostProcessNotExited (SCERR10018): When asked to shut down, the user code process agreed to shut down, but the process didn't exit gracefully in time. Killing it.. (, PID=1271188, Database=default)\r\nVersion: 2.0.0.0.\r\nHelp page: https://github.com/Starcounter/Starcounter/wiki/SCERR10018.
