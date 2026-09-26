using NUnit.Framework;
using StackExchange.Redis;

namespace PowerAutomate.Desktop.Modules.Redis.Actions.Tests;

/// <summary>
///     Checks missing keys, empty values, and client errors.
/// </summary>
[TestFixture]
public sealed class GetValueActionTests : RedisActionFixture
{
    /// <summary>
    ///     Distinguishes a missing key from an empty stored string.
    /// </summary>
    /// <param name="value">The value returned by the stub.</param>
    /// <param name="isFound">Whether the key is expected to exist.</param>
    [TestCase("value", true)]
    [TestCase("", true)]
    [TestCase(null, false)]
    public void Execute_WhenKeyIsRead_ReportsPresence(string? value, bool isFound)
    {
        Client.Value = value;
        var action = new GetValueAction
        {
            Connection = Connection,
            Key = "name"
        };

        RunAction(action);

        Assert.That(action.Value, Is.EqualTo(value));
        Assert.That(action.IsFound, Is.EqualTo(isFound));
        Assert.That(Client.LastKey, Is.EqualTo("name"));
    }

    /// <summary>
    ///     Rejects a read without a shared connection.
    /// </summary>
    [Test]
    public void Execute_WhenConnectionIsMissing_ReportsInvalidInput()
    {
        var action = new GetValueAction
        {
            Key = "name"
        };

        AssertError(ErrorCodes.InvalidArgument, action);
    }

    /// <summary>
    ///     Prevents reads after the multiplexer has been disposed.
    /// </summary>
    [Test]
    public void Execute_WhenConnectionIsClosed_ReportsClosedConnection()
    {
        Connection.Dispose();
        var action = new GetValueAction
        {
            Connection = Connection,
            Key = "name"
        };

        AssertError(ErrorCodes.ClosedConnection, action);
    }

    /// <summary>
    ///     Rejects keys without a name before querying Redis.
    /// </summary>
    [Test]
    public void Execute_WhenKeyIsBlank_ReportsInvalidInput()
    {
        var action = new GetValueAction
        {
            Connection = Connection,
            Key = " "
        };

        AssertError(ErrorCodes.InvalidArgument, action);
    }

    /// <summary>
    ///     Surfaces Redis failures rather than reporting a missing key.
    /// </summary>
    [Test]
    public void Execute_WhenServerFails_ReportsRedisError()
    {
        Client.Failure = new RedisServerException(RedisErrorKind.UnknownError, CommandFlags.None, "failure");
        var action = new GetValueAction
        {
            Connection = Connection,
            Key = "name"
        };

        AssertError(ErrorCodes.Redis, action);
    }
}
