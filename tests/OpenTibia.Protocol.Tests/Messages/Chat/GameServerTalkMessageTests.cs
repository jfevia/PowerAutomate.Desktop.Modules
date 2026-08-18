// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Geometry;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Chat;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Chat;

[TestFixture]
public class GameServerTalkMessageTests
{
    private static byte[] BuildHeader(SpeakType type)
    {
        var writer = new PacketWriter();
        writer.WriteUInt32(42u);
        writer.WriteString("Al");
        writer.WriteUInt16(80);
        writer.WriteByte((byte)type);
        return writer.ToArray();
    }

    private static byte[] BuildPositionPayload(SpeakType type)
    {
        var writer = new PacketWriter();
        writer.WriteBytes(BuildHeader(type));
        PositionCodec.Write(writer, new Position(100, 200, 7));
        writer.WriteString("Hello");
        return writer.ToArray();
    }

    private static byte[] BuildChannelPayload(SpeakType type)
    {
        var writer = new PacketWriter();
        writer.WriteBytes(BuildHeader(type));
        writer.WriteUInt16(5);
        writer.WriteString("Hello channel");
        return writer.ToArray();
    }

    private static byte[] BuildRuleViolationChannelPayload()
    {
        var writer = new PacketWriter();
        writer.WriteBytes(BuildHeader(SpeakType.RuleViolationChannel));
        writer.WriteUInt32(9001u);
        writer.WriteString("Report text");
        return writer.ToArray();
    }

    private static byte[] BuildNothingPayload(SpeakType type)
    {
        var writer = new PacketWriter();
        writer.WriteBytes(BuildHeader(type));
        writer.WriteString("Hello");
        return writer.ToArray();
    }

    [TestCase(SpeakType.Say)]
    [TestCase(SpeakType.Whisper)]
    [TestCase(SpeakType.Yell)]
    [TestCase(SpeakType.MonsterSay)]
    [TestCase(SpeakType.MonsterYell)]
    [TestCase(SpeakType.PrivateNameToPlayer)]
    public void Read_WithPositionSpeakType_ReadsSpeakerPositionOnly(SpeakType type)
    {
        var reader = new PacketReader(BuildPositionPayload(type));

        var message = (GameServerTalkMessage)GameServerTalkMessage.Read(GameServerOpcode.Talk, reader);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.Talk));
            Assert.That(message.StatementId, Is.EqualTo(42u));
            Assert.That(message.SpeakerName, Is.EqualTo("Al"));
            Assert.That(message.SpeakerLevel, Is.EqualTo(80));
            Assert.That(message.Type, Is.EqualTo(type));
            Assert.That(message.SpeakerPosition, Is.EqualTo(new Position(100, 200, 7)));
            Assert.That(message.ChannelId, Is.Null);
            Assert.That(message.RuleViolationChannelTimeDelta, Is.Null);
            Assert.That(message.Text, Is.EqualTo("Hello"));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [TestCase(SpeakType.ChannelYellow)]
    [TestCase(SpeakType.ChannelRed)]
    [TestCase(SpeakType.ChannelRedAnonymous)]
    [TestCase(SpeakType.ChannelOrange)]
    [TestCase(SpeakType.ChannelWhite)]
    public void Read_WithChannelIdSpeakType_ReadsChannelIdOnly(SpeakType type)
    {
        var reader = new PacketReader(BuildChannelPayload(type));

        var message = (GameServerTalkMessage)GameServerTalkMessage.Read(GameServerOpcode.Talk, reader);

        Assert.Multiple(() =>
        {
            Assert.That(message.Type, Is.EqualTo(type));
            Assert.That(message.SpeakerPosition, Is.Null);
            Assert.That(message.ChannelId, Is.EqualTo((ushort)5));
            Assert.That(message.RuleViolationChannelTimeDelta, Is.Null);
            Assert.That(message.Text, Is.EqualTo("Hello channel"));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithRuleViolationChannelSpeakType_ReadsTimeDeltaOnly()
    {
        var reader = new PacketReader(BuildRuleViolationChannelPayload());

        var message = (GameServerTalkMessage)GameServerTalkMessage.Read(GameServerOpcode.Talk, reader);

        Assert.Multiple(() =>
        {
            Assert.That(message.Type, Is.EqualTo(SpeakType.RuleViolationChannel));
            Assert.That(message.SpeakerPosition, Is.Null);
            Assert.That(message.ChannelId, Is.Null);
            Assert.That(message.RuleViolationChannelTimeDelta, Is.EqualTo(9001u));
            Assert.That(message.Text, Is.EqualTo("Report text"));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [TestCase(SpeakType.None)]
    [TestCase(SpeakType.Private)]
    [TestCase(SpeakType.Broadcast)]
    [TestCase(SpeakType.RuleViolationAnswer)]
    public void Read_WithNeitherPositionChannelNorRvrSpeakType_ReadsOnlyText(SpeakType type)
    {
        var reader = new PacketReader(BuildNothingPayload(type));

        var message = (GameServerTalkMessage)GameServerTalkMessage.Read(GameServerOpcode.Talk, reader);

        Assert.Multiple(() =>
        {
            Assert.That(message.Type, Is.EqualTo(type));
            Assert.That(message.SpeakerPosition, Is.Null);
            Assert.That(message.ChannelId, Is.Null);
            Assert.That(message.RuleViolationChannelTimeDelta, Is.Null);
            Assert.That(message.Text, Is.EqualTo("Hello"));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Constructor_WithNullSpeakerName_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new GameServerTalkMessage(1, null!, 0, SpeakType.Say, null, null, null, "Hi"));
    }

    [Test]
    public void Constructor_WithNullText_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new GameServerTalkMessage(1, "Al", 0, SpeakType.Say, null, null, null, null!));
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerTalkMessage.Read(GameServerOpcode.Talk, null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var payload = BuildNothingPayload(SpeakType.Say);
        var reader = new PacketReader(payload, 0, payload.Length - 1);

        Assert.Throws<ProtocolException>(() => GameServerTalkMessage.Read(GameServerOpcode.Talk, reader));
    }
}
