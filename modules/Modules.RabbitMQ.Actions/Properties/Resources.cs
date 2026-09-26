using System.Resources;

namespace PowerAutomate.Desktop.Modules.RabbitMQ.Actions.Properties;

public sealed class Resources
{
    public static ResourceManager ResourceManager { get; } =
        new ResourceManager(typeof(Resources).FullName!, typeof(Resources).Assembly);
}
