namespace PowerAutomate.Desktop.Modules.PostgreSQL.Actions.Tests;

/// <summary>
///     Replaces the external Npgsql adapter in query tests.
/// </summary>
public sealed class StubQueryAction : QueryAction
{
    /// <summary>
    ///     Supplies a hand-written database client.
    /// </summary>
    /// <param name="client">The client to use instead of PostgreSQL.</param>
    public StubQueryAction(IPostgreSqlClient? client) : base(client)
    {
    }
}
