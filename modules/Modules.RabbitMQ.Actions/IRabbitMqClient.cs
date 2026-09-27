using System.Threading;
using System.Threading.Tasks;

namespace PowerAutomate.Desktop.Modules.RabbitMQ.Actions;

/// <summary>
///     Defines the broker operations required by desktop-flow actions.
/// </summary>
public interface IRabbitMqClient
{
    /// <summary>
    ///     Acknowledges a delivery only after the flow has processed it.
    /// </summary>
    /// <param name="deliveryTag">The tag assigned by the broker.</param>
    /// <param name="cancellationToken">Cancels the broker request.</param>
    /// <returns>The broker operation.</returns>
    Task AcknowledgeAsync(ulong deliveryTag, CancellationToken cancellationToken);

    /// <summary>
    ///     Closes both the channel and connection.
    /// </summary>
    /// <param name="cancellationToken">Cancels before shutdown starts.</param>
    /// <returns>The broker operation.</returns>
    Task CloseAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     Publishes a message and waits for broker confirmation.
    /// </summary>
    /// <param name="request">The destination and binary message to publish.</param>
    /// <param name="cancellationToken">Cancels the broker request.</param>
    /// <returns>The confirmed publish operation.</returns>
    Task PublishAsync(RabbitMqPublishRequest request, CancellationToken cancellationToken);

    /// <summary>
    ///     Polls an existing queue once without acknowledging a delivery.
    /// </summary>
    /// <param name="queue">The name of an existing queue.</param>
    /// <param name="cancellationToken">Cancels the broker request.</param>
    /// <returns>One delivery or null when the queue is empty.</returns>
    Task<RabbitMqDelivery?> ReceiveAsync(string queue, CancellationToken cancellationToken);

    /// <summary>
    ///     Rejects a delivery, with requeue controlled by the caller.
    /// </summary>
    /// <param name="deliveryTag">The tag assigned by the broker.</param>
    /// <param name="allowRequeue">Whether the broker may redeliver it.</param>
    /// <param name="cancellationToken">Cancels the broker request.</param>
    /// <returns>The broker operation.</returns>
    Task RejectAsync(ulong deliveryTag, bool allowRequeue, CancellationToken cancellationToken);
}
