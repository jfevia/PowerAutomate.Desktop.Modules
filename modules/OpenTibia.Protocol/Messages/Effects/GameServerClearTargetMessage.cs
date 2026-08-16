// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Effects;

/// <summary>
/// Server notice cancelling the local player's current attack target (s2c 0xA3).
/// </summary>
public sealed class GameServerClearTargetMessage : IProtocolMessage
{
    public GameServerClearTargetMessage(uint reservedValue)
    {
        ReservedValue = reservedValue;
    }

    public byte Opcode => (byte)GameServerOpcode.ClearTarget;

    /// <summary>
    /// TFS always writes 0 for this field; unused in this revision.
    /// </summary>
    public uint ReservedValue { get; }

    public static IProtocolMessage Read(GameServerOpcode opcode, PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        return new GameServerClearTargetMessage(reader.ReadUInt32());
    }
}
