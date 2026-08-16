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
public class GameServerRuleViolationRemoveMessageTests
{
    [Test]
    public void Read_ParsesReporterName()
    {
        var writer = new PacketWriter();
        writer.WriteString("Reporter");
        var reader = new PacketReader(writer.ToArray());

        var message = (GameServerRuleViolationRemoveMessage)GameServerRuleViolationRemoveMessage.Read(GameServerOpcode.RuleViolationRemove, reader);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.RuleViolationRemove));
            Assert.That(message.ReporterName, Is.EqualTo("Reporter"));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Constructor_WithNullReporterName_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new GameServerRuleViolationRemoveMessage(null!));
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerRuleViolationRemoveMessage.Read(GameServerOpcode.RuleViolationRemove, null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var reader = new PacketReader(new byte[] { 1, 0 });

        Assert.Throws<ProtocolException>(() => GameServerRuleViolationRemoveMessage.Read(GameServerOpcode.RuleViolationRemove, reader));
    }
}
