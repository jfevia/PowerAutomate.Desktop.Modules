// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Client.Streaming;
using PowerAutomate.Desktop.OpenTibia.Protocol.Geometry;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Map;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Client.Tests.Streaming;

[TestFixture]
public class MapFloorTrackerTests
{
    private static readonly IReadOnlyList<IReadOnlyList<MapTile?>> NoFloors = Array.Empty<IReadOnlyList<MapTile?>>();

    [Test]
    public void CurrentZ_BeforeAnyMessage_IsGroundFloor()
    {
        var tracker = new MapFloorTracker();

        Assert.That(tracker.CurrentZ, Is.EqualTo((byte)7));
    }

    [Test]
    public void Observe_WithNullMessage_Throws()
    {
        var tracker = new MapFloorTracker();

        Assert.Throws<ArgumentNullException>(() => tracker.Observe(null!));
    }

    [Test]
    public void Observe_WithFullMap_SetsCurrentZFromOwnPosition()
    {
        var tracker = new MapFloorTracker();

        tracker.Observe(new GameServerFullMapMessage(new Position(100, 100, 9), NoFloors));

        Assert.That(tracker.CurrentZ, Is.EqualTo((byte)9));
    }

    [Test]
    public void Observe_WithFloorChangeUp_DecrementsCurrentZ()
    {
        var tracker = new MapFloorTracker();
        tracker.Observe(new GameServerFullMapMessage(new Position(100, 100, 9), NoFloors));

        tracker.Observe(new GameServerFloorChangeUpMessage(NoFloors, NoFloors, NoFloors));

        Assert.That(tracker.CurrentZ, Is.EqualTo((byte)8));
    }

    [Test]
    public void Observe_WithFloorChangeDown_IncrementsCurrentZ()
    {
        var tracker = new MapFloorTracker();
        tracker.Observe(new GameServerFullMapMessage(new Position(100, 100, 7), NoFloors));

        tracker.Observe(new GameServerFloorChangeDownMessage(NoFloors, NoFloors, NoFloors));

        Assert.That(tracker.CurrentZ, Is.EqualTo((byte)8));
    }

    [Test]
    public void Observe_WithUnrelatedMessage_LeavesCurrentZUnchanged()
    {
        var tracker = new MapFloorTracker();
        tracker.Observe(new GameServerFullMapMessage(new Position(100, 100, 9), NoFloors));

        tracker.Observe(new PayloadlessMessage(GameServerOpcode.Ping));

        Assert.That(tracker.CurrentZ, Is.EqualTo((byte)9));
    }
}
