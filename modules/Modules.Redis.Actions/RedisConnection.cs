using System;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Enums;

namespace PowerAutomate.Desktop.Modules.Redis.Actions;

[Type(FriendlyName = nameof(RedisConnection) + "_FriendlyName",
      FriendlyNamePlural = nameof(RedisConnection) + "_FriendlyNamePlural",
      DefaultPropertyVisibility = Visibility.Visible)]
public sealed class RedisConnection : IDisposable
{
    internal RedisConnection(IRedisClient client, int databaseNumber)
    {
        Client = client ?? throw new ArgumentNullException(nameof(client));
        DatabaseNumber = databaseNumber;
    }

    private IRedisClient Client { get; }

    [Property]
    public int DatabaseNumber { get; }

    [Property]
    public bool IsClosed { get; private set; }

    internal IRedisClient GetClient() =>
        IsClosed ? throw new ObjectDisposedException(nameof(RedisConnection)) : Client;

    public void Dispose()
    {
        if (IsClosed)
        {
            return;
        }

        Client.Dispose();
        IsClosed = true;
    }

    public override string ToString() => $"Redis database {DatabaseNumber}";
}
