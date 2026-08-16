// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Services;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Services;

[TestFixture]
public class RangeGuardTests
{
    [Test]
    public void ToUInt16_WithValueInRange_Converts()
    {
        Assert.That(RangeGuard.ToUInt16(100, "p"), Is.EqualTo((ushort)100));
    }

    [Test]
    public void ToUInt16_BelowMinimum_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => RangeGuard.ToUInt16(-1, "p"));
    }

    [Test]
    public void ToUInt16_AboveMaximum_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => RangeGuard.ToUInt16(ushort.MaxValue + 1, "p"));
    }

    [Test]
    public void ToByte_WithValueInRange_Converts()
    {
        Assert.That(RangeGuard.ToByte(200, "p"), Is.EqualTo((byte)200));
    }

    [Test]
    public void ToByte_BelowMinimum_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => RangeGuard.ToByte(-1, "p"));
    }

    [Test]
    public void ToByte_AboveMaximum_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => RangeGuard.ToByte(byte.MaxValue + 1, "p"));
    }

    [Test]
    public void ToUInt32_WithNonNegativeValue_Converts()
    {
        Assert.That(RangeGuard.ToUInt32(42, "p"), Is.EqualTo(42u));
    }

    [Test]
    public void ToUInt32_WithNegativeValue_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => RangeGuard.ToUInt32(-1, "p"));
    }
}
