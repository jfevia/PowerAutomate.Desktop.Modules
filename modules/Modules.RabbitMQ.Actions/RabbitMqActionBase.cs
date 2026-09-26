using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using RabbitMQ.Client.Exceptions;

namespace PowerAutomate.Desktop.Modules.RabbitMQ.Actions;

/// <summary>
///     Runs async broker work without capturing the PAD synchronization context.
/// </summary>
public abstract class RabbitMqActionBase : ActionBase
{
    /// <summary>
    ///     Performs the action-specific broker operation.
    /// </summary>
    /// <param name="context">The desktop-flow action context.</param>
    /// <param name="cancellationToken">Cancels the broker operation.</param>
    /// <returns>The broker operation.</returns>
    protected abstract Task RunAsync(ActionContext context, CancellationToken cancellationToken);

    /// <summary>
    ///     Maps broker failures to desktop-flow error categories.
    /// </summary>
    /// <param name="context">The desktop-flow action context.</param>
    public override void Execute(ActionContext context)
    {
        try
        {
            Task.Run(() => RunAsync(context, CancellationToken.None)).GetAwaiter().GetResult();
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

    /// <summary>
    ///     Validates a connection supplied by an earlier action.
    /// </summary>
    /// <param name="connection">The broker connection supplied by the flow.</param>
    /// <returns>The required broker connection.</returns>
    protected RabbitMqConnection RequireConnection(RabbitMqConnection? connection)
    {
        if (connection is null)
        {
            throw new ArgumentException("A RabbitMQ connection is required.", nameof(connection));
        }

        return connection;
    }

    /// <summary>
    ///     Validates a message supplied by the receive action.
    /// </summary>
    /// <param name="message">The message to acknowledge or reject.</param>
    /// <returns>The required message.</returns>
    protected RabbitMqMessage RequireMessage(RabbitMqMessage? message)
    {
        if (message is null)
        {
            throw new ArgumentException("A RabbitMQ message is required.", nameof(message));
        }

        return message;
    }

    /// <summary>
    ///     Rejects blank routing keys and queue names.
    /// </summary>
    /// <param name="value">The text supplied by the flow.</param>
    /// <param name="name">The name shown for an invalid input.</param>
    /// <returns>A nonblank string.</returns>
    protected string RequireValue(string? value, string name)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("A value is required.", name);
        }

        return value!;
    }
}
