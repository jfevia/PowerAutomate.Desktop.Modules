namespace PowerAutomate.Desktop.Modules.PostgreSQL.Actions.Tests;

/// <summary>
///     Replaces the external Npgsql adapter in statement tests.
/// </summary>
public sealed class StubExecuteAction : ExecuteAction
{
    /// <summary>
    ///     Supplies a hand-written database client.
    /// </summary>
    /// <param name="client">The client to use instead of PostgreSQL.</param>
    public StubExecuteAction(IPostgreSqlClient? client) : base(client)
    {
    }
}
