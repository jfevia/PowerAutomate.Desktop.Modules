using System;
using System.ComponentModel;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.Redis.Actions;

[Action(Id = "Connect")]
[Throws(ErrorCodes.InvalidArgument)]
[Throws(ErrorCodes.ClosedConnection)]
[Throws(ErrorCodes.Redis)]
public class ConnectAction : RedisActionBase
{
    private readonly Func<string, int, IRedisClient> clientFactory;

    public ConnectAction() : this((configuration, databaseNumber) => new RedisClient(configuration, databaseNumber))
    {
    }

    internal ConnectAction(Func<string, int, IRedisClient> clientFactory) =>
        this.clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));

    [InputArgument(Order = 1, Required = true)]
    public string Configuration { get; set; } = null!;

    [InputArgument(Order = 2)]
    [DefaultValue(-1)]
    public int DatabaseNumber { get; set; } = -1;

    [OutputArgument]
    public RedisConnection Connection { get; set; } = null!;

    protected override void Run(ActionContext context)
    {
        var configuration = RequireValue(Configuration, nameof(Configuration));
        if (DatabaseNumber < -1)
        {
            throw new ArgumentOutOfRangeException(nameof(DatabaseNumber));
        }

        Connection = new RedisConnection(clientFactory(configuration, DatabaseNumber), DatabaseNumber);
    }
}
