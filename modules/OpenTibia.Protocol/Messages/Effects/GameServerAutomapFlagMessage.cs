// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Geometry;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Effects;

/// <summary>
/// Server notice adding a marker to the player's automap (s2c 0xDD).
/// </summary>
public sealed class GameServerAutomapFlagMessage : IProtocolMessage
{
    public GameServerAutomapFlagMessage(Position position, byte markType, string description)
    {
        Position = position;
        MarkType = markType;
        Description = description;
    }

    public byte Opcode => (byte)GameServerOpcode.AutomapFlag;

    public Position Position { get; }

    /// <summary>
    /// The marker's icon type, matching TFS's MapMarks_t wire values.
    /// </summary>
    public byte MarkType { get; }

    public string Description { get; }

    public static IProtocolMessage Read(GameServerOpcode opcode, PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var position = PositionCodec.Read(reader);
        var markType = reader.ReadByte();
        var description = reader.ReadString();
        return new GameServerAutomapFlagMessage(position, markType, description);
    }
}
