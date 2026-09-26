using System.Resources;

namespace PowerAutomate.Desktop.Modules.PostgreSQL.Actions.Properties;

/// <summary>
///     Exposes labels required by the PAD module loader.
/// </summary>
public sealed class Resources
{
    private static readonly ResourceManager resourceManager;

    /// <summary>
    ///     Resolves localized action and parameter labels.
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
    ///     Lets the PAD module loader instantiate resource metadata.
    /// </summary>
    public Resources()
    {
    }
}
