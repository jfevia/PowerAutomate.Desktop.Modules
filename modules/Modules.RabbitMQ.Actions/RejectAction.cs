using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.RabbitMQ.Actions;

[Action(Id = "RejectMessage")]
[Throws(ErrorCodes.InvalidArgument)]
[Throws(ErrorCodes.ClosedConnection)]
[Throws(ErrorCodes.MessageSettled)]
[Throws(ErrorCodes.Broker)]
public class RejectAction : RabbitMqActionBase
{
    [InputArgument(Order = 1, Required = true)]
    public RabbitMqMessage Message { get; set; } = null!;

    [InputArgument(Order = 2)]
    [DefaultValue(false)]
    public bool Requeue { get; set; }

    protected override Task RunAsync(ActionContext context) =>
        RequireMessage(Message).SettleAsync(true, Requeue);
}
