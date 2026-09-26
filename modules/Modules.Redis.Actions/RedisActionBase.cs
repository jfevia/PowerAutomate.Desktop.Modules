using System;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using StackExchange.Redis;

namespace PowerAutomate.Desktop.Modules.Redis.Actions;

/// <summary>
///     Maps Redis and validation failures to desktop-flow errors.
/// </summary>
public abstract class RedisActionBase : ActionBase
{
    /// <summary>
    ///     Performs the action-specific Redis operation.
    /// </summary>
    /// <param name="context">The desktop-flow action context.</param>
    protected abstract void Run(ActionContext context);

    /// <summary>
    ///     Runs the operation with a stable desktop-flow error category.
    /// </summary>
    /// <param name="context">The desktop-flow action context.</param>
    public override void Execute(ActionContext context)
    {
        try
        {
            Run(context);
        }
        catch (ArgumentException exception)
        {
            throw new ActionException(ErrorCodes.InvalidArgument, exception.Message, exception);
        }
        catch (ObjectDisposedException exception)
        {
            throw new ActionException(ErrorCodes.ClosedConnection, exception.Message, exception);
        }
        catch (RedisException exception)
        {
            throw new ActionException(ErrorCodes.Redis, exception.Message, exception);
        }
    }

    /// <summary>
    ///     Validates a connection supplied by another action.
    /// </summary>
    /// <param name="connection">The connection supplied by the flow.</param>
    /// <returns>The open client shared between actions.</returns>
    protected IRedisClient RequireConnection(RedisConnection? connection)
    {
        if (connection == null)
        {
            throw new ArgumentException("A Redis connection is required.", nameof(connection));
        }

        return connection.GetClient();
    }

    /// <summary>
    ///     Validates a required text input before sending a request.
    /// </summary>
    /// <param name="value">The input supplied by the flow.</param>
    /// <param name="name">The name displayed for an invalid input.</param>
    /// <returns>A nonblank value.</returns>
    protected string RequireValue(string? value, string name)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("A value is required.", name);
        }

        return value!;
    }
}
