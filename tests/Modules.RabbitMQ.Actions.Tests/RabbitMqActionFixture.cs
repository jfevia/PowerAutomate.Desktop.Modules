using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;

namespace PowerAutomate.Desktop.Modules.RabbitMQ.Actions.Tests;

/// <summary>
///     Supplies a fresh hand-written broker client for every action test.
/// </summary>
public abstract class RabbitMqActionFixture
{
    /// <summary>
    ///     Records broker operations without opening a network socket.
    /// </summary>
    protected RabbitMqClientStub Client { get; private set; }

    /// <summary>
    ///     Serializes actions on one test broker channel.
    /// </summary>
    protected RabbitMqConnection Connection { get; private set; }

    /// <summary>
    ///     Initializes the fixture before NUnit setup.
    /// </summary>
    protected RabbitMqActionFixture()
    {
        Client = new RabbitMqClientStub();
        Connection = new RabbitMqConnection(Client, "rabbit.example:5672");
    }

    /// <summary>
    ///     Isolates the channel state from preceding tests.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        Client = new RabbitMqClientStub();
        Connection = new RabbitMqConnection(Client, "rabbit.example:5672");
    }

    /// <summary>
    ///     Asserts a broker or validation error visible to desktop flows.
    /// </summary>
    /// <param name="errorCode">The expected PAD error identifier.</param>
    /// <param name="action">The action expected to fail.</param>
    protected void AssertError(string errorCode, RabbitMqActionBase action)
    {
        var error = Assert.Throws<ActionException>(() => RunAction(action))!;
        Assert.That(error.Name, Is.EqualTo(errorCode));
    }

    /// <summary>
    ///     Creates an unsettled delivery on this fixture's connection.
    /// </summary>
    /// <returns>A message that must be acknowledged or rejected.</returns>
    protected RabbitMqMessage CreateMessage()
    {
        var body = new byte[]
        {
            1
        };
        var delivery = new RabbitMqDelivery(7, body, false);
        return new RabbitMqMessage(Connection, "queue", delivery);
    }

    /// <summary>
    ///     Executes an action with a fresh desktop-flow context.
    /// </summary>
    /// <param name="action">The broker action to run.</param>
    protected void RunAction(RabbitMqActionBase action)
    {
        var context = new ActionContext();
        action.Execute(context);
    }
}
