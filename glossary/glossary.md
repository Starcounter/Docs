# Glossary

**Executable** is a `exe` file created by building a Starcounter application.

**Source file** is a `cs` file, which contains source code for main entry of a Starcounter application.

**Program** is either **executable** or **source file**.

**Application** is the Starcounter application, which includes code for running server and web related code.

**Database** has two meaning. On the high level it includes data models, data and all executables loaded for the database. On the low level it does not include executables, i.e., **engine** or **host**. It includes only data models and data.

**Engine** or **Host** is a process having all executables loaded for a database or a AppDomain.

**Server** is the Starcounter server (previously personal server). It is the name of the processes started by `scservice.exe`, which includes the gateway and `scadminserver.exe`. The `scadminserver.exe` process exposes all administrative functionality as a REST API that governs all database and codehost specific process management.