using System;
using System.Threading;

namespace PowerAutomate.Desktop.Modules.RabbitMQ.Actions.Tests;

/// <summary>
///     Fails if broker work posts to the caller's synchronization context.
/// </summary>
public sealed class ThrowingSynchronizationContext : SynchronizationContext
{
    /// <summary>
    ///     Rejects a continuation dispatched on the caller's thread.
    /// </summary>
    /// <param name="callback">The callback that must not run here.</param>
    /// <param name="state">The callback state.</param>
    public override void Post(SendOrPostCallback callback, object? state)
    {
        throw new InvalidOperationException("An action used the caller's synchronization context.");
    }
}
