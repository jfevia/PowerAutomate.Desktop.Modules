// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages;

[TestFixture]
public class ClientMessagesTests
{
    [Test]
    public void Logout_UsesLeaveGameOpcode()
    {
        var writer = new PacketWriter();
        var message = ClientMessages.Logout();

        message.Write(writer);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)ClientOpcode.LeaveGame));
            Assert.That(writer.ToArray(), Is.EqualTo(new[] { (byte)ClientOpcode.LeaveGame }));
        });
    }

    [Test]
    public void PingBack_UsesPingBackOpcode()
    {
        var writer = new PacketWriter();
        var message = ClientMessages.PingBack();

        message.Write(writer);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)ClientOpcode.PingBack));
            Assert.That(writer.ToArray(), Is.EqualTo(new[] { (byte)ClientOpcode.PingBack }));
        });
    }

    [Test]
    public void RequestChannels_UsesRequestChannelsOpcode()
    {
        var writer = new PacketWriter();
        var message = ClientMessages.RequestChannels();

        message.Write(writer);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)ClientOpcode.RequestChannels));
            Assert.That(writer.ToArray(), Is.EqualTo(new[] { (byte)ClientOpcode.RequestChannels }));
        });
    }

    [Test]
    public void RequestOutfit_UsesRequestOutfitOpcode()
    {
        var writer = new PacketWriter();
        var message = ClientMessages.RequestOutfit();

        message.Write(writer);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)ClientOpcode.RequestOutfit));
            Assert.That(writer.ToArray(), Is.EqualTo(new[] { (byte)ClientOpcode.RequestOutfit }));
        });
    }

    [Test]
    public void RequestQuestLog_UsesRequestQuestLogOpcode()
    {
        var writer = new PacketWriter();
        var message = ClientMessages.RequestQuestLog();

        message.Write(writer);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)ClientOpcode.RequestQuestLog));
            Assert.That(writer.ToArray(), Is.EqualTo(new[] { (byte)ClientOpcode.RequestQuestLog }));
        });
    }

    [Test]
    public void CancelAttackAndFollow_UsesCancelAttackAndFollowOpcode()
    {
        var writer = new PacketWriter();
        var message = ClientMessages.CancelAttackAndFollow();

        message.Write(writer);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)ClientOpcode.CancelAttackAndFollow));
            Assert.That(writer.ToArray(), Is.EqualTo(new[] { (byte)ClientOpcode.CancelAttackAndFollow }));
        });
    }
}
