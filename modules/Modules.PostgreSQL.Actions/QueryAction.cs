using System;
using System.ComponentModel;
using System.Data;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.PostgreSQL.Actions;

[Action(Id = "Query")]
[Throws(ErrorCodes.InvalidArgument)]
[Throws(ErrorCodes.Database)]
public class QueryAction : PostgreSqlActionBase
{
    private readonly IPostgreSqlClient client;

    public QueryAction() : this(new PostgreSqlClient())
    {
    }

    internal QueryAction(IPostgreSqlClient client) =>
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
    public DataTable Result { get; set; } = null!;

    protected override void Run(ActionContext context)
    {
        var connectionString = RequireConnectionString(ConnectionString);
        using var command = CreateCommand(Sql, Parameters, TimeoutSeconds);
        Result = client.Query(connectionString, command);
    }
}
