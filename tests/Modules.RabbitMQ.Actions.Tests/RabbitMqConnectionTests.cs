using System;
using System.Threading;
using NUnit.Framework;

namespace PowerAutomate.Desktop.Modules.RabbitMQ.Actions.Tests;

/// <summary>
///     Checks connection lifetime and cancellation without a live broker.
/// </summary>
[TestFixture]
public sealed class RabbitMqConnectionTests : RabbitMqActionFixture
{
    /// <summary>
    ///     Requires a client when creating a connection variable.
    /// </summary>
    [Test]
    public void RabbitMqConnection_WhenClientIsNull_Throws()
    {
        var error = Assert.Throws<ArgumentNullException>(() =>
        {
            var connection = new RabbitMqConnection(null, "rabbit.example");
            Assert.Fail($"Unexpected connection: {connection}");
        });

        Assert.That(error?.ParamName, Is.EqualTo("client"));
    }

    /// <summary>
    ///     Requires an endpoint safe to display in the flow.
    /// </summary>
    [Test]
    public void RabbitMqConnection_WhenEndpointIsNull_Throws()
    {
        var error = Assert.Throws<ArgumentNullException>(() =>
        {
            var connection = new RabbitMqConnection(Client, null);
            Assert.Fail($"Unexpected connection: {connection}");
        });

        Assert.That(error?.ParamName, Is.EqualTo("endpoint"));
    }

    /// <summary>
    ///     Honors cancellation before a queue poll starts.
    /// </summary>
    [Test]
    public void ReceiveAsync_WhenCanceled_DoesNotPollQueue()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        var task = Connection.ReceiveAsync("queue", cancellation.Token);

        Assert.That(() => task.GetAwaiter().GetResult(), Throws.InstanceOf<OperationCanceledException>());
        Assert.That(Client.Queue, Is.Null);
    }

    /// <summary>
    ///     Rejects queue polling after the broker resources close.
    /// </summary>
    [Test]
    public void ReceiveAsync_WhenConnectionIsClosed_Throws()
    {
        Connection.CloseAsync(CancellationToken.None).GetAwaiter().GetResult();
        var task = Connection.ReceiveAsync("queue", CancellationToken.None);

        var error = Assert.Throws<ObjectDisposedException>(() => task.GetAwaiter().GetResult());
        Assert.That(error?.ObjectName, Is.EqualTo(nameof(RabbitMqConnection)));
    }

    /// <summary>
    ///     Rejects message rejection after the broker resources close.
    /// </summary>
    [Test]
    public void RejectAsync_WhenConnectionIsClosed_Throws()
    {
        Connection.CloseAsync(CancellationToken.None).GetAwaiter().GetResult();
        var task = Connection.RejectAsync(7, false, CancellationToken.None);

        var error = Assert.Throws<ObjectDisposedException>(() => task.GetAwaiter().GetResult());
        Assert.That(error?.ObjectName, Is.EqualTo(nameof(RabbitMqConnection)));
    }

    /// <summary>
    ///     Does not display passwords from broker URIs.
    /// </summary>
    [Test]
    public void ToString_WhenConnectionIsOpen_DisplaysEndpoint()
    {
        Assert.That(Connection.ToString(), Is.EqualTo("rabbit.example:5672"));
        Assert.That(Connection.IsClosed, Is.False);
    }
}
