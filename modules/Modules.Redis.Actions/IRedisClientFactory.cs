namespace PowerAutomate.Desktop.Modules.Redis.Actions;

/// <summary>
///     Supplies Redis connections for desktop-flow actions.
/// </summary>
public interface IRedisClientFactory
{
    /// <summary>
    ///     Opens a connection using the supplied Redis configuration.
    /// </summary>
    /// <param name="configuration">The Redis endpoint and connection options.</param>
    /// <param name="databaseNumber">The database index, or -1 for the default.</param>
    /// <returns>A client whose connection must be closed.</returns>
    IRedisClient Create(string configuration, int databaseNumber);
}
