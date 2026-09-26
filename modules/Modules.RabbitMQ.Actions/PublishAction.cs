using System;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.RabbitMQ.Actions;

/// <summary>
///     Publishes UTF-8 text with broker confirmation and mandatory routing.
/// </summary>
[Action(Id = "PublishMessage")]
[Throws(ErrorCodes.InvalidArgument)]
[Throws(ErrorCodes.ClosedConnection)]
[Throws(ErrorCodes.Broker)]
public class PublishAction : RabbitMqActionBase
{
    /// <summary>
    ///     Supplies the text encoded as UTF-8 for the broker.
    /// </summary>
    [InputArgument(Order = 4, Required = true, Multiline = true)]
    public string? Body { get; set; }

    /// <summary>
    ///     Supplies the confirmed broker connection.
    /// </summary>
    [InputArgument(Order = 1, Required = true)]
    public RabbitMqConnection? Connection { get; set; }

    /// <summary>
    ///     Selects the default exchange when left empty.
    /// </summary>
    [InputArgument(Order = 2, Required = false)]
    [DefaultValue("")]
    public string? Exchange { get; set; }

    /// <summary>
    ///     Marks the message persistent for durable queues.
    /// </summary>
    [InputArgument(Order = 5)]
    [DefaultValue(true)]
    public bool IsPersistent { get; set; }

    /// <summary>
    ///     Routes through a binding or queue on the default exchange.
    /// </summary>
    [InputArgument(Order = 3, Required = true)]
    public string? RoutingKey { get; set; }

    /// <summary>
    ///     Initializes the documented default exchange and delivery mode.
    /// </summary>
    public PublishAction()
    {
        Body = null;
        Connection = null;
        Exchange = string.Empty;
        IsPersistent = true;
        RoutingKey = null;
    }

    /// <summary>
    ///     Converts text to bytes before publishing.
    /// </summary>
    /// <param name="context">The desktop-flow action context.</param>
    /// <param name="cancellationToken">Cancels the broker publish.</param>
    /// <returns>The broker-confirmed publish operation.</returns>
    protected override Task RunAsync(ActionContext context, CancellationToken cancellationToken)
    {
        var connection = RequireConnection(Connection);
        var routingKey = RequireValue(RoutingKey, nameof(RoutingKey));
        var exchange = Exchange;
        var body = Body;
        if (exchange is null || body is null)
        {
            throw new ArgumentException("An exchange and a message body are required.");
        }

        var payload = Encoding.UTF8.GetBytes(body);
        var request = new RabbitMqPublishRequest(exchange, routingKey, payload, IsPersistent);
        return connection.PublishAsync(request, cancellationToken);
    }
}
