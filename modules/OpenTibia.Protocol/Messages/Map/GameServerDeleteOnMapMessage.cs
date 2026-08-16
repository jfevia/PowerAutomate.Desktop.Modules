// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Geometry;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Map;

/// <summary>
/// Server notice that a thing was removed from a tile (s2c 0x6C).
/// </summary>
public sealed class GameServerDeleteOnMapMessage : IProtocolMessage
{
    public GameServerDeleteOnMapMessage(Position position, byte stackPosition)
    {
        Position = position;
        StackPosition = stackPosition;
    }

    public byte Opcode => (byte)GameServerOpcode.DeleteOnMap;

    public Position Position { get; }

    public byte StackPosition { get; }

    public static IProtocolMessage Read(GameServerOpcode opcode, PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var position = PositionCodec.Read(reader);
        var stackPosition = reader.ReadByte();
        return new GameServerDeleteOnMapMessage(position, stackPosition);
    }
}
