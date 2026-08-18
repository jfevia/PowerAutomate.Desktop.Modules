// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Login;

/// <summary>
/// The game-server's queueing notice, shown while waiting for a world slot.
/// </summary>
public sealed class GameServerLoginWaitMessage : IProtocolMessage
{
    public GameServerLoginWaitMessage(string text, byte waitTimeSeconds)
    {
        Text = text ?? throw new ArgumentNullException(nameof(text));
        WaitTimeSeconds = waitTimeSeconds;
    }

    public byte Opcode => (byte)GameServerOpcode.LoginWait;

    public string Text { get; }

    public byte WaitTimeSeconds { get; }

    public void Write(PacketWriter writer)
    {
        MessageWriter.WriteOpcode(writer, Opcode);
        writer.WriteString(Text);
        writer.WriteByte(WaitTimeSeconds);
    }

    public static GameServerLoginWaitMessage Read(GameServerOpcode opcode, PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var text = reader.ReadString();
        var waitTimeSeconds = reader.ReadByte();
        return new GameServerLoginWaitMessage(text, waitTimeSeconds);
    }
}
