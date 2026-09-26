using NUnit.Framework;

namespace PowerAutomate.Desktop.Modules.RabbitMQ.Actions.Tests;

/// <summary>
///     Checks the broker-bound publication contract.
/// </summary>
[TestFixture]
public sealed class RabbitMqPublishRequestTests
{
    /// <summary>
    ///     Retains payload, destination, and persistence together.
    /// </summary>
    [Test]
    public void RabbitMqPublishRequest_WhenCreated_PreservesOptions()
    {
        var body = new byte[]
        {
            42
        };
        var request = new RabbitMqPublishRequest("events", "task", body, true);

        Assert.That(request.Body, Is.SameAs(body));
        Assert.That(request.Exchange, Is.EqualTo("events"));
        Assert.That(request.IsPersistent, Is.True);
        Assert.That(request.RoutingKey, Is.EqualTo("task"));
    }
}
