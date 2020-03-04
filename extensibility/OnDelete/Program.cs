using System;
using Microsoft.Extensions.DependencyInjection;
using Starcounter.Database;

namespace OnDeleteSample
{
    [Database]
    public abstract class Person : IDeleteAware
    {
        public abstract string Name { get; set; }

        public void OnDelete(IDatabaseContext db)
        {
            Console.WriteLine($"{Name} is about to be deleted.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            using var services = new ServiceCollection()
                .AddStarcounter($"Database=./.database/OnDeleteSample")
                .AddSingleton<MyTransactor>()
                .BuildServiceProvider();

            ITransactor transactor = services.GetRequiredService<MyTransactor>();

            transactor.Transact(db =>
            {
                var per = db.Insert<Person>();
                per.Name = "Per";

                db.Delete(per);
            });

            // Output:
            // Per is about to be deleted.
        }
    }
}