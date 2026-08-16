// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Creatures;

/// <summary>
/// Reads an <see cref="OutfitDescriptor" /> from a packet.
/// </summary>
public static class OutfitDescriptorCodec
{
    /// <summary>
    /// Reads a look type, then either colors/addons (if nonzero) or a disguise item id (if zero).
    /// </summary>
    public static OutfitDescriptor Read(PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var lookType = reader.ReadUInt16();
        if (lookType != 0)
        {
            var head = reader.ReadByte();
            var body = reader.ReadByte();
            var legs = reader.ReadByte();
            var feet = reader.ReadByte();
            var addons = reader.ReadByte();
            return new OutfitDescriptor(lookType, head, body, legs, feet, addons, null);
        }

        var lookTypeEx = reader.ReadUInt16();
        return new OutfitDescriptor(lookType, null, null, null, null, null, lookTypeEx);
    }
}
