# Create a Database Class

Welcome to the first part of our Hello World tutorial!

We will start by creating a Starcounter application in Visual Studio by going to <code>New Project -> Templates -> Visual C# -> Starcounter -> Starcounter Application</code>. We will name the application <code>HelloWorld</code>.


<aside class="read-more">
    <a href="http://starcounter.io/guides/database/creating-database-classes/">Read about the  [Database] attribute</a>
</aside>


Establish a new class called <code>Person</code> with the attribute <code>[Database]</code> inside the <code>HelloWorld</code> namespace. This attribute tag will make all instances of the class persistent. Add the fields <code>FirstName</code> and <code>LastName</code> to this class. Your code should now look like this:


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
<aside class="read-more">
    <a href="http://starcounter.io/guides/transactions/">Read more about transactions in Starcounter</a>
</aside>
Add your first instance to the database by defining a new person, its properties, and wrapping it in a <code>Db.Transact()</code> which will keep this person in the database.

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

The if statement here checks if you already have a <code>Person</code> in the database by accessing the <code>First</code> result that we get from the query. If that is the case, you do not need to create a new one. Without it, we would create a new instance of <code>Person</code> every time we run the program, which we do not intend to do.

Start your program with Starcounter by clicking <kbd>F5</kbd> in Visual Studio. You can find the Starcounter Administrator and access your database by going to localhost:8181 in your web browser.

<section class="see-yourself">Type in localhost:8181/#/databases/default/sql and enter <code>SELECT * FROM HelloWorld.Person</code> into the SQL terminal. This will display all the data in your database.</section>

The database should now contain one row with two data entries which can be found using SQL. Let us improve our application now by adding a UI which will help us to display our data in real time.

![Result gif](/assets/part1resized.gif)

If you get any errors, you can check your code against the [source code](https://github.com/StarcounterSamples/HelloWorld/commit/0969f620acb4e8009445fbcd0c4e1d715601a4b9).