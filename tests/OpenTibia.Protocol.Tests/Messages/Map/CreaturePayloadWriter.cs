// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Movement;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Map;

/// <summary>
/// Hand-writes minimal but valid <see cref="Messages.Creatures.CreatureDescriptor" /> wire bytes for map tests.
/// </summary>
internal static class CreaturePayloadWriter
{
    public static void WriteKnown(PacketWriter writer, uint creatureId)
    {
        writer.WriteUInt16(0x0062);
        writer.WriteUInt32(creatureId);
        writer.WriteByte(100);
        writer.WriteByte((byte)Direction.South);
        writer.WriteUInt16(128);
        writer.WriteByte(10);
        writer.WriteByte(20);
        writer.WriteByte(30);
        writer.WriteByte(40);
        writer.WriteByte(0);
        writer.WriteByte(0);
        writer.WriteByte(0);
        writer.WriteUInt16(200);
        writer.WriteByte(0);
        writer.WriteByte(0);
        writer.WriteByte(0);
    }

    public static void WriteUnknown(PacketWriter writer, uint creatureId, string name)
    {
        writer.WriteUInt16(0x0061);
        writer.WriteUInt32(0u);
        writer.WriteUInt32(creatureId);
        writer.WriteString(name);
        writer.WriteByte(80);
        writer.WriteByte((byte)Direction.North);
        writer.WriteUInt16(130);
        writer.WriteByte(1);
        writer.WriteByte(2);
        writer.WriteByte(3);
        writer.WriteByte(4);
        writer.WriteByte(0);
        writer.WriteByte(0);
        writer.WriteByte(0);
        writer.WriteUInt16(220);
        writer.WriteByte(0);
        writer.WriteByte(0);
        writer.WriteByte(0);
        writer.WriteByte(0);
    }
}
