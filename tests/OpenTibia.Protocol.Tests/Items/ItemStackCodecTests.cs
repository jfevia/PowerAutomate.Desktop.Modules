// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Items;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Items;

/// <summary>
/// Item ids are classified by range so the tests stay independent of any item database.
/// </summary>
internal sealed class FakeItemTypeProvider : IItemTypeProvider
{
    public bool IsStackable(ushort itemId)
    {
        return itemId == 100;
    }

    public bool IsFluidContainer(ushort itemId)
    {
        return itemId == 200;
    }

    public bool IsSplash(ushort itemId)
    {
        return itemId == 300;
    }
}

[TestFixture]
public class ItemStackCodecTests
{
    private static readonly FakeItemTypeProvider ItemTypes = new FakeItemTypeProvider();

    [Test]
    public void Read_WithStackableItem_ConsumesCountByte()
    {
        var reader = new PacketReader(new byte[] { 100, 0, 42 });

        var stack = ItemStackCodec.Read(reader, ItemTypes);

        Assert.Multiple(() =>
        {
            Assert.That(stack.ItemId, Is.EqualTo(100));
            Assert.That(stack.Extra, Is.EqualTo(42));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithFluidContainer_ConsumesFluidByte()
    {
        var reader = new PacketReader(new byte[] { 200, 0, 3 });

        Assert.That(ItemStackCodec.Read(reader, ItemTypes).Extra, Is.EqualTo(3));
    }

    [Test]
    public void Read_WithSplash_ConsumesFluidByte()
    {
        var reader = new PacketReader(new byte[] { 44, 1, 5 });

        Assert.That(ItemStackCodec.Read(reader, ItemTypes).Extra, Is.EqualTo(5));
    }

    [Test]
    public void Read_WithPlainItem_ConsumesOnlyTheIdentifier()
    {
        var reader = new PacketReader(new byte[] { 10, 0, 0xFF });

        var stack = ItemStackCodec.Read(reader, ItemTypes);

        Assert.Multiple(() =>
        {
            Assert.That(stack.ItemId, Is.EqualTo(10));
            Assert.That(stack.Extra, Is.Zero);
            Assert.That(reader.Remaining, Is.EqualTo(1));
        });
    }

    [Test]
    public void Write_WithStackableItem_EmitsCountByte()
    {
        var writer = new PacketWriter();

        ItemStackCodec.Write(writer, new ItemStack(100, 7), ItemTypes);

        Assert.That(writer.ToArray(), Is.EqualTo(new byte[] { 100, 0, 7 }));
    }

    [Test]
    public void Write_WithPlainItem_EmitsOnlyTheIdentifier()
    {
        var writer = new PacketWriter();

        ItemStackCodec.Write(writer, new ItemStack(10, 7), ItemTypes);

        Assert.That(writer.ToArray(), Is.EqualTo(new byte[] { 10, 0 }));
    }

    [Test]
    public void ToString_DescribesTheStack()
    {
        Assert.That(new ItemStack(5, 3).ToString(), Is.EqualTo("item 5 x3"));
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => ItemStackCodec.Read(null!, ItemTypes));
    }

    [Test]
    public void Read_WithNullItemTypes_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => ItemStackCodec.Read(new PacketReader(new byte[2]), null!));
    }

    [Test]
    public void Write_WithNullWriter_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => ItemStackCodec.Write(null!, new ItemStack(1, 0), ItemTypes));
    }

    [Test]
    public void Write_WithNullStack_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => ItemStackCodec.Write(new PacketWriter(), null!, ItemTypes));
    }

    [Test]
    public void Write_WithNullItemTypes_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () => ItemStackCodec.Write(new PacketWriter(), new ItemStack(1, 0), null!));
    }
}
