using System.Threading;
using System.Threading.Tasks;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.RabbitMQ.Actions;

/// <summary>
///     Polls one message without acknowledging it before flow processing.
/// </summary>
[Action(Id = "ReceiveMessage")]
[Throws(ErrorCodes.InvalidArgument)]
[Throws(ErrorCodes.ClosedConnection)]
[Throws(ErrorCodes.Broker)]
public class ReceiveAction : RabbitMqActionBase
{
    /// <summary>
    ///     Supplies the broker connection used to poll the queue.
    /// </summary>
    [InputArgument(Order = 1, Required = true)]
    public RabbitMqConnection? Connection { get; set; }

    /// <summary>
    ///     Distinguishes an empty queue from a delivered message.
    /// </summary>
    [OutputArgument(Order = 1)]
    public bool IsFound { get; set; }

    /// <summary>
    ///     Keeps the unacknowledged delivery attached to its channel.
    /// </summary>
    [OutputArgument(Order = 2)]
    public RabbitMqMessage? Message { get; set; }

    /// <summary>
    ///     Selects an existing queue to poll once.
    /// </summary>
    [InputArgument(Order = 2, Required = true)]
    public string? Queue { get; set; }

    /// <summary>
    ///     Leaves both outputs unset until a message is received.
    /// </summary>
    public ReceiveAction()
    {
        Connection = null;
        IsFound = false;
        Message = null;
        Queue = null;
    }

    /// <summary>
    ///     Returns one unacknowledged delivery or an empty result.
    /// </summary>
    /// <param name="context">The desktop-flow action context.</param>
    /// <param name="cancellationToken">Cancels the broker poll.</param>
    /// <returns>The broker receive operation.</returns>
    protected override async Task RunAsync(ActionContext context, CancellationToken cancellationToken)
    {
        var connection = RequireConnection(Connection);
        var queue = RequireValue(Queue, nameof(Queue));
        var delivery = await connection.ReceiveAsync(queue, cancellationToken).ConfigureAwait(false);
        if (delivery is null)
        {
            IsFound = false;
            Message = null;
            return;
        }

        IsFound = true;
        Message = new RabbitMqMessage(connection, queue, delivery);
    }
}
