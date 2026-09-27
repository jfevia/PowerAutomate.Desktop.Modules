using System;

namespace PowerAutomate.Desktop.Modules.Redis.Actions.Tests;

/// <summary>
///     Records Redis string operations without using a broker.
/// </summary>
public sealed class RedisClientStub : IRedisClient
{
    /// <summary>
    ///     Counts how often the connection is released.
    /// </summary>
    public int DisposeCount { get; private set; }

    /// <summary>
    ///     Injects a Redis failure into the next read.
    /// </summary>
    public Exception? Failure { get; set; }

    /// <summary>
    ///     Determines whether the delete operation removed a key.
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    ///     Determines whether the set operation stored the value.
    /// </summary>
    public bool IsStored { get; set; }

    /// <summary>
    ///     Records the expiry passed to a write.
    /// </summary>
    public TimeSpan? LastExpiry { get; private set; }

    /// <summary>
    ///     Records the key passed to a read, write, or delete.
    /// </summary>
    public string? LastKey { get; private set; }

    /// <summary>
    ///     Records the last value written.
    /// </summary>
    public string? LastValue { get; private set; }

    /// <summary>
    ///     Supplies a value for the next read.
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    ///     Initializes the stub without any pending operations.
    /// </summary>
    public RedisClientStub()
    {
        DisposeCount = 0;
        Failure = null;
        IsDeleted = false;
        IsStored = false;
        LastExpiry = null;
        LastKey = null;
        LastValue = null;
        Value = null;
    }

    /// <summary>
    ///     Records the close request for assertions.
    /// </summary>
    public void Dispose()
    {
        DisposeCount++;
    }

    /// <summary>
    ///     Returns the configured value unless the broker fails.
    /// </summary>
    /// <param name="key">The key requested by the action.</param>
    /// <returns>The configured value or null for a missing key.</returns>
    public string? GetValue(string key)
    {
        if (Failure is not null)
        {
            throw Failure;
        }

        LastKey = key;
        return Value;
    }

    /// <summary>
    ///     Records the key selected for deletion.
    /// </summary>
    /// <param name="key">The key to remove.</param>
    /// <returns>Whether the key existed in the stub.</returns>
    public bool HasDeletedKey(string key)
    {
        LastKey = key;
        return IsDeleted;
    }

    /// <summary>
    ///     Records the value and expiry selected for storage.
    /// </summary>
    /// <param name="key">The key to write.</param>
    /// <param name="value">The value to write.</param>
    /// <param name="expiry">The optional key expiry.</param>
    /// <returns>Whether the stub accepted the value.</returns>
    public bool HasStoredValue(string key, string value, TimeSpan? expiry)
    {
        LastKey = key;
        LastValue = value;
        LastExpiry = expiry;
        return IsStored;
    }
}
