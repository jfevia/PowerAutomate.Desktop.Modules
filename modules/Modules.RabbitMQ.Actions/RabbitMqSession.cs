using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Enums;

namespace PowerAutomate.Desktop.Modules.RabbitMQ.Actions;

[Type(FriendlyName = nameof(RabbitMqSession) + "_FriendlyName",
      FriendlyNamePlural = nameof(RabbitMqSession) + "_FriendlyNamePlural",
      DefaultPropertyVisibility = Visibility.Visible)]
public sealed class RabbitMqSession
{
    private readonly IRabbitMqClient client;
    private readonly SemaphoreSlim channelGate = new SemaphoreSlim(1, 1);

    internal RabbitMqSession(IRabbitMqClient client, string endpoint)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        Endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
    }

    [Property]
    public string Endpoint { get; }

    [Property]
    public bool IsClosed { get; private set; }

    internal async Task<T> RunAsync<T>(Func<IRabbitMqClient, Task<T>> operation)
    {
        await channelGate.WaitAsync().ConfigureAwait(false);
        try
        {
            if (IsClosed)
            {
                throw new ObjectDisposedException(nameof(RabbitMqSession));
            }

            return await operation(client).ConfigureAwait(false);
        }
        finally
        {
            channelGate.Release();
        }
    }

    internal async Task RunAsync(Func<IRabbitMqClient, Task> operation) =>
        await RunAsync(async client =>
        {
            await operation(client).ConfigureAwait(false);
            return true;
        }).ConfigureAwait(false);

    internal async Task CloseAsync()
    {
        await channelGate.WaitAsync().ConfigureAwait(false);
        try
        {
            if (IsClosed)
            {
                return;
            }

            await client.CloseAsync().ConfigureAwait(false);
            IsClosed = true;
        }
        finally
        {
            channelGate.Release();
        }
    }

    public override string ToString() => Endpoint;
}
