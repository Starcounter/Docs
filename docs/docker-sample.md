# Running Starcounter 3.0 applications under Docker

With Starcounter 3.0 it is possible to run Starcounter applications in Linux Docker containers.

## Sample Starcounter 3.0 Docker configuration

Sample files structure:

```text
📁 App
|--📑 App.csproj
|--📑 nuget.config
|--📑 Program.cs
📑 Dockerfile
```

* The `App` folder contains a sample Starcounter Console application files.
* The `Dockerfile` file contains Docker container definition for the app.

### `App.csproj`

```markup
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp3.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Starcounter.Database" Version="3.0.0-*" />
  </ItemGroup>
</Project>
```

### `nuget.config`

```markup
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <!--To inherit the global NuGet package sources remove the <clear/> line below -->
    <clear />
    <add key="local" value="../artifacts" />
    <add
        key="Starcounter"
        value="https://www.myget.org/F/starcounter/api/v3/index.json"
    />
    <add key="nuget" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
```

### `Program.cs`

```csharp
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
    /// for the Starcounter services, to fetch service instances from it,
    /// and then how to use the services to make basic database transactions
    /// with basic database interactions like SQL queries and inserts.
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            string name = args.FirstOrDefault() ?? "Noname";
            string connectionString = "Database=./.database/ConsoleApp;"
                + "OpenMode=CreateIfNotExists;"
                + "StartMode=StartIfNotRunning;"
                + "StopMode=IfWeStarted";

            // Here we create a service collection
            // that we add the Starcounter services to.
            // When we call BuildServiceProvider(),
            // we get an instance that we can use to
            // fetch service instances, for example ITransactor,
            // which we then can use to to make database transactions.
            using var services = new ServiceCollection()
                .AddStarcounter(connectionString)
                .BuildServiceProvider();

            // Here we fetch our ITransactor instance from the service provider.
            var transactor = services.GetRequiredService<ITransactor>();

            // And here we use it to make a database transaction.
            ulong oid = transactor.Transact(db =>
            {
                // Here inside the transaction,
                // we can use the IDatabaseContext instance to
                // interact with the Starcounter database.

                // We can query it using SQL
                // (which returns an IEnumerable<Person>
                // that we can use with LINQ).
                var p = db.Sql<Person>
                (
                    "SELECT p FROM App.Person p WHERE Name = ?",
                    name
                ).FirstOrDefault();

                if (p == null)
                {
                    // We can insert new rows in the database using Insert().
                    p = db.Insert<Person>();

                    // We write to the database row using
                    // standard C# property accessors.
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
```

### `Dockerfile`

```text
FROM mcr.microsoft.com/dotnet/core/sdk:3.0-bionic AS build

WORKDIR /source

# Download & Unpack latest Starcounter 3.0.0
RUN apt-get update \
    && apt-get install -y wget unzip

RUN mkdir artifacts
RUN wget https://starcounter.io/Starcounter/Starcounter.3.0.0-alpha-20190930.zip -O ./artifacts/Starcounter.zip
RUN unzip ./artifacts/Starcounter.zip -d ./artifacts

# Copy source files
RUN mkdir App
COPY ./App/nuget.config ./App/nuget.config
COPY ./App/Program.cs ./App/Program.cs
COPY ./App/App.csproj ./App/App.csproj

WORKDIR /source/App
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-bionic AS runtime

# Install Starcounter dependencies
RUN apt-get update \
    && apt-get install -y wget unzip \
    && apt-get install -y software-properties-common \
    && apt-get install -y libaio1 \
    && add-apt-repository -y ppa:swi-prolog/stable \
    && apt-get update \
    && apt-get install -y swi-prolog-nox=7.\*

ENV ASPNETCORE_URLS http://+:8080
WORKDIR /source/publish
COPY --from=build /source/App/out ./

ENTRYPOINT ["dotnet", "App.dll"]
```

