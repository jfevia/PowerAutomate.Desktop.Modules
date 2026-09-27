using System;
using System.IO;
using NUnit.Framework;
using RabbitMQ.Client.Exceptions;

namespace PowerAutomate.Desktop.Modules.RabbitMQ.Actions.Tests;

/// <summary>
///     Checks broker URI validation and safe connection display.
/// </summary>
[TestFixture]
public sealed class ConnectActionTests : RabbitMqActionFixture
{
    /// <summary>
    ///     Does not invent a connection before the flow executes.
    /// </summary>
    [Test]
    public void ConnectAction_WhenCreated_HasNoConnection()
    {
        var action = new ConnectAction();

        Assert.That(action.Connection, Is.Null);
    }

    /// <summary>
    ///     Requires an injected factory at the test boundary.
    /// </summary>
    [Test]
    public void ConnectAction_WhenFactoryIsNull_Throws()
    {
        var error = Assert.Throws<ArgumentNullException>(() =>
        {
            var action = new StubConnectAction(null);
            Assert.Fail($"Unexpected action: {action}");
        });

        Assert.That(error?.ParamName, Is.EqualTo("clientFactory"));
    }

    /// <summary>
    ///     Rejects non-AMQP and malformed addresses.
    /// </summary>
    /// <param name="address">The invalid broker address.</param>
    [TestCase("not a uri")]
    [TestCase("https://rabbit.example")]
    [TestCase(null)]
    public void Execute_WhenAddressIsInvalid_ReportsInvalidArgument(string? address)
    {
        var factory = new RabbitMqClientFactoryStub(Client);
        var action = new StubConnectAction(factory)
        {
            Address = address
        };

        AssertError(ErrorCodes.InvalidArgument, action);
    }

    /// <summary>
    ///     Reports connection refusal without returning a fake session.
    /// </summary>
    [Test]
    public void Execute_WhenBrokerIsUnavailable_ReportsBrokerError()
    {
        var factory = new RabbitMqClientFactoryStub(Client);
        var connectionFailure = new IOException("offline");
        var failure = new BrokerUnreachableException(connectionFailure);
        factory.SetFailure(failure);
        var action = new StubConnectAction(factory)
        {
            Address = "amqp://rabbit.example"
        };

        AssertError(ErrorCodes.Broker, action);
    }

    /// <summary>
    ///     Reports broker connection timeouts distinctly from invalid input.
    /// </summary>
    [Test]
    public void Execute_WhenBrokerTimesOut_ReportsBrokerError()
    {
        var factory = new RabbitMqClientFactoryStub(Client);
        var failure = new TimeoutException("timed out");
        factory.SetFailure(failure);
        var action = new StubConnectAction(factory)
        {
            Address = "amqp://rabbit.example"
        };

        AssertError(ErrorCodes.Broker, action);
    }

    /// <summary>
    ///     Keeps URI credentials out of the resulting connection variable.
    /// </summary>
    /// <param name="address">The broker URI to connect to.</param>
    /// <param name="endpoint">The expected credential-free endpoint.</param>
    [TestCase("amqp://example:example@rabbit.example:5672/vhost", "rabbit.example:5672")]
    [TestCase("amqps://example:example@rabbit.example:5671/vhost", "rabbit.example:5671")]
    public void Execute_WhenUriHasCredentials_HidesThem(string address, string endpoint)
    {
        var factory = new RabbitMqClientFactoryStub(Client);
        var action = new StubConnectAction(factory)
        {
            Address = address
        };

        RunAction(action);

        Assert.That(factory.GetAddress()?.AbsoluteUri, Is.EqualTo(address));
        Assert.That(action.Connection?.Endpoint, Is.EqualTo(endpoint));
        Assert.That(action.Connection?.ToString(), Does.Not.Contain("example:example"));
    }
}
