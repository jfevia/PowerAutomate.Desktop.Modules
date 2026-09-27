namespace PowerAutomate.Desktop.Modules.Redis.Actions.Tests;

/// <summary>
///     Injects a hand-written connection factory into the action under test.
/// </summary>
public sealed class StubConnectAction : ConnectAction
{
    /// <summary>
    ///     Replaces only the external network dependency.
    /// </summary>
    /// <param name="factory">The factory used by tests.</param>
    public StubConnectAction(IRedisClientFactory? factory) : base(factory)
    {
    }
}
