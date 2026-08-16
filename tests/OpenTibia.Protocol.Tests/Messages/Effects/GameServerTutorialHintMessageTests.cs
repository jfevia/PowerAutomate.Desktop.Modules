// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Effects;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Effects;

[TestFixture]
public class GameServerTutorialHintMessageTests
{
    [Test]
    public void Read_ParsesTheTutorialId()
    {
        var reader = new PacketReader(new byte[] { 3 });

        var message = (GameServerTutorialHintMessage)GameServerTutorialHintMessage.Read(GameServerOpcode.TutorialHint, reader);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.TutorialHint));
            Assert.That(message.TutorialId, Is.EqualTo(3));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerTutorialHintMessage.Read(GameServerOpcode.TutorialHint, null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var reader = new PacketReader(Array.Empty<byte>());

        Assert.Throws<ProtocolException>(() => GameServerTutorialHintMessage.Read(GameServerOpcode.TutorialHint, reader));
    }
}
