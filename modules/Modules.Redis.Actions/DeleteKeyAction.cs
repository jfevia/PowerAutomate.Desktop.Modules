using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.Redis.Actions;

/// <summary>
///     Removes a Redis key and reports whether it existed.
/// </summary>
[Action(Id = "DeleteKey")]
[Throws(ErrorCodes.InvalidArgument)]
[Throws(ErrorCodes.ClosedConnection)]
[Throws(ErrorCodes.Redis)]
public class DeleteKeyAction : RedisActionBase
{
    /// <summary>
    ///     Supplies the shared Redis connection.
    /// </summary>
    [InputArgument(Order = 1, Required = true)]
    public RedisConnection? Connection { get; set; }

    /// <summary>
    ///     Indicates whether a key was actually removed.
    /// </summary>
    [OutputArgument]
    public bool IsDeleted { get; set; }

    /// <summary>
    ///     Identifies the key to remove.
    /// </summary>
    [InputArgument(Order = 2, Required = true)]
    public string? Key { get; set; }

    /// <summary>
    ///     Initializes the action before a flow supplies its inputs.
    /// </summary>
    public DeleteKeyAction()
    {
        Connection = null;
        IsDeleted = false;
        Key = null;
    }

    /// <summary>
    ///     Deletes one key using the shared client.
    /// </summary>
    /// <param name="context">The desktop-flow action context.</param>
    protected override void Run(ActionContext context)
    {
        var client = RequireConnection(Connection);
        var key = RequireValue(Key, nameof(Key));
        IsDeleted = client.HasDeletedKey(key);
    }
}
