// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Creatures;

/// <summary>
/// Server-opened outfit selection window (s2c 0xC8).
/// </summary>
public sealed class GameServerChooseOutfitMessage : IProtocolMessage
{
    public GameServerChooseOutfitMessage(OutfitDescriptor currentOutfit, IReadOnlyList<SelectableOutfit> selectableOutfits)
    {
        CurrentOutfit = currentOutfit ?? throw new ArgumentNullException(nameof(currentOutfit));
        SelectableOutfits = selectableOutfits ?? throw new ArgumentNullException(nameof(selectableOutfits));
    }

    public byte Opcode => (byte)GameServerOpcode.ChooseOutfit;

    public OutfitDescriptor CurrentOutfit { get; }

    public IReadOnlyList<SelectableOutfit> SelectableOutfits { get; }

    public static IProtocolMessage Read(GameServerOpcode opcode, PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var currentOutfit = OutfitDescriptorCodec.Read(reader);
        var outfitCount = reader.ReadByte();
        var selectableOutfits = new List<SelectableOutfit>(outfitCount);
        for (var index = 0; index < outfitCount; index++)
        {
            selectableOutfits.Add(SelectableOutfit.Read(reader));
        }

        return new GameServerChooseOutfitMessage(currentOutfit, selectableOutfits);
    }
}
