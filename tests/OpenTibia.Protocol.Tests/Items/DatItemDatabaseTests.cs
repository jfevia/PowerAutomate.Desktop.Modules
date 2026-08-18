// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Items;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Items;

[TestFixture]
public class DatItemDatabaseTests
{
    private const byte AttributeGround = 0;
    private const byte AttributeStackable = 5;
    private const byte AttributeFluidContainer = 10;
    private const byte AttributeSplash = 11;
    private const byte AttributeLight = 21;
    private const byte AttributeMarket = 33;
    private const byte AttributeLast = 255;

    [Test]
    public void Parse_WithFlaggedThings_ClassifiesByClientId()
    {
        var bytes = new DatBuilder(103)
            .Thing()
            .Thing(AttributeStackable)
            .Thing(AttributeFluidContainer)
            .Thing(AttributeSplash)
            .Build();

        var database = DatItemDatabase.Parse(bytes);

        Assert.Multiple(() =>
        {
            Assert.That(database.Count, Is.EqualTo(4));
            Assert.That(database.Signature, Is.EqualTo(0x4C28B721u));
            Assert.That(database.IsStackable(101), Is.True);
            Assert.That(database.IsFluidContainer(102), Is.True);
            Assert.That(database.IsSplash(103), Is.True);
            Assert.That(database.IsStackable(100), Is.False);
            Assert.That(database.StackableCount, Is.EqualTo(1));
            Assert.That(database.FluidContainerCount, Is.EqualTo(1));
            Assert.That(database.SplashCount, Is.EqualTo(1));
        });
    }

    [Test]
    public void Parse_WithDataCarryingAttributes_StaysAligned()
    {
        // A ground speed, a light and a market block all precede the flag that matters.
        var bytes = new DatBuilder(101)
            .Thing(builder => builder
                .Attribute(AttributeGround, 0x64, 0x00)
                .Attribute(AttributeLight, 0x08, 0x00, 0xD0, 0x00)
                .Attribute(AttributeStackable))
            .Thing(builder => builder.MarketAttribute("gold coin"))
            .Build();

        var database = DatItemDatabase.Parse(bytes);

        Assert.Multiple(() =>
        {
            Assert.That(database.IsStackable(100), Is.True);
            Assert.That(database.IsStackable(101), Is.False);
            Assert.That(database.Count, Is.EqualTo(2));
        });
    }

    [Test]
    public void Parse_WithEveryDataCarryingAttribute_StaysAligned()
    {
        var bytes = new DatBuilder(100)
            .Thing(builder => builder
                .Attribute(8, 0x10, 0x00)
                .Attribute(9, 0x20, 0x00)
                .Attribute(24, 0x08, 0x00, 0x08, 0x00)
                .Attribute(25, 0x02, 0x00)
                .Attribute(28, 0x1E, 0x00)
                .Attribute(29, 0x00, 0x00)
                .Attribute(32, 0x03, 0x00)
                .Attribute(251, 0x01, 0x00)
                .Attribute(AttributeStackable))
            .Build();

        var database = DatItemDatabase.Parse(bytes);

        Assert.That(database.IsStackable(100), Is.True);
    }

    [Test]
    public void Parse_WithTallSprite_ConsumesTheExactSizeByte()
    {
        // Width stays 1 so the second half of the "width > 1 || height > 1" test is exercised.
        var bytes = new DatBuilder(100).ThingWithSprite(1, 2, 2).Build();

        var database = DatItemDatabase.Parse(bytes);

        Assert.That(database.Count, Is.EqualTo(1));
    }

    [Test]
    public void Parse_WithMultiTileSprite_ConsumesTheExactSizeByte()
    {
        var bytes = new DatBuilder(100).ThingWithSprite(2, 2, 4).Build();

        var database = DatItemDatabase.Parse(bytes);

        Assert.That(database.Count, Is.EqualTo(1));
    }

    [Test]
    public void Lookup_ForUnknownId_ReturnsNullAndFalseFlags()
    {
        var database = DatItemDatabase.Parse(new DatBuilder(100).Thing().Build());

        Assert.Multiple(() =>
        {
            Assert.That(database.Lookup(9999), Is.Null);
            Assert.That(database.IsStackable(9999), Is.False);
            Assert.That(database.IsFluidContainer(9999), Is.False);
            Assert.That(database.IsSplash(9999), Is.False);
        });
    }

    [Test]
    public void HasSubTypeByte_IsTrueForEveryClassification()
    {
        Assert.Multiple(() =>
        {
            Assert.That(new DatItemFlags(true, false, false).HasSubTypeByte, Is.True);
            Assert.That(new DatItemFlags(false, true, false).HasSubTypeByte, Is.True);
            Assert.That(new DatItemFlags(false, false, true).HasSubTypeByte, Is.True);
            Assert.That(new DatItemFlags(false, false, false).HasSubTypeByte, Is.False);
        });
    }

    [Test]
    public void Parse_WithImpossibleSpriteCount_Throws()
    {
        // Every sprite dimension at its maximum overflows any sane buffer length.
        var bytes = new List<byte> { 0x21, 0xB7, 0x28, 0x4C, 100, 0, 0, 0, 0, 0, 0, 0, 255 };
        bytes.AddRange(new byte[] { 255, 255, 32, 255, 255, 255, 255, 255 });

        var exception = Assert.Throws<InvalidDataException>(() => DatItemDatabase.Parse(bytes.ToArray()))!;

        Assert.That(exception.Message, Does.Contain("impossible sprite count"));
    }

    [Test]
    public void Parse_WithTooFewThings_Throws()
    {
        var bytes = new List<byte> { 0x21, 0xB7, 0x28, 0x4C, 0x0A, 0x00, 0, 0, 0, 0, 0, 0 };

        var exception = Assert.Throws<InvalidDataException>(() => DatItemDatabase.Parse(bytes.ToArray()))!;

        Assert.That(exception.Message, Does.Contain("fewer than the first item id"));
    }

    [Test]
    public void Parse_WhenTruncated_Throws()
    {
        var full = new DatBuilder(101).Thing().Thing().Build();
        var truncated = new byte[full.Length - 3];
        Array.Copy(full, truncated, truncated.Length);

        var exception = Assert.Throws<InvalidDataException>(() => DatItemDatabase.Parse(truncated))!;

        Assert.That(exception.Message, Does.Contain("Truncated Tibia.dat"));
    }

    [Test]
    public void Parse_WithNullData_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => DatItemDatabase.Parse(null!));
    }

    [Test]
    public void Load_WithNullPath_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => DatItemDatabase.Load(null!));
    }

    [Test]
    public void Load_ReadsFromDisk()
    {
        var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".dat");
        File.WriteAllBytes(path, new DatBuilder(100).Thing(AttributeStackable).Build());
        try
        {
            var database = DatItemDatabase.Load(path);

            Assert.That(database.IsStackable(100), Is.True);
        }
        finally
        {
            File.Delete(path);
        }
    }

    private sealed class DatBuilder
    {
        private readonly List<byte> _bytes = new List<byte>();
        private readonly List<byte> _things = new List<byte>();
        private readonly ushort _lastItemId;

        public DatBuilder(ushort lastItemId)
        {
            _lastItemId = lastItemId;
        }

        public DatBuilder Thing(params byte[] flagAttributes)
        {
            foreach (var attribute in flagAttributes)
            {
                _things.Add(attribute);
            }

            _things.Add(AttributeLast);
            AddSprite(1, 1, 1);
            return this;
        }

        public DatBuilder Thing(Func<AttributeWriter, AttributeWriter> configure)
        {
            var writer = configure(new AttributeWriter());
            _things.AddRange(writer.Bytes);
            _things.Add(AttributeLast);
            AddSprite(1, 1, 1);
            return this;
        }

        public DatBuilder ThingWithSprite(byte width, byte height, int spriteCount)
        {
            _things.Add(AttributeLast);
            _things.Add(width);
            _things.Add(height);
            _things.Add(32);
            _things.AddRange(new byte[] { 1, 1, 1, 1, 1 });
            for (var i = 0; i < spriteCount; i++)
            {
                _things.Add(0);
                _things.Add(0);
            }

            return this;
        }

        public byte[] Build()
        {
            _bytes.Clear();
            _bytes.AddRange(new byte[] { 0x21, 0xB7, 0x28, 0x4C });
            AddUInt16(_bytes, _lastItemId);
            AddUInt16(_bytes, 0);
            AddUInt16(_bytes, 0);
            AddUInt16(_bytes, 0);
            _bytes.AddRange(_things);
            return _bytes.ToArray();
        }

        private void AddSprite(byte layers, byte patterns, byte phases)
        {
            _things.AddRange(new byte[] { 1, 1, layers, patterns, patterns, patterns, phases });
            _things.Add(0);
            _things.Add(0);
        }

        private static void AddUInt16(ICollection<byte> bytes, ushort value)
        {
            bytes.Add((byte)(value & 0xFF));
            bytes.Add((byte)(value >> 8));
        }
    }

    private sealed class AttributeWriter
    {
        public List<byte> Bytes { get; } = new List<byte>();

        public AttributeWriter Attribute(byte id, params byte[] payload)
        {
            Bytes.Add(id);
            Bytes.AddRange(payload);
            return this;
        }

        public AttributeWriter MarketAttribute(string name)
        {
            Bytes.Add(AttributeMarket);
            Bytes.AddRange(new byte[] { 0, 0, 0, 0, 0, 0 });
            Bytes.Add((byte)(name.Length & 0xFF));
            Bytes.Add((byte)(name.Length >> 8));
            foreach (var character in name)
            {
                Bytes.Add((byte)character);
            }

            Bytes.AddRange(new byte[] { 0, 0, 0, 0 });
            return this;
        }
    }
}
