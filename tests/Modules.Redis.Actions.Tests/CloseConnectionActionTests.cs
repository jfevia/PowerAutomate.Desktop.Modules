using NUnit.Framework;

namespace PowerAutomate.Desktop.Modules.Redis.Actions.Tests;

/// <summary>
///     Checks safe disposal across repeated desktop-flow calls.
/// </summary>
[TestFixture]
public sealed class CloseConnectionActionTests : RedisActionFixture
{
    /// <summary>
    ///     Does not dispose the multiplexer twice.
    /// </summary>
    [Test]
    public void Execute_WhenCalledTwice_DisposesOnce()
    {
        var action = new CloseConnectionAction
        {
            Connection = Connection
        };

        RunAction(action);
        RunAction(action);

        Assert.That(Client.DisposeCount, Is.EqualTo(1));
        Assert.That(Connection.IsClosed, Is.True);
    }

    /// <summary>
    ///     Rejects a close without a connection variable.
    /// </summary>
    [Test]
    public void Execute_WhenConnectionIsMissing_ReportsInvalidInput()
    {
        var action = new CloseConnectionAction();

        AssertError(ErrorCodes.InvalidArgument, action);
    }
}
