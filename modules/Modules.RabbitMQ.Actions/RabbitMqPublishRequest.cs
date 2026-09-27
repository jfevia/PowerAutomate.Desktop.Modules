namespace PowerAutomate.Desktop.Modules.RabbitMQ.Actions;

/// <summary>
///     Keeps the destination and payload of one broker publish together.
/// </summary>
public sealed class RabbitMqPublishRequest
{
    /// <summary>
    ///     Contains the binary payload to publish.
    /// </summary>
    public byte[] Body { get; }

    /// <summary>
    ///     Targets an exchange; empty selects the default exchange.
    /// </summary>
    public string Exchange { get; }

    /// <summary>
    ///     Indicates whether the message is marked persistent.
    /// </summary>
    public bool IsPersistent { get; }

    /// <summary>
    ///     Routes the message to a queue or binding.
    /// </summary>
    public string RoutingKey { get; }

    /// <summary>
    ///     Captures the payload before asynchronous publication.
    /// </summary>
    /// <param name="exchange">The target exchange.</param>
    /// <param name="routingKey">The queue or binding key.</param>
    /// <param name="body">The binary payload to publish.</param>
    /// <param name="isPersistent">Whether the message is marked persistent.</param>
    public RabbitMqPublishRequest(string exchange, string routingKey, byte[] body, bool isPersistent)
    {
        Body = body;
        Exchange = exchange;
        IsPersistent = isPersistent;
        RoutingKey = routingKey;
    }
}
