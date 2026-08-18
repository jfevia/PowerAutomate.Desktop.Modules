// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Login;

/// <summary>
/// The game-server's informational popup, shown after login.
/// </summary>
public sealed class GameServerLoginAdviceMessage : IProtocolMessage
{
    public GameServerLoginAdviceMessage(string text)
    {
        Text = text ?? throw new ArgumentNullException(nameof(text));
    }

    public byte Opcode => (byte)GameServerOpcode.LoginAdvice;

    public string Text { get; }

    public void Write(PacketWriter writer)
    {
        MessageWriter.WriteOpcode(writer, Opcode);
        writer.WriteString(Text);
    }

    public static GameServerLoginAdviceMessage Read(GameServerOpcode opcode, PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        return new GameServerLoginAdviceMessage(reader.ReadString());
    }
}
