using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using RabbitMQ.Client.Exceptions;

namespace PowerAutomate.Desktop.Modules.RabbitMQ.Actions.Tests;

[TestFixture]
public class RabbitMqActionTests
{
    private FakeRabbitMqClient client = null!;
    private RabbitMqSession session = null!;

    [SetUp]
    public void SetUp()
    {
        client = new FakeRabbitMqClient();
        session = new RabbitMqSession(client, "rabbit.example:5672");
    }

    [TestCase("amqp://example:example@rabbit.example:5672/vhost", "rabbit.example:5672")]
    [TestCase("amqps://example:example@rabbit.example:5671/vhost", "rabbit.example:5671")]
    public void Connect_UsesFactoryWithoutDisplayingCredentials(string uri, string endpoint)
    {
        Uri? captured = null;
        var action = new ConnectAction(address =>
        {
            captured = address;
            return Task.FromResult<IRabbitMqClient>(client);
        }) { Address = uri };

        action.Execute(new ActionContext());

        Assert.That(captured!.AbsoluteUri, Is.EqualTo(uri));
        Assert.That(action.Session.Endpoint, Is.EqualTo(endpoint));
        Assert.That(action.Session.ToString(), Does.Not.Contain("example:example"));
    }

    [Test]
    public void Connect_DefaultConstructorIsAvailable()
    {
        Assert.That(new ConnectAction().Session, Is.Null);
    }

    [Test]
    public void Connect_WithNullFactory_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new ConnectAction(null!));
    }

    [TestCase("not a uri")]
    [TestCase("https://rabbit.example")]
    public void Connect_WithInvalidAddress_ReportsInvalidArgument(string address)
    {
        var action = new ConnectAction(_ => Task.FromResult<IRabbitMqClient>(client)) { Address = address };

        AssertError(ErrorCodes.InvalidArgument, action);
    }

    [Test]
    public void Connect_WhenBrokerIsUnavailable_ReportsBrokerError()
    {
        var action = new ConnectAction(_ =>
            Task.FromException<IRabbitMqClient>(new BrokerUnreachableException(new IOException("offline"))))
        {
            Address = "amqp://rabbit.example"
        };

        AssertError(ErrorCodes.Broker, action);
    }

    [Test]
    public void Connect_WhenTimedOut_ReportsBrokerError()
    {
        var action = new ConnectAction(_ =>
            Task.FromException<IRabbitMqClient>(new TimeoutException("timed out")))
        {
            Address = "amqp://rabbit.example"
        };

        AssertError(ErrorCodes.Broker, action);
    }

    [Test]
    public void Session_WithNullClientOrEndpoint_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new RabbitMqSession(null!, "rabbit.example"));
        Assert.Throws<ArgumentNullException>(() => new RabbitMqSession(client, null!));
    }

    [Test]
    public void Publish_ConfirmsPersistentUtf8Message()
    {
        var action = new PublishAction
        {
            Session = session,
            RoutingKey = "queue",
            Body = "Hello",
        };

        action.Execute(new ActionContext());

        Assert.That(client.Exchange, Is.Empty);
        Assert.That(client.RoutingKey, Is.EqualTo("queue"));
        Assert.That(client.Body, Is.EqualTo(Encoding.UTF8.GetBytes("Hello")));
        Assert.That(client.Persistent, Is.True);
    }

    [Test]
    public void Publish_AllowsEmptyTextAndNonPersistentCustomExchange()
    {
        var action = new PublishAction
        {
            Session = session,
            Exchange = "events",
            RoutingKey = "task.created",
            Body = "",
            Persistent = false
        };

        action.Execute(new ActionContext());

        Assert.That(client.Exchange, Is.EqualTo("events"));
        Assert.That(client.Body, Is.Empty);
        Assert.That(client.Persistent, Is.False);
    }

    [Test]
    public void Publish_WithMissingSession_ReportsInvalidArgument()
    {
        AssertError(ErrorCodes.InvalidArgument, new PublishAction { RoutingKey = "queue", Body = "text" });
    }

    [Test]
    public void Publish_WithBlankRoutingKey_ReportsInvalidArgument()
    {
        AssertError(ErrorCodes.InvalidArgument, new PublishAction { Session = session, RoutingKey = " ", Body = "text" });
    }

    [Test]
    public void Publish_WithNullExchange_ReportsInvalidArgument()
    {
        AssertError(ErrorCodes.InvalidArgument,
            new PublishAction { Session = session, Exchange = null!, RoutingKey = "queue", Body = "text" });
    }

    [Test]
    public void Publish_WithNullBody_ReportsInvalidArgument()
    {
        AssertError(ErrorCodes.InvalidArgument,
            new PublishAction { Session = session, RoutingKey = "queue", Body = null! });
    }

    [Test]
    public void Publish_WhenBrokerReturnsMessage_ReportsBrokerError()
    {
        client.Failure = new PublishException(1, true);

        AssertError(ErrorCodes.Broker,
            new PublishAction { Session = session, RoutingKey = "missing", Body = "text" });
    }

    [Test]
    public void Publish_UsesWorkerThreadWhenCallerHasSynchronizationContext()
    {
        var previous = SynchronizationContext.Current;
        SynchronizationContext.SetSynchronizationContext(new ThrowingSynchronizationContext());
        try
        {
            new PublishAction { Session = session, RoutingKey = "queue", Body = "text" }
                .Execute(new ActionContext());
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(previous);
        }

        Assert.That(client.RoutingKey, Is.EqualTo("queue"));
    }

    [Test]
    public void Receive_WhenQueueIsEmpty_ReturnsNotFound()
    {
        var action = new ReceiveAction { Session = session, Queue = "queue" };

        action.Execute(new ActionContext());

        Assert.That(client.Queue, Is.EqualTo("queue"));
        Assert.That(action.Found, Is.False);
        Assert.That(action.Message, Is.Null);
    }

    [Test]
    public void Receive_PreservesBinaryBodyAndDeliveryStatus()
    {
        client.Delivery = new RabbitMqDelivery(7, new byte[] { 0, 255, 128 }, true);
        var action = new ReceiveAction { Session = session, Queue = "queue" };

        action.Execute(new ActionContext());

        Assert.That(action.Found, Is.True);
        Assert.That(action.Message!.Queue, Is.EqualTo("queue"));
        Assert.That(Convert.FromBase64String(action.Message.BodyBase64), Is.EqualTo(new byte[] { 0, 255, 128 }));
        Assert.That(action.Message.Redelivered, Is.True);
        Assert.That(action.Message.IsSettled, Is.False);
        Assert.That(action.Message.ToString(), Is.EqualTo("RabbitMQ message from queue"));
    }

    [Test]
    public void Receive_WithBlankQueue_ReportsInvalidArgument()
    {
        AssertError(ErrorCodes.InvalidArgument, new ReceiveAction { Session = session, Queue = "" });
    }

    [Test]
    public void Receive_WithMissingSession_ReportsInvalidArgument()
    {
        AssertError(ErrorCodes.InvalidArgument, new ReceiveAction { Queue = "queue" });
    }

    [Test]
    public void Receive_WhenIoFails_ReportsBrokerError()
    {
        client.Failure = new IOException("disconnected");

        AssertError(ErrorCodes.Broker, new ReceiveAction { Session = session, Queue = "queue" });
    }

    [Test]
    public void Message_WithNullSessionOrQueue_Throws()
    {
        var delivery = new RabbitMqDelivery(7, new byte[] { 1 }, false);
        Assert.Throws<ArgumentNullException>(() => new RabbitMqMessage(null!, "queue", delivery));
        Assert.Throws<ArgumentNullException>(() => new RabbitMqMessage(session, null!, delivery));
    }

    [Test]
    public void Acknowledge_MarksMessageSettledAndRejectsDuplicate()
    {
        var message = ReceivedMessage();
        var action = new AcknowledgeAction { Message = message };

        action.Execute(new ActionContext());

        Assert.That(client.AcknowledgedTag, Is.EqualTo(7));
        Assert.That(message.IsSettled, Is.True);
        AssertError(ErrorCodes.MessageSettled, action);
        AssertError(ErrorCodes.MessageSettled, new RejectAction { Message = message });
    }

    [Test]
    public void Acknowledge_WhenBrokerFails_LeavesMessageUnsettled()
    {
        var message = ReceivedMessage();
        client.Failure = new PublishException(1, false);

        AssertError(ErrorCodes.Broker, new AcknowledgeAction { Message = message });

        Assert.That(message.IsSettled, Is.False);
    }

    [Test]
    public void Acknowledge_ConcurrentAttempts_SettleOnlyOnce()
    {
        var message = ReceivedMessage();
        var started = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        client.AcknowledgeStarted = started;
        client.HoldAcknowledge = release;
        var first = Task.Run(() => new AcknowledgeAction { Message = message }.Execute(new ActionContext()));

        try
        {
            Assert.That(started.Task.Wait(TimeSpan.FromSeconds(5)), Is.True);
            var second = Task.Run(() => AssertError(ErrorCodes.MessageSettled,
                new AcknowledgeAction { Message = message }));
            release.SetResult(true);
            Assert.That(Task.WaitAll(new[] { first, second }, TimeSpan.FromSeconds(5)), Is.True);
        }
        finally
        {
            release.TrySetResult(true);
        }

        Assert.That(client.AcknowledgedCount, Is.EqualTo(1));
    }

    [Test]
    public void Acknowledge_WithMissingMessage_ReportsInvalidArgument()
    {
        AssertError(ErrorCodes.InvalidArgument, new AcknowledgeAction());
    }

    [TestCase(false)]
    [TestCase(true)]
    public void Reject_UsesRequestedRequeueMode(bool requeue)
    {
        var message = ReceivedMessage();
        var action = new RejectAction { Message = message, Requeue = requeue };

        action.Execute(new ActionContext());

        Assert.That(client.RejectedTag, Is.EqualTo(7));
        Assert.That(client.Requeue, Is.EqualTo(requeue));
        Assert.That(message.IsSettled, Is.True);
    }

    [Test]
    public void Reject_WithMissingMessage_ReportsInvalidArgument()
    {
        AssertError(ErrorCodes.InvalidArgument, new RejectAction());
    }

    [Test]
    public void Close_OnlyClosesOnceAndPreventsFurtherUse()
    {
        var message = ReceivedMessage();
        var action = new CloseAction { Session = session };

        action.Execute(new ActionContext());
        action.Execute(new ActionContext());

        Assert.That(client.CloseCount, Is.EqualTo(1));
        Assert.That(session.IsClosed, Is.True);
        AssertError(ErrorCodes.ClosedConnection, new AcknowledgeAction { Message = message });
        AssertError(ErrorCodes.ClosedConnection,
            new PublishAction { Session = session, RoutingKey = "queue", Body = "text" });
    }

    [Test]
    public void Close_WhenBrokerFails_DoesNotPretendToBeClosed()
    {
        client.Failure = new IOException("close failed");
        var action = new CloseAction { Session = session };

        AssertError(ErrorCodes.Broker, action);
        Assert.That(session.IsClosed, Is.False);

        client.Failure = null;
        action.Execute(new ActionContext());
        Assert.That(session.IsClosed, Is.True);
    }

    [Test]
    public void Close_WithMissingSession_ReportsInvalidArgument()
    {
        AssertError(ErrorCodes.InvalidArgument, new CloseAction());
    }

    [Test]
    public void Resources_HaveModuleAndMessageNames()
    {
        var resources = Properties.Resources.ResourceManager;

        Assert.That(resources.GetString("RabbitMQ_FriendlyName"), Is.EqualTo("RabbitMQ"));
        Assert.That(resources.GetString("RabbitMqMessage_FriendlyName"), Is.EqualTo("RabbitMQ message"));
    }

    private RabbitMqMessage ReceivedMessage() =>
        new RabbitMqMessage(session, "queue", new RabbitMqDelivery(7, new byte[] { 1 }, false));

    private static void AssertError(string errorCode, RabbitMqActionBase action)
    {
        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo(errorCode));
    }

    private sealed class ThrowingSynchronizationContext : SynchronizationContext
    {
        public override void Post(SendOrPostCallback callback, object? state) =>
            throw new InvalidOperationException("An action attempted to use the caller's synchronization context.");
    }

    private sealed class FakeRabbitMqClient : IRabbitMqClient
    {
        public string? Exchange { get; private set; }
        public string? RoutingKey { get; private set; }
        public byte[]? Body { get; private set; }
        public bool Persistent { get; private set; }
        public string? Queue { get; private set; }
        public RabbitMqDelivery? Delivery { get; set; }
        public ulong? AcknowledgedTag { get; private set; }
        public int AcknowledgedCount { get; private set; }
        public TaskCompletionSource<bool>? AcknowledgeStarted { get; set; }
        public TaskCompletionSource<bool>? HoldAcknowledge { get; set; }
        public ulong? RejectedTag { get; private set; }
        public bool Requeue { get; private set; }
        public int CloseCount { get; private set; }
        public Exception? Failure { get; set; }

        public async Task PublishAsync(string exchange, string routingKey, byte[] body, bool persistent)
        {
            await Task.Yield();
            if (Failure != null) throw Failure;
            Exchange = exchange;
            RoutingKey = routingKey;
            Body = body;
            Persistent = persistent;
        }

        public Task<RabbitMqDelivery?> ReceiveAsync(string queue)
        {
            if (Failure != null) throw Failure;
            Queue = queue;
            return Task.FromResult(Delivery);
        }

        public async Task AcknowledgeAsync(ulong deliveryTag)
        {
            if (Failure != null) throw Failure;
            AcknowledgeStarted?.TrySetResult(true);
            if (HoldAcknowledge != null) await HoldAcknowledge.Task.ConfigureAwait(false);
            AcknowledgedTag = deliveryTag;
            AcknowledgedCount++;
        }

        public Task RejectAsync(ulong deliveryTag, bool requeue)
        {
            if (Failure != null) throw Failure;
            RejectedTag = deliveryTag;
            Requeue = requeue;
            return Task.CompletedTask;
        }

        public Task CloseAsync()
        {
            if (Failure != null) throw Failure;
            CloseCount++;
            return Task.CompletedTask;
        }
    }
}
