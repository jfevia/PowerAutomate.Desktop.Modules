using NUnit.Framework;
using PowerAutomate.Desktop.Modules.PostgreSQL.Actions.Properties;

namespace PowerAutomate.Desktop.Modules.PostgreSQL.Actions.Tests;

/// <summary>
///     Checks localized labels required by the PAD module loader.
/// </summary>
[TestFixture]
public sealed class ResourcesTests
{
    /// <summary>
    ///     Preserves labels needed for action discovery.
    /// </summary>
    [Test]
    public void ResourceManager_WhenModuleLoads_ContainsLabels()
    {
        var resourceProvider = new Resources();
        var resources = Resources.ResourceManager;

        Assert.That(resourceProvider, Is.Not.Null);
        Assert.That(resources.GetString("PostgreSQL_FriendlyName"), Is.EqualTo("PostgreSQL"));
        Assert.That(resources.GetString("Query_FriendlyName"), Is.EqualTo("Query PostgreSQL"));
    }
}
