using System.IO;
using NUnit.Framework;

namespace PowerAutomate.Desktop.Modules.RabbitMQ.Actions.Tests;

/// <summary>
///     Checks idempotent cleanup and safe access after shutdown.
/// </summary>
[TestFixture]
public sealed class CloseActionTests : RabbitMqActionFixture
{
    /// <summary>
    ///     Allows a retry if channel shutdown fails.
    /// </summary>
    [Test]
    public void Execute_WhenBrokerFails_RemainsOpenForRetry()
    {
        Client.Failure = new IOException("close failed");
        var action = new CloseAction
        {
            Connection = Connection
        };

        AssertError(ErrorCodes.Broker, action);
        Assert.That(Connection.IsClosed, Is.False);

        Client.Failure = null;
        RunAction(action);
        Assert.That(Connection.IsClosed, Is.True);
    }

    /// <summary>
    ///     Closes once and rejects subsequent operations on the channel.
    /// </summary>
    [Test]
    public void Execute_WhenCalledTwice_ClosesOnce()
    {
        var message = CreateMessage();
        var action = new CloseAction
        {
            Connection = Connection
        };

        RunAction(action);
        RunAction(action);

        Assert.That(Client.CloseCount, Is.EqualTo(1));
        Assert.That(Connection.IsClosed, Is.True);
        var acknowledge = new AcknowledgeAction
        {
            Message = message
        };
        AssertError(ErrorCodes.ClosedConnection, acknowledge);
        var publish = new PublishAction
        {
            Connection = Connection,
            RoutingKey = "queue",
            Body = "text"
        };
        AssertError(ErrorCodes.ClosedConnection, publish);
    }

    /// <summary>
    ///     Requires a connection variable for cleanup.
    /// </summary>
    [Test]
    public void Execute_WhenConnectionIsMissing_ReportsInvalidArgument()
    {
        var action = new CloseAction();

        AssertError(ErrorCodes.InvalidArgument, action);
    }
}
