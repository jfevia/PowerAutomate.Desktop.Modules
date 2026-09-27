# SQLite actions

Use these actions for local SQLite databases without installing an ODBC driver. The module CAB includes the native SQLite library for 32-bit and 64-bit Windows.

| Action | Purpose |
| --- | --- |
| Query SQLite database | Run a query against an existing database and return a DataTable. |
| Execute SQLite statement | Run a statement and return the number of affected rows. |

Set **Database path** to an absolute file path. An existing file is required unless **Create database if missing** is enabled on *Execute SQLite statement*; the default is **off**. The connection closes after each action so the file is not held open between actions.

For bound parameters, pass a DataTable with columns `Name` (text) and `Value` (any supported scalar or database null). Include the placeholder prefix in `Name`, such as `$id`, and use the same placeholder in SQL:

| Name | Value |
| --- | --- |
| `$id` | `42` |

For example, `SELECT name FROM records WHERE id = $id` uses the table above. Do not build SQL by concatenating untrusted values. Standard SQLite databases are not encrypted; do not store secrets in plaintext.
