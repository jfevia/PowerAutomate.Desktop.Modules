// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Chat;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Chat;

[TestFixture]
public class GameServerChannelsMessageTests
{
    [Test]
    public void Read_ParsesEveryChannelEntry()
    {
        var writer = new PacketWriter();
        writer.WriteByte(2);
        writer.WriteUInt16(1);
        writer.WriteString("Trade");
        writer.WriteUInt16(2);
        writer.WriteString("Help");
        var reader = new PacketReader(writer.ToArray());

        var message = (GameServerChannelsMessage)GameServerChannelsMessage.Read(GameServerOpcode.Channels, reader);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.Channels));
            Assert.That(message.Channels, Has.Count.EqualTo(2));
            Assert.That(message.Channels[0].ChannelId, Is.EqualTo(1));
            Assert.That(message.Channels[0].ChannelName, Is.EqualTo("Trade"));
            Assert.That(message.Channels[1].ChannelId, Is.EqualTo(2));
            Assert.That(message.Channels[1].ChannelName, Is.EqualTo("Help"));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithZeroChannels_ReturnsEmptyList()
    {
        var reader = new PacketReader(new byte[] { 0 });

        var message = (GameServerChannelsMessage)GameServerChannelsMessage.Read(GameServerOpcode.Channels, reader);

        Assert.That(message.Channels, Is.Empty);
    }

    [Test]
    public void Constructor_WithNullChannels_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new GameServerChannelsMessage(null!));
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerChannelsMessage.Read(GameServerOpcode.Channels, null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var reader = new PacketReader(new byte[] { 1, 0x01 });

        Assert.Throws<ProtocolException>(() => GameServerChannelsMessage.Read(GameServerOpcode.Channels, reader));
    }
}
