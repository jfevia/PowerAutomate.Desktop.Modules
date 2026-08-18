// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Creatures;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Creatures;

[TestFixture]
public class LightInfoTests
{
    [Test]
    public void Constructor_KeepsFields()
    {
        var light = new LightInfo(4, 215);

        Assert.Multiple(() =>
        {
            Assert.That(light.Level, Is.EqualTo(4));
            Assert.That(light.Color, Is.EqualTo(215));
        });
    }

    [Test]
    public void Read_ConsumesLevelThenColor()
    {
        var reader = new PacketReader(new byte[] { 6, 210 });

        var light = LightInfo.Read(reader);

        Assert.Multiple(() =>
        {
            Assert.That(light.Level, Is.EqualTo(6));
            Assert.That(light.Color, Is.EqualTo(210));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => LightInfo.Read(null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var reader = new PacketReader(new byte[] { 6 });
        reader.ReadByte();

        Assert.Throws<ProtocolException>(() => LightInfo.Read(reader));
    }
}
