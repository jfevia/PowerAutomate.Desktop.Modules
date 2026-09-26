using System;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Enums;

namespace PowerAutomate.Desktop.Modules.Redis.Actions;

/// <summary>
///     Carries a reusable Redis connection between desktop-flow actions.
/// </summary>
[Type(FriendlyName = nameof(RedisConnection) + "_FriendlyName",
      FriendlyNamePlural = nameof(RedisConnection) + "_FriendlyNamePlural",
      DefaultPropertyVisibility = Visibility.Visible)]
public sealed class RedisConnection : IDisposable
{
    private readonly IRedisClient client;

    /// <summary>
    ///     Shows the selected index without displaying credentials.
    /// </summary>
    [Property]
    public int DatabaseNumber { get; }

    /// <summary>
    ///     Indicates whether the shared multiplexer has been released.
    /// </summary>
    [Property]
    public bool IsClosed { get; private set; }

    /// <summary>
    ///     Retains the client while exposing only safe connection metadata.
    /// </summary>
    /// <param name="client">The client owned by this connection.</param>
    /// <param name="databaseNumber">The selected database index.</param>
    public RedisConnection(IRedisClient? client, int databaseNumber)
    {
        if (client is null)
        {
            throw new ArgumentNullException(nameof(client));
        }

        this.client = client;
        DatabaseNumber = databaseNumber;
        IsClosed = false;
    }

    /// <summary>
    ///     Releases the connection at most once.
    /// </summary>
    public void Dispose()
    {
        if (IsClosed)
        {
            return;
        }

        client.Dispose();
        IsClosed = true;
    }

    /// <summary>
    ///     Prevents actions from using a previously closed connection.
    /// </summary>
    /// <returns>The reusable Redis client.</returns>
    public IRedisClient GetClient()
    {
        if (IsClosed)
        {
            throw new ObjectDisposedException(nameof(RedisConnection));
        }

        return client;
    }

    /// <summary>
    ///     Displays the database index without disclosing the connection string.
    /// </summary>
    /// <returns>A safe description of the connection.</returns>
    public override string ToString()
    {
        return $"Redis database {DatabaseNumber}";
    }
}
