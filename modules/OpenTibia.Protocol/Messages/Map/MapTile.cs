// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System.Collections.Generic;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Map;

/// <summary>
/// One tile's decoded stack contents, in wire order.
/// </summary>
public sealed class MapTile
{
    public MapTile(IReadOnlyList<TileThing> things)
    {
        Things = things;
    }

    public IReadOnlyList<TileThing> Things { get; }
}
