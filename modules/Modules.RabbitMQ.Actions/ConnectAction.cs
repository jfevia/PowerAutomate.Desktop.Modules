using System;
using System.Threading.Tasks;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.RabbitMQ.Actions;

[Action(Id = "ConnectRabbitMq")]
[Throws(ErrorCodes.InvalidArgument)]
[Throws(ErrorCodes.Broker)]
public class ConnectAction : RabbitMqActionBase
{
    private readonly Func<Uri, Task<IRabbitMqClient>> clientFactory;

    public ConnectAction() : this(RabbitMqClient.ConnectAsync)
    {
    }

    internal ConnectAction(Func<Uri, Task<IRabbitMqClient>> clientFactory) =>
        this.clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));

    [InputArgument(Required = true)]
    public string Address { get; set; } = null!;

    [OutputArgument]
    public RabbitMqSession Session { get; set; } = null!;

    protected override async Task RunAsync(ActionContext context)
    {
        if (!Uri.TryCreate(Address, UriKind.Absolute, out var address) ||
            (address.Scheme != "amqp" && address.Scheme != "amqps"))
        {
            throw new ArgumentException("An amqp or amqps address is required.", nameof(Address));
        }

        var client = await clientFactory(address).ConfigureAwait(false);
        Session = new RabbitMqSession(client, address.Authority);
    }
}
