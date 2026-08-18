// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Login;

/// <summary>
/// The game-server's login rejection, shown when a character fails to enter the world.
/// </summary>
public sealed class GameServerLoginErrorMessage : IProtocolMessage
{
    public GameServerLoginErrorMessage(string errorText)
    {
        ErrorText = errorText ?? throw new ArgumentNullException(nameof(errorText));
    }

    public byte Opcode => (byte)GameServerOpcode.LoginError;

    public string ErrorText { get; }

    public void Write(PacketWriter writer)
    {
        MessageWriter.WriteOpcode(writer, Opcode);
        writer.WriteString(ErrorText);
    }

    public static GameServerLoginErrorMessage Read(GameServerOpcode opcode, PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        return new GameServerLoginErrorMessage(reader.ReadString());
    }
}
