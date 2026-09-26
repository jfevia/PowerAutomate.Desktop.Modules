namespace PowerAutomate.Desktop.Modules.Redis.Actions;

/// <summary>
///     Identifies Redis errors handled by desktop flows.
/// </summary>
public static class ErrorCodes
{
    /// <summary>
    ///     Reports an attempt to use a closed connection.
    /// </summary>
    public const string ClosedConnection = "ClosedConnectionError";

    /// <summary>
    ///     Reports invalid key, connection, or expiry input.
    /// </summary>
    public const string InvalidArgument = "InvalidArgumentError";

    /// <summary>
    ///     Reports a failure returned by the Redis client.
    /// </summary>
    public const string Redis = "RedisError";
}
