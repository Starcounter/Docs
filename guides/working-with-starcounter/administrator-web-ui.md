# Administrator Web UI

Administrator is a web based administration tool that comes with Starcounter. After the server is started, Administrator should be accessible by pointing your web browser to [http://localhost:8181](http://localhost:8181).

The default port is `8181`. This can be changed during installation or in the server configuration.

<!--This page explains the Administrator features:

- Database
   - Database home screen
      - Creating and deleting databases
      - Starting and stopping of a database
      - Reading app console output
      - Controlling apps in a database
   - [SQL browser](#sql-browser)
      - [Executing SQL queries](#executing-sql-queries)
      - Examining SQL query plan
   - [App Warehouse](#appstore)
      - Downloading and installing apps
   - Start executable
   - Database configuration
- Log
- Network
- Server configuration
-->

## Database

In Starcounter, your classes are your tables and the class instances are your rows. To revise data object concept in Starcounter please visit <a href="http://localhost:8181/#/databases">Database</a>. 
Using <strong>Databases tool</strong> Administrator can manage one or multiple databases, create new ones and more. It is accessible after starting <code>Starcounter Personal Administrator.exe</code> on the link <a href="http://127.0.0.1:8181/#/databases">localhost:8181/#/databases</a>  

### Database home screen

It is possible to work with databases from <a href="http://127.0.0.1:8181/#/databases">Web tool</a> and from Console.
In web you can see, that Starcounter provides a <code>default</code> database in which Applications are operating.

<a href="http://starcounter.io/wp-content/uploads/2015/09/1.png" rel="attachment wp-att-15246"><img src="http://starcounter.io/wp-content/uploads/2015/09/1.png" alt="Web Administrator" width="531" height="189" class="alignnone size-full wp-image-15246" /></a>

When no applications are running default database is uninitialized.

<blockquote>You can read more about StarAdmin Command Line Interface <a href="http://starcounter.io/guides/tools/staradmin/">here</a>.</blockquote>



#### Creating and deleting databases

It is possible to create/delete Starcounter Database trough web Administrator.
Databases are created as hosts for multiple applications running on them. By default database port is set to <code>8080</code>. If you plan to have several databases running in parallel, you need to change port parameter to be different, otherwise there will be a conflict.

From Web panel it is easy to create a new Database by pressing <a href="http://127.0.0.1:8181/#/databaseNew">New database</a> in the main menu.  
If you already work with several databases but experience a need in a new one, follow to <code>Select Database -> New database</code>

<a href="http://starcounter.io/wp-content/uploads/2015/09/3.png" rel="attachment wp-att-15259"><img src="http://starcounter.io/wp-content/uploads/2015/09/3.png" alt="3" width="606" height="244" class="alignnone size-full wp-image-15259" /></a>

Deleting database can be done by using the same control panel. This operation will need verification y typing in a Database name to delete in a popped-up dialog window (case-sensitive).
You can specify database port in <code>Advanced</code> settings.
You will need to verify the DELETE operation by typing in the DB name.

#### Starting and stopping of a database

In Web Administrator interface  you can start and stop existing databases by pressing corresponding buttons.

<a href="http://starcounter.io/wp-content/uploads/2015/09/56.png" rel="attachment wp-att-15271"><img src="http://starcounter.io/wp-content/uploads/2015/09/56.png" alt="56" width="772" height="260" class="alignnone size-full wp-image-15271" /></a>

Green check icon represents that the database is currently running.
If you redirect to <code>Select Database -> DB Name</code> it is possible as well to do the same operations from dedicated database descriptions. Note, that you can't start more than one database on the same port.

#### Viewing applications' output

Navigate to the database by clicking on its name (e.g., default) in the list of databases. Click on the application name in the "Applications" list that are currently started in the database, opens a view that shows the output of the application.

<a href="http://starcounter.io/wp-content/uploads/2015/09/appoutput2.gif" rel="attachment wp-att-15278"><img src="http://starcounter.io/wp-content/uploads/2015/09/appoutput2.gif" alt="appoutput2" width="628" height="315" class="alignnone size-full wp-image-15278" /></a>


#### Controlling apps in a database

On the Database page apps can be Started, Stopped and Deleted.

```Auto-Start``` indicates if the app should start along with the database. The ```Padlock``` icon is used to lock an app from being deleted.

<a href="http://starcounter.io/wp-content/uploads/2015/09/Database.png"><img class="alignnone size-full wp-image-13782" src="http://starcounter.io/wp-content/uploads/2015/09/Database.png" alt="Database" width="867" height="328" /></a>

### SQL browser

#### Executing SQL queries

In the SQL browser of a database you can write queries to that database. See [SQL reference](/guides/sql/) for details on syntax.

The queries that are supported by the method <code>Db.SQL</code> are also supported here with few differences:

- [literals](/guides/sql/literals/) are supported,
- [variables](/guides/database/variables/) are <strong>not</strong> supported.
- literals of type <code>Binary</code> are <strong>not</strong> supported.

Before running a query an executable that defines the class should be started in the targeted database. Then queries are issued in terms of database classes and properties defined in the executable(s). Classes correspond to tables and properties or fields - to columns in SQL queries.

For example, the class `Person` can be queried using interactive SQL:

```sql
SELECT Person.FullName, Text FROM Quote WHERE Person.FirstName = 'Albert'
```

The administrator will show the following result:

<a href="http://starcounter.io/wp-content/uploads/2015/09/Screenshot-2015-10-02-17.23.40.png"><img src="http://starcounter.io/wp-content/uploads/2015/09/Screenshot-2015-10-02-17.23.40-1024x438.png" alt="Screenshot 2015-10-02 17.23.40" class="alignnone size-large wp-image-13789" style="border: 1px solid gray" /></a>

#### Examining SQL query plan

If you navigate to <code>Query Plan</code> after the SQL request compilation, you can see the steps performed to access the output data.

<a href="http://starcounter.io/wp-content/uploads/2015/09/5.png" rel="attachment wp-att-15280"><img src="http://starcounter.io/wp-content/uploads/2015/09/5.png" alt="5" width="457" height="547" class="alignnone size-full wp-image-15280" /></a>

### App Warehouse

Apps can be downloaded to a database from the ```App Warehouse``` tab. Once an app has been downloaded, it can be started and stopped from its database page.

<a href="http://starcounter.io/wp-content/uploads/2015/09/AppStoreTab.png"><img class="alignnone size-full wp-image-13781" src="http://starcounter.io/wp-content/uploads/2015/09/AppStoreTab.png" alt="AppStoreTab" width="842" height="92" /></a>

#### Downloading and installing apps

Click the ```Download``` button to download an app. Downloaded apps can be controlled on the database page.

<a href="http://starcounter.io/wp-content/uploads/2015/09/Appstore1.png"><img class="alignnone size-full wp-image-13779" src="http://starcounter.io/wp-content/uploads/2015/09/Appstore1.png" alt="Appstore1" width="863" height="551" /></a>

### Start executable

You can launch your application on a currently running database by redirecting to <a href="http://127.0.0.1:8181/#/databases/default/executabeStart">Start Executable</a>. 
Specify the path to your <code>.exe</code>application in the dedicated field.
<a href="http://starcounter.io/wp-content/uploads/2015/09/6.png" rel="attachment wp-att-15287"><img src="http://starcounter.io/wp-content/uploads/2015/09/6.png" alt="6" width="772" height="346" class="alignnone size-full wp-image-15287" /></a>

### Database configuration

You can access your database settings by redirecting to <a href="http://127.0.0.1:8181/#/databases/default/settings">Settings Icon</a>.
It is possible to specify database port to have databases running in parallel on one kernel along with Scheduler Count that defines the degree of  parallelization and Chunks (internal setting, shouldn't be modified). 

<a href="http://starcounter.io/wp-content/uploads/2015/09/7.png" rel="attachment wp-att-15288"><img src="http://starcounter.io/wp-content/uploads/2015/09/7.png" alt="7" width="772" height="478" class="alignnone size-full wp-image-15288" /></a>

## Log

To have a track of activity within your environment redirect to <a href="http://127.0.0.1:8181/#/server/log">Log</a>. You can see debug steps, notices, warnings and errors with explicit descriptions. After sorting log notes by "Source" it is possible to track the behavior of specific component. 

<a href="http://starcounter.io/wp-content/uploads/2015/09/8.png" rel="attachment wp-att-15290"><img src="http://starcounter.io/wp-content/uploads/2015/09/8.png" alt="8" width="751" height="574" class="alignnone size-full wp-image-15290" /></a>


## Network

By accessing <a href="http://127.0.0.1:8181/#/server/network">Network </a>tab it is possible to see internal environment, the information about network facilities for Starcounter installation that comes by the means of network gateway. 

<a href="http://starcounter.io/wp-content/uploads/2015/09/9.png" rel="attachment wp-att-15291"><img src="http://starcounter.io/wp-content/uploads/2015/09/9.png" alt="9" width="754" height="441" class="alignnone size-full wp-image-15291" /></a> 

## Server configuration

To access server network settings redirect to <a href="http://127.0.0.1:8181/#/server/settings">Settings</a>. 
For internal system communications and management specify <strong>System port</strong>. 
For outbound operations specify <strong>Gateway port</strong>.

<a href="http://starcounter.io/wp-content/uploads/2015/09/10.png" rel="attachment wp-att-15292"><img src="http://starcounter.io/wp-content/uploads/2015/09/10.png" alt="10" width="768" height="401" class="alignnone size-full wp-image-15292" /></a> 