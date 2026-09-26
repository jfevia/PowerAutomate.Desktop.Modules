using System;
using System.Data;
using System.IO;
using Microsoft.Data.Sqlite;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;

namespace PowerAutomate.Desktop.Modules.SQLite.Actions;

/// <summary>
///     Translates SQLite and validation failures into desktop-flow errors.
/// </summary>
public abstract class SQLiteActionBase : ActionBase
{
    /// <summary>
    ///     Performs the action-specific database operation.
    /// </summary>
    /// <param name="context">The desktop-flow action context.</param>
    protected abstract void Run(ActionContext context);

    static SQLiteActionBase()
    {
        var provider = new SQLitePCL.SQLite3Provider_winsqlite3();
        SQLitePCL.raw.SetProvider(provider);
    }

    /// <summary>
    ///     Runs the operation and preserves its failure category.
    /// </summary>
    /// <param name="context">The desktop-flow action context.</param>
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

    /// <summary>
    ///     Validates and binds parameters before opening the database.
    /// </summary>
    /// <param name="connection">The connection to use for the command.</param>
    /// <param name="sql">The SQL containing named placeholders.</param>
    /// <param name="parameters">Optional Name and Value parameter rows.</param>
    /// <returns>A command ready for execution.</returns>
    protected SqliteCommand CreateCommand(SqliteConnection connection, string sql, DataTable? parameters)
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
                if (row["Name"] is not string name || string.IsNullOrWhiteSpace(name))
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

    /// <summary>
    ///     Creates a connection without opening the database file.
    /// </summary>
    /// <param name="databasePath">The absolute path to the database file.</param>
    /// <param name="allowCreate">Whether a missing database may be created.</param>
    /// <returns>An unopened, unpooled SQLite connection.</returns>
    protected SqliteConnection CreateConnection(string databasePath, bool allowCreate)
    {
        if (string.IsNullOrWhiteSpace(databasePath) || !Path.IsPathRooted(databasePath) ||
            Path.GetPathRoot(databasePath)!.Length < 3)
        {
            throw new ArgumentException("An absolute database file path is required.", nameof(databasePath));
        }

        var options = new SqliteConnectionStringBuilder
        {
            DataSource = databasePath,
            Mode = allowCreate ? SqliteOpenMode.ReadWriteCreate : SqliteOpenMode.ReadWrite,
            Pooling = false
        };
        return new SqliteConnection(options.ToString());
    }
}
