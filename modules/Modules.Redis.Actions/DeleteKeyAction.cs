using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.Redis.Actions;

[Action(Id = "DeleteKey")]
[Throws(ErrorCodes.InvalidArgument)]
[Throws(ErrorCodes.ClosedConnection)]
[Throws(ErrorCodes.Redis)]
public class DeleteKeyAction : RedisActionBase
{
    [InputArgument(Order = 1, Required = true)]
    public RedisConnection Connection { get; set; } = null!;

    [InputArgument(Order = 2, Required = true)]
    public string Key { get; set; } = null!;

    [OutputArgument]
    public bool Deleted { get; set; }

    protected override void Run(ActionContext context) =>
        Deleted = RequireConnection(Connection).Delete(RequireValue(Key, nameof(Key)));
}
