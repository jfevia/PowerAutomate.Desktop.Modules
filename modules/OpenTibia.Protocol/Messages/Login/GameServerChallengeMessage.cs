// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Login;

/// <summary>
/// The game-server's connection challenge, sent immediately on connect before login.
/// </summary>
public sealed class GameServerChallengeMessage : IProtocolMessage
{
    public GameServerChallengeMessage(uint timestamp, byte random)
    {
        Timestamp = timestamp;
        Random = random;
    }

    public byte Opcode => (byte)GameServerOpcode.Challenge;

    public uint Timestamp { get; }

    public byte Random { get; }

    public void Write(PacketWriter writer)
    {
        MessageWriter.WriteOpcode(writer, Opcode);
        writer.WriteUInt32(Timestamp);
        writer.WriteByte(Random);
    }

    public static GameServerChallengeMessage Read(GameServerOpcode opcode, PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var timestamp = reader.ReadUInt32();
        var random = reader.ReadByte();
        return new GameServerChallengeMessage(timestamp, random);
    }
}
