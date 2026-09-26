using System;
using System.Data;
using System.IO;
using Microsoft.Data.Sqlite;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;

namespace PowerAutomate.Desktop.Modules.SQLite.Actions;

public abstract class SQLiteActionBase : ActionBase
{
    static SQLiteActionBase() => SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_winsqlite3());

    public override void Execute(ActionContext context)
    {
        try
        {
            Run(context);
        }
        catch (ArgumentException exception)
        {
            throw new ActionException(ErrorCodes.InvalidArgument, exception.Message, exception);
        }
        catch (SqliteException exception)
        {
            throw new ActionException(ErrorCodes.Database, exception.Message, exception);
        }
    }

    protected abstract void Run(ActionContext context);

    protected static SqliteConnection CreateConnection(string databasePath, bool createIfMissing)
    {
        if (string.IsNullOrWhiteSpace(databasePath) || !Path.IsPathRooted(databasePath) ||
            Path.GetPathRoot(databasePath)!.Length < 3)
        {
            throw new ArgumentException("An absolute database file path is required.", nameof(databasePath));
        }

        var options = new SqliteConnectionStringBuilder
        {
            DataSource = databasePath,
            Mode = createIfMissing ? SqliteOpenMode.ReadWriteCreate : SqliteOpenMode.ReadWrite,
            Pooling = false
        };
        return new SqliteConnection(options.ToString());
    }

    protected static SqliteCommand CreateCommand(SqliteConnection connection, string sql, DataTable? parameters)
    {
        if (string.IsNullOrWhiteSpace(sql))
        {
            throw new ArgumentException("A SQL statement is required.", nameof(sql));
        }

        var command = connection.CreateCommand();
        try
        {
            command.CommandText = sql;
            if (parameters == null)
            {
                return command;
            }

            if (!parameters.Columns.Contains("Name") || !parameters.Columns.Contains("Value"))
            {
                throw new ArgumentException("Parameters must have Name and Value columns.", nameof(parameters));
            }

            foreach (DataRow row in parameters.Rows)
            {
                var name = row["Name"] as string;
                if (string.IsNullOrWhiteSpace(name))
                {
                    throw new ArgumentException("Each parameter must have a name.", nameof(parameters));
                }

                command.Parameters.AddWithValue(name, row["Value"]);
            }

            return command;
        }
        catch (ArgumentException)
        {
            command.Dispose();
            throw;
        }
    }
}
