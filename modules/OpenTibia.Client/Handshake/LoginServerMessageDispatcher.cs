// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using PowerAutomate.Desktop.OpenTibia.Protocol;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Login;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Client.Handshake;

/// <summary>
/// Decodes the login-server stream, which uses its own small opcode set.
/// </summary>
public static class LoginServerMessageDispatcher
{
    public static IReadOnlyList<IProtocolMessage> ReadAll(byte[] payload)
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

    private static IProtocolMessage ReadOne(PacketReader reader)
    {
        var raw = reader.ReadByte();

        switch ((LoginServerOpcode)raw)
        {
            case LoginServerOpcode.Error:
                return LoginServerErrorMessage.Read(LoginServerOpcode.Error, reader);
            case LoginServerOpcode.Motd:
                return LoginServerMotdMessage.Read(LoginServerOpcode.Motd, reader);
            case LoginServerOpcode.CharacterList:
                return LoginServerCharacterListMessage.Read(LoginServerOpcode.CharacterList, reader);
            default:
                throw new ProtocolException($"Login server opcode 0x{raw:X2} is not supported.");
        }
    }
}
