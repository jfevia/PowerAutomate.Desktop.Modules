using NUnit.Framework;

namespace PowerAutomate.Desktop.Modules.Redis.Actions.Tests;

/// <summary>
///     Checks string writes and their optional expiration.
/// </summary>
[TestFixture]
public sealed class SetValueActionTests : RedisActionFixture
{
    /// <summary>
    ///     Interprets zero as no expiry and positive values as seconds.
    /// </summary>
    /// <param name="expirySeconds">The value supplied to the action.</param>
    /// <param name="expectedExpiry">The expiry expected by the client.</param>
    [TestCase(0, null)]
    [TestCase(60, 60)]
    public void Execute_WhenExpiryIsAllowed_PassesExpiry(int expirySeconds, int? expectedExpiry)
    {
        Client.IsStored = true;
        var action = new SetValueAction
        {
            Connection = Connection,
            Key = "name",
            Value = string.Empty,
            ExpirySeconds = expirySeconds
        };

        RunAction(action);

        Assert.That(action.IsStored, Is.True);
        Assert.That(Client.LastKey, Is.EqualTo("name"));
        Assert.That(Client.LastValue, Is.Empty);
        Assert.That(Client.LastExpiry?.TotalSeconds, Is.EqualTo(expectedExpiry));
    }

    /// <summary>
    ///     Prevents negative expiration values from reaching Redis.
    /// </summary>
    [Test]
    public void Execute_WhenExpiryIsNegative_ReportsInvalidInput()
    {
        var action = new SetValueAction
        {
            Connection = Connection,
            Key = "name",
            Value = "value",
            ExpirySeconds = -1
        };

        AssertError(ErrorCodes.InvalidArgument, action);
    }

    /// <summary>
    ///     Rejects keys that contain no usable characters.
    /// </summary>
    [Test]
    public void Execute_WhenKeyIsBlank_ReportsInvalidInput()
    {
        var action = new SetValueAction
        {
            Connection = Connection,
            Key = string.Empty
        };

        AssertError(ErrorCodes.InvalidArgument, action);
    }

    /// <summary>
    ///     Rejects an absent value instead of storing a null.
    /// </summary>
    [Test]
    public void Execute_WhenValueIsNull_ReportsInvalidInput()
    {
        var action = new SetValueAction
        {
            Connection = Connection,
            Key = "name"
        };

        AssertError(ErrorCodes.InvalidArgument, action);
    }

    /// <summary>
    ///     Reports a rejected write rather than claiming success.
    /// </summary>
    [Test]
    public void Execute_WhenWriteIsRejected_ReportsNotStored()
    {
        var action = new SetValueAction
        {
            Connection = Connection,
            Key = "name",
            Value = "value"
        };

        RunAction(action);

        Assert.That(action.IsStored, Is.False);
    }
}
