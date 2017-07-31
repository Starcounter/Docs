# Administrator Web UI

Administrator is a web based administration tool that comes with Starcounter. It can be started in three different ways:
1. Run `staradmin start server`
2. Run an app
3. Execute `scservice.exe` - the `Starcounter Personal Server` shortcut that is added to the desktop after installation does this

After starting the Administrator, it's accessible from `localhost:8181`.

The default port `8181` can be changed during installation or in the server configuration.

## Database Home Screen

It's possible to work with databases from [Web tool](http://127.0.0.1:8181/#/databases) and from Console.
In web you can see, that Starcounter provides a `default` database in which Applications are operating.

![Database home screen screenshot](/assets/1.png)

When no applications are running default database is uninitialized.

> You can read more about StarAdmin Command Line Interface [here](/guides/working-with-starcounter/staradmin-cli).


### Creating and Deleting Databases

It is possible to create/delete Starcounter Database trough web Administrator.
Databases are created as hosts for multiple applications running on them. By default database port is set to `8080`. If you plan to have several databases running in parallel, you need to change port parameter to be different, otherwise there will be a conflict.

From Web panel it is easy to create a new Database by pressing [New database](http://127.0.0.1:8181/#/databaseNew) in the main menu.  
If you already work with several databases but experience a need in a new one, follow to `Select Database -> New database`

![Creating and deleting databases](/assets/3.png)

Deleting database can be done by using the same control panel. This operation will need verification y typing in a Database name to delete in a popped-up dialog window (case-sensitive).
You can specify database port in `Advanced` settings.
You will need to verify the DELETE operation by typing in the DB name.

### Starting and Stopping Databases

In Web Administrator interface  you can start and stop existing databases by pressing corresponding buttons.

![starting and stopping a database](/assets/56.png)

Green check icon represents that the database is currently running.
If you redirect to `Select Database -> DB Name` it is possible as well to do the same operations from dedicated database descriptions. Note, that you can't start more than one database on the same port.

### Viewing Application Output

Navigate to the database by clicking on its name (e.g., default) in the list of databases. Click on the application name in the "Applications" list that are currently started in the database, opens a view that shows the output of the application.

![application output](/assets/appoutput2.gif)

### Controlling Apps

On the Database page apps can be Started, Stopped and Deleted.

`Auto-Start` indicates if the app should start along with the database. The `Padlock` icon is used to lock an app from being deleted.

![database control](/assets/Database.png)

## SQL Browser

### Executing SQL Queries

In the SQL browser of a database you can write queries to that database. See [SQL reference](/guides/SQL/) for details on syntax.

The queries that are supported by the method `Db.SQL` are also supported here with few differences:

- [literals](/guides/SQL/literals) are supported,
- [variables](/guides/database/variables) are **not** supported.
- literals of type `Binary` are **not** supported.

Before running a query an executable that defines the class should be started in the targeted database. Then queries are issued in terms of database classes and properties defined in the executable(s). Classes correspond to tables and properties or fields - to columns in SQL queries.

For example, the class `Person` can be queried using interactive SQL:

```sql
SELECT Person.FullName, Text FROM Quote WHERE Person.FirstName = 'Albert'
```

The administrator will show the following result:

![query result](/assets/Screenshot-2015-10-02-17.23.40.png)

### SQL Query Plan

If you navigate to `Query Plan` after the SQL request compilation, you can see the steps performed to access the output data.

![examine query plan](/assets/5.png)

## App Warehouse

Apps can be downloaded to a database from the `App Warehouse` tab. Once an app has been downloaded, it can be started and stopped from its database page.

![app store tab](/assets/AppStoreTab.png)

### Downloading and Installing Apps

Click the `Download` button to download an app. Downloaded apps can be controlled on the database page.

![Download and install apps](/assets/Appstore1.png)

## Starting Executables

You can launch your application on a currently running database by redirecting to [Start Executable](http://127.0.0.1:8181/#/databases/default/executabeStart).
Specify the path to your `.exe`application in the dedicated field.

![start executable](/assets/6.png)

## Database Configuration

You can access your database settings by redirecting to [Settings Icon](http://127.0.0.1:8181/#/databases/default/settings).
It is possible to specify database port to have databases running in parallel on one kernel along with Scheduler Count that defines the degree of  parallelization and Chunks (internal setting, shouldn't be modified).

![database configuration](/assets/7.png)

## Log

To have a track of activity within your environment redirect to [Log](http://127.0.0.1:8181/#/server/log). You can see debug steps, notices, warnings and errors with explicit descriptions. After sorting log notes by "Source" it is possible to track the behavior of specific component.

![Log screenshot](/assets/8.png)

## Network

By accessing [Network](http://127.0.0.1:8181/#/server/network) tab it is possible to see internal environment, the information about network facilities for Starcounter installation that comes by the means of network gateway.

![Network screenshot](/assets/9.png)

## Server Configuration

To access server network settings redirect to [Settings](http://127.0.0.1:8181/#/server/settings).
For internal system communications and management specify **System port**.
For outbound operations specify **Gateway port**.

![Server configuration](/assets/10.png)
