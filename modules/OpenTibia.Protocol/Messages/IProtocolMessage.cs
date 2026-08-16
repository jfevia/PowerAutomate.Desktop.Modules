// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages;

/// <summary>
/// Any decoded or encodable protocol message.
/// </summary>
public interface IProtocolMessage
{
    /// <summary>
    /// The leading opcode byte that identifies this message on the wire.
    /// </summary>
    byte Opcode { get; }
}

/// <summary>
/// A message the client sends, able to serialize its own body.
/// </summary>
public interface IClientMessage : IProtocolMessage
{
    /// <summary>
    /// Writes the opcode followed by the message body.
    /// </summary>
    void Write(PacketWriter writer);
}
