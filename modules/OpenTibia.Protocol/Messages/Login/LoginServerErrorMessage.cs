// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Login;

/// <summary>
/// The login-server's rejection reason, sent before any character list.
/// </summary>
public sealed class LoginServerErrorMessage : IProtocolMessage
{
    public LoginServerErrorMessage(string errorText)
    {
        ErrorText = errorText ?? throw new ArgumentNullException(nameof(errorText));
    }

    public byte Opcode => (byte)LoginServerOpcode.Error;

    public string ErrorText { get; }

    public void Write(PacketWriter writer)
    {
        MessageWriter.WriteOpcode(writer, Opcode);
        writer.WriteString(ErrorText);
    }

    public static LoginServerErrorMessage Read(LoginServerOpcode opcode, PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        return new LoginServerErrorMessage(reader.ReadString());
    }
}
