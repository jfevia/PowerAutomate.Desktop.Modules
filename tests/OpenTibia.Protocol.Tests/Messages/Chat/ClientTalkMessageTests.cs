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
public class ClientTalkMessageTests
{
    [Test]
    public void Opcode_IsTalk()
    {
        var message = new ClientTalkMessage(SpeakType.Say, null, null, "Hi");

        Assert.That(message.Opcode, Is.EqualTo((byte)ClientOpcode.Talk));
    }

    [Test]
    public void Constructor_KeepsFields()
    {
        var message = new ClientTalkMessage(SpeakType.Private, "Al", null, "Hi");

        Assert.Multiple(() =>
        {
            Assert.That(message.Type, Is.EqualTo(SpeakType.Private));
            Assert.That(message.ReceiverName, Is.EqualTo("Al"));
            Assert.That(message.ChannelId, Is.Null);
            Assert.That(message.Text, Is.EqualTo("Hi"));
        });
    }

    [TestCase(SpeakType.Private, (byte)0x06)]
    [TestCase(SpeakType.PrivateRed, (byte)0x0E)]
    [TestCase(SpeakType.RuleViolationAnswer, (byte)0x0A)]
    public void Write_WithReceiverNameSpeakType_EmitsReceiverNameNotChannel(SpeakType type, byte expected)
    {
        var writer = new PacketWriter();

        new ClientTalkMessage(type, "Al", null, "Hi").Write(writer);

        Assert.That(writer.ToArray(), Is.EqualTo(new byte[]
        {
            (byte)ClientOpcode.Talk, expected,
            0x02, 0x00, 0x41, 0x6C,
            0x02, 0x00, 0x48, 0x69
        }));
    }

    [TestCase(SpeakType.ChannelYellow, (byte)0x07)]
    [TestCase(SpeakType.ChannelRed, (byte)0x0D)]
    [TestCase(SpeakType.ChannelRedAnonymous, (byte)0x11)]
    public void Write_WithChannelIdSpeakType_EmitsChannelIdNotReceiver(SpeakType type, byte expected)
    {
        var writer = new PacketWriter();

        new ClientTalkMessage(type, null, 7, "Hi").Write(writer);

        Assert.That(writer.ToArray(), Is.EqualTo(new byte[]
        {
            (byte)ClientOpcode.Talk, expected,
            0x07, 0x00,
            0x02, 0x00, 0x48, 0x69
        }));
    }

    [TestCase(SpeakType.Say)]
    [TestCase(SpeakType.ChannelWhite)]
    [TestCase(SpeakType.ChannelOrange)]
    public void Write_WithNeitherSpeakType_EmitsOnlyTypeAndText(SpeakType type)
    {
        var writer = new PacketWriter();

        new ClientTalkMessage(type, null, null, "Hi").Write(writer);

        Assert.That(writer.ToArray(), Is.EqualTo(new byte[]
        {
            (byte)ClientOpcode.Talk, (byte)type,
            0x02, 0x00, 0x48, 0x69
        }));
    }

    [Test]
    public void Constructor_WithNullText_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ClientTalkMessage(SpeakType.Say, null, null, null!));
    }

    [Test]
    public void Constructor_WithReceiverNameSpeakTypeAndNullReceiver_ThrowsProtocolException()
    {
        Assert.Throws<ProtocolException>(() => new ClientTalkMessage(SpeakType.Private, null, null, "Hi"));
    }

    [Test]
    public void Constructor_WithChannelIdSpeakTypeAndNullChannel_ThrowsProtocolException()
    {
        Assert.Throws<ProtocolException>(() => new ClientTalkMessage(SpeakType.ChannelYellow, null, null, "Hi"));
    }
}
