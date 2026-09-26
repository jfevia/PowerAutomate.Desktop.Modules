using System;
using System.Diagnostics.CodeAnalysis;
using StackExchange.Redis;

namespace PowerAutomate.Desktop.Modules.Redis.Actions;

/// <summary>
///     Forwards string-key operations into the Redis SDK.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class RedisClient : IRedisClient
{
    private readonly ConnectionMultiplexer connection;
    private readonly int databaseNumber;

    /// <summary>
    ///     Establishes one reusable multiplexer for the selected database.
    /// </summary>
    /// <param name="configuration">The Redis connection options.</param>
    /// <param name="databaseNumber">The database index, or -1 for the default.</param>
    public RedisClient(string configuration, int databaseNumber)
    {
        connection = ConnectionMultiplexer.Connect(configuration);
        this.databaseNumber = databaseNumber;
    }

    /// <summary>
    ///     Releases the shared Redis connection.
    /// </summary>
    public void Dispose()
    {
        connection.Dispose();
    }

    /// <summary>
    ///     Distinguishes an absent key from an empty value.
    /// </summary>
    /// <param name="key">The Redis key to read.</param>
    /// <returns>The stored value or null if absent.</returns>
    public string? GetValue(string key)
    {
        RedisValue value = connection.GetDatabase(databaseNumber).StringGet(key);
        return value.IsNull ? null : value.ToString();
    }

    /// <summary>
    ///     Deletes an existing key without affecting others.
    /// </summary>
    /// <param name="key">The Redis key to delete.</param>
    /// <returns>Whether a key was removed.</returns>
    public bool HasDeletedKey(string key)
    {
        return connection.GetDatabase(databaseNumber).KeyDelete(key);
    }

    /// <summary>
    ///     Stores a string with an optional expiry.
    /// </summary>
    /// <param name="key">The Redis key to write.</param>
    /// <param name="value">The value to store.</param>
    /// <param name="expiry">The optional time to live.</param>
    /// <returns>Whether the write succeeded.</returns>
    public bool HasStoredValue(string key, string value, TimeSpan? expiry)
    {
        return connection.GetDatabase(databaseNumber).StringSet(key, value, expiry, When.Always);
    }
}
