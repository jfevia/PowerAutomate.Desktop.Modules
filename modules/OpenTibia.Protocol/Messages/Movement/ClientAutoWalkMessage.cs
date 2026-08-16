// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Movement;

/// <summary>
/// Client request to walk an auto-walk path (opcode 0x64).
/// </summary>
public sealed class ClientAutoWalkMessage : IClientMessage
{
    public ClientAutoWalkMessage(IReadOnlyList<Direction> path)
    {
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        if (path.Count == 0 || path.Count > 255)
        {
            throw new ProtocolException($"Auto-walk path must contain 1-255 steps but had {path.Count}.");
        }

        Path = path;
    }

    /// <summary>
    /// The ordered steps the server will walk one at a time.
    /// </summary>
    public IReadOnlyList<Direction> Path { get; }

    public byte Opcode => (byte)ClientOpcode.AutoWalk;

    public void Write(PacketWriter writer)
    {
        MessageWriter.WriteOpcode(writer, Opcode);
        writer.WriteByte((byte)Path.Count);
        for (var index = 0; index < Path.Count; index++)
        {
            writer.WriteByte(DirectionOpcodes.ToPathByte(Path[index]));
        }
    }
}
