using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace PowerAutomate.Desktop.Modules.RabbitMQ.Actions;

/// <summary>
///     Forwards channel operations to the RabbitMQ SDK.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class RabbitMqClient : IRabbitMqClient
{
    private readonly IConnection connection;
    private readonly IChannel channel;

    /// <summary>
    ///     Owns both transport resources needed to settle deliveries.
    /// </summary>
    /// <param name="connection">The broker connection.</param>
    /// <param name="channel">The broker channel.</param>
    public RabbitMqClient(IConnection connection, IChannel channel)
    {
        this.connection = connection;
        this.channel = channel;
    }

    /// <summary>
    ///     Confirms processing only after the flow completes its work.
    /// </summary>
    /// <param name="deliveryTag">The broker's delivery tag.</param>
    /// <param name="cancellationToken">Cancels the acknowledgement request.</param>
    /// <returns>The broker operation.</returns>
    public async Task AcknowledgeAsync(ulong deliveryTag, CancellationToken cancellationToken)
    {
        await channel.BasicAckAsync(deliveryTag, false, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    ///     Releases both broker resources even if channel disposal fails.
    /// </summary>
    /// <param name="cancellationToken">Cancels before shutdown begins.</param>
    /// <returns>The completed disposal operation.</returns>
    public async Task CloseAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            await channel.DisposeAsync().ConfigureAwait(false);
        }
        finally
        {
            await connection.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    ///     Publishes with mandatory routing and broker confirmations.
    /// </summary>
    /// <param name="request">The destination and message bytes.</param>
    /// <param name="cancellationToken">Cancels the publish request.</param>
    /// <returns>The broker-confirmed operation.</returns>
    public async Task PublishAsync(RabbitMqPublishRequest request, CancellationToken cancellationToken)
    {
        var properties = new BasicProperties
        {
            Persistent = request.IsPersistent,
            ContentType = "text/plain",
            ContentEncoding = "utf-8"
        };
        await channel.BasicPublishAsync(request.Exchange, request.RoutingKey, true, properties, request.Body, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    ///     Copies a polled payload before the broker reuses its memory.
    /// </summary>
    /// <param name="queue">The existing queue to poll once.</param>
    /// <param name="cancellationToken">Cancels the broker request.</param>
    /// <returns>A delivery or null when no message is available.</returns>
    public async Task<RabbitMqDelivery?> ReceiveAsync(string queue, CancellationToken cancellationToken)
    {
        var result = await channel.BasicGetAsync(queue, false, cancellationToken).ConfigureAwait(false);
        if (result is null)
        {
            return null;
        }

        var body = result.Body.ToArray();
        return new RabbitMqDelivery(result.DeliveryTag, body, result.Redelivered);
    }

    /// <summary>
    ///     Rejects a delivery without requeue unless explicitly requested.
    /// </summary>
    /// <param name="deliveryTag">The broker's delivery tag.</param>
    /// <param name="allowRequeue">Whether redelivery is allowed.</param>
    /// <param name="cancellationToken">Cancels the rejection request.</param>
    /// <returns>The broker operation.</returns>
    public async Task RejectAsync(ulong deliveryTag, bool allowRequeue, CancellationToken cancellationToken)
    {
        await channel.BasicNackAsync(deliveryTag, false, allowRequeue, cancellationToken).ConfigureAwait(false);
    }
}
