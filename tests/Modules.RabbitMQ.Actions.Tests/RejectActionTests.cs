using NUnit.Framework;

namespace PowerAutomate.Desktop.Modules.RabbitMQ.Actions.Tests;

/// <summary>
///     Checks explicit rejection and poison-message requeue decisions.
/// </summary>
[TestFixture]
public sealed class RejectActionTests : RabbitMqActionFixture
{
    /// <summary>
    ///     Requires a delivery before sending a broker rejection.
    /// </summary>
    [Test]
    public void Execute_WhenMessageIsMissing_ReportsInvalidArgument()
    {
        var action = new RejectAction();

        AssertError(ErrorCodes.InvalidArgument, action);
    }

    /// <summary>
    ///     Preserves whether the flow opted into redelivery.
    /// </summary>
    /// <param name="allowRequeue">Whether the broker should requeue.</param>
    [TestCase(false)]
    [TestCase(true)]
    public void Execute_WhenMessageIsRejected_UsesRequeueChoice(bool allowRequeue)
    {
        var message = CreateMessage();
        var action = new RejectAction
        {
            AllowRequeue = allowRequeue,
            Message = message
        };

        RunAction(action);

        Assert.That(Client.RejectedTag, Is.EqualTo(7));
        Assert.That(Client.IsRequeued, Is.EqualTo(allowRequeue));
        Assert.That(message.IsSettled, Is.True);
    }
}
