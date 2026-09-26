using System;
using System.Diagnostics.CodeAnalysis;
using StackExchange.Redis;

namespace PowerAutomate.Desktop.Modules.Redis.Actions;

[ExcludeFromCodeCoverage]
internal sealed class RedisClient : IRedisClient
{
    private readonly ConnectionMultiplexer connection;
    private readonly int databaseNumber;

    public RedisClient(string configuration, int databaseNumber)
    {
        connection = ConnectionMultiplexer.Connect(configuration);
        this.databaseNumber = databaseNumber;
    }

    public string? Get(string key)
    {
        RedisValue value = connection.GetDatabase(databaseNumber).StringGet(key);
        return value.IsNull ? null : value.ToString();
    }

    public bool Set(string key, string value, TimeSpan? expiry) =>
        connection.GetDatabase(databaseNumber).StringSet(key, value, expiry, When.Always);

    public bool Delete(string key) => connection.GetDatabase(databaseNumber).KeyDelete(key);

    public void Dispose() => connection.Dispose();
}
