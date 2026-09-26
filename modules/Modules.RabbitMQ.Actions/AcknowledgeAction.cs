using System.Threading.Tasks;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.RabbitMQ.Actions;

[Action(Id = "AcknowledgeMessage")]
[Throws(ErrorCodes.InvalidArgument)]
[Throws(ErrorCodes.ClosedConnection)]
[Throws(ErrorCodes.MessageSettled)]
[Throws(ErrorCodes.Broker)]
public class AcknowledgeAction : RabbitMqActionBase
{
    [InputArgument(Required = true)]
    public RabbitMqMessage Message { get; set; } = null!;

    protected override Task RunAsync(ActionContext context) =>
        RequireMessage(Message).SettleAsync(false, false);
}
