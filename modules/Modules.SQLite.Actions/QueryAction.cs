using System.Data;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.SQLite.Actions;

[Action(Id = "Query")]
[Throws(ErrorCodes.InvalidArgument)]
[Throws(ErrorCodes.Database)]
public class QueryAction : SQLiteActionBase
{
    [InputArgument(Order = 1, Required = true)]
    public string DatabasePath { get; set; } = null!;

    [InputArgument(Order = 2, Required = true, Multiline = true)]
    public string Sql { get; set; } = null!;

    [InputArgument(Order = 3, Required = false)]
    public DataTable? Parameters { get; set; }

    [OutputArgument]
    public DataTable Result { get; set; } = null!;

    protected override void Run(ActionContext context)
    {
        using var connection = CreateConnection(DatabasePath, false);
        using var command = CreateCommand(connection, Sql, Parameters);
        connection.Open();
        using var reader = command.ExecuteReader();
        var result = new DataTable();
        result.Load(reader);
        Result = result;
    }
}
