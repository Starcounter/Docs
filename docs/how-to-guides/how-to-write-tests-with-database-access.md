# How to write tests with database access

## Goal

Create a test project that lets you write unit tests that uses the database.

## Steps

### 1. Create a new test project with references

#### Add project

Start by creating a test with `File -> New... -> Project` and choose `Visual C# -> Test -> Unit Test Project`.

#### Add XUnit

To add XUnit to the project and make it possible to run the tests in Visual Studio, add the packages [xunit](https://www.nuget.org/packages/xunit/2.3.0) and [xunit.runner.visualstudio](https://www.nuget.org/packages/xunit.runner.visualstudio/).

For more information, see the [XUnit docs](https://xunit.github.io/docs/getting-started-desktop.html).

#### Reference project to test

Add a reference to the project you want to test by right-clicking on References in the test project and choose `Add Reference...`. In the Projects tab, select the project to test.

#### Add Starcounter

Four Starcounter references should be added:

1. `Starcounter`
2. `Starcounter.Hosting`
3. `Starcounter.Internal`
4. `Starcounter.XSON`

These can be found by searching for "Starcounter" in the Assemblies tab of the Reference Manager.

#### Set up the test environment

Go to project properties and set Platform target to x64. Before you run tests for the first time, set default processor architecture: `Test -> Test Settings -> Default Processor Architecture -> x64`

###  2. Add utility to recreate database

To ensure that the tests run under the same condition every time, the test database should be recreated before every run. For that to happen, we need to be able to manage the server, host, and database. We'll do that by calling the [staradmin CLI](../topic-guides/working-with-starcounter/staradmin-cli.md):

{% code-tabs %}
{% code-tabs-item title="TestSetup.cs" %}
```csharp
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace UnitTestDemo.Tests
{
    public class TestSetup
    {
        readonly string databaseName;

        // Exit codes we might want to explicitly ignore
        class KnownExitCodes
        {
            public const int HostNotRunning = 10024;
            public const int DatabaseDoesNotExist = 10002;
        }

        public TestSetup(string databaseName)
        {
            this.databaseName = databaseName;
        }

        public TestSetup StartServer()
        {
            RunStarAdmin("start server");
            return this;
        }

        public TestSetup StopHost()
        {
            RunStarAdmin($"-d={databaseName} stop host",
                KnownExitCodes.HostNotRunning,
                KnownExitCodes.DatabaseDoesNotExist
            );
            return this;
        }

        public TestSetup DeleteDatabase()
        {
            RunStarAdmin($"-d={databaseName} delete --force db");
            return this;
        }

        public TestSetup CreateDatabase()
        {
            RunStarAdmin($"-d={databaseName} new db");
            return this;
        }

        public TestSetup RecreateDatabase()
        {
            return DeleteDatabase().CreateDatabase();
        }

        private void RunStarAdmin(
            string args, 
            params int[] allowedExits)
        {
            List<int> exits = new List<int>() { 0 };

            if (allowedExits != null)
            {
                exits.AddRange(allowedExits);
            }

            var processInfo = new ProcessStartInfo()
            {
                FileName = "staradmin",
                Arguments = args,
                CreateNoWindow = true,
                UseShellExecute = false
            };

            var process = Process.Start(processInfo);

            process.WaitForExit();

            if (!exits.Contains(process.ExitCode))
            {
                throw new Exception($"'staradmin {args}' exited with unexpected code {process.ExitCode}");
            }
        }
    }
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

This class creates a fluent interface where the methods calls the staradmin CLI and checks the returned exit codes.

### 3. Create test context with app host

Testing towards the database is possible because apps can be self hosted; the app host is configured and initialized by the developer and not by the platform. Thus, we can specify the app and database to run in the host. 

For testing, we want to create a designated test database to use with the application we want to test. We'll do this in a class we'll call `TestContext` where the constructor recreates the database, builds and runs the app host:

{% code-tabs %}
{% code-tabs-item title="TestContext.cs" %}
```csharp
using System;
using Starcounter;
using Starcounter.Hosting;

namespace UnitTestDemo.Tests
{
    public class TestContext : IDisposable
    {
        readonly ICodeHost host;
        
        // Retrieve the name from the test assembly
        // You can use any name as long as it's not occupied
        public string DatabaseName { get; } =
            typeof(TestContext)
                .Assembly.GetName().Name
                .Replace(".", string.Empty);

        public TestContext()
        {
            // Start server to interact with host 
            // Stop host in case a database is already running
            // Recreate the database to start clean
            new TestSetup(DatabaseName)
                .StartServer()
                .StopHost()
                .RecreateDatabase();

            // Load the database into the app host and
            // point to the assembly to test
            host = new AppHostBuilder()
                .UseDatabase(DatabaseName)
                .UseApplication(typeof(UnitTestDemo.Program).Assembly)
                .Build();

            host.Start();
        }

        // Clean up the host after the test are finished
        void IDisposable.Dispose()
        {
            host.Dispose();
        }
    }
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

### 4. Initiate test context on every run

To instantiate `TestContext` on every run, we create an `ICollectionFixture<TestContext>` and a base class that will be inherited by the test classes. Read more about it in the [XUnit docs](https://xunit.github.io/docs/shared-context.html#collection-fixture).

In code, it looks like this:

{% code-tabs %}
{% code-tabs-item title="TestContextCollection.cs" %}
```csharp
using Xunit;

namespace UnitTestDemo.Tests
{
    [CollectionDefinition(nameof(TestContext))]
    public class TestContextCollection : 
        ICollectionFixture<TestContext>
    {
    }
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

{% code-tabs %}
{% code-tabs-item title="BaseTest.cs" %}
```csharp
using Xunit;

namespace UnitTestDemo.Tests
{
    [Collection(nameof(TestContext))]
    public abstract class BaseTest
    {
    }
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

### 5. Add scheduling helper

To use Starcounter functionality in the tests, they have to be wrapped in a `Scheduling.ScheduleTask`. In addition to that, they will also need to be in a transaction, so to simplify this, we'll define a new method `ScheduleTransact`: 

{% code-tabs %}
{% code-tabs-item title="StarcounterContext.cs" %}
```csharp
using System;
using Starcounter;
using System.Threading.Tasks;

namespace UnitTestDemo.Tests
{
    public static class StarcounterContext
    {
        public static Task ScheduleTransact(Action action)
        {
            return ScheduleTask(() => Db.Transact(action));
        }
        
        public static Task ScheduleTask(Action action)
        {
            return Scheduling.RunTask(action);
        }
    }
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

If `ScheduleTask` is not used, Starcounter will throw `ScErrThreadNotAttached (SCERR1004)`.

### 6. Write tests

With this setup, you can start to write tests. We'll write a simple `Program.cs` test for a method that retrieves a `Person` based on its passport number:

{% code-tabs %}
{% code-tabs-item title="Program.cs" %}
```csharp
using System.Linq;
using Starcounter;

namespace UnitTestDemo
{
    [Database]
    public class Person
    {        
        public string Name { get; set; }
        public int PassportNumber { get; set; }
    }
    
    public class Program
    {        
        public static Person GetPersonWithPassportNum(int passportNumber)
        {            
            var selectWherePassportNumber =
                $"SELECT p FROM {typeof(Person)} p " +
                $"WHERE p.{nameof(Person.PassportNumber)} = ?";
            var person = Db.SQL<Person>(
                selectWherePassportNumber, passportNumber);
                
            return person.FirstOrDefault();
        }
    
        static void Main()
        {
        }
    }
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

The test will add a `Person` to the database and then check if the correct `Person` is returned.

{% code-tabs %}
{% code-tabs-item title="DatabaseTest.cs" %}
```csharp
using Xunit;
using Starcounter;
using System.Threading.Tasks;

namespace UnitTestDemo.Tests
{
    // Inherit from BaseTest to run in the test context
    public class DatabaseTest : BaseTest
    {
        [Fact]
        public async Task GetWithPassportNum_WithExistingNum_ReturnsPerson()
        {
            await StarcounterContext.ScheduleTransact(() =>
            {
                // Setup
                var passportNum = 12345678;
                var expectedPerson = new Person() // Add to database
                {
                    Name = "Gimli",
                    PassportNumber = passportNum
                };
                
                // Execution
                var actualPerson = 
                    Program.GetPersonWithPassportNum(passportNum);

                // Assertion
                Assert.True(expectedPerson.Equals(actualPerson));
            });
        }
    }
}
```
{% endcode-tabs-item %}
{% endcode-tabs %}

{% hint style="info" %}
`Assert.True` with `Equals` is used instead of `Assert.Equal` since `Assert.Equal` will compare after the database class is weaved which won't work. Using `Equals` is the right way to compare database objects.
{% endhint %}

Starcounter features can be used freely in tests, such as creating a new instance in the database with the `new` keyword in the example above; as long as the test class inherits from `BaseClass` and uses `ScheduleTask`.

## Summary

With this setup done, you are free to write unit tests that involve the database.

Even if this is one way to make tests work, there are variations of this setup that might work better for you. For example, by varying the frequency of the database recreation:

* You might not want to recreate the database every time since it takes some time, instead, you could run a batch file or Python script that does it when you want. With that approach, you'd have to make sure that the created database has the same name as the one fed to the `AppHostBuilder`  method `UseDatabase`.
* You could recreate the database for every test class or even every method by instantiating and disposing the `TestContext` manually. This will cause the tests to run much slower since database recreation is slow but will make sure that the database is always fresh.



