// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Items;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Creatures;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Map;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Movement;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Map;

[TestFixture]
public class TileThingTests
{
    [Test]
    public void Constructor_WithItem_ExposesItemAndNullCreature()
    {
        var item = new ItemStack(100, 1);

        var thing = new TileThing(item, null);

        Assert.Multiple(() =>
        {
            Assert.That(thing.Item, Is.SameAs(item));
            Assert.That(thing.Creature, Is.Null);
        });
    }

    [Test]
    public void Constructor_WithCreature_ExposesCreatureAndNullItem()
    {
        var creature = new CreatureDescriptor(
            1, true, null, null, 100, Direction.North,
            new OutfitDescriptor(128, 0, 0, 0, 0, 0, null), new LightInfo(0, 0), 200, 0, 0, null, false);

        var thing = new TileThing(null, creature);

        Assert.Multiple(() =>
        {
            Assert.That(thing.Creature, Is.SameAs(creature));
            Assert.That(thing.Item, Is.Null);
        });
    }
}
