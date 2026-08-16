// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Creatures;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Movement;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Creatures;

[TestFixture]
public class CreatureDescriptorCodecTests
{
    private static byte[] BuildKnownPayload()
    {
        var writer = new PacketWriter();
        writer.WriteUInt16(0x0062);
        writer.WriteUInt32(1001u);
        writer.WriteByte(100);
        writer.WriteByte((byte)Direction.South);
        writer.WriteUInt16(128);
        writer.WriteByte(10);
        writer.WriteByte(20);
        writer.WriteByte(30);
        writer.WriteByte(40);
        writer.WriteByte(0);
        writer.WriteByte(5);
        writer.WriteByte(200);
        writer.WriteUInt16(220);
        writer.WriteByte(0);
        writer.WriteByte(0);
        writer.WriteByte(0);
        return writer.ToArray();
    }

    private static byte[] BuildUnknownPayload()
    {
        var writer = new PacketWriter();
        writer.WriteUInt16(0x0061);
        writer.WriteUInt32(999u);
        writer.WriteUInt32(1001u);
        writer.WriteString("BotTester");
        writer.WriteByte(100);
        writer.WriteByte((byte)Direction.North);
        writer.WriteUInt16(128);
        writer.WriteByte(10);
        writer.WriteByte(20);
        writer.WriteByte(30);
        writer.WriteByte(40);
        writer.WriteByte(3);
        writer.WriteByte(0);
        writer.WriteByte(200);
        writer.WriteUInt16(220);
        writer.WriteByte(1);
        writer.WriteByte(2);
        writer.WriteByte(0);
        writer.WriteByte(1);
        return writer.ToArray();
    }

    [Test]
    public void Read_WithKnownMarker_DecodesOnlyCreatureId()
    {
        var reader = new PacketReader(BuildKnownPayload());

        var descriptor = CreatureDescriptorCodec.Read(reader);

        Assert.Multiple(() =>
        {
            Assert.That(descriptor.IsKnown, Is.True);
            Assert.That(descriptor.CreatureId, Is.EqualTo(1001u));
            Assert.That(descriptor.Name, Is.Null);
            Assert.That(descriptor.RemovedCreatureId, Is.Null);
            Assert.That(descriptor.Emblem, Is.Null);
            Assert.That(descriptor.HealthPercent, Is.EqualTo(100));
            Assert.That(descriptor.Direction, Is.EqualTo(Direction.South));
            Assert.That(descriptor.Outfit.LookType, Is.EqualTo(128));
            Assert.That(descriptor.Light.Level, Is.EqualTo(5));
            Assert.That(descriptor.Light.Color, Is.EqualTo(200));
            Assert.That(descriptor.Speed, Is.EqualTo(220));
            Assert.That(descriptor.Skull, Is.Zero);
            Assert.That(descriptor.Shield, Is.Zero);
            Assert.That(descriptor.IsUnpassable, Is.False);
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithUnknownMarker_DecodesFullDescriptor()
    {
        var reader = new PacketReader(BuildUnknownPayload());

        var descriptor = CreatureDescriptorCodec.Read(reader);

        Assert.Multiple(() =>
        {
            Assert.That(descriptor.IsKnown, Is.False);
            Assert.That(descriptor.RemovedCreatureId, Is.EqualTo(999u));
            Assert.That(descriptor.CreatureId, Is.EqualTo(1001u));
            Assert.That(descriptor.Name, Is.EqualTo("BotTester"));
            Assert.That(descriptor.Direction, Is.EqualTo(Direction.North));
            Assert.That(descriptor.Outfit.LookType, Is.EqualTo(128));
            Assert.That(descriptor.Speed, Is.EqualTo(220));
            Assert.That(descriptor.Skull, Is.EqualTo((byte)1));
            Assert.That(descriptor.Shield, Is.EqualTo((byte)2));
            Assert.That(descriptor.Emblem, Is.EqualTo((byte)0));
            Assert.That(descriptor.IsUnpassable, Is.True);
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => CreatureDescriptorCodec.Read(null!));
    }

    [Test]
    public void Read_WithTruncatedKnownPayload_ThrowsProtocolException()
    {
        var payload = BuildKnownPayload();
        var reader = new PacketReader(payload, 0, payload.Length - 1);

        Assert.Throws<ProtocolException>(() => CreatureDescriptorCodec.Read(reader));
    }

    [Test]
    public void Read_WithTruncatedUnknownPayload_ThrowsProtocolException()
    {
        var payload = BuildUnknownPayload();
        var reader = new PacketReader(payload, 0, payload.Length - 1);

        Assert.Throws<ProtocolException>(() => CreatureDescriptorCodec.Read(reader));
    }
}
