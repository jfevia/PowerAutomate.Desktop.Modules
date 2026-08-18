// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Enums;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Services;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Services;

[TestFixture]
public class DirectionPathTests
{
    [Test]
    public void Parse_WithShortAliases_ReturnsThePath()
    {
        var path = DirectionPath.Parse("n,n,e,s,s,w");

        Assert.That(path, Is.EqualTo(new[]
        {
            Direction.North, Direction.North, Direction.East,
            Direction.South, Direction.South, Direction.West
        }));
    }

    [Test]
    public void Parse_WithDiagonalAliases_ReturnsThePath()
    {
        var path = DirectionPath.Parse("ne,se,sw,nw");

        Assert.That(path, Is.EqualTo(new[]
        {
            Direction.NorthEast, Direction.SouthEast, Direction.SouthWest, Direction.NorthWest
        }));
    }

    [Test]
    public void Parse_WithFullNames_IgnoresCase()
    {
        var path = DirectionPath.Parse("north, SOUTHeast");

        Assert.That(path, Is.EqualTo(new[] { Direction.North, Direction.SouthEast }));
    }

    [Test]
    public void Parse_WithSpacesAndTabs_SplitsOnThemToo()
    {
        var path = DirectionPath.Parse("n e\ts");

        Assert.That(path, Is.EqualTo(new[] { Direction.North, Direction.East, Direction.South }));
    }

    [Test]
    public void Parse_WithNull_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => DirectionPath.Parse(null!));
    }

    [Test]
    public void Parse_WithNoTokens_Throws()
    {
        var exception = Assert.Throws<ArgumentException>(() => DirectionPath.Parse("  "))!;

        Assert.That(exception.Message, Does.Contain("At least one direction"));
    }

    [Test]
    public void Parse_WithAnUnknownToken_NamesTheAcceptedValues()
    {
        var exception = Assert.Throws<ArgumentException>(() => DirectionPath.Parse("n,up"))!;

        Assert.That(exception.Message, Does.Contain("'up' is not a direction"));
    }

    [Test]
    public void Parse_WithANumericValue_IsRejected()
    {
        // Enum.TryParse accepts numbers, so an out of range number must not slip through.
        Assert.Throws<ArgumentException>(() => DirectionPath.Parse("99"));
    }
}
