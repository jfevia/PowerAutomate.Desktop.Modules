using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace PowerAutomate.Desktop.Modules.RabbitMQ.Actions;

[ExcludeFromCodeCoverage]
internal sealed class RabbitMqClient : IRabbitMqClient
{
    private readonly IConnection connection;
    private readonly IChannel channel;

    private RabbitMqClient(IConnection connection, IChannel channel)
    {
        this.connection = connection;
        this.channel = channel;
    }

    internal static async Task<IRabbitMqClient> ConnectAsync(Uri address)
    {
        var connection = await new ConnectionFactory { Uri = address }.CreateConnectionAsync().ConfigureAwait(false);
        IChannel? channel = null;
        try
        {
            channel = await connection.CreateChannelAsync(new CreateChannelOptions(true, true)).ConfigureAwait(false);
            return new RabbitMqClient(connection, channel);
        }
        finally
        {
            if (channel == null)
            {
                await connection.DisposeAsync().ConfigureAwait(false);
            }
        }
    }

    public async Task PublishAsync(string exchange, string routingKey, byte[] body, bool persistent)
    {
        var properties = new BasicProperties { Persistent = persistent, ContentType = "text/plain", ContentEncoding = "utf-8" };
        await channel.BasicPublishAsync(exchange, routingKey, true, properties, body).ConfigureAwait(false);
    }

    public async Task<RabbitMqDelivery?> ReceiveAsync(string queue)
    {
        var result = await channel.BasicGetAsync(queue, false).ConfigureAwait(false);
        return result == null ? null : new RabbitMqDelivery(result.DeliveryTag, result.Body.ToArray(), result.Redelivered);
    }

    public async Task AcknowledgeAsync(ulong deliveryTag) =>
        await channel.BasicAckAsync(deliveryTag, false).ConfigureAwait(false);

    public async Task RejectAsync(ulong deliveryTag, bool requeue) =>
        await channel.BasicNackAsync(deliveryTag, false, requeue).ConfigureAwait(false);

    public async Task CloseAsync()
    {
        try
        {
            await channel.DisposeAsync().ConfigureAwait(false);
        }
        finally
        {
            await connection.DisposeAsync().ConfigureAwait(false);
        }
    }
}
