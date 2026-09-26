using System;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.RabbitMQ.Actions;

[Action(Id = "PublishMessage")]
[Throws(ErrorCodes.InvalidArgument)]
[Throws(ErrorCodes.ClosedConnection)]
[Throws(ErrorCodes.Broker)]
public class PublishAction : RabbitMqActionBase
{
    [InputArgument(Order = 1, Required = true)]
    public RabbitMqSession Session { get; set; } = null!;

    [InputArgument(Order = 2, Required = false)]
    [DefaultValue("")]
    public string Exchange { get; set; } = string.Empty;

    [InputArgument(Order = 3, Required = true)]
    public string RoutingKey { get; set; } = null!;

    [InputArgument(Order = 4, Required = true, Multiline = true)]
    public string Body { get; set; } = null!;

    [InputArgument(Order = 5)]
    [DefaultValue(true)]
    public bool Persistent { get; set; } = true;

    protected override Task RunAsync(ActionContext context)
    {
        var session = RequireSession(Session);
        var routingKey = RequireValue(RoutingKey, nameof(RoutingKey));
        if (Exchange == null || Body == null)
        {
            throw new ArgumentException("An exchange and a message body are required.");
        }

        var body = Encoding.UTF8.GetBytes(Body);
        return session.RunAsync(client => client.PublishAsync(Exchange, routingKey, body, Persistent));
    }
}
