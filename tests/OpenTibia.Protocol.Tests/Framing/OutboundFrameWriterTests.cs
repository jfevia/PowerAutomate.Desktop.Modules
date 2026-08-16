// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Framing;

[TestFixture]
public class OutboundFrameWriterTests
{
    [Test]
    public void Compose_WithOneMessage_WritesJustThatMessage()
    {
        var payload = OutboundFrameWriter.Compose(
            new IClientMessage[] { new PayloadlessMessage(ClientOpcode.PingBack) });

        Assert.That(payload, Is.EqualTo(new[] { (byte)ClientOpcode.PingBack }));
    }

    [Test]
    public void Compose_WithSeveralMessages_ConcatenatesThemInOrder()
    {
        var payload = OutboundFrameWriter.Compose(new IClientMessage[]
        {
            new PayloadlessMessage(ClientOpcode.PingBack),
            new PayloadlessMessage(ClientOpcode.RequestChannels),
            new PayloadlessMessage(ClientOpcode.LeaveGame)
        });

        Assert.That(payload, Is.EqualTo(new[]
        {
            (byte)ClientOpcode.PingBack,
            (byte)ClientOpcode.RequestChannels,
            (byte)ClientOpcode.LeaveGame
        }));
    }

    [Test]
    public void Compose_WithNullList_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => OutboundFrameWriter.Compose(null!));
    }

    [Test]
    public void Compose_WithEmptyList_Throws()
    {
        Assert.Throws<ArgumentException>(() => OutboundFrameWriter.Compose(new List<IClientMessage>()));
    }

    [Test]
    public void Compose_WithNullEntry_Throws()
    {
        var messages = new IClientMessage[] { new PayloadlessMessage(ClientOpcode.PingBack), null! };

        var exception = Assert.Throws<ArgumentException>(() => OutboundFrameWriter.Compose(messages))!;

        Assert.That(exception.Message, Does.Contain("index 1"));
    }
}
