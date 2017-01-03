# Query processing errors

If a query can not be processed due to some syntax or type checking error then the method <code>Db.SQL<T>(String query)</code> will throw an <code>SqlException</code> (namespace <code>Starcounter</code>).

```cs
try {
  string query = "SELECT e.NonExistingProperty FROM Person p";
  QueryResultRows<Person> result = Db.SQL<Person>(query);
  foreach(Person person in result) {
    Console.WriteLine(person.Name);
  }
}
catch (SqlException exception) {
  Console.WriteLine("Incorrect query: " + exception.Message);
}
```
