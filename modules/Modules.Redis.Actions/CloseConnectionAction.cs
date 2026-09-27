using System;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.Redis.Actions;

/// <summary>
///     Releases a Redis connection used by desktop-flow actions.
/// </summary>
[Action(Id = "CloseConnection")]
[Throws(ErrorCodes.InvalidArgument)]
[Throws(ErrorCodes.ClosedConnection)]
[Throws(ErrorCodes.Redis)]
public class CloseConnectionAction : RedisActionBase
{
    /// <summary>
    ///     Identifies the connection to close.
    /// </summary>
    [InputArgument(Required = true)]
    public RedisConnection? Connection { get; set; }

    /// <summary>
    ///     Initializes the connection input before PAD sets it.
    /// </summary>
    public CloseConnectionAction()
    {
        Connection = null;
    }

    /// <summary>
    ///     Disposes the connection once, even if called again.
    /// </summary>
    /// <param name="context">The desktop-flow action context.</param>
    protected override void Run(ActionContext context)
    {
        if (Connection == null)
        {
            throw new ArgumentException("A Redis connection is required.", nameof(Connection));
        }

        Connection.Dispose();
    }
}
