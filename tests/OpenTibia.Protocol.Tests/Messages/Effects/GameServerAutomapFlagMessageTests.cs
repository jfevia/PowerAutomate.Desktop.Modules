// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Geometry;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Effects;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Effects;

[TestFixture]
public class GameServerAutomapFlagMessageTests
{
    private static byte[] BuildPayload()
    {
        var writer = new PacketWriter();
        PositionCodec.Write(writer, new Position(50, 60, 7));
        writer.WriteByte(9);
        writer.WriteString("Quest here");
        return writer.ToArray();
    }

    [Test]
    public void Read_ParsesPositionMarkTypeAndDescription()
    {
        var reader = new PacketReader(BuildPayload());

        var message = (GameServerAutomapFlagMessage)GameServerAutomapFlagMessage.Read(GameServerOpcode.AutomapFlag, reader);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.AutomapFlag));
            Assert.That(message.Position, Is.EqualTo(new Position(50, 60, 7)));
            Assert.That(message.MarkType, Is.EqualTo(9));
            Assert.That(message.Description, Is.EqualTo("Quest here"));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerAutomapFlagMessage.Read(GameServerOpcode.AutomapFlag, null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var payload = BuildPayload();
        var reader = new PacketReader(payload, 0, payload.Length - 1);

        Assert.Throws<ProtocolException>(() => GameServerAutomapFlagMessage.Read(GameServerOpcode.AutomapFlag, reader));
    }
}
