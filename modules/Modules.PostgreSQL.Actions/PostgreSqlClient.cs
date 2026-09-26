using System.Data;
using System.Diagnostics.CodeAnalysis;
using Npgsql;

namespace PowerAutomate.Desktop.Modules.PostgreSQL.Actions;

/// <summary>
///     Forwards prepared commands to the Npgsql client.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class PostgreSqlClient : IPostgreSqlClient
{
    /// <summary>
    ///     Executes a bound statement using a short-lived pooled connection.
    /// </summary>
    /// <param name="connectionString">The PostgreSQL connection settings.</param>
    /// <param name="command">The statement to execute.</param>
    /// <returns>The number of affected rows.</returns>
    public int Execute(string connectionString, NpgsqlCommand command)
    {
        using var connection = new NpgsqlConnection(connectionString);
        command.Connection = connection;
        connection.Open();
        return command.ExecuteNonQuery();
    }

    /// <summary>
    ///     Materializes query rows before the pooled connection is released.
    /// </summary>
    /// <param name="connectionString">The PostgreSQL connection settings.</param>
    /// <param name="command">The query to execute.</param>
    /// <returns>A table of result rows.</returns>
    public DataTable Query(string connectionString, NpgsqlCommand command)
    {
        using var connection = new NpgsqlConnection(connectionString);
        command.Connection = connection;
        connection.Open();
        using var reader = command.ExecuteReader();
        var result = new DataTable();
        result.Load(reader);
        return result;
    }
}
