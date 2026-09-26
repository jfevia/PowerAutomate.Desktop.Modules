using System;
using NUnit.Framework;
using StackExchange.Redis;

namespace PowerAutomate.Desktop.Modules.Redis.Actions.Tests;

/// <summary>
///     Checks connection validation without requiring a Redis server.
/// </summary>
[TestFixture]
public sealed class ConnectActionTests : RedisActionFixture
{
    /// <summary>
    ///     Leaves the output unset until a connection is opened.
    /// </summary>
    [Test]
    public void ConnectAction_WhenCreated_HasNoConnection()
    {
        var action = new ConnectAction();

        Assert.That(action.Connection, Is.Null);
        Assert.That(action.DatabaseNumber, Is.EqualTo(-1));
    }

    /// <summary>
    ///     Refuses a missing connection factory at the test boundary.
    /// </summary>
    [Test]
    public void ConnectAction_WhenFactoryIsNull_Throws()
    {
        var error = Assert.Throws<ArgumentNullException>(() =>
        {
            var action = new StubConnectAction(null);
            Assert.Fail($"Unexpected action: {action}");
        });

        Assert.That(error!.ParamName, Is.EqualTo("clientFactory"));
    }

    /// <summary>
    ///     Does not expose authentication values in the connection display.
    /// </summary>
    [Test]
    public void Execute_WhenConfigurationContainsPassword_KeepsItPrivate()
    {
        var factory = new RedisClientFactoryStub(Client);
        var action = new StubConnectAction(factory)
        {
            Configuration = "localhost,password=private",
            DatabaseNumber = 2
        };

        RunAction(action);

        Assert.That(factory.GetConfiguration(), Is.EqualTo("localhost,password=private"));
        Assert.That(factory.GetDatabaseNumber(), Is.EqualTo(2));
        Assert.That(action.Connection?.DatabaseNumber, Is.EqualTo(2));
        Assert.That(action.Connection?.ToString(), Is.EqualTo("Redis database 2"));
        Assert.That(action.Connection?.ToString(), Does.Not.Contain("private"));
    }

    /// <summary>
    ///     Refuses a configuration without an endpoint.
    /// </summary>
    /// <param name="configuration">The invalid Redis configuration.</param>
    [TestCase("")]
    [TestCase(" ")]
    public void Execute_WhenConfigurationIsBlank_ReportsInvalidInput(string configuration)
    {
        var factory = new RedisClientFactoryStub(Client);
        var action = new StubConnectAction(factory)
        {
            Configuration = configuration
        };

        AssertError(ErrorCodes.InvalidArgument, action);
    }

    /// <summary>
    ///     Uses the connection string's default database when none is selected.
    /// </summary>
    [Test]
    public void Execute_WhenDatabaseIsUnspecified_UsesDefaultDatabase()
    {
        var factory = new RedisClientFactoryStub(Client);
        var action = new StubConnectAction(factory)
        {
            Configuration = "localhost"
        };

        RunAction(action);

        Assert.That(action.Connection?.DatabaseNumber, Is.EqualTo(-1));
    }

    /// <summary>
    ///     Rejects database numbers below the SDK's default sentinel.
    /// </summary>
    [Test]
    public void Execute_WhenDatabaseNumberInvalid_ReportsInvalidInput()
    {
        var factory = new RedisClientFactoryStub(Client);
        var action = new StubConnectAction(factory)
        {
            Configuration = "localhost",
            DatabaseNumber = -2
        };

        AssertError(ErrorCodes.InvalidArgument, action);
    }

    /// <summary>
    ///     Reports a broker failure instead of returning an empty connection.
    /// </summary>
    [Test]
    public void Execute_WhenServerFails_ReportsRedisError()
    {
        var factory = new RedisClientFactoryStub(Client);
        var failure = new RedisServerException(RedisErrorKind.UnknownError, CommandFlags.None, "failure");
        factory.SetFailure(failure);
        var action = new StubConnectAction(factory)
        {
            Configuration = "localhost"
        };

        AssertError(ErrorCodes.Redis, action);
    }
}
