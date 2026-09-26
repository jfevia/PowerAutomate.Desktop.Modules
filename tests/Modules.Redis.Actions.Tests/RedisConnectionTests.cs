using System;
using NUnit.Framework;

namespace PowerAutomate.Desktop.Modules.Redis.Actions.Tests;

/// <summary>
///     Checks ownership and safe display of the shared connection.
/// </summary>
[TestFixture]
public sealed class RedisConnectionTests : RedisActionFixture
{
    /// <summary>
    ///     Prevents reuse of a released multiplexer.
    /// </summary>
    [Test]
    public void GetClient_WhenConnectionIsClosed_Throws()
    {
        Connection.Dispose();

        var error = Assert.Throws<ObjectDisposedException>(() => Connection.GetClient());
        Assert.That(error?.ObjectName, Is.EqualTo(nameof(RedisConnection)));
    }

    /// <summary>
    ///     Exposes the client while the connection is open.
    /// </summary>
    [Test]
    public void GetClient_WhenConnectionIsOpen_ReturnsSharedClient()
    {
        Assert.That(Connection.GetClient(), Is.SameAs(Client));
        Assert.That(Connection.ToString(), Is.EqualTo("Redis database 2"));
    }

    /// <summary>
    ///     Requires a real client before exposing the connection variable.
    /// </summary>
    [Test]
    public void RedisConnection_WhenClientIsNull_Throws()
    {
        var error = Assert.Throws<ArgumentNullException>(() =>
        {
            var connection = new RedisConnection(null, 0);
            Assert.Fail($"Unexpected connection: {connection}");
        });

        Assert.That(error?.ParamName, Is.EqualTo("client"));
    }
}
