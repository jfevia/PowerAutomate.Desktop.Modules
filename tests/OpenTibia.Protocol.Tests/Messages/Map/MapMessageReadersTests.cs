// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Geometry;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Map;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Map;

[TestFixture]
public class MapMessageReadersTests
{
    private static readonly FakeItemTypeProvider ItemTypes = new FakeItemTypeProvider();

    [Test]
    public void RegisterTo_WithDefaultProvider_RegistersEveryMapOpcode()
    {
        var registry = new GameServerMessageRegistry();

        MapMessageReaders.RegisterTo(registry);

        AssertAllRegistered(registry);
    }

    [Test]
    public void RegisterTo_WithItemTypesProvider_RegistersEveryMapOpcode()
    {
        var registry = new GameServerMessageRegistry();

        MapMessageReaders.RegisterTo(registry, ItemTypes);

        AssertAllRegistered(registry);
    }

    [Test]
    public void RegisterTo_WithNullRegistry_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => MapMessageReaders.RegisterTo(null!));
    }

    [Test]
    public void RegisterTo_WithNullRegistryAndItemTypes_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => MapMessageReaders.RegisterTo(null!, ItemTypes));
    }

    [Test]
    public void RegisterTo_WithNullItemTypes_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => MapMessageReaders.RegisterTo(new GameServerMessageRegistry(), null!));
    }

    [Test]
    public void ReadAll_WithFullMapOpcode_DecodesFullMapMessage()
    {
        var registry = new GameServerMessageRegistry();
        MapMessageReaders.RegisterTo(registry, ItemTypes);

        var writer = new PacketWriter();
        writer.WriteByte((byte)GameServerOpcode.FullMap);
        PositionCodec.Write(writer, new Position(1, 2, 0));
        MapPayloadWriter.WriteSingleTileFloors(writer, 18 * 14 * 8, 100, 5);

        var messages = registry.ReadAll(writer.ToArray());

        Assert.That(messages, Has.Count.EqualTo(1));
        Assert.That(messages[0], Is.InstanceOf<GameServerFullMapMessage>());
    }

    [Test]
    public void ReadAll_WithUpdateTileOpcode_DecodesUpdateTileMessage()
    {
        var registry = new GameServerMessageRegistry();
        MapMessageReaders.RegisterTo(registry, ItemTypes);

        var writer = new PacketWriter();
        writer.WriteByte((byte)GameServerOpcode.UpdateTile);
        PositionCodec.Write(writer, new Position(1, 2, 0));
        MapPayloadWriter.WriteTerminator(writer, 0);

        var messages = registry.ReadAll(writer.ToArray());

        Assert.That(messages[0], Is.InstanceOf<GameServerUpdateTileMessage>());
    }

    [Test]
    public void ReadAll_WithCreateOnMapOpcode_DecodesCreateOnMapMessage()
    {
        var registry = new GameServerMessageRegistry();
        MapMessageReaders.RegisterTo(registry, ItemTypes);

        var writer = new PacketWriter();
        writer.WriteByte((byte)GameServerOpcode.CreateOnMap);
        PositionCodec.Write(writer, new Position(1, 2, 0));
        writer.WriteByte(0);
        writer.WriteUInt16(100);
        writer.WriteByte(1);

        var messages = registry.ReadAll(writer.ToArray());

        Assert.That(messages[0], Is.InstanceOf<GameServerCreateOnMapMessage>());
    }

    [Test]
    public void ReadAll_WithChangeOnMapOpcode_DecodesChangeOnMapMessage()
    {
        var registry = new GameServerMessageRegistry();
        MapMessageReaders.RegisterTo(registry, ItemTypes);

        var writer = new PacketWriter();
        writer.WriteByte((byte)GameServerOpcode.ChangeOnMap);
        PositionCodec.Write(writer, new Position(1, 2, 0));
        writer.WriteByte(0);
        writer.WriteUInt16(100);
        writer.WriteByte(1);

        var messages = registry.ReadAll(writer.ToArray());

        Assert.That(messages[0], Is.InstanceOf<GameServerChangeOnMapMessage>());
    }

    [Test]
    public void ReadAll_WithDeleteOnMapOpcode_DecodesDeleteOnMapMessage()
    {
        var registry = new GameServerMessageRegistry();
        MapMessageReaders.RegisterTo(registry, ItemTypes);

        var writer = new PacketWriter();
        writer.WriteByte((byte)GameServerOpcode.DeleteOnMap);
        PositionCodec.Write(writer, new Position(1, 2, 0));
        writer.WriteByte(0);

        var messages = registry.ReadAll(writer.ToArray());

        Assert.That(messages[0], Is.InstanceOf<GameServerDeleteOnMapMessage>());
    }

    [TestCase(GameServerOpcode.MapTopRow)]
    [TestCase(GameServerOpcode.MapRightRow)]
    [TestCase(GameServerOpcode.MapBottomRow)]
    [TestCase(GameServerOpcode.MapLeftRow)]
    [TestCase(GameServerOpcode.FloorChangeUp)]
    [TestCase(GameServerOpcode.FloorChangeDown)]
    public void ReadAll_WithFloorContextOpcode_ThrowsNotSupportedException(GameServerOpcode opcode)
    {
        var registry = new GameServerMessageRegistry();
        MapMessageReaders.RegisterTo(registry, ItemTypes);

        var payload = new[] { (byte)opcode };

        Assert.Throws<NotSupportedException>(() => registry.ReadAll(payload));
    }

    private static void AssertAllRegistered(GameServerMessageRegistry registry)
    {
        Assert.Multiple(() =>
        {
            Assert.That(registry.IsRegistered(GameServerOpcode.FullMap), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.MapTopRow), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.MapRightRow), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.MapBottomRow), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.MapLeftRow), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.UpdateTile), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.CreateOnMap), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.ChangeOnMap), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.DeleteOnMap), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.FloorChangeUp), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.FloorChangeDown), Is.True);
        });
    }
}
