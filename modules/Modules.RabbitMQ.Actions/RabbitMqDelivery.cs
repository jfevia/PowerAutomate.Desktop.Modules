namespace PowerAutomate.Desktop.Modules.RabbitMQ.Actions;

internal sealed class RabbitMqDelivery
{
    internal RabbitMqDelivery(ulong deliveryTag, byte[] body, bool redelivered)
    {
        DeliveryTag = deliveryTag;
        Body = body;
        Redelivered = redelivered;
    }

    internal ulong DeliveryTag { get; }

    internal byte[] Body { get; }

    internal bool Redelivered { get; }
}
