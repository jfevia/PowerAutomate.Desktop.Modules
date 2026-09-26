using System;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using StackExchange.Redis;

namespace PowerAutomate.Desktop.Modules.Redis.Actions;

public abstract class RedisActionBase : ActionBase
{
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

    protected abstract void Run(ActionContext context);

    protected static string RequireValue(string? value, string name)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("A value is required.", name);
        }

        return value!;
    }

    private protected static IRedisClient RequireConnection(RedisConnection? connection)
    {
        if (connection == null)
        {
            throw new ArgumentException("A Redis connection is required.", nameof(connection));
        }

        return connection.GetClient();
    }
}
