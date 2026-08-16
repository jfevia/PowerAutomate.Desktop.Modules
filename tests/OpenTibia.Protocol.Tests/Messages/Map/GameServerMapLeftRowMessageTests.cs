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
public class GameServerMapLeftRowMessageTests
{
    private const int TotalTiles = 14 * 8;

    private static readonly FakeItemTypeProvider ItemTypes = new FakeItemTypeProvider();

    [Test]
    public void Read_WithSurfaceCurrentZ_DecodesEightFloorColumns()
    {
        var writer = new PacketWriter();
        MapPayloadWriter.WriteSingleTileFloors(writer, TotalTiles, 100, 3);
        var reader = new PacketReader(writer.ToArray());

        var message = (GameServerMapLeftRowMessage)GameServerMapLeftRowMessage.Read(GameServerOpcode.MapLeftRow, reader, ItemTypes, 0);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.MapLeftRow));
            Assert.That(message.Floors, Has.Count.EqualTo(8));
            Assert.That(message.Floors[0][0]!.Things[0].Item!.ItemId, Is.EqualTo(100));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerMapLeftRowMessage.Read(GameServerOpcode.MapLeftRow, null!, ItemTypes, 0));
    }

    [Test]
    public void Read_WithNullItemTypes_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            GameServerMapLeftRowMessage.Read(GameServerOpcode.MapLeftRow, new PacketReader(new byte[2]), null!, 0));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var reader = new PacketReader(new byte[] { 100, 0 });

        Assert.Throws<ProtocolException>(() => GameServerMapLeftRowMessage.Read(GameServerOpcode.MapLeftRow, reader, ItemTypes, 0));
    }
}
