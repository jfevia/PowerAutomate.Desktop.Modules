using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.RabbitMQ.Actions;

/// <summary>
///     Rejects a delivery without creating an unbounded retry loop.
/// </summary>
[Action(Id = "RejectMessage")]
[Throws(ErrorCodes.InvalidArgument)]
[Throws(ErrorCodes.ClosedConnection)]
[Throws(ErrorCodes.MessageSettled)]
[Throws(ErrorCodes.Broker)]
public class RejectAction : RabbitMqActionBase
{
    /// <summary>
    ///     Allows broker redelivery only when explicitly selected.
    /// </summary>
    [InputArgument(Order = 2)]
    [DefaultValue(false)]
    public bool AllowRequeue { get; set; }

    /// <summary>
    ///     Supplies the unacknowledged delivery to reject.
    /// </summary>
    [InputArgument(Order = 1, Required = true)]
    public RabbitMqMessage? Message { get; set; }

    /// <summary>
    ///     Disables requeue by default for poison messages.
    /// </summary>
    public RejectAction()
    {
        AllowRequeue = false;
        Message = null;
    }

    /// <summary>
    ///     Settles the message only after the broker accepts its rejection.
    /// </summary>
    /// <param name="context">The desktop-flow action context.</param>
    /// <param name="cancellationToken">Cancels the rejection.</param>
    /// <returns>The broker settlement operation.</returns>
    protected override Task RunAsync(ActionContext context, CancellationToken cancellationToken)
    {
        var message = RequireMessage(Message);
        return message.SettleAsync(true, AllowRequeue, cancellationToken);
    }
}
