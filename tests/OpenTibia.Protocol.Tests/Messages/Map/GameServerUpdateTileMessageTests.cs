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
public class GameServerUpdateTileMessageTests
{
    private static readonly FakeItemTypeProvider ItemTypes = new FakeItemTypeProvider();

    [Test]
    public void Read_WithItemOnTile_DecodesNonNullTileWithThings()
    {
        var writer = new PacketWriter();
        PositionCodec.Write(writer, new Position(1, 2, 3));
        writer.WriteUInt16(100);
        writer.WriteByte(7);
        MapPayloadWriter.WriteTerminator(writer, 0);
        var reader = new PacketReader(writer.ToArray());

        var message = (GameServerUpdateTileMessage)GameServerUpdateTileMessage.Read(GameServerOpcode.UpdateTile, reader, ItemTypes);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.UpdateTile));
            Assert.That(message.Position, Is.EqualTo(new Position(1, 2, 3)));
            Assert.That(message.Tile!.Things[0].Item!.ItemId, Is.EqualTo(100));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithRemovedTileFlag_DecodesNullTile()
    {
        var writer = new PacketWriter();
        PositionCodec.Write(writer, new Position(1, 2, 3));
        MapPayloadWriter.WriteTerminator(writer, 1);
        var reader = new PacketReader(writer.ToArray());

        var message = (GameServerUpdateTileMessage)GameServerUpdateTileMessage.Read(GameServerOpcode.UpdateTile, reader, ItemTypes);

        Assert.Multiple(() =>
        {
            Assert.That(message.Tile, Is.Null);
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithEmptyTileFlag_DecodesNonNullEmptyTile()
    {
        var writer = new PacketWriter();
        PositionCodec.Write(writer, new Position(1, 2, 3));
        MapPayloadWriter.WriteTerminator(writer, 0);
        var reader = new PacketReader(writer.ToArray());

        var message = (GameServerUpdateTileMessage)GameServerUpdateTileMessage.Read(GameServerOpcode.UpdateTile, reader, ItemTypes);

        Assert.Multiple(() =>
        {
            Assert.That(message.Tile, Is.Not.Null);
            Assert.That(message.Tile!.Things, Is.Empty);
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerUpdateTileMessage.Read(GameServerOpcode.UpdateTile, null!, ItemTypes));
    }

    [Test]
    public void Read_WithNullItemTypes_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            GameServerUpdateTileMessage.Read(GameServerOpcode.UpdateTile, new PacketReader(new byte[5]), null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var writer = new PacketWriter();
        PositionCodec.Write(writer, new Position(1, 2, 3));
        var reader = new PacketReader(writer.ToArray());

        Assert.Throws<ProtocolException>(() => GameServerUpdateTileMessage.Read(GameServerOpcode.UpdateTile, reader, ItemTypes));
    }
}
