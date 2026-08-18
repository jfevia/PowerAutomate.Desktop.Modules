// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Enums;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Services;

/// <summary>
/// Parses a comma separated walking path such as n,n,e,s,s,w.
/// </summary>
public static class DirectionPath
{
    private static readonly Dictionary<string, Direction> Aliases =
        new Dictionary<string, Direction>(StringComparer.OrdinalIgnoreCase)
        {
            { "n", Direction.North },
            { "e", Direction.East },
            { "s", Direction.South },
            { "w", Direction.West },
            { "ne", Direction.NorthEast },
            { "se", Direction.SouthEast },
            { "sw", Direction.SouthWest },
            { "nw", Direction.NorthWest }
        };

    /// <summary>
    /// Accepts short aliases and full enum names; throws with the accepted set on anything else.
    /// </summary>
    public static IReadOnlyList<Direction> Parse(string path)
    {
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        var tokens = path.Split(new[] { ',', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        var directions = new List<Direction>(tokens.Length);

        foreach (var raw in tokens)
        {
            var token = raw.Trim();
            if (Aliases.TryGetValue(token, out var alias))
            {
                directions.Add(alias);
                continue;
            }

            if (Enum.TryParse<Direction>(token, true, out var parsed) && Enum.IsDefined(typeof(Direction), parsed))
            {
                directions.Add(parsed);
                continue;
            }

            throw new ArgumentException(
                $"'{token}' is not a direction. Use {string.Join(", ", Aliases.Keys)} or a full name such as "
                + string.Join(", ", Enum.GetNames(typeof(Direction)).Take(4)) + ".",
                nameof(path));
        }

        if (directions.Count == 0)
        {
            throw new ArgumentException("At least one direction is required.", nameof(path));
        }

        return directions;
    }
}
