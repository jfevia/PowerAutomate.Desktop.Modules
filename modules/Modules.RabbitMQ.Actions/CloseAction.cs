using System.Threading;
using System.Threading.Tasks;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.RabbitMQ.Actions;

/// <summary>
///     Releases a broker connection after the flow finishes.
/// </summary>
[Action(Id = "CloseRabbitMq")]
[Throws(ErrorCodes.InvalidArgument)]
[Throws(ErrorCodes.Broker)]
public class CloseAction : RabbitMqActionBase
{
    /// <summary>
    ///     Supplies the connection to close.
    /// </summary>
    [InputArgument(Required = true)]
    public RabbitMqConnection? Connection { get; set; }

    /// <summary>
    ///     Initializes the connection input before PAD sets it.
    /// </summary>
    public CloseAction()
    {
        Connection = null;
    }

    /// <summary>
    ///     Closes the channel while permitting repeated cleanup calls.
    /// </summary>
    /// <param name="context">The desktop-flow action context.</param>
    /// <param name="cancellationToken">Cancels the broker shutdown.</param>
    /// <returns>The broker shutdown operation.</returns>
    protected override Task RunAsync(ActionContext context, CancellationToken cancellationToken)
    {
        var connection = RequireConnection(Connection);
        return connection.CloseAsync(cancellationToken);
    }
}
