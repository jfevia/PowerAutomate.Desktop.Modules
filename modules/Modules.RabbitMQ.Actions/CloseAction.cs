using System.Threading.Tasks;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.RabbitMQ.Actions;

[Action(Id = "CloseRabbitMq")]
[Throws(ErrorCodes.InvalidArgument)]
[Throws(ErrorCodes.Broker)]
public class CloseAction : RabbitMqActionBase
{
    [InputArgument(Required = true)]
    public RabbitMqSession Session { get; set; } = null!;

    protected override Task RunAsync(ActionContext context) =>
        RequireSession(Session).CloseAsync();
}
