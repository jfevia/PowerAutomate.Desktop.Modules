using System.ComponentModel;
using System.Data;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.SQLite.Actions;

/// <summary>
///     Executes a SQLite statement with optional file creation.
/// </summary>
[Action(Id = "Execute")]
[Throws(ErrorCodes.InvalidArgument)]
[Throws(ErrorCodes.Database)]
public class ExecuteAction : SQLiteActionBase
{
    /// <summary>
    ///     Counts rows changed by the successful statement.
    /// </summary>
    [OutputArgument]
    public int AffectedRows { get; set; }

    /// <summary>
    ///     Prevents new database files unless explicitly enabled.
    /// </summary>
    [InputArgument(Order = 4)]
    [DefaultValue(false)]
    public bool AllowCreate { get; set; }

    /// <summary>
    ///     Absolute path to the database file.
    /// </summary>
    [InputArgument(Order = 1, Required = true)]
    public string DatabasePath { get; set; }

    /// <summary>
    ///     Optional Name and Value rows used to bind SQL placeholders.
    /// </summary>
    [InputArgument(Order = 3, Required = false)]
    public DataTable? Parameters { get; set; }

    /// <summary>
    ///     SQL statement whose named placeholders may be bound.
    /// </summary>
    [InputArgument(Order = 2, Required = true, Multiline = true)]
    public string Sql { get; set; }

    /// <summary>
    ///     Initializes safe defaults for optional inputs.
    /// </summary>
    public ExecuteAction()
    {
        AffectedRows = 0;
        AllowCreate = false;
        DatabasePath = string.Empty;
        Parameters = null;
        Sql = string.Empty;
    }

    /// <summary>
    ///     Executes a parameterized statement after validating its inputs.
    /// </summary>
    /// <param name="context">The desktop-flow action context.</param>
    protected override void Run(ActionContext context)
    {
        using var connection = CreateConnection(DatabasePath, AllowCreate);
        using var command = CreateCommand(connection, Sql, Parameters);
        connection.Open();
        AffectedRows = command.ExecuteNonQuery();
    }
}
