using System;
using System.ComponentModel;
using System.Data;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.PostgreSQL.Actions;

/// <summary>
///     Runs parameterized PostgreSQL statements without retaining connections.
/// </summary>
[Action(Id = "Execute")]
[Throws(ErrorCodes.InvalidArgument)]
[Throws(ErrorCodes.Database)]
public class ExecuteAction : PostgreSqlActionBase
{
    private readonly IPostgreSqlClient client;

    /// <summary>
    ///     Records the affected row count after successful execution.
    /// </summary>
    [OutputArgument]
    public int AffectedRows { get; set; }

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
    ///     Supplies the SQL statement containing named placeholders.
    /// </summary>
    [InputArgument(Order = 2, Required = true, Multiline = true)]
    public string? Sql { get; set; }

    /// <summary>
    ///     Bounds command execution time in seconds.
    /// </summary>
    [InputArgument(Order = 4)]
    [DefaultValue(30)]
    public int TimeoutSeconds { get; set; }

    /// <summary>
    ///     Installs the Npgsql client for desktop-flow execution.
    /// </summary>
    public ExecuteAction()
    {
        client = new PostgreSqlClient();
        AffectedRows = 0;
        ConnectionString = null;
        Parameters = null;
        Sql = null;
        TimeoutSeconds = 30;
    }

    /// <summary>
    ///     Injects a client so tests need no PostgreSQL server.
    /// </summary>
    /// <param name="client">The database client used by the action.</param>
    protected ExecuteAction(IPostgreSqlClient? client)
    {
        if (client is null)
        {
            throw new ArgumentNullException(nameof(client));
        }

        this.client = client;
        AffectedRows = 0;
        ConnectionString = null;
        Parameters = null;
        Sql = null;
        TimeoutSeconds = 30;
    }

    /// <summary>
    ///     Executes the bound statement after validating its inputs.
    /// </summary>
    /// <param name="context">The desktop-flow action context.</param>
    protected override void Run(ActionContext context)
    {
        var connectionString = RequireConnectionString(ConnectionString);
        using var command = CreateCommand(Sql, Parameters, TimeoutSeconds);
        AffectedRows = client.Execute(connectionString, command);
    }
}
