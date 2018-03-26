# Database configuration

## Introduction

The database can be configured in three different ways: with the [staradmin CLI](../working-with-starcounter/staradmin-cli.md), in the [Administrator](../working-with-starcounter/administrator-web-ui.md), or directly in the configuration file.

## Configuration file

By default, the database configuration file is located inside the server directory `Databases\[DatabaseName]\[DatabaseName].db.config`. Further instructions on finding the database configuration file can be found on the [Configuration Structure](../working-with-starcounter/configuration-structure.md) page.

The options in this file can be configured either directly in the file or when creating the database with the staradmin CLI. Alternatively, a limited subset of the options can be configured from the Administrator.

For configurations regarding the network gateway, check the [Network Gateway](../network/network-gateway.md) page.

## Configuration options

Here are the most important database configuration options with their default values:

1. Should edition libraries be loaded in databases: `LoadEditionLibraries: true`
2. Should JSON responses be wrapped in application name: `WrapJsonInNamespaces: true`
3. Should applications be forced to register handlers starting with application name prefix: `EnforceURINamespaces: false`
4. Should JSON responses from multiple applications be merged: `MergeJsonSiblings: true`
5. Should the request filters be enabled \(previously middleware filters\): `RequestFiltersEnabled: true`
6. Should static files HTTP responses have a special header `X-File-Path` that contains a full path to the actual file on the server \(default value is `False`\). This HTTP header is often useful for debugging purposes, but should be disabled in production: `XFilePathHeader: true`
7. The HTTP port bound to the database: `DefaultUserHttpPort: 8080`

