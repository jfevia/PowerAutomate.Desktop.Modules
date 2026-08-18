// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using PowerAutomate.Desktop.OpenTibia.Protocol.Items;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Container;

/// <summary>
/// Item ids are classified by range so the tests stay independent of any item database.
/// </summary>
internal sealed class FakeItemTypeProvider : IItemTypeProvider
{
    public bool IsStackable(ushort itemId)
    {
        return itemId == 100;
    }

    public bool IsFluidContainer(ushort itemId)
    {
        return itemId == 200;
    }

    public bool IsSplash(ushort itemId)
    {
        return itemId == 300;
    }
}
