using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace PowerAutomate.Desktop.Modules.RabbitMQ.Actions;

/// <summary>
///     Opens broker resources while keeping actions independent of transport.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class RabbitMqClientFactory : IRabbitMqClientFactory
{
    /// <summary>
    ///     Opens a connection and a channel with publisher confirms.
    /// </summary>
    /// <param name="address">The RabbitMQ broker address.</param>
    /// <param name="cancellationToken">Cancels connection establishment.</param>
    /// <returns>A client owning both broker resources.</returns>
    public async Task<IRabbitMqClient> CreateAsync(Uri address, CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            Uri = address
        };
        var connection = await factory.CreateConnectionAsync(cancellationToken).ConfigureAwait(false);
        IChannel? channel = null;
        try
        {
            var options = new CreateChannelOptions(true, true);
            channel = await connection.CreateChannelAsync(options, cancellationToken).ConfigureAwait(false);
            return new RabbitMqClient(connection, channel);
        }
        finally
        {
            if (channel is null)
            {
                await connection.DisposeAsync().ConfigureAwait(false);
            }
        }
    }
}
