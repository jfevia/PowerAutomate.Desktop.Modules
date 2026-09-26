using System.Threading;
using System.Threading.Tasks;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.RabbitMQ.Actions;

/// <summary>
///     Acknowledges a delivery only after the flow processes it.
/// </summary>
[Action(Id = "AcknowledgeMessage")]
[Throws(ErrorCodes.InvalidArgument)]
[Throws(ErrorCodes.ClosedConnection)]
[Throws(ErrorCodes.MessageSettled)]
[Throws(ErrorCodes.Broker)]
public class AcknowledgeAction : RabbitMqActionBase
{
    /// <summary>
    ///     Supplies the unacknowledged delivery to settle.
    /// </summary>
    [InputArgument(Required = true)]
    public RabbitMqMessage? Message { get; set; }

    /// <summary>
    ///     Initializes the message input before PAD sets it.
    /// </summary>
    public AcknowledgeAction()
    {
        Message = null;
    }

    /// <summary>
    ///     Marks the message settled after broker acknowledgement.
    /// </summary>
    /// <param name="context">The desktop-flow action context.</param>
    /// <param name="cancellationToken">Cancels the acknowledgement.</param>
    /// <returns>The broker settlement operation.</returns>
    protected override Task RunAsync(ActionContext context, CancellationToken cancellationToken)
    {
        var message = RequireMessage(Message);
        return message.SettleAsync(false, false, cancellationToken);
    }
}
