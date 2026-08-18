// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Services;

/// <summary>
/// A client message built from a raw opcode and payload, the escape hatch for undecoded traffic.
/// </summary>
public sealed class RawClientMessage : IClientMessage
{
    private readonly byte[] _payload;

    public RawClientMessage(byte opcode, byte[] payload)
    {
        Opcode = opcode;
        _payload = payload ?? throw new ArgumentNullException(nameof(payload));
    }

    public byte Opcode { get; }

    public void Write(PacketWriter writer)
    {
        MessageWriter.WriteOpcode(writer, Opcode);
        writer.WriteBytes(_payload);
    }
}
