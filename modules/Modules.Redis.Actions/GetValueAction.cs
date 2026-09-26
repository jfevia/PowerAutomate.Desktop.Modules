using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.Redis.Actions;

[Action(Id = "GetValue")]
[Throws(ErrorCodes.InvalidArgument)]
[Throws(ErrorCodes.ClosedConnection)]
[Throws(ErrorCodes.Redis)]
public class GetValueAction : RedisActionBase
{
    [InputArgument(Order = 1, Required = true)]
    public RedisConnection Connection { get; set; } = null!;

    [InputArgument(Order = 2, Required = true)]
    public string Key { get; set; } = null!;

    [OutputArgument(Order = 1)]
    public string? Value { get; set; }

    [OutputArgument(Order = 2)]
    public bool Found { get; set; }

    protected override void Run(ActionContext context)
    {
        var client = RequireConnection(Connection);
        Value = client.Get(RequireValue(Key, nameof(Key)));
        Found = Value != null;
    }
}
