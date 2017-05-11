# Query processing errors

If a query can not be processed due to some syntax or type checking error then the method `Db.SQL<T>(String query)` will throw an `SqlException` (namespace `Starcounter`).

```cs
try
{
  string query = "SELECT e.NonExistingProperty FROM Person p";
  QueryResultRows<Person> result = Db.SQL<Person>(query);
  foreach(Person person in result)
  {
    Console.WriteLine(person.Name);
  }
}
catch (SqlException exception)
{
  Console.WriteLine("Incorrect query: " + exception.Message);
}
```
