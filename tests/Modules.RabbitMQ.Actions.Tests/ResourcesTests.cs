using NUnit.Framework;
using PowerAutomate.Desktop.Modules.RabbitMQ.Actions.Properties;

namespace PowerAutomate.Desktop.Modules.RabbitMQ.Actions.Tests;

/// <summary>
///     Checks resource labels expected by the PAD module loader.
/// </summary>
[TestFixture]
public sealed class ResourcesTests
{
    /// <summary>
    ///     Preserves the module and message type labels.
    /// </summary>
    [Test]
    public void ResourceManager_WhenModuleLoads_ContainsLabels()
    {
        var resourceProvider = new Resources();
        var resources = Resources.ResourceManager;

        Assert.That(resourceProvider, Is.Not.Null);
        Assert.That(resources.GetString("RabbitMQ_FriendlyName"), Is.EqualTo("RabbitMQ"));
        Assert.That(resources.GetString("RabbitMqMessage_FriendlyName"), Is.EqualTo("RabbitMQ message"));
        Assert.That(resources.GetString("RabbitMqConnection_FriendlyName"), Is.EqualTo("RabbitMQ connection"));
    }
}
