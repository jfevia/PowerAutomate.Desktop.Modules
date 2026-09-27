using System;
using System.ComponentModel;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.Redis.Actions;

/// <summary>
///     Stores a Redis string key with optional expiration.
/// </summary>
[Action(Id = "SetValue")]
[Throws(ErrorCodes.InvalidArgument)]
[Throws(ErrorCodes.ClosedConnection)]
[Throws(ErrorCodes.Redis)]
public class SetValueAction : RedisActionBase
{
    /// <summary>
    ///     Supplies the shared Redis connection.
    /// </summary>
    [InputArgument(Order = 1, Required = true)]
    public RedisConnection? Connection { get; set; }

    /// <summary>
    ///     Sets how long the key exists; zero disables expiry.
    /// </summary>
    [InputArgument(Order = 4)]
    [DefaultValue(0)]
    public int ExpirySeconds { get; set; }

    /// <summary>
    ///     Indicates whether Redis stored the value.
    /// </summary>
    [OutputArgument]
    public bool IsStored { get; set; }

    /// <summary>
    ///     Identifies the Redis key to write.
    /// </summary>
    [InputArgument(Order = 2, Required = true)]
    public string? Key { get; set; }

    /// <summary>
    ///     Stores an empty string when provided rather than removing the key.
    /// </summary>
    [InputArgument(Order = 3, Required = true)]
    public string? Value { get; set; }

    /// <summary>
    ///     Initializes defaults for the optional expiration.
    /// </summary>
    public SetValueAction()
    {
        Connection = null;
        ExpirySeconds = 0;
        IsStored = false;
        Key = null;
        Value = null;
    }

    /// <summary>
    ///     Validates and writes a string key with an optional expiry.
    /// </summary>
    /// <param name="context">The desktop-flow action context.</param>
    protected override void Run(ActionContext context)
    {
        var client = RequireConnection(Connection);
        var key = RequireValue(Key, nameof(Key));
        var value = Value;
        if (value == null || ExpirySeconds < 0)
        {
            throw new ArgumentException("A value and a nonnegative expiry are required.");
        }

        var expiry = ExpirySeconds == 0 ? (TimeSpan?)null : TimeSpan.FromSeconds(ExpirySeconds);
        IsStored = client.HasStoredValue(key, value, expiry);
    }
}
