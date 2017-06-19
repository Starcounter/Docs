# Shared Data Model

It would be useless to visually combine two apps if they didn't share data. In this step we'll merge the data model of PetList and MedicalRecordProvider so that they can act on the same data.

## Create SharedModel Project

The merged data model should be in a separate project. To create this, right click on the solution and choose `Add -> New project...`. Pick `Starcounter Class Library` and name it `SharedModel`.

![Shared data model project](/assets/SharedModelProject.PNG)

## Reference the Project

To access the data model from the apps, they should have a reference to it.

Add a reference by right-clicking on `References` for the apps in the Solution Explorer and choose `Add reference...`. Then, check the box for `SharedModel` at `Project -> Solution`.

Do this for both PetList and MedicalRecordProvider.

![Reference the project](/assets/SharedModelReference.PNG)

## Move the Database Classes

There are three database classes:

* `Examination` in MedicalRecordProvider
* `Patient` in MedicalRecordProvider
* `Pet` in PetList

These three classes can simply be cut from the existing projects and copied into the new project. When you have moved them to the `SharedModel` project, it should look like this: 

![Shared Model Structure](/assets/SharedModelStructure.PNG)

<div class="code-name">Examination.cs</div>

```cs
using System;
using Starcounter;

namespace SharedModel
{
    [Database]
    public class Examination
    {
        public DateTime Date { get; set; }
        public string Clinic { get; set; }
        public bool RoutineCheck { get; set; }
        public Patient Patient { get; set; }
        public string NoteFromExaminer { get; set; }
    }
}
```

<div class="code-name">Patient.cs</div>

```cs
using System.Collections.Generic;
using Starcounter;

namespace SharedModel
{
    [Database]
    public class Patient
    {
        public string Name { get; set; }
        public IEnumerable<Examination> MedicalExaminations => 
            Db.SQL<Examination>($"SELECT e FROM {typeof(Examination)} e WHERE e.{nameof(Examination.Patient)} = ?", this);
    }
}
```

<div class="code-name">Pet.cs</div>

```cs
using Starcounter;

namespace SharedModel
{
    [Database]
    public class Pet
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Animal { get; set; }
        public string OwnerName { get; set; }
        public double Weight { get; set; }
    }
}
```

## Merge the Database Classes

It should now be possible to access the database classes the same way as before, the only difference is that they can access each other's database classes. This is not useful for us since PetList only uses `Pet` and MedicalRecordProvider only uses `Examination` and `Patient`. 

In order for this to be useful for us, we have to establish relationships between the classes. With these three database classes, this is trivial. A `Pet` is a `Patient` so we make `Pet` inherit from `Patient` and delete the overlapping property `Name`.

The resulting `Pet` class should look like this:

<div class="code-name">Pet.cs</div>

```cs
using Starcounter;

namespace SharedModel
{
    [Database]
    public class Pet : Patient
    {
        public int Age { get; set; }
        public string Animal { get; set; }
        public string OwnerName { get; set; }
        public double Weight { get; set; }
    }
}
```

The `Patient` class will not change. 

These two apps now have a meaningful way to interact.

## Create Dummy Data in One Place

Right now we create dummy data in the entry point for both PetList and MedicalRecordProvider. Since their database classes now are related, creating dummy data this way requires us to start the apps in a certain order. To fix this, we'll create all the dummy data in `PetList`:

<div class="code-name">Program.cs</div>

```cs
using System;
using Starcounter;
using PetList.Api;
using System.Linq;
using SharedModel;

namespace PetList
{
    class Program
    {
        static void Main()
        {
            bool noPets = Db.SQL($"SELECT e FROM {typeof(Pet)} e").FirstOrDefault() == null;
            if (noPets)
            {
                PopulateDatabase();
            }

            var mainHandlers = new MainHandlers();
            var partialHandlers = new PartialHandlers();

            mainHandlers.Register();
            partialHandlers.Register();
        }

        private static void PopulateDatabase()
        {
            Db.Transact(() =>
            {
                var fluffy = new Pet()
                {
                    Name = "Fluffy",
                    Age = 7,
                    Animal = "Dog",
                    OwnerName = "Alice",
                    Weight = 4.2
                };

                var biblo = new Pet()
                {
                    Name = "Biblo",
                    Age = 3,
                    Animal = "Cat",
                    OwnerName = "Bob",
                    Weight = 1.7
                };

                var lassie = new Pet()
                {
                    Name = "Lassie",
                    Age = 13,
                    Animal = "Dog",
                    OwnerName = "Eve",
                    Weight = 18.3
                };

                new Examination()
                {
                    Date = DateTime.Now.AddYears(-2),
                    Clinic = "Happy Animals Clinic NY",
                    RoutineCheck = true,
                    Patient = fluffy,
                    NoteFromExaminer = "Went well, there was nothing wrong"
                };

                new Examination()
                {
                    Date = DateTime.Now.AddMonths(-1),
                    Clinic = "Happy Animals Clinic Oslo",
                    RoutineCheck = false,
                    Patient = biblo,
                    NoteFromExaminer = "Had a bad bite in the leg, got the normal treatment. Should be okay now"
                };

                new Examination()
                {
                    Date = DateTime.Now.AddDays(-13),
                    Clinic = "Happy Animals Clinic Stockholm",
                    RoutineCheck = true,
                    Patient = lassie,
                    NoteFromExaminer = "Needed some nutrients. Everything else was good"
                };

                new Examination()
                {
                    Date = DateTime.Now.AddYears(-1),
                    Clinic = "Happy Animals Clinic WY",
                    RoutineCheck = false,
                    Patient = fluffy,
                    NoteFromExaminer = "The owners panicked a little, turned out okay anyway"
                };

                new Examination()
                {
                    Date = DateTime.Now.AddYears(-3),
                    Clinic = "Happy Animals Clinic NY",
                    RoutineCheck = true,
                    Patient = fluffy,
                    NoteFromExaminer = "Nothing wrong at all"
                }; 
            });
        }
    }
}
```

With this change, we can delete the `PopulateDatabase` method from the `Program.cs` class in MedicalRecordProvider and delete this code: 

```cs
bool noExaminations = Db.SQL($"SELECT e FROM {typeof(Examination)} e").FirstOrDefault() == null;
if (noExaminations)
{
     PopulateDatabase();
}
```

## Run the Apps

After modifying the database schema, it's smart to recreate the database to ensure that the data with the old schema doesn't get in the way. When the administrator server is running you can do it this way:

```git
staradmin delete db default
staradmin new db default
```

## Summary

Now with the views adapted to Declarative Shadow DOM and a shared data model, we can get started with the actual blending.