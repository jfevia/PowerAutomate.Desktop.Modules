using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Enums;

namespace PowerAutomate.Desktop.Modules.RabbitMQ.Actions;

/// <summary>
///     Serializes access to one RabbitMQ channel across desktop-flow actions.
/// </summary>
[Type(FriendlyName = nameof(RabbitMqConnection) + "_FriendlyName",
      FriendlyNamePlural = nameof(RabbitMqConnection) + "_FriendlyNamePlural",
      DefaultPropertyVisibility = Visibility.Visible)]
public sealed class RabbitMqConnection
{
    private readonly IRabbitMqClient client;
    private readonly SemaphoreSlim channelGate;

    /// <summary>
    ///     Displays only the broker endpoint, never URI credentials.
    /// </summary>
    [Property]
    public string Endpoint { get; }

    /// <summary>
    ///     Indicates whether the broker channel has been released.
    /// </summary>
    [Property]
    public bool IsClosed { get; private set; }

    /// <summary>
    ///     Owns a broker client shared between actions.
    /// </summary>
    /// <param name="client">The broker client used by this connection.</param>
    /// <param name="endpoint">The broker endpoint safe to display.</param>
    public RabbitMqConnection(IRabbitMqClient? client, string? endpoint)
    {
        if (client is null)
        {
            throw new ArgumentNullException(nameof(client));
        }

        if (endpoint is null)
        {
            throw new ArgumentNullException(nameof(endpoint));
        }

        this.client = client;
        channelGate = new SemaphoreSlim(1, 1);
        Endpoint = endpoint;
        IsClosed = false;
    }

    /// <summary>
    ///     Acknowledges a processed delivery on its originating channel.
    /// </summary>
    /// <param name="deliveryTag">The channel-specific delivery tag.</param>
    /// <param name="cancellationToken">Cancels the broker request.</param>
    /// <returns>The broker operation.</returns>
    public async Task AcknowledgeAsync(ulong deliveryTag, CancellationToken cancellationToken)
    {
        await channelGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            EnsureOpen();
            await client.AcknowledgeAsync(deliveryTag, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            channelGate.Release();
        }
    }

    /// <summary>
    ///     Releases the broker resources once, requeuing unsettled deliveries.
    /// </summary>
    /// <param name="cancellationToken">Cancels the broker shutdown request.</param>
    /// <returns>The broker shutdown operation.</returns>
    public async Task CloseAsync(CancellationToken cancellationToken)
    {
        await channelGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (IsClosed)
            {
                return;
            }

            await client.CloseAsync(cancellationToken).ConfigureAwait(false);
            IsClosed = true;
        }
        finally
        {
            channelGate.Release();
        }
    }

    /// <summary>
    ///     Publishes through a channel with broker confirmations enabled.
    /// </summary>
    /// <param name="request">The destination and binary payload.</param>
    /// <param name="cancellationToken">Cancels the broker request.</param>
    /// <returns>The confirmed broker operation.</returns>
    public async Task PublishAsync(RabbitMqPublishRequest request, CancellationToken cancellationToken)
    {
        await channelGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            EnsureOpen();
            await client.PublishAsync(request, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            channelGate.Release();
        }
    }

    /// <summary>
    ///     Polls once without acknowledging the returned delivery.
    /// </summary>
    /// <param name="queue">The name of the queue to poll.</param>
    /// <param name="cancellationToken">Cancels the broker request.</param>
    /// <returns>A delivery or null if the queue is empty.</returns>
    public async Task<RabbitMqDelivery?> ReceiveAsync(string queue, CancellationToken cancellationToken)
    {
        await channelGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            EnsureOpen();
            return await client.ReceiveAsync(queue, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            channelGate.Release();
        }
    }

    /// <summary>
    ///     Rejects a delivery with explicit requeue behavior.
    /// </summary>
    /// <param name="deliveryTag">The channel-specific delivery tag.</param>
    /// <param name="allowRequeue">Whether the broker may redeliver the message.</param>
    /// <param name="cancellationToken">Cancels the broker request.</param>
    /// <returns>The broker operation.</returns>
    public async Task RejectAsync(ulong deliveryTag, bool allowRequeue, CancellationToken cancellationToken)
    {
        await channelGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            EnsureOpen();
            await client.RejectAsync(deliveryTag, allowRequeue, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            channelGate.Release();
        }
    }

    /// <summary>
    ///     Displays only the broker endpoint.
    /// </summary>
    /// <returns>The credential-free endpoint.</returns>
    public override string ToString()
    {
        return Endpoint;
    }

    private void EnsureOpen()
    {
        if (IsClosed)
        {
            throw new ObjectDisposedException(nameof(RabbitMqConnection));
        }
    }
}
