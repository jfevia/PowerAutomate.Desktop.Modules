# PostgreSQL actions

Use Npgsql to run PostgreSQL queries and statements with bound parameters. Connections are opened and released for each action; Npgsql pools physical connections by default.

| Action | Purpose |
| --- | --- |
| Query PostgreSQL | Return matching rows as a DataTable. |
| Execute PostgreSQL statement | Run a statement and return the number of affected rows. |

Pass a Npgsql **Connection string**, **SQL**, and optionally a DataTable of parameters with `Name` (text) and `Value` (scalar or database null) columns. Include the placeholder prefix in `Name`, such as `@id`, matching the SQL:

| Name | Value |
| --- | --- |
| `@id` | `42` |

For example, `SELECT name FROM records WHERE id = @id` uses the table above. The command timeout defaults to 30 seconds and must be positive. Use a protected variable for connection credentials; do not concatenate untrusted values into SQL. Transactions across separate actions are not supported.

This module uses Npgsql 8.x, the last major version supporting Power Automate for desktop's .NET Framework 4.7.2 target.
