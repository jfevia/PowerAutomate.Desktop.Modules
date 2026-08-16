// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Items;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Map;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Map;

[TestFixture]
public class MapTileTests
{
    [Test]
    public void Constructor_WithThings_ExposesThemInOrder()
    {
        var things = new[] { new TileThing(new ItemStack(100, 1), null), new TileThing(new ItemStack(200, 2), null) };

        var tile = new MapTile(things);

        Assert.That(tile.Things, Is.EqualTo(things));
    }
}
