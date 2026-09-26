using System;
using System.ComponentModel;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.Redis.Actions;

/// <summary>
///     Opens one Redis connection for reuse across desktop-flow actions.
/// </summary>
[Action(Id = "Connect")]
[Throws(ErrorCodes.InvalidArgument)]
[Throws(ErrorCodes.ClosedConnection)]
[Throws(ErrorCodes.Redis)]
public class ConnectAction : RedisActionBase
{
    private readonly IRedisClientFactory clientFactory;

    /// <summary>
    ///     Supplies the endpoint and any Redis authentication settings.
    /// </summary>
    [InputArgument(Order = 1, Required = true)]
    public string? Configuration { get; set; }

    /// <summary>
    ///     Shares the connection without exposing its credentials.
    /// </summary>
    [OutputArgument]
    public RedisConnection? Connection { get; set; }

    /// <summary>
    ///     Selects the database; -1 uses the configured default.
    /// </summary>
    [InputArgument(Order = 2)]
    [DefaultValue(-1)]
    public int DatabaseNumber { get; set; }

    /// <summary>
    ///     Configures the production Redis client factory.
    /// </summary>
    public ConnectAction()
    {
        clientFactory = new RedisClientFactory();
        Configuration = null;
        Connection = null;
        DatabaseNumber = -1;
    }

    /// <summary>
    ///     Permits deterministic tests without connecting to Redis.
    /// </summary>
    /// <param name="clientFactory">The client factory used by the action.</param>
    protected ConnectAction(IRedisClientFactory? clientFactory)
    {
        if (clientFactory is null)
        {
            throw new ArgumentNullException(nameof(clientFactory));
        }

        this.clientFactory = clientFactory;
        Configuration = null;
        Connection = null;
        DatabaseNumber = -1;
    }

    /// <summary>
    ///     Opens the selected database after validating inputs.
    /// </summary>
    /// <param name="context">The desktop-flow action context.</param>
    protected override void Run(ActionContext context)
    {
        var configuration = RequireValue(Configuration, nameof(Configuration));
        if (DatabaseNumber < -1)
        {
            throw new ArgumentOutOfRangeException(nameof(DatabaseNumber));
        }

        var client = clientFactory.Create(configuration, DatabaseNumber);
        Connection = new RedisConnection(client, DatabaseNumber);
    }
}
