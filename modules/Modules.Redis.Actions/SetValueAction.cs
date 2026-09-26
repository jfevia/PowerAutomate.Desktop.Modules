using System;
using System.ComponentModel;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.Redis.Actions;

[Action(Id = "SetValue")]
[Throws(ErrorCodes.InvalidArgument)]
[Throws(ErrorCodes.ClosedConnection)]
[Throws(ErrorCodes.Redis)]
public class SetValueAction : RedisActionBase
{
    [InputArgument(Order = 1, Required = true)]
    public RedisConnection Connection { get; set; } = null!;

    [InputArgument(Order = 2, Required = true)]
    public string Key { get; set; } = null!;

    [InputArgument(Order = 3, Required = true)]
    public string Value { get; set; } = null!;

    [InputArgument(Order = 4)]
    [DefaultValue(0)]
    public int ExpirySeconds { get; set; }

    [OutputArgument]
    public bool WasSet { get; set; }

    protected override void Run(ActionContext context)
    {
        var client = RequireConnection(Connection);
        var key = RequireValue(Key, nameof(Key));
        if (Value == null || ExpirySeconds < 0)
        {
            throw new ArgumentException("A value and a nonnegative expiry are required.");
        }

        var expiry = ExpirySeconds == 0 ? (TimeSpan?)null : TimeSpan.FromSeconds(ExpirySeconds);
        WasSet = client.Set(key, Value, expiry);
    }
}
