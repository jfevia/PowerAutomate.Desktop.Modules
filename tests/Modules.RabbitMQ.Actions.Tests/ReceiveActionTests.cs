using System;
using System.IO;
using NUnit.Framework;

namespace PowerAutomate.Desktop.Modules.RabbitMQ.Actions.Tests;

/// <summary>
///     Checks bounded polling and unacknowledged delivery results.
/// </summary>
[TestFixture]
public sealed class ReceiveActionTests : RabbitMqActionFixture
{
    /// <summary>
    ///     Distinguishes an empty queue from a successful delivery.
    /// </summary>
    [Test]
    public void Execute_WhenQueueIsEmpty_ReturnsNotFound()
    {
        var action = new ReceiveAction
        {
            Connection = Connection,
            Queue = "queue"
        };

        RunAction(action);

        Assert.That(Client.Queue, Is.EqualTo("queue"));
        Assert.That(action.IsFound, Is.False);
        Assert.That(action.Message, Is.Null);
    }

    /// <summary>
    ///     Preserves binary payloads and the redelivery indicator.
    /// </summary>
    [Test]
    public void Execute_WhenMessageExists_PreservesBinaryBody()
    {
        var expected = new byte[]
        {
            0,
            255,
            128
        };
        Client.Delivery = new RabbitMqDelivery(7, expected, true);
        var action = new ReceiveAction
        {
            Connection = Connection,
            Queue = "queue"
        };

        RunAction(action);

        Assert.That(action.IsFound, Is.True);
        Assert.That(action.Message?.Queue, Is.EqualTo("queue"));
        Assert.That(Convert.FromBase64String(action.Message!.BodyBase64), Is.EqualTo(expected));
        Assert.That(action.Message.IsRedelivered, Is.True);
        Assert.That(action.Message.IsSettled, Is.False);
    }

    /// <summary>
    ///     Rejects a poll with no queue name.
    /// </summary>
    [Test]
    public void Execute_WhenQueueIsBlank_ReportsInvalidArgument()
    {
        var action = new ReceiveAction
        {
            Connection = Connection,
            Queue = string.Empty
        };

        AssertError(ErrorCodes.InvalidArgument, action);
    }

    /// <summary>
    ///     Rejects a poll without the channel that owns deliveries.
    /// </summary>
    [Test]
    public void Execute_WhenConnectionIsMissing_ReportsInvalidArgument()
    {
        var action = new ReceiveAction
        {
            Queue = "queue"
        };

        AssertError(ErrorCodes.InvalidArgument, action);
    }

    /// <summary>
    ///     Reports an I/O failure rather than an empty queue.
    /// </summary>
    [Test]
    public void Execute_WhenIoFails_ReportsBrokerError()
    {
        Client.Failure = new IOException("disconnected");
        var action = new ReceiveAction
        {
            Connection = Connection,
            Queue = "queue"
        };

        AssertError(ErrorCodes.Broker, action);
    }
}
