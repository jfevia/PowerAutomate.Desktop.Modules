using System.Threading.Tasks;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.RabbitMQ.Actions;

[Action(Id = "ReceiveMessage")]
[Throws(ErrorCodes.InvalidArgument)]
[Throws(ErrorCodes.ClosedConnection)]
[Throws(ErrorCodes.Broker)]
public class ReceiveAction : RabbitMqActionBase
{
    [InputArgument(Order = 1, Required = true)]
    public RabbitMqSession Session { get; set; } = null!;

    [InputArgument(Order = 2, Required = true)]
    public string Queue { get; set; } = null!;

    [OutputArgument(Order = 1)]
    public bool Found { get; set; }

    [OutputArgument(Order = 2)]
    public RabbitMqMessage? Message { get; set; }

    protected override async Task RunAsync(ActionContext context)
    {
        var session = RequireSession(Session);
        var queue = RequireValue(Queue, nameof(Queue));
        var delivery = await session.RunAsync(client => client.ReceiveAsync(queue)).ConfigureAwait(false);
        Found = delivery != null;
        Message = delivery == null ? null : new RabbitMqMessage(session, queue, delivery);
    }
}
