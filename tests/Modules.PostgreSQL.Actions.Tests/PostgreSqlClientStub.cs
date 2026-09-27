using System;
using System.Data;
using Npgsql;

namespace PowerAutomate.Desktop.Modules.PostgreSQL.Actions.Tests;

/// <summary>
///     Captures prepared commands without requiring a database server.
/// </summary>
public sealed class PostgreSqlClientStub : IPostgreSqlClient
{
    /// <summary>
    ///     Determines the number of rows reported by a write.
    /// </summary>
    public int AffectedRows { get; set; }

    /// <summary>
    ///     Captures the SQL of the last command.
    /// </summary>
    public string? CommandText { get; private set; }

    /// <summary>
    ///     Captures the connection options without opening a socket.
    /// </summary>
    public string? ConnectionString { get; private set; }

    /// <summary>
    ///     Injects a server error into the next command.
    /// </summary>
    public Exception? Failure { get; set; }

    /// <summary>
    ///     Counts the parameters on the last command.
    /// </summary>
    public int ParameterCount { get; private set; }

    /// <summary>
    ///     Captures the first parameter name.
    /// </summary>
    public string? ParameterName { get; private set; }

    /// <summary>
    ///     Captures the first parameter value.
    /// </summary>
    public object? ParameterValue { get; private set; }

    /// <summary>
    ///     Supplies rows returned by a query.
    /// </summary>
    public DataTable QueryResult { get; }

    /// <summary>
    ///     Captures the timeout on the last command.
    /// </summary>
    public int TimeoutSeconds { get; private set; }

    /// <summary>
    ///     Initializes the table and recorded command state.
    /// </summary>
    public PostgreSqlClientStub()
    {
        AffectedRows = 0;
        CommandText = null;
        ConnectionString = null;
        Failure = null;
        ParameterCount = 0;
        ParameterName = null;
        ParameterValue = null;
        QueryResult = new DataTable();
        QueryResult.Columns.Add("name", typeof(string));
        TimeoutSeconds = 0;
    }

    /// <summary>
    ///     Records a write and returns its affected row count.
    /// </summary>
    /// <param name="connectionString">The selected database connection.</param>
    /// <param name="command">The prepared statement to inspect.</param>
    /// <returns>The configured affected row count.</returns>
    public int Execute(string connectionString, NpgsqlCommand command)
    {
        Record(connectionString, command);
        if (Failure is not null)
        {
            throw Failure;
        }

        return AffectedRows;
    }

    /// <summary>
    ///     Records a query and returns configured rows.
    /// </summary>
    /// <param name="connectionString">The selected database connection.</param>
    /// <param name="command">The prepared query to inspect.</param>
    /// <returns>The configured result table.</returns>
    public DataTable Query(string connectionString, NpgsqlCommand command)
    {
        Record(connectionString, command);
        if (Failure is not null)
        {
            throw Failure;
        }

        return QueryResult;
    }

    private void Record(string connectionString, NpgsqlCommand command)
    {
        ConnectionString = connectionString;
        CommandText = command.CommandText;
        TimeoutSeconds = command.CommandTimeout;
        ParameterCount = command.Parameters.Count;
        if (ParameterCount > 0)
        {
            ParameterName = command.Parameters[0].ParameterName;
            ParameterValue = command.Parameters[0].Value;
        }
    }
}
