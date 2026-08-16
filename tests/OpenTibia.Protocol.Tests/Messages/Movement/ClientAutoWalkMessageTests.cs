// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Movement;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Movement;

[TestFixture]
public class ClientAutoWalkMessageTests
{
    [Test]
    public void Opcode_IsAutoWalk()
    {
        var message = new ClientAutoWalkMessage(new[] { Direction.East });

        Assert.That(message.Opcode, Is.EqualTo((byte)ClientOpcode.AutoWalk));
    }

    [Test]
    public void Write_WithSingleStep_EmitsCountAndStepByte()
    {
        var writer = new PacketWriter();

        new ClientAutoWalkMessage(new[] { Direction.East }).Write(writer);

        Assert.That(writer.ToArray(), Is.EqualTo(new byte[] { (byte)ClientOpcode.AutoWalk, 1, 1 }));
    }

    [Test]
    public void Write_WithEveryDirection_EncodesEachStepAsPathByte()
    {
        var writer = new PacketWriter();
        var path = new[]
        {
            Direction.North, Direction.East, Direction.South, Direction.West,
            Direction.NorthEast, Direction.SouthEast, Direction.SouthWest, Direction.NorthWest
        };

        new ClientAutoWalkMessage(path).Write(writer);

        Assert.That(writer.ToArray(), Is.EqualTo(new byte[]
        {
            (byte)ClientOpcode.AutoWalk, 8,
            3, 1, 7, 5, 2, 8, 6, 4
        }));
    }

    [Test]
    public void Write_WithMaximumPathLength_EmitsCountByteAs255()
    {
        var writer = new PacketWriter();
        var path = new Direction[255];

        new ClientAutoWalkMessage(path).Write(writer);

        Assert.Multiple(() =>
        {
            Assert.That(writer.ToArray()[1], Is.EqualTo(255));
            Assert.That(writer.Length, Is.EqualTo(2 + 255));
        });
    }

    [Test]
    public void Constructor_WithNullPath_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ClientAutoWalkMessage(null!));
    }

    [Test]
    public void Constructor_WithEmptyPath_ThrowsProtocolException()
    {
        Assert.Throws<ProtocolException>(() => new ClientAutoWalkMessage(Array.Empty<Direction>()));
    }

    [Test]
    public void Constructor_WithPathLongerThan255_ThrowsProtocolException()
    {
        Assert.Throws<ProtocolException>(() => new ClientAutoWalkMessage(new Direction[256]));
    }

    [Test]
    public void Constructor_KeepsPath()
    {
        var path = new List<Direction> { Direction.North, Direction.South };

        var message = new ClientAutoWalkMessage(path);

        Assert.That(message.Path, Is.EqualTo(path));
    }
}
