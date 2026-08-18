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
public class GameServerFloorChangeDownMessageTests
{
    private const int PreRevealTiles = 18 * 14;
    private const int EastRowTiles = 14 * 8;
    private const int SouthRowTiles = 18 * 8;

    private static readonly FakeItemTypeProvider ItemTypes = new FakeItemTypeProvider();

    [Test]
    public void Read_WithThreePreRevealFloors_DecodesAllThreeSections()
    {
        var writer = new PacketWriter();
        MapPayloadWriter.WriteSingleTileFloors(writer, PreRevealTiles * 3, 100, 11);
        MapPayloadWriter.WriteSingleTileFloors(writer, EastRowTiles, 100, 22);
        MapPayloadWriter.WriteSingleTileFloors(writer, SouthRowTiles, 100, 33);
        var reader = new PacketReader(writer.ToArray());

        var message = (GameServerFloorChangeDownMessage)GameServerFloorChangeDownMessage.Read(
            GameServerOpcode.FloorChangeDown, reader, ItemTypes, 3, 7);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.FloorChangeDown));
            Assert.That(message.PreRevealFloors, Has.Count.EqualTo(3));
            Assert.That(message.PreRevealFloors[0][0]!.Things[0].Item!.Extra, Is.EqualTo(11));
            Assert.That(message.EastRow, Has.Count.EqualTo(8));
            Assert.That(message.EastRow[0][0]!.Things[0].Item!.Extra, Is.EqualTo(22));
            Assert.That(message.SouthRow, Has.Count.EqualTo(8));
            Assert.That(message.SouthRow[0][0]!.Things[0].Item!.Extra, Is.EqualTo(33));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            GameServerFloorChangeDownMessage.Read(GameServerOpcode.FloorChangeDown, null!, ItemTypes, 1, 7));
    }

    [Test]
    public void Read_WithNullItemTypes_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            GameServerFloorChangeDownMessage.Read(GameServerOpcode.FloorChangeDown, new PacketReader(new byte[2]), null!, 1, 7));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var reader = new PacketReader(new byte[] { 100, 0 });

        Assert.Throws<ProtocolException>(() =>
            GameServerFloorChangeDownMessage.Read(GameServerOpcode.FloorChangeDown, reader, ItemTypes, 1, 7));
    }
}
