// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Player;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Player;

[TestFixture]
public class SkillLevelTests
{
    [Test]
    public void Constructor_KeepsFields()
    {
        var skill = new SkillLevel(80, 42);

        Assert.Multiple(() =>
        {
            Assert.That(skill.Level, Is.EqualTo(80));
            Assert.That(skill.Percent, Is.EqualTo(42));
        });
    }

    [Test]
    public void Read_ConsumesLevelThenPercent()
    {
        var reader = new PacketReader(new byte[] { 55, 90 });

        var skill = SkillLevel.Read(reader);

        Assert.Multiple(() =>
        {
            Assert.That(skill.Level, Is.EqualTo(55));
            Assert.That(skill.Percent, Is.EqualTo(90));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => SkillLevel.Read(null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var reader = new PacketReader(new byte[] { 1 });
        reader.ReadByte();

        Assert.Throws<ProtocolException>(() => SkillLevel.Read(reader));
    }
}
