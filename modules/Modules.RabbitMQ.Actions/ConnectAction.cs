using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.RabbitMQ.Actions;

/// <summary>
///     Opens a broker channel with publisher confirmations enabled.
/// </summary>
[Action(Id = "ConnectRabbitMq")]
[Throws(ErrorCodes.InvalidArgument)]
[Throws(ErrorCodes.Broker)]
public class ConnectAction : RabbitMqActionBase
{
    private readonly IRabbitMqClientFactory clientFactory;

    /// <summary>
    ///     Supplies an amqp or amqps broker URI.
    /// </summary>
    [InputArgument(Required = true)]
    public string? Address { get; set; }

    /// <summary>
    ///     Shares a broker channel without displaying URI credentials.
    /// </summary>
    [OutputArgument]
    public RabbitMqConnection? Connection { get; set; }

    /// <summary>
    ///     Uses the RabbitMQ SDK for broker connections.
    /// </summary>
    public ConnectAction()
    {
        clientFactory = new RabbitMqClientFactory();
        Address = null;
        Connection = null;
    }

    /// <summary>
    ///     Permits network-free tests through an injected client factory.
    /// </summary>
    /// <param name="clientFactory">The provider of broker clients.</param>
    protected ConnectAction(IRabbitMqClientFactory? clientFactory)
    {
        if (clientFactory is null)
        {
            throw new ArgumentNullException(nameof(clientFactory));
        }

        this.clientFactory = clientFactory;
        Address = null;
        Connection = null;
    }

    /// <summary>
    ///     Validates the URI before connecting to the broker.
    /// </summary>
    /// <param name="context">The desktop-flow action context.</param>
    /// <param name="cancellationToken">Cancels broker connection setup.</param>
    /// <returns>The broker connection operation.</returns>
    protected override async Task RunAsync(ActionContext context, CancellationToken cancellationToken)
    {
        if (!Uri.TryCreate(Address, UriKind.Absolute, out var address) ||
            (address.Scheme != "amqp" && address.Scheme != "amqps"))
        {
            throw new ArgumentException("An amqp or amqps address is required.", nameof(Address));
        }

        var client = await clientFactory.CreateAsync(address, cancellationToken).ConfigureAwait(false);
        Connection = new RabbitMqConnection(client, address.Authority);
    }
}
