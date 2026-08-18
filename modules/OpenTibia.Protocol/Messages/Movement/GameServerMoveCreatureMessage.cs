// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Geometry;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Movement;

/// <summary>
/// Server notice that a creature moved one step without teleporting (s2c 0x6D).
/// </summary>
public sealed class GameServerMoveCreatureMessage : IProtocolMessage
{
    public GameServerMoveCreatureMessage(Position fromPosition, byte fromStackPosition, Position toPosition)
    {
        FromPosition = fromPosition;
        FromStackPosition = fromStackPosition;
        ToPosition = toPosition;
    }

    public byte Opcode => (byte)GameServerOpcode.MoveCreature;

    public Position FromPosition { get; }

    public byte FromStackPosition { get; }

    public Position ToPosition { get; }

    public static IProtocolMessage Read(GameServerOpcode opcode, PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var fromPosition = PositionCodec.Read(reader);
        var fromStackPosition = reader.ReadByte();
        var toPosition = PositionCodec.Read(reader);
        return new GameServerMoveCreatureMessage(fromPosition, fromStackPosition, toPosition);
    }
}
