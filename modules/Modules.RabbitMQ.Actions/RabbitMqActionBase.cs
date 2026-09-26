using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using RabbitMQ.Client.Exceptions;

namespace PowerAutomate.Desktop.Modules.RabbitMQ.Actions;

public abstract class RabbitMqActionBase : ActionBase
{
    public override void Execute(ActionContext context)
    {
        try
        {
            Task.Run(() => RunAsync(context)).GetAwaiter().GetResult();
        }
        catch (ArgumentException exception)
        {
            throw new ActionException(ErrorCodes.InvalidArgument, exception.Message, exception);
        }
        catch (ObjectDisposedException exception)
        {
            throw new ActionException(ErrorCodes.ClosedConnection, exception.Message, exception);
        }
        catch (RabbitMQClientException exception)
        {
            throw new ActionException(ErrorCodes.Broker, exception.Message, exception);
        }
        catch (IOException exception)
        {
            throw new ActionException(ErrorCodes.Broker, exception.Message, exception);
        }
        catch (TimeoutException exception)
        {
            throw new ActionException(ErrorCodes.Broker, exception.Message, exception);
        }
    }

    protected abstract Task RunAsync(ActionContext context);

    protected static string RequireValue(string? value, string name)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("A value is required.", name);
        }

        return value!;
    }

    protected static RabbitMqSession RequireSession(RabbitMqSession? session) =>
        session ?? throw new ArgumentException("A RabbitMQ session is required.", nameof(session));

    protected static RabbitMqMessage RequireMessage(RabbitMqMessage? message) =>
        message ?? throw new ArgumentException("A RabbitMQ message is required.", nameof(message));
}
