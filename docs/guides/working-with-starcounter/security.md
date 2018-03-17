# Security

## Starcounter service account

The Starcounter service account is by default set to [LocalSystem](https://msdn.microsoft.com/en-us/library/windows/desktop/ms684190%28v=vs.85%29.aspx). This means that any local user can run any code under the Starcounter service account. Among other things it means:

1. Any local user can read, write to or write from any database.
2. An administrator should be prudent about the account he assigns to the Starcounter service since running it under LocalSystem means that every local user can gain administrative rights.



