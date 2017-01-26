# Data manipulation

There are three data manipulation statements in SQL92: <code>INSERT</code>, <code>UPDATE</code> and <code>DELETE</code>. None of these statements are supported in Starcounter SQL. Objects are instead created, updated, and deleted directly in the programming code.

The same way a database object is created with the native program code operator `new`, a database object can be updated using the native program code assign operator `=`.

A database object is deleted by calling the method `Delete()` on the database object.

All modifications are directly reflected in the database for the current transaction, and when the current transaction is committed the modifications are saved and visible to other transactions.

<div class="code-name code-title">Create a database class</div>
```cs
[Database]
public class Employee
{
    public String FirstName;
    public String LastName;
    public Department Department;
    public Employee Manager;
}
```

<div class="code-name code-title">Create, update, delete object</div>
```cs
Db.Transact(() =>
{
    // Creating database object.
    Employee emp = new Employee();
    // Updating database object.
    emp.FirstName = "John";
    emp.LastName = "Smith";
    // Deleting database object.
    emp.Delete();
});
```
The following example shows how to update the `LastName` of all
`Employee` objects to upper case.

<div class="code-name code-title">Update properties in loop</div>
```cs
Db.Transact(() =>
{ 
    string query = "SELECT e FROM Employee e";
    foreach (Employee emp in Db.SQL<Employee>(query))
    {
        emp.LastName = emp.LastName.ToUpper();
    }
});
```
