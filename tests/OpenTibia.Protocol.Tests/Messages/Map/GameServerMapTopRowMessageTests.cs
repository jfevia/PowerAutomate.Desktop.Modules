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
public class GameServerMapTopRowMessageTests
{
    private const int TotalTiles = 18 * 8;

    private static readonly FakeItemTypeProvider ItemTypes = new FakeItemTypeProvider();

    [Test]
    public void Read_WithSurfaceCurrentZ_DecodesEightFloorRows()
    {
        var writer = new PacketWriter();
        MapPayloadWriter.WriteSingleTileFloors(writer, TotalTiles, 100, 3);
        var reader = new PacketReader(writer.ToArray());

        var message = (GameServerMapTopRowMessage)GameServerMapTopRowMessage.Read(GameServerOpcode.MapTopRow, reader, ItemTypes, 0);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.MapTopRow));
            Assert.That(message.Floors, Has.Count.EqualTo(8));
            Assert.That(message.Floors[0][0]!.Things[0].Item!.ItemId, Is.EqualTo(100));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerMapTopRowMessage.Read(GameServerOpcode.MapTopRow, null!, ItemTypes, 0));
    }

    [Test]
    public void Read_WithNullItemTypes_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            GameServerMapTopRowMessage.Read(GameServerOpcode.MapTopRow, new PacketReader(new byte[2]), null!, 0));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var reader = new PacketReader(new byte[] { 100, 0 });

        Assert.Throws<ProtocolException>(() => GameServerMapTopRowMessage.Read(GameServerOpcode.MapTopRow, reader, ItemTypes, 0));
    }
}
