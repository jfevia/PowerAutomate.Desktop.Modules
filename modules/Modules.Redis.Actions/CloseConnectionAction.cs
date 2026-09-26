using System;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.Redis.Actions;

[Action(Id = "CloseConnection")]
[Throws(ErrorCodes.InvalidArgument)]
[Throws(ErrorCodes.ClosedConnection)]
[Throws(ErrorCodes.Redis)]
public class CloseConnectionAction : RedisActionBase
{
    [InputArgument(Required = true)]
    public RedisConnection Connection { get; set; } = null!;

    protected override void Run(ActionContext context)
    {
        if (Connection == null)
        {
            throw new ArgumentException("A Redis connection is required.", nameof(Connection));
        }

        Connection.Dispose();
    }
}
