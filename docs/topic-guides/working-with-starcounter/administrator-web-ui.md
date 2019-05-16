# Administrator web UI

## Introduction

Administrator is a web based administration tool that comes with Starcounter. It can be started in three different ways:

1. Run `staradmin start server` 
2. Run an app 
3. Execute `scservice.exe` - the `Starcounter Personal Server` shortcut that is added to the desktop after installation does this

After starting the Administrator, it's accessible from `localhost:8181`.

The default port `8181` can be changed during installation or in the server configuration.

## Handling databases <a id="handling-databases"></a>

Databases are handled from `localhost:8181/#/databases`.

![](https://blobscdn.gitbook.com/v0/b/gitbook-28427.appspot.com/o/assets%2Fstarcounter%2Fe48b74d0-8680-11e7-9944-1f85270462c6%2Fe59fa3f0-8680-11e7-9944-1f85270462c6%2F1.png?generation=1503327413854494&alt=media)

### Default database <a id="default-database"></a>

Starcounter creates a default database if an application is started and there is no existing database. Apps in the default database are available at port `8080` by default.

### Create and delete databases <a id="create-and-delete-databases"></a>

It's possible to create and delete databases in the Administrator. To prevent conflicts when running several parallel databases, their ports have to be different. The port can be specified under the "Advanced" options when creating a new database.

Create new databases at `localhost:8181/#/databases` by pressing "New database".

![](https://blobscdn.gitbook.com/v0/b/gitbook-28427.appspot.com/o/assets%2Fstarcounter%2Fe48b74d0-8680-11e7-9944-1f85270462c6%2Fe59fcb00-8680-11e7-9944-1f85270462c6%2F3.png?generation=1503327411377299&alt=media)

Databases are also deleted in the same view. Deletions have to be verified by entering the name of the database in the pop-up window.

### Start and stop databases <a id="start-and-stop-databases"></a>

Start and stop databases by clicking the "Start" or "Stop" buttons at `localhost:8181/#/databases`:

![](https://blobscdn.gitbook.com/v0/b/gitbook-28427.appspot.com/o/assets%2Fstarcounter%2Fe48b74d0-8680-11e7-9944-1f85270462c6%2Fe59fcb01-8680-11e7-9944-1f85270462c6%2F56.png?generation=1503327411281226&alt=media)

Databases with a green checkmark are running.

## View application output <a id="view-application-output"></a>

Go to a database by clicking on its name in the list of databases. Then, click on the application name in the "Applications" list. This will open up a view that displays the output of the application.

![](https://blobscdn.gitbook.com/v0/b/gitbook-28427.appspot.com/o/assets%2Fstarcounter%2Fe48b74d0-8680-11e7-9944-1f85270462c6%2Fe59ff210-8680-11e7-9944-1f85270462c6%2Fappoutput2.gif?generation=1503327412539433&alt=media)

## Control apps <a id="control-apps"></a>

Apps will start together with the database if "Auto-Start" is clicked. The padlock icon shows if an app can be deleted.

![](https://blobscdn.gitbook.com/v0/b/gitbook-28427.appspot.com/o/assets%2Fstarcounter%2Fe48b74d0-8680-11e7-9944-1f85270462c6%2Fe59ff211-8680-11e7-9944-1f85270462c6%2FDatabase.png?generation=1503327412739918&alt=media)

## SQL browser <a id="sql-browser"></a>

### Execute SQL queries <a id="execute-sql-queries"></a>

The data of a database can be queried in the SQL browser. See [SQL reference](../sql/) for details on the syntax.

The queries that are supported by the method `Db.SQL` are also supported except that [literals](../sql/literals.md) are used in the SQL browser instead of [variables](../database/querying-with-sql.md#using-variables).

The app that defines that table needs to run in order to query it.

For example, the class `Person` can be queried this way:

```sql
SELECT Person.FullName, Text FROM Quote WHERE Person.FirstName = 'Albert'
```

This is the result:

![](https://blobscdn.gitbook.com/v0/b/gitbook-28427.appspot.com/o/assets%2Fstarcounter%2Fe48b74d0-8680-11e7-9944-1f85270462c6%2Fe5a08e50-8680-11e7-9944-1f85270462c6%2FScreenshot-2015-10-02-17.23.40.png?generation=1503327412628857&alt=media)

### SQL query plan <a id="sql-query-plan"></a>

If you navigate to "Query Plan" after the SQL request, you can see the steps to access the data.

![](https://blobscdn.gitbook.com/v0/b/gitbook-28427.appspot.com/o/assets%2Fstarcounter%2Fe48b74d0-8680-11e7-9944-1f85270462c6%2Fe5a0dc70-8680-11e7-9944-1f85270462c6%2F5.png?generation=1503327413735311&alt=media)

## App warehouse <a id="app-warehouse"></a>

Apps can be downloaded to a database from the `App Warehouse` tab. Once an app has been downloaded, it can be started and stopped from its database page.

![](https://blobscdn.gitbook.com/v0/b/gitbook-28427.appspot.com/o/assets%2Fstarcounter%2Fe48b74d0-8680-11e7-9944-1f85270462c6%2Fe5a0dc71-8680-11e7-9944-1f85270462c6%2FAppStoreTab.png?generation=1503327413783128&alt=media)

### Download and install apps <a id="download-and-install-apps"></a>

Click the `Download` button to download an app. Downloaded apps can be controlled on the database page.

![](https://blobscdn.gitbook.com/v0/b/gitbook-28427.appspot.com/o/assets%2Fstarcounter%2Fe48b74d0-8680-11e7-9944-1f85270462c6%2Fe5a10380-8680-11e7-9944-1f85270462c6%2FAppstore1.png?generation=1503327411075154&alt=media)

## Starting executables <a id="starting-executables"></a>

You can launch an application in a database by going to `localhost:8181/#/databases/default/executabeStart`. Specify the path to your `.exe` application in the field.

![](../../.gitbook/assets/capture-1.PNG)

## Database configuration <a id="database-configuration"></a>

Access the database settings by going to `localhost:8181/#/databases/default/settings`. The available settings are:

* Database port - 8080 by default
* Scheduler count, defines the degree of parallelization - the default value is the number of available logical CPU cores. The max number of schedulers is 31. Running two code hosts with 16 schedulers each will fail since it's more than 31.
* Chunks, for advanced users, should not be modified for most databases - 65536 by default

![](https://blobscdn.gitbook.com/v0/b/gitbook-28427.appspot.com/o/assets%2Fstarcounter%2Fe48b74d0-8680-11e7-9944-1f85270462c6%2Fe5a151a0-8680-11e7-9944-1f85270462c6%2F7.png?generation=1503327412884434&alt=media)

## Log <a id="log"></a>

Go to `localhost:8181/#/server/log` to see debug steps, notices, warnings, and errors. Sort the log by "Source" to see the behavior of specific components.

![](https://blobscdn.gitbook.com/v0/b/gitbook-28427.appspot.com/o/assets%2Fstarcounter%2Fe48b74d0-8680-11e7-9944-1f85270462c6%2Fe5a178b0-8680-11e7-9944-1f85270462c6%2F8.png?generation=1503327413042646&alt=media)

## Network <a id="network"></a>

Go to `localhost:8181/#/server/network` to see internal environment, the information about network facilities for Starcounter installation that comes with the network gateway.

![](https://blobscdn.gitbook.com/v0/b/gitbook-28427.appspot.com/o/assets%2Fstarcounter%2Fe48b74d0-8680-11e7-9944-1f85270462c6%2Fe5a19fc0-8680-11e7-9944-1f85270462c6%2F9.png?generation=1503327410916449&alt=media)

## Server configuration <a id="server-configuration"></a>

The system port and gateway port can be changed at `localhost:8181/#/server/settings`.

![](../../.gitbook/assets/changing_ports%20%281%29.png)

The "Allow Remote Access" option determines if the Administrator accepts requests from outside localhost. If it's set to "yes", any other machine in the same network can access the Administrator and if it's "no", then the Administrator will only accept requests from localhost. The default value is "no".

