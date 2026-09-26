namespace PowerAutomate.Desktop.Modules.PostgreSQL.Actions;

/// <summary>
///     Identifies PostgreSQL failures handled by desktop flows.
/// </summary>
public static class ErrorCodes
{
    /// <summary>
    ///     Reports a database or connection failure.
    /// </summary>
    public const string Database = "DatabaseError";

    /// <summary>
    ///     Reports an invalid connection, query, timeout, or parameter.
    /// </summary>
    public const string InvalidArgument = "InvalidArgumentError";
}
