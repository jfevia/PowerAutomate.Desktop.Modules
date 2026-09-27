using NUnit.Framework;
using PowerAutomate.Desktop.Modules.SQLite.Actions.Properties;

namespace PowerAutomate.Desktop.Modules.SQLite.Actions.Tests;

/// <summary>
///     Checks the localized metadata consumed by the module loader.
/// </summary>
[TestFixture]
public sealed class ResourcesTests
{
    /// <summary>
    ///     Preserves the labels needed to load the module in PAD.
    /// </summary>
    [Test]
    public void ResourceManager_WhenModuleLoads_ContainsLabels()
    {
        var resourceProvider = new Resources();
        var resources = Resources.ResourceManager;

        Assert.That(resourceProvider, Is.Not.Null);
        Assert.That(resources.GetString("SQLite_FriendlyName"), Is.EqualTo("SQLite"));
        Assert.That(resources.GetString("Query_FriendlyName"), Is.EqualTo("Query SQLite database"));
    }
}
