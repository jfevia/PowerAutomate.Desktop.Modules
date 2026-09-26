using NUnit.Framework;

namespace PowerAutomate.Desktop.Modules.Redis.Actions.Tests;

/// <summary>
///     Checks delete results and invalid keys.
/// </summary>
[TestFixture]
public sealed class DeleteKeyActionTests : RedisActionFixture
{
    /// <summary>
    ///     Preserves whether a key actually existed.
    /// </summary>
    /// <param name="isDeleted">The result supplied by the stub.</param>
    [TestCase(true)]
    [TestCase(false)]
    public void Execute_WhenKeyIsDeleted_ReportsExistence(bool isDeleted)
    {
        Client.IsDeleted = isDeleted;
        var action = new DeleteKeyAction
        {
            Connection = Connection,
            Key = "name"
        };

        RunAction(action);

        Assert.That(action.IsDeleted, Is.EqualTo(isDeleted));
        Assert.That(Client.LastKey, Is.EqualTo("name"));
    }

    /// <summary>
    ///     Rejects a delete with no key name.
    /// </summary>
    [Test]
    public void Execute_WhenKeyIsBlank_ReportsInvalidInput()
    {
        var action = new DeleteKeyAction
        {
            Connection = Connection,
            Key = " "
        };

        AssertError(ErrorCodes.InvalidArgument, action);
    }
}
