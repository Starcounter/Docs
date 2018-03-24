# How to delete tables and columns

## Introduction

Starcounter only deletes data when explicitly told to. In code, data is deleted with the `Delete` method or with `DELETE` SQL queries, read more in [Data manipulation - delete](../topic-guides/database/data-manipulation.md#delete). 

For example, if a database class is deleted from the code, the underlying table and the data in that table will not be deleted even if it's not referenced by any database class. The reason for this is that there's no way for Starcounter to determine if the data is abandoned and will never be used again. 

To delete tables and columns that will not be used, you can mark that they can be dropped with SQL and then delete them by executing the command `staradmin start cleaner`.

`staradmin start cleaner` launches the cleaning process by looking up all `DROP TABLE` or `ALTER TABLE` commands that were executed since the last time it ran. It will run until all those commands have been processed and then terminate. This will permanently delete the tables and columns.

{% hint style="danger" %}
Data that is deleted with `start cleaner` can't be recovered. Backup your data before deleting anything to have a safe point to revert to. 
{% endhint %}

## How to drop a table

Tables should be dropped if it's not referenced by a database class and the data in the table is not useful anymore. The reason to do this is to free up memory. For this example, we have a table `UselessTable` that is not needed anymore:

```csharp
[Database] 
public class UselessTable
{
    public string Nothing { get; set; }
}
```

### 1. Delete the database class from the code

In order to delete a table, the database class that references it has to be deleted. Thus, we delete the class `UselessTable` from the code and all the references to the class.

### 2. Restart the application

To delete the reference between the database class and the table, rebuild and restart the application. If you don't do this, the Administrator will throw `ScErrColumnIsMapped (SCERR15014)`.

### 3. Run SQL command DROP TABLE

To mark that the underlying table should be deleted with all its data, run this command in the [Administrator](../topic-guides/working-with-starcounter/administrator-web-ui.md): 

```sql
DROP TABLE MyApp.UselessTable
```

{% hint style="warning" %}
If you try to run `DROP TABLE` without deleting all the references to the table in all the apps that are running, Starcounter will throw  `ScErrTableIsReferenced (SCERR15012)`. 
{% endhint %}

### 4. Run the cleaner

To permanently delete `UselessTable`, execute `staradmin start cleaner`, when it's finished, it will print:  `Done (nothing more to clean)`.

If you add `UselessTable` back to the code, executing `SELECT * FROM MyApp.UselessTable` will yield no results. The data has successfully been deleted.

{% hint style="warning" %}
Running `staradmin start cleaner` might produce transaction conflicts with running applications, thus, run the cleaning when the load is low to avoid performance drops.
{% endhint %}

## How to drop a column

Deleting unused columns works similarly to how unused tables are deleted: a SQL command is used to mark that it should be dropped, and `staradmin start cleaner` deletes the data and column that has been marked.

For this example, `IrrelevantFact` should be deleted from the `Person` table:

```csharp
[Database] 
public class Person 
{
  public string Name { get; set; }
  public string IrrelevantFact { get; set; }
}
```

### 1. Delete the property from the database class

To drop a column, the corresponding property has to be deleted. In this case, the class will then look like this:

```csharp
[Database] 
public class Person 
{
  public string Name { get; set; }
}
```

### 2. Restart the application

To delete the reference between the property and the column, rebuild and restart the application. If you don't do this, the Administrator will throw `ScErrColumnIsMapped (SCERR15014)`.

### 3. Stop the app

Since the `ALTER TABLE` command can't be executed while the table it alters is referenced in any running app - you have to stop the app before running `ALTER TABLE`, you can do this by running `staradmin stop app MyApp`.

### 4. Run SQL command DROP COLUMN

In the [Administrator](../topic-guides/working-with-starcounter/administrator-web-ui.md), run this query to mark that the column should be dropped:

```sql
ALTER TABLE MyApp.Person DROP COLUMN IrrelevantFact
```

### 5. Run the cleaner

To delete the data and the column, run this command:

```text
staradmin start cleaner
```

After this, the column is deleted and all the data that was stored in it is permanently deleted.

## Summary

Data definition language queries \(DDL\) are executed in the Administrator to delete tables or columns. Running the queries by themselves does not delete the data - you also have to run the cleaner to delete the data. To run DDL queries with either `DROP COLUMN` or `DROP TABLE`, you can't have the affected tables loaded. This means that the apps with references to those tables can't run unless the references to the tables are removed in the app.

To help you with find unused tables and columns you can use the tool [WhatsNotReferenced](https://github.com/per-samuelsson/WhatsNotReferenced).  It checks, when you start the database, for all tables and columns that are not in use by any currently running application

Again, be careful when permanently deleting data. If the data is of any value, you should make a backup before cleaning it.



