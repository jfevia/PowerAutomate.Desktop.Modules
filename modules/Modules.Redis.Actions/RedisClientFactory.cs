using System.Diagnostics.CodeAnalysis;

namespace PowerAutomate.Desktop.Modules.Redis.Actions;

/// <summary>
///     Forwards connection creation to the external Redis SDK.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class RedisClientFactory : IRedisClientFactory
{
    /// <summary>
    ///     Creates the SDK connection held by a desktop-flow variable.
    /// </summary>
    /// <param name="configuration">The Redis endpoint and connection options.</param>
    /// <param name="databaseNumber">The selected database index.</param>
    /// <returns>A connection for the selected database.</returns>
    public IRedisClient Create(string configuration, int databaseNumber)
    {
        return new RedisClient(configuration, databaseNumber);
    }
}
