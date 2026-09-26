using System;
using System.Threading;
using System.Threading.Tasks;

namespace PowerAutomate.Desktop.Modules.RabbitMQ.Actions.Tests;

/// <summary>
///     Records broker requests and injects deterministic failures.
/// </summary>
public sealed class RabbitMqClientStub : IRabbitMqClient
{
    /// <summary>
    ///     Signals when acknowledgement is ready to be released.
    /// </summary>
    public TaskCompletionSource<bool>? AcknowledgeStarted { get; set; }

    /// <summary>
    ///     Counts successful acknowledgements.
    /// </summary>
    public int AcknowledgedCount { get; private set; }

    /// <summary>
    ///     Identifies the last acknowledged delivery.
    /// </summary>
    public ulong? AcknowledgedTag { get; private set; }

    /// <summary>
    ///     Captures the last published payload.
    /// </summary>
    public byte[]? Body { get; private set; }

    /// <summary>
    ///     Counts successful connection closes.
    /// </summary>
    public int CloseCount { get; private set; }

    /// <summary>
    ///     Supplies the delivery returned by a poll.
    /// </summary>
    public RabbitMqDelivery? Delivery { get; set; }

    /// <summary>
    ///     Captures the last target exchange.
    /// </summary>
    public string? Exchange { get; private set; }

    /// <summary>
    ///     Injects a failure into the next broker request.
    /// </summary>
    public Exception? Failure { get; set; }

    /// <summary>
    ///     Pauses acknowledgement until tests release it.
    /// </summary>
    public TaskCompletionSource<bool>? HoldAcknowledge { get; set; }

    /// <summary>
    ///     Captures the persistent delivery-mode flag.
    /// </summary>
    public bool IsPersistent { get; private set; }

    /// <summary>
    ///     Captures whether a rejected message may be redelivered.
    /// </summary>
    public bool IsRequeued { get; private set; }

    /// <summary>
    ///     Captures the queue selected for polling.
    /// </summary>
    public string? Queue { get; private set; }

    /// <summary>
    ///     Identifies the last rejected delivery.
    /// </summary>
    public ulong? RejectedTag { get; private set; }

    /// <summary>
    ///     Captures the last routing key used for publication.
    /// </summary>
    public string? RoutingKey { get; private set; }

    /// <summary>
    ///     Initializes the broker stub in an idle state.
    /// </summary>
    public RabbitMqClientStub()
    {
        AcknowledgeStarted = null;
        AcknowledgedCount = 0;
        AcknowledgedTag = null;
        Body = null;
        CloseCount = 0;
        Delivery = null;
        Exchange = null;
        Failure = null;
        HoldAcknowledge = null;
        IsPersistent = false;
        IsRequeued = false;
        Queue = null;
        RejectedTag = null;
        RoutingKey = null;
    }

    /// <summary>
    ///     Optionally waits for a test before recording acknowledgement.
    /// </summary>
    /// <param name="deliveryTag">The delivery acknowledged by the flow.</param>
    /// <param name="cancellationToken">Cancels the broker request.</param>
    /// <returns>The acknowledgement operation.</returns>
    public async Task AcknowledgeAsync(ulong deliveryTag, CancellationToken cancellationToken)
    {
        if (Failure is not null)
        {
            throw Failure;
        }

        AcknowledgeStarted?.TrySetResult(true);
        if (HoldAcknowledge is not null)
        {
            await HoldAcknowledge.Task.ConfigureAwait(false);
        }

        cancellationToken.ThrowIfCancellationRequested();
        AcknowledgedTag = deliveryTag;
        AcknowledgedCount++;
    }

    /// <summary>
    ///     Records a successful release of the connection.
    /// </summary>
    /// <param name="cancellationToken">Cancels before the close operation.</param>
    /// <returns>The close operation.</returns>
    public Task CloseAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (Failure is not null)
        {
            throw Failure;
        }

        CloseCount++;
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Copies a message request after one asynchronous yield.
    /// </summary>
    /// <param name="request">The payload submitted by the flow.</param>
    /// <param name="cancellationToken">Cancels the broker publish.</param>
    /// <returns>The publish operation.</returns>
    public async Task PublishAsync(RabbitMqPublishRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        cancellationToken.ThrowIfCancellationRequested();
        if (Failure is not null)
        {
            throw Failure;
        }

        Body = request.Body;
        Exchange = request.Exchange;
        IsPersistent = request.IsPersistent;
        RoutingKey = request.RoutingKey;
    }

    /// <summary>
    ///     Returns a configured delivery or an empty result.
    /// </summary>
    /// <param name="queue">The queue polled by the flow.</param>
    /// <param name="cancellationToken">Cancels the broker poll.</param>
    /// <returns>The configured delivery or null.</returns>
    public Task<RabbitMqDelivery?> ReceiveAsync(string queue, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (Failure is not null)
        {
            throw Failure;
        }

        Queue = queue;
        return Task.FromResult(Delivery);
    }

    /// <summary>
    ///     Records the explicit requeue decision for a delivery.
    /// </summary>
    /// <param name="deliveryTag">The delivery rejected by the flow.</param>
    /// <param name="allowRequeue">Whether redelivery is allowed.</param>
    /// <param name="cancellationToken">Cancels the broker request.</param>
    /// <returns>The rejection operation.</returns>
    public Task RejectAsync(ulong deliveryTag, bool allowRequeue, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (Failure is not null)
        {
            throw Failure;
        }

        IsRequeued = allowRequeue;
        RejectedTag = deliveryTag;
        return Task.CompletedTask;
    }
}
