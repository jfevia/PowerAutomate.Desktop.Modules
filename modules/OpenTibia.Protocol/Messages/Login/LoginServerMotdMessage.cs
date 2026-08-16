// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Login;

/// <summary>
/// The login-server's message of the day, sent together with the character list.
/// </summary>
public sealed class LoginServerMotdMessage : IProtocolMessage
{
    public LoginServerMotdMessage(string motd)
    {
        Motd = motd ?? throw new ArgumentNullException(nameof(motd));
    }

    public byte Opcode => (byte)LoginServerOpcode.Motd;

    public string Motd { get; }

    public void Write(PacketWriter writer)
    {
        MessageWriter.WriteOpcode(writer, Opcode);
        writer.WriteString(Motd);
    }

    public static LoginServerMotdMessage Read(LoginServerOpcode opcode, PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        return new LoginServerMotdMessage(reader.ReadString());
    }
}
