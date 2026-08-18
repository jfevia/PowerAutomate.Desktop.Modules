// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Login;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Login;

[TestFixture]
public class CharacterListEntryTests
{
    [Test]
    public void Write_EmitsFieldsInOrder()
    {
        var writer = new PacketWriter();
        var entry = new CharacterListEntry("Hero", "Antica", 0x0100007Fu, 7172);

        entry.Write(writer);

        var expectedWriter = new PacketWriter();
        expectedWriter.WriteString("Hero");
        expectedWriter.WriteString("Antica");
        expectedWriter.WriteUInt32(0x0100007Fu);
        expectedWriter.WriteUInt16(7172);

        Assert.That(writer.ToArray(), Is.EqualTo(expectedWriter.ToArray()));
    }

    [Test]
    public void Read_ParsesFieldsInOrder()
    {
        var payloadWriter = new PacketWriter();
        payloadWriter.WriteString("Hero");
        payloadWriter.WriteString("Antica");
        payloadWriter.WriteUInt32(0x0100007Fu);
        payloadWriter.WriteUInt16(7172);
        var reader = new PacketReader(payloadWriter.ToArray());

        var entry = CharacterListEntry.Read(reader);

        Assert.Multiple(() =>
        {
            Assert.That(entry.Name, Is.EqualTo("Hero"));
            Assert.That(entry.World, Is.EqualTo("Antica"));
            Assert.That(entry.Address, Is.EqualTo(0x0100007Fu));
            Assert.That(entry.Port, Is.EqualTo((ushort)7172));
        });
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var payloadWriter = new PacketWriter();
        payloadWriter.WriteString("Hero");
        var reader = new PacketReader(payloadWriter.ToArray());

        Assert.Throws<ProtocolException>(() => CharacterListEntry.Read(reader));
    }

    [Test]
    public void HostName_RendersAddressAsLittleEndianDottedQuad()
    {
        var loopback = new CharacterListEntry("n", "w", 0x0100007Fu, 1);
        var routable = new CharacterListEntry("n", "w", 192u | (168u << 8) | (1u << 16) | (10u << 24), 1);

        Assert.Multiple(() =>
        {
            Assert.That(loopback.HostName, Is.EqualTo("127.0.0.1"));
            Assert.That(routable.HostName, Is.EqualTo("192.168.1.10"));
        });
    }

    [Test]
    public void Constructor_WithNullName_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new CharacterListEntry(null!, "w", 0, 0));
    }

    [Test]
    public void Constructor_WithNullWorld_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new CharacterListEntry("n", null!, 0, 0));
    }

    [Test]
    public void Write_WithNullWriter_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new CharacterListEntry("n", "w", 0, 0).Write(null!));
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => CharacterListEntry.Read(null!));
    }
}
