using System;
using System.Threading;
using System.Threading.Tasks;

namespace PowerAutomate.Desktop.Modules.RabbitMQ.Actions;

/// <summary>
///     Supplies broker connections without coupling actions to a transport.
/// </summary>
public interface IRabbitMqClientFactory
{
    /// <summary>
    ///     Opens a connection and a confirmed-publishing channel.
    /// </summary>
    /// <param name="address">The broker URI supplied by the flow.</param>
    /// <param name="cancellationToken">Cancels the connection request.</param>
    /// <returns>The client owning the connection and channel.</returns>
    Task<IRabbitMqClient> CreateAsync(Uri address, CancellationToken cancellationToken);
}
