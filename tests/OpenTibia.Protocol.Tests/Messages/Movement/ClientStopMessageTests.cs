// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Movement;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Movement;

[TestFixture]
public class ClientStopMessageTests
{
    [Test]
    public void Opcode_IsStop()
    {
        Assert.That(new ClientStopMessage().Opcode, Is.EqualTo((byte)ClientOpcode.Stop));
    }

    [Test]
    public void Write_EmitsOnlyTheOpcode()
    {
        var writer = new PacketWriter();

        new ClientStopMessage().Write(writer);

        Assert.That(writer.ToArray(), Is.EqualTo(new[] { (byte)ClientOpcode.Stop }));
    }
}
