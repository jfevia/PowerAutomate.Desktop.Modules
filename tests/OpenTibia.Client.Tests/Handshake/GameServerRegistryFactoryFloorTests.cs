// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Client.Handshake;
using PowerAutomate.Desktop.OpenTibia.Client.Streaming;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Geometry;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Map;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Client.Tests.Handshake;

[TestFixture]
public class GameServerRegistryFactoryFloorTests
{
    [Test]
    public void CreateDefault_WithNullItemTypes_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerRegistryFactory.CreateDefault(null!, new MapFloorTracker()));
    }

    [Test]
    public void CreateDefault_WithNullFloors_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerRegistryFactory.CreateDefault(new DefaultItemTypeProvider(), null!));
    }

    [TestCase(GameServerOpcode.MapTopRow, MapFloorPayloadWriter.FloorWidth, 1)]
    [TestCase(GameServerOpcode.MapBottomRow, MapFloorPayloadWriter.FloorWidth, 1)]
    [TestCase(GameServerOpcode.MapRightRow, 1, MapFloorPayloadWriter.FloorHeight)]
    [TestCase(GameServerOpcode.MapLeftRow, 1, MapFloorPayloadWriter.FloorHeight)]
    public void CreateDefault_RowOpcodeAtDefaultFloor_DecodesInsteadOfThrowing(GameServerOpcode opcode, int width, int height)
    {
        var floors = new MapFloorTracker();
        var registry = GameServerRegistryFactory.CreateDefault(new DefaultItemTypeProvider(), floors);

        var writer = new PacketWriter();
        writer.WriteByte((byte)opcode);
        MapFloorPayloadWriter.WriteEmptyTiles(writer, width * height * MapFloorPayloadWriter.FloorCountFor(floors.CurrentZ));

        var messages = registry.ReadAll(writer.ToArray());

        Assert.That(messages, Has.Count.EqualTo(1));
    }

    [TestCase((byte)8, 6)] // reaching the surface: newZ == 7
    [TestCase((byte)9, 1)] // still underground: newZ > 7
    [TestCase((byte)1, 0)] // already above the surface: newZ < 7
    public void CreateDefault_FloorChangeUp_UsesTheDerivedPreRevealCount(byte startZ, int expectedPreRevealFloors)
    {
        var floors = new MapFloorTracker();
        floors.Observe(GameServerFullMapAt(startZ));
        var registry = GameServerRegistryFactory.CreateDefault(new DefaultItemTypeProvider(), floors);

        var newZ = (byte)(startZ - 1);
        var writer = new PacketWriter();
        writer.WriteByte((byte)GameServerOpcode.FloorChangeUp);
        WritePreRevealAndRowSegments(writer, expectedPreRevealFloors, newZ);

        var messages = registry.ReadAll(writer.ToArray());
        var message = (GameServerFloorChangeUpMessage)messages[0];

        Assert.Multiple(() =>
        {
            Assert.That(messages, Has.Count.EqualTo(1));
            Assert.That(message.PreRevealFloors, Has.Count.EqualTo(expectedPreRevealFloors));
        });
    }

    [TestCase((byte)7, 3)] // entering underground: newZ == 8
    [TestCase((byte)9, 1)] // still descending: newZ in 9..13
    [TestCase((byte)13, 0)] // past the tracked range: newZ == 14
    [TestCase((byte)0, 0)] // not yet at the underground threshold: newZ == 1
    public void CreateDefault_FloorChangeDown_UsesTheDerivedPreRevealCount(byte startZ, int expectedPreRevealFloors)
    {
        var floors = new MapFloorTracker();
        floors.Observe(GameServerFullMapAt(startZ));
        var registry = GameServerRegistryFactory.CreateDefault(new DefaultItemTypeProvider(), floors);

        var newZ = (byte)(startZ + 1);
        var writer = new PacketWriter();
        writer.WriteByte((byte)GameServerOpcode.FloorChangeDown);
        WritePreRevealAndRowSegments(writer, expectedPreRevealFloors, newZ);

        var messages = registry.ReadAll(writer.ToArray());
        var message = (GameServerFloorChangeDownMessage)messages[0];

        Assert.Multiple(() =>
        {
            Assert.That(messages, Has.Count.EqualTo(1));
            Assert.That(message.PreRevealFloors, Has.Count.EqualTo(expectedPreRevealFloors));
        });
    }

    private static GameServerFullMapMessage GameServerFullMapAt(byte z)
    {
        return new GameServerFullMapMessage(new Position(100, 100, z), Array.Empty<IReadOnlyList<MapTile?>>());
    }

    private static void WritePreRevealAndRowSegments(PacketWriter writer, int preRevealFloorCount, byte newZ)
    {
        MapFloorPayloadWriter.WriteEmptyTiles(
            writer, preRevealFloorCount * MapFloorPayloadWriter.FloorWidth * MapFloorPayloadWriter.FloorHeight);
        MapFloorPayloadWriter.WriteEmptyTiles(
            writer, MapFloorPayloadWriter.FloorHeight * MapFloorPayloadWriter.FloorCountFor(newZ));
        MapFloorPayloadWriter.WriteEmptyTiles(
            writer, MapFloorPayloadWriter.FloorWidth * MapFloorPayloadWriter.FloorCountFor(newZ));
    }
}
