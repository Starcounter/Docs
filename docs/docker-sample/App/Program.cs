using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System;
using Starcounter.Database;

namespace App
{
    /// <summary>
    /// Here is our database table. It contains people. With names.
    /// </summary>
    [Database]
    public abstract class Person
    {
        public abstract string Name { get; set; }
    }
    
    /// <summary>
    /// This simple console application demonstrates how to build a service provider
    /// for the Starcounter services, to fetch service instances from it, and then 
    /// how to use the services to make basic database transactions with basic database 
    /// interactions like SQL queries and inserts.
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            string name = args.FirstOrDefault() ?? "Noname";
            
            // Here we create a service collection that we add the Starcounter services to.
            // When we call BuildServiceProvider(), we get an instance that we can use to 
            // fetch service instances, for example ITransactor, which we then can use to 
            // to make database transactions.
            using var services = new ServiceCollection()
                .AddStarcounter("Database=./.database/ConsoleApp;OpenMode=CreateIfNotExists;StartMode=StartIfNotRunning;StopMode=IfWeStarted")
                .BuildServiceProvider();

            // Here we fetch our ITransactor instance from the service provider.
            var transactor = services.GetRequiredService<ITransactor>();

            // And here we use it to make a database transaction.
            ulong oid = transactor.Transact(db =>
            {
                // Here inside the transaction, we can use the IDatabaseContext instance to
                // interact with the Starcounter database.

                // We can query it using SQL (which returns an IEnumerable<Person> that we can
                // use with LINQ).
                var p = db.Sql<Person>("SELECT p FROM App.Person p WHERE Name = ?", name).FirstOrDefault();

                if (p == null)
                {
                    // We can insert new rows in the database using Insert().
                    p = db.Insert<Person>();

                    // We write to the database row using standard C# property accessors.
                    p.Name = name;
                }

                // Let's return the object id as result of the transaction.
                return db.GetOid(p);
            });

            // And let's print it in the console.
            Console.WriteLine($"{oid}: {name}");
        }
    }
}
