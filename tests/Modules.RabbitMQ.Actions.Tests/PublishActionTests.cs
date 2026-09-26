using System.Text;
using System.Threading;
using NUnit.Framework;
using RabbitMQ.Client.Exceptions;

namespace PowerAutomate.Desktop.Modules.RabbitMQ.Actions.Tests;

/// <summary>
///     Checks confirmed UTF-8 publication and input validation.
/// </summary>
[TestFixture]
public sealed class PublishActionTests : RabbitMqActionFixture
{
    /// <summary>
    ///     Keeps persistent publication enabled by default.
    /// </summary>
    [Test]
    public void Execute_WhenDefaultExchangeIsUsed_PublishesPersistentUtf8()
    {
        var action = new PublishAction
        {
            Connection = Connection,
            RoutingKey = "queue",
            Body = "Hello"
        };

        RunAction(action);

        Assert.That(Client.Exchange, Is.Empty);
        Assert.That(Client.RoutingKey, Is.EqualTo("queue"));
        Assert.That(Client.Body, Is.EqualTo(Encoding.UTF8.GetBytes("Hello")));
        Assert.That(Client.IsPersistent, Is.True);
    }

    /// <summary>
    ///     Permits empty payloads and a nonpersistent custom exchange.
    /// </summary>
    [Test]
    public void Execute_WhenBodyIsEmpty_PublishesCustomExchange()
    {
        var action = new PublishAction
        {
            Body = string.Empty,
            Connection = Connection,
            Exchange = "events",
            IsPersistent = false,
            RoutingKey = "task.created"
        };

        RunAction(action);

        Assert.That(Client.Exchange, Is.EqualTo("events"));
        Assert.That(Client.Body, Is.Empty);
        Assert.That(Client.IsPersistent, Is.False);
    }

    /// <summary>
    ///     Rejects publication without a broker connection.
    /// </summary>
    [Test]
    public void Execute_WhenConnectionIsMissing_ReportsInvalidArgument()
    {
        var action = new PublishAction
        {
            RoutingKey = "queue",
            Body = "text"
        };

        AssertError(ErrorCodes.InvalidArgument, action);
    }

    /// <summary>
    ///     Rejects an empty routing key before contacting the broker.
    /// </summary>
    [Test]
    public void Execute_WhenRoutingKeyIsBlank_ReportsInvalidArgument()
    {
        var action = new PublishAction
        {
            Body = "text",
            Connection = Connection,
            RoutingKey = " "
        };

        AssertError(ErrorCodes.InvalidArgument, action);
    }

    /// <summary>
    ///     Rejects a missing exchange instead of choosing an unintended one.
    /// </summary>
    [Test]
    public void Execute_WhenExchangeIsNull_ReportsInvalidArgument()
    {
        var action = new PublishAction
        {
            Body = "text",
            Connection = Connection,
            Exchange = null,
            RoutingKey = "queue"
        };

        AssertError(ErrorCodes.InvalidArgument, action);
    }

    /// <summary>
    ///     Rejects an absent message body rather than publishing it.
    /// </summary>
    [Test]
    public void Execute_WhenBodyIsNull_ReportsInvalidArgument()
    {
        var action = new PublishAction
        {
            Connection = Connection,
            RoutingKey = "queue"
        };

        AssertError(ErrorCodes.InvalidArgument, action);
    }

    /// <summary>
    ///     Surfaces an unroutable message as a broker error.
    /// </summary>
    [Test]
    public void Execute_WhenBrokerReturnsMessage_ReportsBrokerError()
    {
        Client.Failure = new PublishException(1, true);
        var action = new PublishAction
        {
            Body = "text",
            Connection = Connection,
            RoutingKey = "missing"
        };

        AssertError(ErrorCodes.Broker, action);
    }

    /// <summary>
    ///     Avoids deadlocking the PAD caller's synchronization context.
    /// </summary>
    [Test]
    public void Execute_WhenCallerHasSynchronizationContext_UsesWorkerThread()
    {
        var original = SynchronizationContext.Current;
        var throwing = new ThrowingSynchronizationContext();
        var action = new PublishAction
        {
            Body = "text",
            Connection = Connection,
            RoutingKey = "queue"
        };

        SynchronizationContext.SetSynchronizationContext(throwing);
        try
        {
            RunAction(action);
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(original);
        }

        Assert.That(Client.RoutingKey, Is.EqualTo("queue"));
    }
}
