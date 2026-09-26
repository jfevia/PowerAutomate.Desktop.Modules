using System;
using System.Threading;
using NUnit.Framework;

namespace PowerAutomate.Desktop.Modules.RabbitMQ.Actions.Tests;

/// <summary>
///     Checks message ownership, binary bodies, and unsettled state.
/// </summary>
[TestFixture]
public sealed class RabbitMqMessageTests : RabbitMqActionFixture
{
    /// <summary>
    ///     Requires the connection that owns the delivery tag.
    /// </summary>
    [Test]
    public void RabbitMqMessage_WhenConnectionIsNull_Throws()
    {
        var body = new byte[]
        {
            1
        };
        var delivery = new RabbitMqDelivery(7, body, false);
        var error = Assert.Throws<ArgumentNullException>(() =>
        {
            var message = new RabbitMqMessage(null, "queue", delivery);
            Assert.Fail($"Unexpected message: {message}");
        });

        Assert.That(error?.ParamName, Is.EqualTo("connection"));
    }

    /// <summary>
    ///     Requires the originating queue for the displayed delivery.
    /// </summary>
    [Test]
    public void RabbitMqMessage_WhenQueueIsNull_Throws()
    {
        var body = new byte[]
        {
            1
        };
        var delivery = new RabbitMqDelivery(7, body, false);
        var error = Assert.Throws<ArgumentNullException>(() =>
        {
            var message = new RabbitMqMessage(Connection, null, delivery);
            Assert.Fail($"Unexpected message: {message}");
        });

        Assert.That(error?.ParamName, Is.EqualTo("queue"));
    }

    /// <summary>
    ///     Displays a queue name without leaking the payload.
    /// </summary>
    [Test]
    public void ToString_WhenMessageIsReceived_DisplaysQueueOnly()
    {
        var message = CreateMessage();

        Assert.That(message.ToString(), Is.EqualTo("RabbitMQ message from queue"));
        Assert.That(message.IsSettled, Is.False);
    }

    /// <summary>
    ///     Leaves the message unsettled if the operation is canceled first.
    /// </summary>
    [Test]
    public void SettleAsync_WhenCanceled_LeavesMessageUnsettled()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        var message = CreateMessage();
        var task = message.SettleAsync(false, false, cancellation.Token);

        Assert.That(() => task.GetAwaiter().GetResult(), Throws.InstanceOf<OperationCanceledException>());
        Assert.That(message.IsSettled, Is.False);
    }
}
