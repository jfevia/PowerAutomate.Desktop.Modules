using System;

namespace PowerAutomate.Desktop.Modules.Redis.Actions;

/// <summary>
///     Defines the Redis string operations used by desktop flows.
/// </summary>
public interface IRedisClient : IDisposable
{
    /// <summary>
    ///     Retrieves the value, or null if the key is absent.
    /// </summary>
    /// <param name="key">The Redis key to read.</param>
    /// <returns>The stored value or null.</returns>
    string? GetValue(string key);

    /// <summary>
    ///     Deletes a key if it exists.
    /// </summary>
    /// <param name="key">The Redis key to delete.</param>
    /// <returns>Whether an existing key was removed.</returns>
    bool HasDeletedKey(string key);

    /// <summary>
    ///     Writes a string value with an optional expiry.
    /// </summary>
    /// <param name="key">The Redis key to write.</param>
    /// <param name="value">The string value to store.</param>
    /// <param name="expiry">The optional expiry of the key.</param>
    /// <returns>Whether Redis accepted the value.</returns>
    bool HasStoredValue(string key, string value, TimeSpan? expiry);
}
