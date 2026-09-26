using System.Data;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.SQLite.Actions;

/// <summary>
///     Reads an existing SQLite database without creating a file.
/// </summary>
[Action(Id = "Query")]
[Throws(ErrorCodes.InvalidArgument)]
[Throws(ErrorCodes.Database)]
public class QueryAction : SQLiteActionBase
{
    /// <summary>
    ///     Absolute path to an existing database file.
    /// </summary>
    [InputArgument(Order = 1, Required = true)]
    public string DatabasePath { get; set; }

    /// <summary>
    ///     Optional Name and Value rows used to bind SQL placeholders.
    /// </summary>
    [InputArgument(Order = 3, Required = false)]
    public DataTable? Parameters { get; set; }

    /// <summary>
    ///     Contains rows only after the query succeeds.
    /// </summary>
    [OutputArgument]
    public DataTable Result { get; set; }

    /// <summary>
    ///     SQL query whose named placeholders may be bound.
    /// </summary>
    [InputArgument(Order = 2, Required = true, Multiline = true)]
    public string Sql { get; set; }

    /// <summary>
    ///     Initializes the inputs supplied by the desktop-flow designer.
    /// </summary>
    public QueryAction()
    {
        DatabasePath = string.Empty;
        Parameters = null;
        Result = new DataTable();
        Sql = string.Empty;
    }

    /// <summary>
    ///     Executes a parameterized query on an existing database.
    /// </summary>
    /// <param name="context">The desktop-flow action context.</param>
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
