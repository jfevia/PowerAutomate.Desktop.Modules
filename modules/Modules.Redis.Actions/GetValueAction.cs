using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.Redis.Actions;

/// <summary>
///     Distinguishes a missing Redis key from an empty string value.
/// </summary>
[Action(Id = "GetValue")]
[Throws(ErrorCodes.InvalidArgument)]
[Throws(ErrorCodes.ClosedConnection)]
[Throws(ErrorCodes.Redis)]
public class GetValueAction : RedisActionBase
{
    /// <summary>
    ///     Supplies the shared Redis connection.
    /// </summary>
    [InputArgument(Order = 1, Required = true)]
    public RedisConnection? Connection { get; set; }

    /// <summary>
    ///     Indicates whether a value exists, even if it is empty.
    /// </summary>
    [OutputArgument(Order = 2)]
    public bool IsFound { get; set; }

    /// <summary>
    ///     Identifies the Redis key to read.
    /// </summary>
    [InputArgument(Order = 2, Required = true)]
    public string? Key { get; set; }

    /// <summary>
    ///     Contains null only when the requested key is absent.
    /// </summary>
    [OutputArgument(Order = 1)]
    public string? Value { get; set; }

    /// <summary>
    ///     Initializes the action before inputs are supplied by PAD.
    /// </summary>
    public GetValueAction()
    {
        Connection = null;
        IsFound = false;
        Key = null;
        Value = null;
    }

    /// <summary>
    ///     Reads one key from the reusable connection.
    /// </summary>
    /// <param name="context">The desktop-flow action context.</param>
    protected override void Run(ActionContext context)
    {
        var client = RequireConnection(Connection);
        var key = RequireValue(Key, nameof(Key));
        Value = client.GetValue(key);
        IsFound = Value != null;
    }
}
