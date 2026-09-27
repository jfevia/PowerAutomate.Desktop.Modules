using System.Data;
using Npgsql;

namespace PowerAutomate.Desktop.Modules.PostgreSQL.Actions;

/// <summary>
///     Defines database operations used by the desktop-flow actions.
/// </summary>
public interface IPostgreSqlClient
{
    /// <summary>
    ///     Executes a prepared statement and returns the affected row count.
    /// </summary>
    /// <param name="connectionString">The PostgreSQL connection settings.</param>
    /// <param name="command">The bound statement to execute.</param>
    /// <returns>The count reported by PostgreSQL.</returns>
    int Execute(string connectionString, NpgsqlCommand command);

    /// <summary>
    ///     Executes a prepared query and materializes its rows.
    /// </summary>
    /// <param name="connectionString">The PostgreSQL connection settings.</param>
    /// <param name="command">The bound query to execute.</param>
    /// <returns>A table containing query results.</returns>
    DataTable Query(string connectionString, NpgsqlCommand command);
}
