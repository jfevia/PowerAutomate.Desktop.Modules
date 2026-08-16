// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages;

/// <summary>
/// Decodes the body of one game-server message, the opcode having already been consumed.
/// </summary>
public delegate IProtocolMessage GameServerMessageReader(GameServerOpcode opcode, PacketReader reader);

/// <summary>
/// Opcode to reader dispatch for the game-server stream.
/// </summary>
/// <remarks>
/// An unmapped opcode is fatal: 8.60 bodies are not self-describing, so the stream position is lost.
/// </remarks>
public sealed class GameServerMessageRegistry
{
    private readonly GameServerMessageReader?[] _readers = new GameServerMessageReader?[256];

    public void Register(GameServerOpcode opcode, GameServerMessageReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        _readers[(byte)opcode] = reader;
    }

    public bool IsRegistered(GameServerOpcode opcode)
    {
        return _readers[(byte)opcode] != null;
    }

    /// <summary>
    /// Reads every message in a decoded payload until it is exhausted.
    /// </summary>
    public IReadOnlyList<IProtocolMessage> ReadAll(byte[] payload)
    {
        if (payload == null)
        {
            throw new ArgumentNullException(nameof(payload));
        }

        var reader = new PacketReader(payload);
        var messages = new List<IProtocolMessage>();

        while (reader.Remaining > 0)
        {
            messages.Add(ReadOne(reader));
        }

        return messages;
    }

    private IProtocolMessage ReadOne(PacketReader reader)
    {
        var raw = reader.ReadByte();
        var handler = _readers[raw];
        if (handler == null)
        {
            throw new ProtocolException(
                $"Opcode 0x{raw:X2} has no reader; the remaining {reader.Remaining} byte(s) cannot be interpreted.");
        }

        return handler((GameServerOpcode)raw, reader);
    }
}
