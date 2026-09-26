using System.Threading.Tasks;

namespace PowerAutomate.Desktop.Modules.RabbitMQ.Actions;

internal interface IRabbitMqClient
{
    Task PublishAsync(string exchange, string routingKey, byte[] body, bool persistent);

    Task<RabbitMqDelivery?> ReceiveAsync(string queue);

    Task AcknowledgeAsync(ulong deliveryTag);

    Task RejectAsync(ulong deliveryTag, bool requeue);

    Task CloseAsync();
}
