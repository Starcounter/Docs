# Variables

In programming code you are not allowed to use literals in SQL queries. Instead you should use SQL variables in the query string and pass the current values of the variables as parameters when executing the query. The reason is to achieve better performance and to save system resources.

SQL variables are represented by question marks (?) in the query string, and you pass the current values of the variables as parameters to the method <code>Db.SQL(String query, params Object[] values)</code>.

```cs
QueryResultRows<Employee> result = Db.SQL<Employee>("SELECT e FROM Employee e WHERE e.FirstName = ?", "Joe");
foreach (Employee emp in result) {
  Console.WriteLine(emp.FirstName + " " + emp.LastName);
}
```

You can pass an arbitrary number of variable values to the method <code>SQL</code>, but the number of variables values needs to be exactly the same as the number of variables (question marks) in the query string. Otherwise an <code>ArgumentException</code> will be thrown.

```cs
string lastName = "Smith";
Employee manager; //Assume some value is assigned to the variable manager.
QueryResultRows<Employee> result = Db.SQL<Employee>("SELECT 
             e FROM Employee e 
             WHERE e.LastName = ? AND e.Manager = ?", lastName, manager);
foreach (Employee emp in result) {
  Console.WriteLine(emp.LastName + "; " + emp.Manager.LastName);
}
```

Each variable will have an implicit type depending on its context in the query string. For example a variable that is compared with a property of type <code>String</code> will implicitly be of type <code>String</code>. If a variable is given a value of some incompatible type then an <code>InvalidCastException</code> will be thrown. All numerical types are compatible to each other.

Note that you can only use <code>?</code> to variables after the <code>WHERE</code> clause. You cannot, for instance, use <code>?</code> to replace the class name of a query.
