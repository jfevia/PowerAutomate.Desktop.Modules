using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Enums;

namespace PowerAutomate.Desktop.Modules.RabbitMQ.Actions;

/// <summary>
///     Keeps an unacknowledged delivery tied to its originating connection.
/// </summary>
[Type(FriendlyName = nameof(RabbitMqMessage) + "_FriendlyName",
      FriendlyNamePlural = nameof(RabbitMqMessage) + "_FriendlyNamePlural",
      DefaultPropertyVisibility = Visibility.Visible)]
public sealed class RabbitMqMessage
{
    private readonly RabbitMqConnection connection;
    private readonly ulong deliveryTag;
    private readonly SemaphoreSlim settlementGate;

    /// <summary>
    ///     Preserves binary message bodies without assuming text encoding.
    /// </summary>
    [Property]
    public string BodyBase64 { get; }

    /// <summary>
    ///     Indicates the broker has redelivered this message.
    /// </summary>
    [Property]
    public bool IsRedelivered { get; }

    /// <summary>
    ///     Prevents a delivery from being acknowledged or rejected twice.
    /// </summary>
    [Property]
    public bool IsSettled { get; private set; }

    /// <summary>
    ///     Identifies the queue from which this message was polled.
    /// </summary>
    [Property]
    public string Queue { get; }

    /// <summary>
    ///     Records a delivery and prepares it for explicit settlement.
    /// </summary>
    /// <param name="connection">The channel that owns the delivery tag.</param>
    /// <param name="queue">The queue the message came from.</param>
    /// <param name="delivery">The copied binary payload and broker metadata.</param>
    public RabbitMqMessage(RabbitMqConnection? connection, string? queue, RabbitMqDelivery delivery)
    {
        if (connection is null)
        {
            throw new ArgumentNullException(nameof(connection));
        }

        if (queue is null)
        {
            throw new ArgumentNullException(nameof(queue));
        }

        this.connection = connection;
        deliveryTag = delivery.DeliveryTag;
        settlementGate = new SemaphoreSlim(1, 1);
        BodyBase64 = Convert.ToBase64String(delivery.Body);
        IsRedelivered = delivery.IsRedelivered;
        IsSettled = false;
        Queue = queue;
    }

    /// <summary>
    ///     Settles at most once, without acknowledging a broker failure.
    /// </summary>
    /// <param name="isRejected">Whether the message should be rejected.</param>
    /// <param name="allowRequeue">Whether a rejected message may be redelivered.</param>
    /// <param name="cancellationToken">Cancels the broker operation.</param>
    /// <returns>The completed settlement operation.</returns>
    public async Task SettleAsync(bool isRejected, bool allowRequeue, CancellationToken cancellationToken)
    {
        await settlementGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (IsSettled)
            {
                throw new ActionException(ErrorCodes.MessageSettled, "The message was already acknowledged or rejected.");
            }

            if (isRejected)
            {
                await connection.RejectAsync(deliveryTag, allowRequeue, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                await connection.AcknowledgeAsync(deliveryTag, cancellationToken).ConfigureAwait(false);
            }

            IsSettled = true;
        }
        finally
        {
            settlementGate.Release();
        }
    }

    /// <summary>
    ///     Displays the source queue without exposing the message contents.
    /// </summary>
    /// <returns>A credential-free message description.</returns>
    public override string ToString()
    {
        return $"RabbitMQ message from {Queue}";
    }
}
