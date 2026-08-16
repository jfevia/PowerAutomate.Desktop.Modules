// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using PowerAutomate.Desktop.OpenTibia.Protocol.Items;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Creatures;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Map;

/// <summary>
/// One tile-stack entry: either an item or a creature, never both.
/// </summary>
public sealed class TileThing
{
    public TileThing(ItemStack? item, CreatureDescriptor? creature)
    {
        Item = item;
        Creature = creature;
    }

    public ItemStack? Item { get; }

    public CreatureDescriptor? Creature { get; }
}
