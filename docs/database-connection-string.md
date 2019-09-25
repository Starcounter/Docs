# Database Connection String

**Note**: Starcounter 3.0.0 is currently in preview stage. This API might be changed in the future releases without backwards compatibility.

## Connection String options

| Name           | Type     | Required | Default Value       | Description                                                          |
|----------------|----------|----------|---------------------|----------------------------------------------------------------------|
| `Database`     | `string` | True     | None                | Absolute or relative database directory path.                        |
| `OpenMode`     | `enum`   | False    | `CreateIfNotExists` | Specifies database creation strategy.                                |
| `StartMode`    | `enum`   | False    | `StartIfNotRunning` | Specifies database startup strategy.                                 |
| `StopMode`     | `enum`   | False    | `IfWeStarted`       | Specifies database shut down strategy.                               |
| `ContextCount` | `int`    | False    | 2 - 24              | Specifies number of database contexts allocated for this connection. |

### The `DatabaseOpenMode` enumeration values

- `Open` - Opens the database if it exist.
- `CreateIfNotExists` - Opens the database if it exist, and creates one if it doesn't.

### The `DatabaseStartMode` enumeration values

- `RequireStarted` - Expects database processes to be running, and won't attempt starting them.
- `StartIfNotRunning` - Connects if database processes are already running, and start them first if they are not.
- `StartExclusive` - Expects no database processes to be running, and start them if they are not. Refuse to connect if database processes are already running.

### The `DatabaseStopMode` enumeration values

- `IfWeStarted` - Stops database processes if they where started by the current application.
- `Never` - Leaves database processes running, even after the application shuts down and even if the application started them.

### The `ContextCount` option

Starcounter has maximum of `31` database contexts available for allocation between multiple processes.
By default, Starcounter application tries to allocate double as many contexts as CPU cores available, but minimum `2` and maximum `24`.

The ideal configuration is to have a database context per CPU core.
Having more database contexts than CPU cores might increase performance in certain concurrent database access workloads.
Having less database contexts than CPU cores might decrease performance of concurrent database access during high concurrent load.

Use `ContextCount` option to manually adjust database contexts allocation.

## Connection String sample

```
Database=./.database/Sample;OpenMode=CreateIfNotExists;StartMode=StartIfNotRunning;StopMode=IfWeStarted;ContextCount=10
```