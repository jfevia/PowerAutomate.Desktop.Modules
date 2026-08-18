// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Creatures;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Creatures;

[TestFixture]
public class GameServerChooseOutfitMessageTests
{
    [Test]
    public void Read_WithSelectableOutfits_ParsesCurrentOutfitThenEveryEntry()
    {
        var writer = new PacketWriter();
        WriteOutfit(writer, 128, 10, 20, 30, 40, 3);
        writer.WriteByte(2);
        writer.WriteUInt16(128);
        writer.WriteString("Citizen");
        writer.WriteByte(0);
        writer.WriteUInt16(129);
        writer.WriteString("Hunter");
        writer.WriteByte(1);
        var reader = new PacketReader(writer.ToArray());

        var message = (GameServerChooseOutfitMessage)GameServerChooseOutfitMessage.Read(GameServerOpcode.ChooseOutfit, reader);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.ChooseOutfit));
            Assert.That(message.CurrentOutfit.LookType, Is.EqualTo(128));
            Assert.That(message.SelectableOutfits, Has.Count.EqualTo(2));
            Assert.That(message.SelectableOutfits[0].Name, Is.EqualTo("Citizen"));
            Assert.That(message.SelectableOutfits[1].Addons, Is.EqualTo(1));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithNoSelectableOutfits_DecodesEmptyList()
    {
        var writer = new PacketWriter();
        WriteOutfit(writer, 128, 10, 20, 30, 40, 3);
        writer.WriteByte(0);
        var reader = new PacketReader(writer.ToArray());

        var message = (GameServerChooseOutfitMessage)GameServerChooseOutfitMessage.Read(GameServerOpcode.ChooseOutfit, reader);

        Assert.That(message.SelectableOutfits, Is.Empty);
    }

    [Test]
    public void Constructor_WithNullCurrentOutfit_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new GameServerChooseOutfitMessage(null!, Array.Empty<SelectableOutfit>()));
    }

    [Test]
    public void Constructor_WithNullSelectableOutfits_Throws()
    {
        var outfit = OutfitDescriptorCodec.Read(new PacketReader(new byte[] { 0, 0, 0, 0 }));

        Assert.Throws<ArgumentNullException>(() => new GameServerChooseOutfitMessage(outfit, null!));
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerChooseOutfitMessage.Read(GameServerOpcode.ChooseOutfit, null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var writer = new PacketWriter();
        WriteOutfit(writer, 128, 10, 20, 30, 40, 3);
        var reader = new PacketReader(writer.ToArray());

        Assert.Throws<ProtocolException>(() => GameServerChooseOutfitMessage.Read(GameServerOpcode.ChooseOutfit, reader));
    }

    private static void WriteOutfit(PacketWriter writer, ushort lookType, byte head, byte body, byte legs, byte feet, byte addons)
    {
        writer.WriteUInt16(lookType);
        writer.WriteByte(head);
        writer.WriteByte(body);
        writer.WriteByte(legs);
        writer.WriteByte(feet);
        writer.WriteByte(addons);
    }
}
