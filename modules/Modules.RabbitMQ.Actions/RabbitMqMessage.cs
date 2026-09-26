using System;
using System.Threading.Tasks;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Enums;

namespace PowerAutomate.Desktop.Modules.RabbitMQ.Actions;

[Type(FriendlyName = nameof(RabbitMqMessage) + "_FriendlyName",
      FriendlyNamePlural = nameof(RabbitMqMessage) + "_FriendlyNamePlural",
      DefaultPropertyVisibility = Visibility.Visible)]
public sealed class RabbitMqMessage
{
    private readonly RabbitMqSession session;
    private readonly ulong deliveryTag;

    internal RabbitMqMessage(RabbitMqSession session, string queue, RabbitMqDelivery delivery)
    {
        this.session = session ?? throw new ArgumentNullException(nameof(session));
        Queue = queue ?? throw new ArgumentNullException(nameof(queue));
        deliveryTag = delivery.DeliveryTag;
        BodyBase64 = Convert.ToBase64String(delivery.Body);
        Redelivered = delivery.Redelivered;
    }

    [Property]
    public string Queue { get; }

    [Property]
    public string BodyBase64 { get; }

    [Property]
    public bool Redelivered { get; }

    [Property]
    public bool IsSettled { get; private set; }

    internal Task SettleAsync(bool reject, bool requeue) =>
        session.RunAsync(async client =>
        {
            if (IsSettled)
            {
                throw new ActionException(ErrorCodes.MessageSettled, "The message was already acknowledged or rejected.");
            }

            if (reject)
            {
                await client.RejectAsync(deliveryTag, requeue).ConfigureAwait(false);
            }
            else
            {
                await client.AcknowledgeAsync(deliveryTag).ConfigureAwait(false);
            }

            IsSettled = true;
        });

    public override string ToString() => $"RabbitMQ message from {Queue}";
}
