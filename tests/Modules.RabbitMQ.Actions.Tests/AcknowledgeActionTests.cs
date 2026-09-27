using System;
using System.Threading.Tasks;
using NUnit.Framework;
using RabbitMQ.Client.Exceptions;

namespace PowerAutomate.Desktop.Modules.RabbitMQ.Actions.Tests;

/// <summary>
///     Checks manual acknowledgements and duplicate-settlement prevention.
/// </summary>
[TestFixture]
public sealed class AcknowledgeActionTests : RabbitMqActionFixture
{
    /// <summary>
    ///     Leaves a message available for retry if the broker rejects acknowledgement.
    /// </summary>
    [Test]
    public void Execute_WhenBrokerFails_LeavesMessageUnsettled()
    {
        var message = CreateMessage();
        Client.Failure = new PublishException(1, false);
        var action = new AcknowledgeAction
        {
            Message = message
        };

        AssertError(ErrorCodes.Broker, action);

        Assert.That(message.IsSettled, Is.False);
    }

    /// <summary>
    ///     Serializes two simultaneous settlement attempts on one message.
    /// </summary>
    [Test]
    public void Execute_WhenConcurrent_PreventsDuplicateAcknowledgements()
    {
        var message = CreateMessage();
        var started = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        Client.AcknowledgeStarted = started;
        Client.HoldAcknowledge = release;
        var action = new AcknowledgeAction
        {
            Message = message
        };
        var first = Task.Run(() => RunAction(action));

        try
        {
            Assert.That(started.Task.Wait(TimeSpan.FromSeconds(5)), Is.True);
            var duplicate = new AcknowledgeAction
            {
                Message = message
            };
            var second = Task.Run(() => AssertError(ErrorCodes.MessageSettled, duplicate));
            release.SetResult(true);
            var tasks = new Task[]
            {
                first,
                second
            };
            Assert.That(Task.WaitAll(tasks, TimeSpan.FromSeconds(5)), Is.True);
        }
        finally
        {
            release.TrySetResult(true);
        }

        Assert.That(Client.AcknowledgedCount, Is.EqualTo(1));
    }

    /// <summary>
    ///     Confirms one delivery and prevents either settlement path repeating.
    /// </summary>
    [Test]
    public void Execute_WhenDeliveryIsAcknowledged_PreventsDuplicateSettlement()
    {
        var message = CreateMessage();
        var action = new AcknowledgeAction
        {
            Message = message
        };

        RunAction(action);

        Assert.That(Client.AcknowledgedTag, Is.EqualTo(7));
        Assert.That(message.IsSettled, Is.True);
        AssertError(ErrorCodes.MessageSettled, action);
        var rejection = new RejectAction
        {
            Message = message
        };
        AssertError(ErrorCodes.MessageSettled, rejection);
    }

    /// <summary>
    ///     Rejects acknowledgement without a received message.
    /// </summary>
    [Test]
    public void Execute_WhenMessageIsMissing_ReportsInvalidArgument()
    {
        var action = new AcknowledgeAction();

        AssertError(ErrorCodes.InvalidArgument, action);
    }
}
