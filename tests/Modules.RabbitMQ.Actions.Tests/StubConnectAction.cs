namespace PowerAutomate.Desktop.Modules.RabbitMQ.Actions.Tests;

/// <summary>
///     Replaces broker connection setup with a test factory.
/// </summary>
public sealed class StubConnectAction : ConnectAction
{
    /// <summary>
    ///     Injects a hand-written broker client factory.
    /// </summary>
    /// <param name="clientFactory">The client factory used by the action.</param>
    public StubConnectAction(IRabbitMqClientFactory? clientFactory) : base(clientFactory)
    {
    }
}
