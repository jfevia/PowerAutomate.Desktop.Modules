namespace PowerAutomate.Desktop.Modules.RabbitMQ.Actions;

/// <summary>
///     Holds a copied payload and its broker delivery tag.
/// </summary>
public sealed class RabbitMqDelivery
{
    /// <summary>
    ///     Contains a payload copied before the broker reuses its buffer.
    /// </summary>
    public byte[] Body { get; }

    /// <summary>
    ///     Identifies this delivery on its originating channel.
    /// </summary>
    public ulong DeliveryTag { get; }

    /// <summary>
    ///     Indicates the broker has delivered this message before.
    /// </summary>
    public bool IsRedelivered { get; }

    /// <summary>
    ///     Records one unacknowledged delivery.
    /// </summary>
    /// <param name="deliveryTag">The broker's channel-specific tag.</param>
    /// <param name="body">The copied binary payload.</param>
    /// <param name="isRedelivered">Whether the broker previously delivered it.</param>
    public RabbitMqDelivery(ulong deliveryTag, byte[] body, bool isRedelivered)
    {
        Body = body;
        DeliveryTag = deliveryTag;
        IsRedelivered = isRedelivered;
    }
}
