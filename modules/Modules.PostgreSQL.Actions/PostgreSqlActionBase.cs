using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Npgsql;

namespace PowerAutomate.Desktop.Modules.PostgreSQL.Actions;

public abstract class PostgreSqlActionBase : ActionBase
{
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

    protected abstract void Run(ActionContext context);

    protected static string RequireConnectionString(string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("A PostgreSQL connection string is required.", nameof(connectionString));
        }

        return connectionString!;
    }

    protected static NpgsqlCommand CreateCommand(string sql, DataTable? parameters, int timeoutSeconds)
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
                var name = row["Name"] as string;
                if (string.IsNullOrWhiteSpace(name))
                {
                    throw new ArgumentException("Each parameter must have a name.", nameof(parameters));
                }

                values.Add(new NpgsqlParameter(name, row["Value"]));
            }
        }

        var command = new NpgsqlCommand(sql) { CommandTimeout = timeoutSeconds };
        command.Parameters.AddRange(values.ToArray());
        return command;
    }
}
