using System;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using StackExchange.Redis;

namespace PowerAutomate.Desktop.Modules.Redis.Actions.Tests;

[TestFixture]
public class RedisActionTests
{
    private FakeRedisClient client = null!;
    private RedisConnection connection = null!;

    [SetUp]
    public void SetUp()
    {
        client = new FakeRedisClient();
        connection = new RedisConnection(client, 2);
    }

    [Test]
    public void Connect_CreatesReusableConnectionWithoutExposingConfiguration()
    {
        string? suppliedConfiguration = null;
        int suppliedDatabase = -2;
        var action = new ConnectAction((configuration, database) =>
        {
            suppliedConfiguration = configuration;
            suppliedDatabase = database;
            return client;
        })
        {
            Configuration = "localhost,password=private",
            DatabaseNumber = 2
        };

        action.Execute(new ActionContext());

        Assert.That(suppliedConfiguration, Is.EqualTo("localhost,password=private"));
        Assert.That(suppliedDatabase, Is.EqualTo(2));
        Assert.That(action.Connection.DatabaseNumber, Is.EqualTo(2));
        Assert.That(action.Connection.ToString(), Is.EqualTo("Redis database 2"));
        Assert.That(action.Connection.ToString(), Does.Not.Contain("private"));
    }

    [Test]
    public void Connect_UsesDefaultDatabase()
    {
        var action = new ConnectAction((_, _) => client) { Configuration = "localhost" };

        action.Execute(new ActionContext());

        Assert.That(action.Connection.DatabaseNumber, Is.EqualTo(-1));
    }

    [Test]
    public void Connect_DefaultConstructorIsAvailable()
    {
        Assert.That(new ConnectAction().Connection, Is.Null);
    }

    [Test]
    public void Connect_WithNullFactory_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new ConnectAction(null!));
    }

    [Test]
    public void Connection_WithNullClient_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new RedisConnection(null!, 0));
    }

    [TestCase("")]
    [TestCase(" ")]
    public void Connect_WithBlankConfiguration_ReportsInvalidInput(string configuration)
    {
        var action = new ConnectAction((_, _) => client) { Configuration = configuration };

        AssertError(ErrorCodes.InvalidArgument, action);
    }

    [Test]
    public void Connect_WithInvalidDatabase_ReportsInvalidInput()
    {
        var action = new ConnectAction((_, _) => client) { Configuration = "localhost", DatabaseNumber = -2 };

        AssertError(ErrorCodes.InvalidArgument, action);
    }

    [Test]
    public void Connect_WhenServerFails_ReportsRedisError()
    {
        var action = new ConnectAction((_, _) =>
            throw new RedisServerException(RedisErrorKind.UnknownError, CommandFlags.None, "failure"))
        {
            Configuration = "localhost"
        };

        AssertError(ErrorCodes.Redis, action);
    }

    [TestCase("value", true)]
    [TestCase("", true)]
    [TestCase(null, false)]
    public void Get_ReturnsValueAndFoundFlag(string? value, bool expectedFound)
    {
        client.Value = value;
        var action = new GetValueAction { Connection = connection, Key = "name" };

        action.Execute(new ActionContext());

        Assert.That(action.Value, Is.EqualTo(value));
        Assert.That(action.Found, Is.EqualTo(expectedFound));
        Assert.That(client.LastKey, Is.EqualTo("name"));
    }

    [Test]
    public void Get_WithMissingConnection_ReportsInvalidInput()
    {
        AssertError(ErrorCodes.InvalidArgument, new GetValueAction { Key = "name" });
    }

    [Test]
    public void Get_WithClosedConnection_ReportsClosedConnection()
    {
        connection.Dispose();

        AssertError(ErrorCodes.ClosedConnection, new GetValueAction { Connection = connection, Key = "name" });
    }

    [Test]
    public void Get_WithBlankKey_ReportsInvalidInput()
    {
        AssertError(ErrorCodes.InvalidArgument, new GetValueAction { Connection = connection, Key = " " });
    }

    [Test]
    public void Get_WhenServerFails_ReportsRedisError()
    {
        client.Failure = new RedisServerException(RedisErrorKind.UnknownError, CommandFlags.None, "failure");

        AssertError(ErrorCodes.Redis, new GetValueAction { Connection = connection, Key = "name" });
    }

    [TestCase(0, null)]
    [TestCase(60, 60)]
    public void Set_PassesExpiry(int expirySeconds, int? expectedExpiry)
    {
        client.SetResult = true;
        var action = new SetValueAction
        {
            Connection = connection,
            Key = "name",
            Value = "",
            ExpirySeconds = expirySeconds
        };

        action.Execute(new ActionContext());

        Assert.That(action.WasSet, Is.True);
        Assert.That(client.LastKey, Is.EqualTo("name"));
        Assert.That(client.LastValue, Is.Empty);
        Assert.That(client.LastExpiry?.TotalSeconds, Is.EqualTo(expectedExpiry));
    }

    [Test]
    public void Set_WhenServerDoesNotStoreValue_ReturnsFalse()
    {
        var action = new SetValueAction { Connection = connection, Key = "name", Value = "value" };

        action.Execute(new ActionContext());

        Assert.That(action.WasSet, Is.False);
    }

    [Test]
    public void Set_WithBlankKey_ReportsInvalidInput()
    {
        AssertError(ErrorCodes.InvalidArgument, new SetValueAction { Connection = connection, Key = "" });
    }

    [Test]
    public void Set_WithNullValue_ReportsInvalidInput()
    {
        AssertError(ErrorCodes.InvalidArgument, new SetValueAction { Connection = connection, Key = "name" });
    }

    [Test]
    public void Set_WithNegativeExpiry_ReportsInvalidInput()
    {
        AssertError(ErrorCodes.InvalidArgument,
            new SetValueAction { Connection = connection, Key = "name", Value = "value", ExpirySeconds = -1 });
    }

    [TestCase(true)]
    [TestCase(false)]
    public void Delete_ReturnsWhetherTheKeyExisted(bool deleted)
    {
        client.DeleteResult = deleted;
        var action = new DeleteKeyAction { Connection = connection, Key = "name" };

        action.Execute(new ActionContext());

        Assert.That(action.Deleted, Is.EqualTo(deleted));
        Assert.That(client.LastKey, Is.EqualTo("name"));
    }

    [Test]
    public void Delete_WithMissingKey_ReportsInvalidInput()
    {
        AssertError(ErrorCodes.InvalidArgument, new DeleteKeyAction { Connection = connection, Key = " " });
    }

    [Test]
    public void Close_DisposesConnectionOnlyOnce()
    {
        var action = new CloseConnectionAction { Connection = connection };

        action.Execute(new ActionContext());
        action.Execute(new ActionContext());

        Assert.That(client.DisposeCount, Is.EqualTo(1));
        Assert.That(connection.IsClosed, Is.True);
    }

    [Test]
    public void Close_WithMissingConnection_ReportsInvalidInput()
    {
        AssertError(ErrorCodes.InvalidArgument, new CloseConnectionAction());
    }

    [Test]
    public void Resources_HaveModuleAndConnectionNames()
    {
        var resources = Properties.Resources.ResourceManager;

        Assert.That(resources.GetString("Redis_FriendlyName"), Is.EqualTo("Redis"));
        Assert.That(resources.GetString("RedisConnection_FriendlyName"), Is.EqualTo("Redis connection"));
    }

    private static void AssertError(string errorCode, RedisActionBase action)
    {
        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo(errorCode));
    }

    private sealed class FakeRedisClient : IRedisClient
    {
        public string? Value { get; set; }
        public string? LastKey { get; private set; }
        public string? LastValue { get; private set; }
        public TimeSpan? LastExpiry { get; private set; }
        public bool SetResult { get; set; }
        public bool DeleteResult { get; set; }
        public int DisposeCount { get; private set; }
        public Exception? Failure { get; set; }

        public string? Get(string key)
        {
            if (Failure != null) throw Failure;
            LastKey = key;
            return Value;
        }

        public bool Set(string key, string value, TimeSpan? expiry)
        {
            LastKey = key;
            LastValue = value;
            LastExpiry = expiry;
            return SetResult;
        }

        public bool Delete(string key)
        {
            LastKey = key;
            return DeleteResult;
        }

        public void Dispose() => DisposeCount++;
    }
}
