// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Map;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Map;

[TestFixture]
public class GameServerFloorChangeUpMessageTests
{
    private const int PreRevealTiles = 18 * 14;
    private const int WestRowTiles = 14 * 8;
    private const int NorthRowTiles = 18 * 8;

    private static readonly FakeItemTypeProvider ItemTypes = new FakeItemTypeProvider();

    [Test]
    public void Read_WithOnePreRevealFloor_DecodesAllThreeSections()
    {
        var writer = new PacketWriter();
        MapPayloadWriter.WriteSingleTileFloors(writer, PreRevealTiles, 100, 11);
        MapPayloadWriter.WriteSingleTileFloors(writer, WestRowTiles, 100, 22);
        MapPayloadWriter.WriteSingleTileFloors(writer, NorthRowTiles, 100, 33);
        var reader = new PacketReader(writer.ToArray());

        var message = (GameServerFloorChangeUpMessage)GameServerFloorChangeUpMessage.Read(
            GameServerOpcode.FloorChangeUp, reader, ItemTypes, 1, 0);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.FloorChangeUp));
            Assert.That(message.PreRevealFloors, Has.Count.EqualTo(1));
            Assert.That(message.PreRevealFloors[0][0]!.Things[0].Item!.Extra, Is.EqualTo(11));
            Assert.That(message.WestRow, Has.Count.EqualTo(8));
            Assert.That(message.WestRow[0][0]!.Things[0].Item!.Extra, Is.EqualTo(22));
            Assert.That(message.NorthRow, Has.Count.EqualTo(8));
            Assert.That(message.NorthRow[0][0]!.Things[0].Item!.Extra, Is.EqualTo(33));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithNoPreRevealFloors_DecodesEmptyPreRevealList()
    {
        var writer = new PacketWriter();
        MapPayloadWriter.WriteSingleTileFloors(writer, WestRowTiles, 100, 22);
        MapPayloadWriter.WriteSingleTileFloors(writer, NorthRowTiles, 100, 33);
        var reader = new PacketReader(writer.ToArray());

        var message = (GameServerFloorChangeUpMessage)GameServerFloorChangeUpMessage.Read(
            GameServerOpcode.FloorChangeUp, reader, ItemTypes, 0, 0);

        Assert.Multiple(() =>
        {
            Assert.That(message.PreRevealFloors, Is.Empty);
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            GameServerFloorChangeUpMessage.Read(GameServerOpcode.FloorChangeUp, null!, ItemTypes, 1, 0));
    }

    [Test]
    public void Read_WithNullItemTypes_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            GameServerFloorChangeUpMessage.Read(GameServerOpcode.FloorChangeUp, new PacketReader(new byte[2]), null!, 1, 0));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var reader = new PacketReader(new byte[] { 100, 0 });

        Assert.Throws<ProtocolException>(() =>
            GameServerFloorChangeUpMessage.Read(GameServerOpcode.FloorChangeUp, reader, ItemTypes, 1, 0));
    }
}
