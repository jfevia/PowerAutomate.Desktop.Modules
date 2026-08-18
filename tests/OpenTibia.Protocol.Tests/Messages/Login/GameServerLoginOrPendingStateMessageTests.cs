// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Login;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Login;

[TestFixture]
public class GameServerLoginOrPendingStateMessageTests
{
    private static byte[] MakeViolationReasonFlags(byte fill)
    {
        var flags = new byte[GameServerLoginOrPendingStateMessage.ViolationReasonFlagCount];
        for (var index = 0; index < flags.Length; index++)
        {
            flags[index] = fill;
        }

        return flags;
    }

    [Test]
    public void Write_WithoutViolationReasonFlags_EmitsFixedFieldsOnly()
    {
        var writer = new PacketWriter();
        var message = new GameServerLoginOrPendingStateMessage(0x11223344u, 0x0032, false, null);

        message.Write(writer);

        var expectedWriter = new PacketWriter();
        expectedWriter.WriteByte((byte)GameServerOpcode.LoginOrPendingState);
        expectedWriter.WriteUInt32(0x11223344u);
        expectedWriter.WriteUInt16(0x0032);
        expectedWriter.WriteByte(0);

        Assert.That(writer.ToArray(), Is.EqualTo(expectedWriter.ToArray()));
    }

    [Test]
    public void Write_WithViolationReasonFlags_EmitsMarkerAndFlagBytes()
    {
        var writer = new PacketWriter();
        var flags = MakeViolationReasonFlags(0x07);
        var message = new GameServerLoginOrPendingStateMessage(0x11223344u, 0x0032, true, flags);

        message.Write(writer);

        var expectedWriter = new PacketWriter();
        expectedWriter.WriteByte((byte)GameServerOpcode.LoginOrPendingState);
        expectedWriter.WriteUInt32(0x11223344u);
        expectedWriter.WriteUInt16(0x0032);
        expectedWriter.WriteByte(1);
        expectedWriter.WriteByte(GameServerLoginOrPendingStateMessage.ViolationReasonMarker);
        expectedWriter.WriteBytes(flags);

        Assert.That(writer.ToArray(), Is.EqualTo(expectedWriter.ToArray()));
    }

    [Test]
    public void Read_WithNoTrailingBytes_LeavesViolationReasonFlagsNull()
    {
        var payloadWriter = new PacketWriter();
        payloadWriter.WriteUInt32(0x11223344u);
        payloadWriter.WriteUInt16(0x0032);
        payloadWriter.WriteByte(1);
        var reader = new PacketReader(payloadWriter.ToArray());

        var message = GameServerLoginOrPendingStateMessage.Read(GameServerOpcode.LoginOrPendingState, reader);

        Assert.Multiple(() =>
        {
            Assert.That(message.PlayerId, Is.EqualTo(0x11223344u));
            Assert.That(message.ServerBeat, Is.EqualTo((ushort)0x0032));
            Assert.That(message.IsBugReportingAllowed, Is.True);
            Assert.That(message.ViolationReasonFlags, Is.Null);
        });
    }

    [Test]
    public void Read_WithNonMarkerTrailingByte_LeavesViolationReasonFlagsNullAndByteUnread()
    {
        var payloadWriter = new PacketWriter();
        payloadWriter.WriteUInt32(0x11223344u);
        payloadWriter.WriteUInt16(0x0032);
        payloadWriter.WriteByte(0);
        payloadWriter.WriteByte(0xFF);
        var reader = new PacketReader(payloadWriter.ToArray());

        var message = GameServerLoginOrPendingStateMessage.Read(GameServerOpcode.LoginOrPendingState, reader);

        Assert.Multiple(() =>
        {
            Assert.That(message.IsBugReportingAllowed, Is.False);
            Assert.That(message.ViolationReasonFlags, Is.Null);
            Assert.That(reader.Remaining, Is.EqualTo(1));
        });
    }

    [Test]
    public void Read_WithViolationReasonMarker_ParsesFlagBytes()
    {
        var flags = MakeViolationReasonFlags(0x09);
        var payloadWriter = new PacketWriter();
        payloadWriter.WriteUInt32(0x11223344u);
        payloadWriter.WriteUInt16(0x0032);
        payloadWriter.WriteByte(1);
        payloadWriter.WriteByte(GameServerLoginOrPendingStateMessage.ViolationReasonMarker);
        payloadWriter.WriteBytes(flags);
        var reader = new PacketReader(payloadWriter.ToArray());

        var message = GameServerLoginOrPendingStateMessage.Read(GameServerOpcode.LoginOrPendingState, reader);

        Assert.That(message.ViolationReasonFlags, Is.EqualTo(flags));
    }

    [Test]
    public void Read_WithTruncatedFixedFields_ThrowsProtocolException()
    {
        var reader = new PacketReader(new byte[] { 1, 2, 3 });

        Assert.Throws<ProtocolException>(() =>
            GameServerLoginOrPendingStateMessage.Read(GameServerOpcode.LoginOrPendingState, reader));
    }

    [Test]
    public void Read_WithMarkerButTruncatedFlagBytes_ThrowsProtocolException()
    {
        var payloadWriter = new PacketWriter();
        payloadWriter.WriteUInt32(0x11223344u);
        payloadWriter.WriteUInt16(0x0032);
        payloadWriter.WriteByte(0);
        payloadWriter.WriteByte(GameServerLoginOrPendingStateMessage.ViolationReasonMarker);
        payloadWriter.WriteBytes(new byte[10]);
        var reader = new PacketReader(payloadWriter.ToArray());

        Assert.Throws<ProtocolException>(() =>
            GameServerLoginOrPendingStateMessage.Read(GameServerOpcode.LoginOrPendingState, reader));
    }

    [Test]
    public void Opcode_IsLoginOrPendingState()
    {
        var message = new GameServerLoginOrPendingStateMessage(0, 0, false, null);

        Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.LoginOrPendingState));
    }

    [Test]
    public void Constructor_WithWrongSizedViolationReasonFlags_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            new GameServerLoginOrPendingStateMessage(0, 0, false, new byte[5]));
    }

    [Test]
    public void Write_WithNullWriter_Throws()
    {
        var message = new GameServerLoginOrPendingStateMessage(0, 0, false, null);

        Assert.Throws<ArgumentNullException>(() => message.Write(null!));
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            GameServerLoginOrPendingStateMessage.Read(GameServerOpcode.LoginOrPendingState, null!));
    }
}
