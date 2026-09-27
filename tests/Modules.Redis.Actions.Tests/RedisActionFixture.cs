using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;

namespace PowerAutomate.Desktop.Modules.Redis.Actions.Tests;

/// <summary>
///     Gives each Redis action test a fresh in-memory client.
/// </summary>
public abstract class RedisActionFixture
{
    /// <summary>
    ///     Records operations without opening a network connection.
    /// </summary>
    protected RedisClientStub Client { get; private set; }

    /// <summary>
    ///     Shares a client between the actions in one test.
    /// </summary>
    protected RedisConnection Connection { get; private set; }

    /// <summary>
    ///     Initializes the required client and connection before NUnit setup.
    /// </summary>
    protected RedisActionFixture()
    {
        Client = new RedisClientStub();
        Connection = new RedisConnection(Client, 2);
    }

    /// <summary>
    ///     Prevents one test from reusing another test's connection.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        Client = new RedisClientStub();
        Connection = new RedisConnection(Client, 2);
    }

    /// <summary>
    ///     Asserts the stable error name exposed to desktop flows.
    /// </summary>
    /// <param name="errorCode">The expected PAD error code.</param>
    /// <param name="action">The action expected to fail.</param>
    protected void AssertError(string errorCode, RedisActionBase action)
    {
        var exception = Assert.Throws<ActionException>(() => RunAction(action))!;
        Assert.That(exception.Name, Is.EqualTo(errorCode));
    }

    /// <summary>
    ///     Executes an action with a fresh desktop-flow context.
    /// </summary>
    /// <param name="action">The Redis action to run.</param>
    protected void RunAction(RedisActionBase action)
    {
        var context = new ActionContext();
        action.Execute(context);
    }
}
