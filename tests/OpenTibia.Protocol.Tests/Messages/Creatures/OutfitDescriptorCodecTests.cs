// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Creatures;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Creatures;

[TestFixture]
public class OutfitDescriptorCodecTests
{
    [Test]
    public void Read_WithNonzeroLookType_DecodesColorsInsteadOfLookTypeEx()
    {
        var writer = new PacketWriter();
        writer.WriteUInt16(128);
        writer.WriteByte(10);
        writer.WriteByte(20);
        writer.WriteByte(30);
        writer.WriteByte(40);
        writer.WriteByte(3);
        var reader = new PacketReader(writer.ToArray());

        var outfit = OutfitDescriptorCodec.Read(reader);

        Assert.Multiple(() =>
        {
            Assert.That(outfit.LookType, Is.EqualTo(128));
            Assert.That(outfit.Head, Is.EqualTo((byte)10));
            Assert.That(outfit.Body, Is.EqualTo((byte)20));
            Assert.That(outfit.Legs, Is.EqualTo((byte)30));
            Assert.That(outfit.Feet, Is.EqualTo((byte)40));
            Assert.That(outfit.Addons, Is.EqualTo((byte)3));
            Assert.That(outfit.LookTypeEx, Is.Null);
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithZeroLookType_DecodesLookTypeExInsteadOfColors()
    {
        var writer = new PacketWriter();
        writer.WriteUInt16(0);
        writer.WriteUInt16(3031);
        var reader = new PacketReader(writer.ToArray());

        var outfit = OutfitDescriptorCodec.Read(reader);

        Assert.Multiple(() =>
        {
            Assert.That(outfit.LookType, Is.Zero);
            Assert.That(outfit.LookTypeEx, Is.EqualTo((ushort)3031));
            Assert.That(outfit.Head, Is.Null);
            Assert.That(outfit.Body, Is.Null);
            Assert.That(outfit.Legs, Is.Null);
            Assert.That(outfit.Feet, Is.Null);
            Assert.That(outfit.Addons, Is.Null);
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => OutfitDescriptorCodec.Read(null!));
    }

    [Test]
    public void Read_WithTruncatedNonzeroLookTypePayload_ThrowsProtocolException()
    {
        var writer = new PacketWriter();
        writer.WriteUInt16(128);
        writer.WriteByte(10);
        var reader = new PacketReader(writer.ToArray(), 0, writer.Length - 1);

        Assert.Throws<ProtocolException>(() => OutfitDescriptorCodec.Read(reader));
    }

    [Test]
    public void Read_WithTruncatedZeroLookTypePayload_ThrowsProtocolException()
    {
        var writer = new PacketWriter();
        writer.WriteUInt16(0);
        var reader = new PacketReader(writer.ToArray());

        Assert.Throws<ProtocolException>(() => OutfitDescriptorCodec.Read(reader));
    }
}
