// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Services;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Services;

[TestFixture]
public class RawClientMessageTests
{
    [Test]
    public void Constructor_WithNullPayload_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new RawClientMessage(0x01, null!));
    }

    [Test]
    public void Opcode_ReturnsTheConstructorValue()
    {
        Assert.That(new RawClientMessage(0x42, Array.Empty<byte>()).Opcode, Is.EqualTo(0x42));
    }

    [Test]
    public void Write_EmitsOpcodeFollowedByThePayload()
    {
        var message = new RawClientMessage(0x42, new byte[] { 0xDE, 0xAD });
        var writer = new PacketWriter();

        message.Write(writer);

        Assert.That(writer.ToArray(), Is.EqualTo(new byte[] { 0x42, 0xDE, 0xAD }));
    }
}
