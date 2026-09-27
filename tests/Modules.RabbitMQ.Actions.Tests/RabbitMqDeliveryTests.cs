using NUnit.Framework;

namespace PowerAutomate.Desktop.Modules.RabbitMQ.Actions.Tests;

/// <summary>
///     Checks the metadata preserved from the broker delivery.
/// </summary>
[TestFixture]
public sealed class RabbitMqDeliveryTests
{
    /// <summary>
    ///     Preserves the copied body and channel-specific delivery tag.
    /// </summary>
    [Test]
    public void RabbitMqDelivery_WhenCreated_PreservesMetadata()
    {
        var body = new byte[]
        {
            0,
            255
        };
        var delivery = new RabbitMqDelivery(7, body, true);

        Assert.That(delivery.Body, Is.SameAs(body));
        Assert.That(delivery.DeliveryTag, Is.EqualTo(7));
        Assert.That(delivery.IsRedelivered, Is.True);
    }
}
