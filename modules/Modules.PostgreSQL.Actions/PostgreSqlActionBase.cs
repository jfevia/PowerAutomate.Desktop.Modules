using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Npgsql;

namespace PowerAutomate.Desktop.Modules.PostgreSQL.Actions;

/// <summary>
///     Converts Npgsql and validation failures into desktop-flow errors.
/// </summary>
public abstract class PostgreSqlActionBase : ActionBase
{
    /// <summary>
    ///     Performs the action-specific SQL operation.
    /// </summary>
    /// <param name="context">The desktop-flow action context.</param>
    protected abstract void Run(ActionContext context);

    /// <summary>
    ///     Runs the operation with a stable failure category.
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
        catch (NpgsqlException exception)
        {
            throw new ActionException(ErrorCodes.Database, exception.Message, exception);
        }
    }

    /// <summary>
    ///     Binds parameters before opening a database connection.
    /// </summary>
    /// <param name="sql">The SQL containing named placeholders.</param>
    /// <param name="parameters">Optional Name and Value parameter rows.</param>
    /// <param name="timeoutSeconds">The positive command timeout.</param>
    /// <returns>A command ready for execution.</returns>
    protected NpgsqlCommand CreateCommand(string? sql, DataTable? parameters, int timeoutSeconds)
    {
        if (string.IsNullOrWhiteSpace(sql) || timeoutSeconds <= 0)
        {
            throw new ArgumentException("A SQL statement and a positive timeout are required.");
        }

        var values = new List<NpgsqlParameter>();
        if (parameters != null)
        {
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

                var parameter = new NpgsqlParameter(name, row["Value"]);
                values.Add(parameter);
            }
        }

        var command = new NpgsqlCommand(sql!)
        {
            CommandTimeout = timeoutSeconds
        };
        command.Parameters.AddRange(values.ToArray());
        return command;
    }

    /// <summary>
    ///     Rejects missing connection settings before creating a command.
    /// </summary>
    /// <param name="connectionString">The connection settings supplied by the flow.</param>
    /// <returns>A nonblank PostgreSQL connection string.</returns>
    protected string RequireConnectionString(string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("A PostgreSQL connection string is required.", nameof(connectionString));
        }

        return connectionString!;
    }
}
