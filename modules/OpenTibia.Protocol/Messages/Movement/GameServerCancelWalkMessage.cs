// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Movement;

/// <summary>
/// Server notice that the local player's auto-walk was cancelled (s2c 0xB5).
/// </summary>
public sealed class GameServerCancelWalkMessage : IProtocolMessage
{
    public GameServerCancelWalkMessage(Direction facingDirection)
    {
        FacingDirection = facingDirection;
    }

    public byte Opcode => (byte)GameServerOpcode.CancelWalk;

    public Direction FacingDirection { get; }

    public static IProtocolMessage Read(GameServerOpcode opcode, PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var facingDirection = (Direction)reader.ReadByte();
        return new GameServerCancelWalkMessage(facingDirection);
    }
}
