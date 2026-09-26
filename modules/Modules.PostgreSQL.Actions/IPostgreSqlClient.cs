using System.Data;
using Npgsql;

namespace PowerAutomate.Desktop.Modules.PostgreSQL.Actions;

internal interface IPostgreSqlClient
{
    DataTable Query(string connectionString, NpgsqlCommand command);

    int Execute(string connectionString, NpgsqlCommand command);
}
