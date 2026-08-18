// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using PowerAutomate.Desktop.OpenTibia.Protocol.Items;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Map;

/// <summary>
/// Fallback used when no real items.otb-derived provider is available; assumes no extra byte.
/// </summary>
public sealed class DefaultItemTypeProvider : IItemTypeProvider
{
    public bool IsStackable(ushort itemId)
    {
        return false;
    }

    public bool IsFluidContainer(ushort itemId)
    {
        return false;
    }

    public bool IsSplash(ushort itemId)
    {
        return false;
    }
}
