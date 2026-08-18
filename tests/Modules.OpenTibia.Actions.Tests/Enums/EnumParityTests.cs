// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Linq;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Enums;
using ProtocolDirection = PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Movement.Direction;
using ProtocolSpeakType = PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Chat.SpeakType;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Enums;

[TestFixture]
public class EnumParityTests
{
    [Test]
    public void Direction_MirrorsTheProtocolEnumExactly()
    {
        AssertParity(typeof(Direction), typeof(ProtocolDirection));
    }

    [Test]
    public void SpeakType_MirrorsTheProtocolEnumExactly()
    {
        AssertParity(typeof(SpeakType), typeof(ProtocolSpeakType));
    }

    [Test]
    public void ToProtocol_MapsEveryDirection()
    {
        foreach (Direction direction in Enum.GetValues(typeof(Direction)))
        {
            Assert.That(direction.ToProtocol().ToString(), Is.EqualTo(direction.ToString()));
        }
    }

    [Test]
    public void ToProtocol_MapsEverySpeakType()
    {
        foreach (SpeakType speakType in Enum.GetValues(typeof(SpeakType)))
        {
            Assert.That(speakType.ToProtocol().ToString(), Is.EqualTo(speakType.ToString()));
        }
    }

    [Test]
    public void ToProtocol_MapsASequenceInOrder()
    {
        var mapped = new[] { Direction.NorthWest, Direction.East }.ToProtocol();

        Assert.That(mapped, Is.EqualTo(new[] { ProtocolDirection.NorthWest, ProtocolDirection.East }));
    }

    /// <summary>
    /// The module declares its own enums so the designer can resolve literals; they must not drift.
    /// </summary>
    private static void AssertParity(Type moduleEnum, Type protocolEnum)
    {
        var moduleValues = Enum.GetNames(moduleEnum).OrderBy(name => name, StringComparer.Ordinal).ToList();
        var protocolValues = Enum.GetNames(protocolEnum).OrderBy(name => name, StringComparer.Ordinal).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(moduleValues, Is.EqualTo(protocolValues), $"{moduleEnum.Name} names differ from the protocol enum.");
            Assert.That(Enum.GetUnderlyingType(moduleEnum), Is.EqualTo(typeof(int)),
                $"{moduleEnum.Name} must be backed by int. The module loader reads values with (int)Enum.Parse and "
                + "silently drops the enum when the underlying type is narrower, which leaves the designer unable to "
                + "resolve a literal for it.");

            foreach (var name in moduleValues)
            {
                var moduleValue = Convert.ToInt64(Enum.Parse(moduleEnum, name));
                var protocolValue = Convert.ToInt64(Enum.Parse(protocolEnum, name));
                Assert.That(moduleValue, Is.EqualTo(protocolValue), $"{moduleEnum.Name}.{name} has a different value.");
            }
        });
    }
}
