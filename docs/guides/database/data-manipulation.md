# Data manipulation

There are three data manipulation statements in SQL92: `INSERT`, `UPDATE` and `DELETE`. None of these statements are supported in Starcounter SQL. Objects are instead created, updated, and deleted directly in the programming code.

The same way a database object is created with the native program code operator `new`, a database object can be updated using the native program code assign operator `=`.

A database object is deleted by calling the `Delete` method on the database object.

All modifications are directly reflected in the database for the current transaction, and when the current transaction is committed the modifications are saved and visible to other transactions.

```
[Database]
public class Employee
{
    public String FirstName { get; set; }
    public String LastName { get; set; }
    public Department Department { get; set; }
    public Employee Manager { get; set; }
}
```

```
Db.Transact(() =>
{
    Employee emp = new Employee(); // Create database object.

    emp.FirstName = "John"; // Update database object.

    emp.Delete(); // Delete database object.
});
```

The following example shows how to update the `LastName` of all  
`Employee` objects to upper case.

```
Db.Transact(() =>
{ 
    string query = "SELECT e FROM Employee e";
    foreach (Employee emp in Db.SQL<Employee>(query))
    {
        emp.LastName = emp.LastName.ToUpper();
    }
});
```

