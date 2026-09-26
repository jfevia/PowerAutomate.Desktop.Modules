using System;
using System.ComponentModel;
using System.Data;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.PostgreSQL.Actions;

[Action(Id = "Execute")]
[Throws(ErrorCodes.InvalidArgument)]
[Throws(ErrorCodes.Database)]
public class ExecuteAction : PostgreSqlActionBase
{
    private readonly IPostgreSqlClient client;

    public ExecuteAction() : this(new PostgreSqlClient())
    {
    }

    internal ExecuteAction(IPostgreSqlClient client) =>
        this.client = client ?? throw new ArgumentNullException(nameof(client));

    [InputArgument(Order = 1, Required = true)]
    public string ConnectionString { get; set; } = null!;

    [InputArgument(Order = 2, Required = true, Multiline = true)]
    public string Sql { get; set; } = null!;

    [InputArgument(Order = 3, Required = false)]
    public DataTable? Parameters { get; set; }

    [InputArgument(Order = 4)]
    [DefaultValue(30)]
    public int TimeoutSeconds { get; set; } = 30;

    [OutputArgument]
    public int AffectedRows { get; set; }

    protected override void Run(ActionContext context)
    {
        var connectionString = RequireConnectionString(ConnectionString);
        using var command = CreateCommand(Sql, Parameters, TimeoutSeconds);
        AffectedRows = client.Execute(connectionString, command);
    }
}
