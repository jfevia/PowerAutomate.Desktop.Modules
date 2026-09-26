using NUnit.Framework;
using PowerAutomate.Desktop.Modules.Redis.Actions.Properties;

namespace PowerAutomate.Desktop.Modules.Redis.Actions.Tests;

/// <summary>
///     Checks the localized metadata required by the PAD loader.
/// </summary>
[TestFixture]
public sealed class ResourcesTests
{
    /// <summary>
    ///     Provides labels for the module and its connection variable.
    /// </summary>
    [Test]
    public void ResourceManager_WhenModuleLoads_ContainsLabels()
    {
        var resourceProvider = new Resources();
        var resources = Resources.ResourceManager;

        Assert.That(resourceProvider, Is.Not.Null);
        Assert.That(resources.GetString("Redis_FriendlyName"), Is.EqualTo("Redis"));
        Assert.That(resources.GetString("RedisConnection_FriendlyName"), Is.EqualTo("Redis connection"));
    }
}
