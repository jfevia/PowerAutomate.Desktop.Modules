// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Editable;

/// <summary>
/// Server-opened text window for reading/writing an item's text (s2c 0x96).
/// </summary>
public sealed class GameServerEditTextMessage : IProtocolMessage
{
    public GameServerEditTextMessage(
        uint windowTextId,
        ushort itemId,
        ushort maxOrCurrentLength,
        string text,
        string writerName,
        string writtenDate)
    {
        WindowTextId = windowTextId;
        ItemId = itemId;
        MaxOrCurrentLength = maxOrCurrentLength;
        Text = text ?? throw new ArgumentNullException(nameof(text));
        WriterName = writerName ?? throw new ArgumentNullException(nameof(writerName));
        WrittenDate = writtenDate ?? throw new ArgumentNullException(nameof(writtenDate));
    }

    public byte Opcode => (byte)GameServerOpcode.EditText;

    public uint WindowTextId { get; }

    public ushort ItemId { get; }

    /// <summary>
    /// The maximum allowed length when writable, or the current text's length when read-only.
    /// </summary>
    public ushort MaxOrCurrentLength { get; }

    public string Text { get; }

    /// <summary>
    /// The name of the player who last wrote this text, or empty if never written.
    /// </summary>
    public string WriterName { get; }

    /// <summary>
    /// The formatted date the text was last written, or empty if never written.
    /// </summary>
    public string WrittenDate { get; }

    public static IProtocolMessage Read(GameServerOpcode opcode, PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var windowTextId = reader.ReadUInt32();
        var itemId = reader.ReadUInt16();
        var maxOrCurrentLength = reader.ReadUInt16();
        var text = reader.ReadString();
        var writerName = reader.ReadString();
        var writtenDate = reader.ReadString();
        return new GameServerEditTextMessage(windowTextId, itemId, maxOrCurrentLength, text, writerName, writtenDate);
    }
}
