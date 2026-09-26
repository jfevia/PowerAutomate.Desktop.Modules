using System.Resources;

namespace PowerAutomate.Desktop.Modules.SQLite.Actions.Properties;

/// <summary>
///     Exposes labels required by the desktop-flow module loader.
/// </summary>
public sealed class Resources
{
    private static readonly ResourceManager resourceManager;

    /// <summary>
    ///     Resolves localized action and error labels.
    /// </summary>
    public static ResourceManager ResourceManager
    {
        get
        {
            return resourceManager;
        }
    }

    static Resources()
    {
        resourceManager = new ResourceManager(typeof(Resources).FullName!, typeof(Resources).Assembly);
    }

    /// <summary>
    ///     Allows the desktop-flow loader to discover resource labels.
    /// </summary>
    public Resources()
    {
    }
}
