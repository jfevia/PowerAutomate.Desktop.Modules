// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Moderation;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Moderation;

[TestFixture]
public class GameServerRuleViolationChannelMessageTests
{
    [Test]
    public void Read_ParsesChannelId()
    {
        var reader = new PacketReader(new byte[] { 5, 0 });

        var message = (GameServerRuleViolationChannelMessage)GameServerRuleViolationChannelMessage.Read(GameServerOpcode.RuleViolationChannel, reader);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.RuleViolationChannel));
            Assert.That(message.ChannelId, Is.EqualTo(5));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerRuleViolationChannelMessage.Read(GameServerOpcode.RuleViolationChannel, null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var reader = new PacketReader(new byte[] { 5 });

        Assert.Throws<ProtocolException>(() => GameServerRuleViolationChannelMessage.Read(GameServerOpcode.RuleViolationChannel, reader));
    }
}
