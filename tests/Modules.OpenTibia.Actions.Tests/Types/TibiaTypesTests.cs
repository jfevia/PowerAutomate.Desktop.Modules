// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Types;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Types;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Types;

[TestFixture]
public class TibiaCharacterTests
{
    [Test]
    public void Constructor_KeepsEveryField()
    {
        var character = new TibiaCharacter("Rook", "Antica", "10.0.0.1", 7172);

        Assert.Multiple(() =>
        {
            Assert.That(character.Name, Is.EqualTo("Rook"));
            Assert.That(character.World, Is.EqualTo("Antica"));
            Assert.That(character.Host, Is.EqualTo("10.0.0.1"));
            Assert.That(character.Port, Is.EqualTo(7172));
        });
    }

    [Test]
    public void ToString_DescribesTheCharacter()
    {
        Assert.That(
            new TibiaCharacter("Rook", "Antica", "10.0.0.1", 7172).ToString(),
            Is.EqualTo("Rook @ Antica (10.0.0.1:7172)"));
    }

    [Test]
    public void Constructor_WithNullName_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new TibiaCharacter(null!, "w", "h", 1));
    }

    [Test]
    public void Constructor_WithNullWorld_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new TibiaCharacter("n", null!, "h", 1));
    }

    [Test]
    public void Constructor_WithNullHost_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new TibiaCharacter("n", "w", null!, 1));
    }
}

[TestFixture]
public class TibiaServerMessageTests
{
    private static readonly DateTime Timestamp = new DateTime(2026, 8, 16, 0, 0, 0, DateTimeKind.Utc);

    [Test]
    public void Constructor_KeepsEveryField()
    {
        var payload = new CustomObject();

        var message = new TibiaServerMessage(0xB4, "TextMessage", Timestamp, payload);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo(0xB4));
            Assert.That(message.Name, Is.EqualTo("TextMessage"));
            Assert.That(message.ReceivedAt, Is.EqualTo(Timestamp));
            Assert.That(message.Payload, Is.SameAs(payload));
        });
    }

    [Test]
    public void ToString_RendersNameAndHexOpcode()
    {
        Assert.That(
            new TibiaServerMessage(0xB4, "TextMessage", Timestamp, new CustomObject()).ToString(),
            Is.EqualTo("TextMessage (0xB4)"));
    }

    [Test]
    public void Constructor_WithNullName_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () => new TibiaServerMessage(1, null!, Timestamp, new CustomObject()));
    }

    [Test]
    public void Constructor_WithNullPayload_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new TibiaServerMessage(1, "n", Timestamp, null!));
    }
}
