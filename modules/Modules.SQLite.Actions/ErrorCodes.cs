namespace PowerAutomate.Desktop.Modules.SQLite.Actions;

/// <summary>
///     Identifies errors that desktop flows can handle separately.
/// </summary>
public static class ErrorCodes
{
    /// <summary>
    ///     Reports a failure returned by SQLite.
    /// </summary>
    public const string Database = "DatabaseError";

    /// <summary>
    ///     Reports invalid database input before a connection opens.
    /// </summary>
    public const string InvalidArgument = "InvalidArgumentError";
}
