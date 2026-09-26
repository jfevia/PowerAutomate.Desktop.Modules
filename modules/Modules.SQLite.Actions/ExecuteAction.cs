using System.ComponentModel;
using System.Data;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.SQLite.Actions;

[Action(Id = "Execute")]
[Throws(ErrorCodes.InvalidArgument)]
[Throws(ErrorCodes.Database)]
public class ExecuteAction : SQLiteActionBase
{
    [InputArgument(Order = 1, Required = true)]
    public string DatabasePath { get; set; } = null!;

    [InputArgument(Order = 2, Required = true, Multiline = true)]
    public string Sql { get; set; } = null!;

    [InputArgument(Order = 3, Required = false)]
    public DataTable? Parameters { get; set; }

    [InputArgument(Order = 4)]
    [DefaultValue(false)]
    public bool CreateIfMissing { get; set; }

    [OutputArgument]
    public int AffectedRows { get; set; }

    protected override void Run(ActionContext context)
    {
        using var connection = CreateConnection(DatabasePath, CreateIfMissing);
        using var command = CreateCommand(connection, Sql, Parameters);
        connection.Open();
        AffectedRows = command.ExecuteNonQuery();
    }
}
