// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Geometry;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Map;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Map;

[TestFixture]
public class GameServerFullMapMessageTests
{
    private const int TotalTiles = 18 * 14 * 8;

    private static readonly FakeItemTypeProvider ItemTypes = new FakeItemTypeProvider();

    [Test]
    public void Read_WithSurfacePosition_DecodesPositionAndEightFloors()
    {
        var writer = new PacketWriter();
        PositionCodec.Write(writer, new Position(100, 200, 0));
        MapPayloadWriter.WriteSingleTileFloors(writer, TotalTiles, 100, 7);
        var reader = new PacketReader(writer.ToArray());

        var message = (GameServerFullMapMessage)GameServerFullMapMessage.Read(GameServerOpcode.FullMap, reader, ItemTypes);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.FullMap));
            Assert.That(message.OwnPosition, Is.EqualTo(new Position(100, 200, 0)));
            Assert.That(message.Floors, Has.Count.EqualTo(8));
            Assert.That(message.Floors[0][0]!.Things[0].Item!.ItemId, Is.EqualTo(100));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerFullMapMessage.Read(GameServerOpcode.FullMap, null!, ItemTypes));
    }

    [Test]
    public void Read_WithNullItemTypes_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            GameServerFullMapMessage.Read(GameServerOpcode.FullMap, new PacketReader(new byte[5]), null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var writer = new PacketWriter();
        PositionCodec.Write(writer, new Position(100, 200, 0));
        var reader = new PacketReader(writer.ToArray());

        Assert.Throws<ProtocolException>(() => GameServerFullMapMessage.Read(GameServerOpcode.FullMap, reader, ItemTypes));
    }
}
