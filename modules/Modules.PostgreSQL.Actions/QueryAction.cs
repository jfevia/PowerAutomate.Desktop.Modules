using System;
using System.ComponentModel;
using System.Data;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.PostgreSQL.Actions;

/// <summary>
///     Returns rows from a parameterized PostgreSQL query.
/// </summary>
[Action(Id = "Query")]
[Throws(ErrorCodes.InvalidArgument)]
[Throws(ErrorCodes.Database)]
public class QueryAction : PostgreSqlActionBase
{
    private readonly IPostgreSqlClient client;

    /// <summary>
    ///     Supplies database connection options to Npgsql.
    /// </summary>
    [InputArgument(Order = 1, Required = true)]
    public string? ConnectionString { get; set; }

    /// <summary>
    ///     Binds Name and Value rows to SQL placeholders.
    /// </summary>
    [InputArgument(Order = 3, Required = false)]
    public DataTable? Parameters { get; set; }

    /// <summary>
    ///     Contains rows only after the query succeeds.
    /// </summary>
    [OutputArgument]
    public DataTable Result { get; set; }

    /// <summary>
    ///     Supplies the SQL query containing named placeholders.
    /// </summary>
    [InputArgument(Order = 2, Required = true, Multiline = true)]
    public string? Sql { get; set; }

    /// <summary>
    ///     Bounds query execution time in seconds.
    /// </summary>
    [InputArgument(Order = 4)]
    [DefaultValue(30)]
    public int TimeoutSeconds { get; set; }

    /// <summary>
    ///     Installs the Npgsql client for desktop-flow execution.
    /// </summary>
    public QueryAction()
    {
        client = new PostgreSqlClient();
        ConnectionString = null;
        Parameters = null;
        Result = new DataTable();
        Sql = null;
        TimeoutSeconds = 30;
    }

    /// <summary>
    ///     Injects a client so tests need no PostgreSQL server.
    /// </summary>
    /// <param name="client">The database client used by the action.</param>
    protected QueryAction(IPostgreSqlClient? client)
    {
        if (client is null)
        {
            throw new ArgumentNullException(nameof(client));
        }

        this.client = client;
        ConnectionString = null;
        Parameters = null;
        Result = new DataTable();
        Sql = null;
        TimeoutSeconds = 30;
    }

    /// <summary>
    ///     Materializes query results after validating inputs.
    /// </summary>
    /// <param name="context">The desktop-flow action context.</param>
    protected override void Run(ActionContext context)
    {
        var connectionString = RequireConnectionString(ConnectionString);
        using var command = CreateCommand(Sql, Parameters, TimeoutSeconds);
        Result = client.Query(connectionString, command);
    }
}
