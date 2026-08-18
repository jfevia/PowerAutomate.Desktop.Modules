// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Creatures;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Creatures;

[TestFixture]
public class SelectableOutfitTests
{
    [Test]
    public void Read_ParsesLookTypeNameThenAddons()
    {
        var writer = new PacketWriter();
        writer.WriteUInt16(128);
        writer.WriteString("Citizen");
        writer.WriteByte(3);
        var reader = new PacketReader(writer.ToArray());

        var outfit = SelectableOutfit.Read(reader);

        Assert.Multiple(() =>
        {
            Assert.That(outfit.LookType, Is.EqualTo(128));
            Assert.That(outfit.Name, Is.EqualTo("Citizen"));
            Assert.That(outfit.Addons, Is.EqualTo(3));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Constructor_WithNullName_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new SelectableOutfit(128, null!, 0));
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => SelectableOutfit.Read(null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var reader = new PacketReader(new byte[] { 128, 0 });

        Assert.Throws<ProtocolException>(() => SelectableOutfit.Read(reader));
    }
}
