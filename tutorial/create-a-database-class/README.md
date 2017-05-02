# Create a Database Class

Welcome to the first part of our Hello World tutorial!

We will start by creating a Starcounter application in Visual Studio by going to `New Project -> Templates -> Visual C# -> Starcounter -> Starcounter Application`. We will name the application `HelloWorld`.

<<<<<<< HEAD
Establish a new class called `Person` with the [`[Database]` attribute](/guides/database/creating-database-classes) inside the `HelloWorld` namespace. This attribute tag will make all instances of the class persistent. Add the fields `FirstName` and `LastName` to this class. Your code should now look like this:
=======
Establish a new class called `Person` with the attribute `[Database]` inside the `HelloWorld` namespace. This attribute tag will make all instances of the class persistent. Add the fields `FirstName` and `LastName` to this class. Your code should now look like this:

<aside class="read-more">
    <a href="/guides/database/creating-database-classes">Read about the  [Database] attribute</a>
</aside>
>>>>>>> 2.2.1.3234

<div class="code-name">Program.cs</div><div class="code-name code-title">Define database</div>

```cs
using Starcounter;

namespace HelloWorld
{
    [Database]
    public class Person
    {
        public string FirstName;
        public string LastName;
    }

    class Program
    {
        static void Main()
        {

        }
    }
}
```

Add your first instance to the database by defining a new person, its properties, and wrapping it in a [`Db.Transact()`](/guides/transactions/using-transactions/#dbtransact) which will keep this person in the database.

<div class="code-name">Program.cs</div><div class="code-name code-title">Add instance</div>

```cs
class Program
{
    static void Main()
    {
        Db.Transact(() =>
        {
            var anyone = Db.SQL<Person>("SELECT p FROM Person p").First;
            if (anyone == null)
            {
                new Person
                {
                    FirstName = "John",
                    LastName = "Doe"
                };
            }
        });
    }
}
```

The if statement here checks if you already have a `Person` in the database by accessing the `First` result that we get from the query. If that is the case, you do not need to create a new one. Without it, we would create a new instance of `Person` every time we run the program, which we do not intend to do.

Start your program with Starcounter by clicking <kbd>F5</kbd> in [Visual Studio](/guides/working-with-starcounter/working-in-visual-studio/). You can find the [Starcounter Administrator](/guides/working-with-starcounter/administrator-web-ui/) and access your database by going to localhost:8181 in your web browser.

<section class="see-yourself">Type in localhost:8181/#/databases/default/sql and enter `SELECT * FROM HelloWorld.Person` into the SQL terminal. This will display all the data in your database.</section>

The database should now contain one row with two data entries which can be found using SQL. Let us improve our application now by adding a UI which will help us to display our data in real time.

![Result gif](/assets/part1resized.gif)

If you get any errors, you can check your code against the [source code](https://github.com/StarcounterApps/HelloWorld/commit/0c9c19a92477b064014da766008efae0040b6768).
