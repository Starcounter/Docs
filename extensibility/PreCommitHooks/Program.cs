using System;
using Microsoft.Extensions.DependencyInjection;
using Starcounter.Database;

namespace PreCommitHooksSample
{
    [Database]
    public abstract class Person
    {
        public abstract string Name { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            using var services = new ServiceCollection()
                .AddStarcounter($"Database=./.database/PreCommitHooksSample")
                .Configure<PreCommitHookOptions>(o => 
                {
                    // Here we register a delegate that will be called when an
                    // object of type Person is either inserted or updated. The
                    // delegate will execute as part of the same transaction that
                    // executed the change.
                    o.Hook<Person>((db, change) =>
                    {
                        Console.WriteLine($"{change.ChangeType} of person with id {change.Id}.");
                    });
                })
                .AddSingleton<MyTransactor>()
                .BuildServiceProvider();

            ITransactor transactor = services.GetRequiredService<MyTransactor>();

            // Execute 2 transactions. The first one inserts a person,
            // and returns the unique identity of the person. The second
            // one updates the person created in the first.

            var id = transactor.Transact(db =>
            {
                var per = db.Insert<Person>();
                per.Name = "Per";
                return db.GetOid(per);
            });

            transactor.Transact(db =>
            {
                var per = db.Get<Person>(id);
                per.Name = "Per Samuelsson";
            });

            // Output:
            // Insert of person with id 1
            // Update of person with id 1
        }
    }
}