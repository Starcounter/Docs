# Migrating from 2.2 to 2.3+

## The StarDump tool

In Starcounter 2.3 and 2.4 the old `staradmin unload` and `staradmin reload` commands have been replaced with a new tool - [StarDump](https://github.com/Starcounter/StarDump).

The tool is available from the installation folder starting from `2.4.0.369` and `2.3.0.5427` Starcounter versions. By default the StarDump tool is located in `C:\Program Files\Starcounter\stardump` folder.

The StarDump tool unloads Starcounter database into an SQLite dump. The dump file can be viewed or modified by any tool compatible with SQLite. The recommended tool is - [DB Browser for SQLite](http://sqlitebrowser.org/).

### Usage

**Unload database**

```
StarDump.exe unload --database [DatabaseName] --dump [FilePath]
```

Example

```
StarDump.exe unload --database default --dump C:\Temp\default.sqlite3
```

**Reload database**

```
StarDump.exe reload --database [DatabaseName] --dump [FilePath]
```

Example

```
StarDump.exe reload --database default --dump C:\Temp\default.sqlite3
```

**Note:** the database should be dropped and created prior to reload.

```
staradmin -d=default delete --force db
staradmin -d=default new db DefaultUserHttpPort=8080
```

## Migrating from Starcounter 2.2

The [StarDump.Migrator.2.2](https://github.com/Starcounter/StarDump.Migrator.2.2) app produces an SQLite dump from Starcounter 2.2 database.

Things to know about `StarDump.Migrator.2.2`.

- It is only compatible with Starcounter 2.2 and won't work with 2.3 or 2.4.
- It is a Starcounter app which has to be started the same way as any other apps do.
- It is not able to reload database dump, only unload.
- All the apps defining `[Database]` classes should be started prior to unload with `StarDump.Migrator.2.2` app.

### Usage

1. Start database.
2. Start all of the applications which contains classes marked with `[Database]` attribute.
3. Start `StarDump.Migrator.2.2`.
4. Execute `http://localhost:8080/StarDump/Migrator22/Unload/{FilePath}`.

**Example**

```
http://localhost:8080/StarDump/Migrator22/Unload/D:/Temp/default.sqlite3
```

The produced SQLite dump is compatible with the StarDump tool and can be used to reload database into Starcounter 2.3 or higher.

**Note**: the tools are in alpha stage and crashes are expected. Please use the [StarDump](https://github.com/Starcounter/StarDump) and [StarDump.Migrator.2.2](https://github.com/Starcounter/StarDump.Migrator.2.2) repositories to report any issue found.
