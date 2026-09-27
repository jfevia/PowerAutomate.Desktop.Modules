using System;

namespace PowerAutomate.Desktop.Modules.Redis.Actions.Tests;

/// <summary>
///     Captures connection options instead of opening a socket.
/// </summary>
public sealed class RedisClientFactoryStub : IRedisClientFactory
{
    private readonly IRedisClient client;
    private Exception? failure;
    private string? configuration;
    private int databaseNumber;

    /// <summary>
    ///     Uses a hand-written client as the connection.
    /// </summary>
    /// <param name="client">The client returned by the factory.</param>
    public RedisClientFactoryStub(IRedisClient client)
    {
        this.client = client;
        failure = null;
        configuration = null;
        databaseNumber = -2;
    }

    /// <summary>
    ///     Captures the options selected by the action.
    /// </summary>
    /// <param name="configuration">The Redis configuration to record.</param>
    /// <param name="databaseNumber">The requested database index.</param>
    /// <returns>The configured client unless a failure was injected.</returns>
    public IRedisClient Create(string configuration, int databaseNumber)
    {
        this.configuration = configuration;
        this.databaseNumber = databaseNumber;
        if (failure is not null)
        {
            throw failure;
        }

        return client;
    }

    /// <summary>
    ///     Returns the configuration captured by the last call.
    /// </summary>
    /// <returns>The configuration passed to the factory.</returns>
    public string? GetConfiguration()
    {
        return configuration;
    }

    /// <summary>
    ///     Returns the database index captured by the last call.
    /// </summary>
    /// <returns>The database index passed to the factory.</returns>
    public int GetDatabaseNumber()
    {
        return databaseNumber;
    }

    /// <summary>
    ///     Causes the next connection attempt to fail.
    /// </summary>
    /// <param name="exception">The failure returned to the action.</param>
    public void SetFailure(Exception exception)
    {
        failure = exception;
    }
}
