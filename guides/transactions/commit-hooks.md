# Commit hooks

Commit hook is a logic flow control pattern similar to [trigger](https://en.wikipedia.org/wiki/Trigger) in relational databases. It enables to hook the [CRUD](https://en.wikipedia.org/wiki/Create,_read,_update_and_delete) events per objects of particular class. For cases when an object is being created (with a ```new``` operator), updated (by writing to a field) and deleted (when the ```.Delete()``` is called, and after the committed delete), additional event handlers of code might be added for execution.

The app ```TestHooks``` shows a full set of use cases for commit hooks.

{% raw %}
```cs
using System;
using Starcounter;

namespace TestHooks
{
   [Database]
   public class Hooked
   {
      public string state;
   }

   [Database]
   public class YetAnotherClass
   {
      public int Stock;
   }

   class Program {
      static void Main()
      {
         Hook<Hooked>.BeforeDelete += (s, obj) =>
         {
            obj.state = "is about to be deleted";
            Console.WriteLine("Hooked: Object {0} is to be deleted", obj.GetObjectNo());
         };

         Hook<Hooked>.CommitInsert += (s, obj) =>
         {
            obj.state = "is created";
            Console.WriteLine("Hooked: Object {0} is created", obj.GetObjectNo());
            var nobj = new YetAnotherClass() { Stock = 42 };
         };

         Hook<Hooked>.CommitUpdate += (s, obj) =>
         {
            obj.state = "is updated";
            Console.WriteLine("Hooked: Object {0} is updated", obj.GetObjectNo());
         };

         Hook<Hooked>.CommitUpdate += (s, obj) => // a second callback
         {
            Console.WriteLine("Hooked: We promise you, object {0} is updated", obj.GetObjectNo());
         };

         Hook<Hooked>.CommitDelete += (s, onum) =>
         {
            Console.WriteLine("Hooked: Object {0} is deleted", onum);
            Hooked rp = (Hooked)DbHelper.FromID(onum); // returns null here
            // the following will cause an exception
            // Console.WriteLine("We cannot do like this: {0}", rp.state);
         };

         Hook<YetAnotherClass>.CommitInsert += (s, obj) =>
         {
            Console.WriteLine("Never triggered in this app, since it happens to get invoked inside another hook");
         };

         Hooked p = null;
         Db.Transact(() =>
         {
           p = new Hooked() { state = "created" };
         });

         Db.Transact(() =>
         {
            p.state = "property changed";
            Console.WriteLine("01: The changed object isn't yet commited", p.GetObjectNo());
         });

         Console.WriteLine("02: Change for property of {0} is committed", p.GetObjectNo());

         Db.Transact(() =>
         {
            Console.WriteLine("03: We have entered the transaction scope");
            Console.WriteLine("04: We are about to delete an object {0}, yet it still exists", p.GetObjectNo());
            p.state = "deleted";
            p.Delete();
            Console.WriteLine("05: The deleted object {0} is no longer be available", p.GetObjectNo());
            Console.WriteLine("06: Were are about to commit the deletion");
         });
         Console.WriteLine("07: Deletion is committed");
      }
   }
}
```
{% endraw %}

The output produced is as follows (accurate to [ObjectNo](/guides/database/object-identity-and-object-references.html)):

```
Hooked: Object 29 is created
01: The changed object isn't yet commited
Hooked: Object 29 is updated
Hooked: We promise you, object 29 is updated
02: Change for property of 29 is committed
03: We have entered the transaction scope
04: We are about to delete an object 29, yet it still exists
Hooked: Object 29 is to be deleted
05: The deleted object 29 is no longer be available
06: Were are about to commit the deletion
Hooked: Object 29 is deleted
07: Deletion is committed
```

Those familiar with .NET recognize Starcounter follows a convention of .NET [EventHandler<T>](https://msdn.microsoft.com/en-us/library/db0etb8x(v=vs.110).aspx) for commit hooks. Currently, the first argument of the callback isn't used. The second argument is a reference to an object being transacted (for create, update and pre-delete events) or an ```ObjectNo``` of the object which itself is already deleted (for post-delete event). As in the .NET convention one can have an arbitrary number of event handlers registered per event, which will be triggered in the order of registration on the event occurrence.

__Why there are separate pre-delete (```BeforeDelete```) and post-delete (```CommitDelete```) hooks?__ Remember that after object is physically deleted in the end of a successful transaction scope, you can no longer access it in a post-delete commit hook delegate. However you might still want to do something meaningful with it just around the moment of deletion. That is why the pre-delete hook is introduced. Note that a pre-delete hook triggers callback inside the transaction scope, but not in the end of transaction. It means that, in case a transaction has been [retried ```N``` times](/guides/transactions/more-on-transactions.html), any pre-delete hook for any object deleted inside this transaction will also be executed ```N``` times, while all other hooks will be executed exactly once, right after a successful transaction commit. Thus, consider pre-delete hook behaving as a transaction side-effect.

__In general, in situations where you can choose, we recommend to avoid using commit hooks__. They introduce non-linear flows in the logic, hence producing more complicated and less maintainable code. Commit hooks is a powerful tool that should only be used in situations where benefits of using them overweight the drawbacks. One popular example is separate logging of changes in objects of selected classes.

__Can I do DB operations inside commit hooks?__ The answer is "Yes", since all commit hooks relate to write operations (create/update/delete), thus there must always be a transaction spanning these operations, and all event handlers are run inside this transaction. For example, in ```TestHooks``` we create an instance of a class ```YetAnotherClass``` inside ```CommitInsert```, but do not introduce a transaction scope around this line. The reason being for it is that there is already a transaction from ```Main``` which spans this call.

__Notes.__

1. It is currently not possible to detach commit hook event handlers.

2. CRUD operations introduced inside a hook are not triggering additional hooks. For instance, in ```TestHooks``` the insert hook for ```YetAnotherClass``` is never invoked, because the only place for it triggered is in ```CommitInsert```, which is itself a commit hook.
