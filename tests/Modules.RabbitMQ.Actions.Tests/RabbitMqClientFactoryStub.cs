using System;
using System.Threading;
using System.Threading.Tasks;

namespace PowerAutomate.Desktop.Modules.RabbitMQ.Actions.Tests;

/// <summary>
///     Supplies a hand-written client instead of opening a broker socket.
/// </summary>
public sealed class RabbitMqClientFactoryStub : IRabbitMqClientFactory
{
    private readonly IRabbitMqClient client;
    private Uri? address;
    private Exception? failure;

    /// <summary>
    ///     Stores the client returned on a successful connection attempt.
    /// </summary>
    /// <param name="client">The client to return to the action.</param>
    public RabbitMqClientFactoryStub(IRabbitMqClient client)
    {
        this.client = client;
        address = null;
        failure = null;
    }

    /// <summary>
    ///     Records the URI and returns a client or a configured failure.
    /// </summary>
    /// <param name="address">The selected broker URI.</param>
    /// <param name="cancellationToken">Cancels the connection request.</param>
    /// <returns>The configured broker client.</returns>
    public Task<IRabbitMqClient> CreateAsync(Uri address, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        this.address = address;
        if (failure is not null)
        {
            return Task.FromException<IRabbitMqClient>(failure);
        }

        return Task.FromResult(client);
    }

    /// <summary>
    ///     Returns the URI captured by the last call.
    /// </summary>
    /// <returns>The selected broker URI.</returns>
    public Uri? GetAddress()
    {
        return address;
    }

    /// <summary>
    ///     Injects a failure into the next connection attempt.
    /// </summary>
    /// <param name="exception">The error to return to the action.</param>
    public void SetFailure(Exception exception)
    {
        failure = exception;
    }
}
