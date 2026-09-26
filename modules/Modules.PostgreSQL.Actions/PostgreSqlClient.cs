using System.Data;
using System.Diagnostics.CodeAnalysis;
using Npgsql;

namespace PowerAutomate.Desktop.Modules.PostgreSQL.Actions;

[ExcludeFromCodeCoverage]
internal sealed class PostgreSqlClient : IPostgreSqlClient
{
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

    public int Execute(string connectionString, NpgsqlCommand command)
    {
        using var connection = new NpgsqlConnection(connectionString);
        command.Connection = connection;
        connection.Open();
        return command.ExecuteNonQuery();
    }
}
