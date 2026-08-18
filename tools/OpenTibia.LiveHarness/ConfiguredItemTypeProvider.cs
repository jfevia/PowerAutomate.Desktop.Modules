// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
using PowerAutomate.Desktop.OpenTibia.Protocol.Items;

namespace OpenTibia.LiveHarness;

/// <summary>
/// Item classification loaded from server data files, so map tiles decode correctly.
/// </summary>
/// <remarks>
/// Without this an unclassified stackable item consumes the wrong byte count and the map parse desynchronizes.
/// </remarks>
public sealed class ConfiguredItemTypeProvider : IItemTypeProvider
{
    private readonly HashSet<ushort> _stackable = new HashSet<ushort>();
    private readonly HashSet<ushort> _fluids = new HashSet<ushort>();
    private readonly HashSet<ushort> _splashes = new HashSet<ushort>();

    public int StackableCount => _stackable.Count;

    public int FluidCount => _fluids.Count;

    public int SplashCount => _splashes.Count;

    public bool IsEmpty => _stackable.Count == 0 && _fluids.Count == 0 && _splashes.Count == 0;

    public bool IsStackable(ushort itemId) => _stackable.Contains(itemId);

    public bool IsFluidContainer(ushort itemId) => _fluids.Contains(itemId);

    public bool IsSplash(ushort itemId) => _splashes.Contains(itemId);

    /// <summary>
    /// Loads a plain id list, accepting single values and inclusive ranges such as 2148-2160.
    /// </summary>
    public void LoadIdList(string path, string kind)
    {
        var target = Resolve(kind);

        foreach (var raw in File.ReadAllLines(path))
        {
            var line = raw.Trim();
            if (line.Length == 0 || line.StartsWith("#", StringComparison.Ordinal))
            {
                continue;
            }

            foreach (var token in line.Split(new[] { ',', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries))
            {
                AddToken(target, token);
            }
        }
    }

    /// <summary>
    /// Reads a TFS items.otb, classifying by the client id the map description actually carries.
    /// </summary>
    public void LoadItemsOtb(string path)
    {
        var items = OtbItemDatabase.Load(path);

        foreach (var item in items.Values)
        {
            if (item.IsStackable)
            {
                _stackable.Add(item.ClientId);
            }

            if (item.IsFluidContainer)
            {
                _fluids.Add(item.ClientId);
            }

            if (item.IsSplash)
            {
                _splashes.Add(item.ClientId);
            }
        }
    }

    /// <summary>
    /// Reads a TFS items.xml, picking up whichever classification attributes it carries.
    /// </summary>
    public void LoadItemsXml(string path)
    {
        var document = XDocument.Load(path);

        foreach (var item in document.Descendants("item"))
        {
            foreach (var id in ItemIds(item))
            {
                foreach (var attribute in item.Elements("attribute"))
                {
                    Classify(id, attribute);
                }

                var type = (string?)item.Attribute("type");
                if (string.Equals(type, "fluidcontainer", StringComparison.OrdinalIgnoreCase))
                {
                    _fluids.Add(id);
                }
                else if (string.Equals(type, "splash", StringComparison.OrdinalIgnoreCase))
                {
                    _splashes.Add(id);
                }
            }
        }
    }

    private void Classify(ushort id, XElement attribute)
    {
        var key = (string?)attribute.Attribute("key");
        var value = (string?)attribute.Attribute("value");

        if (key == null || value == null)
        {
            return;
        }

        var enabled = value == "1" || string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);

        if (string.Equals(key, "stackable", StringComparison.OrdinalIgnoreCase) && enabled)
        {
            _stackable.Add(id);
        }
        else if (string.Equals(key, "type", StringComparison.OrdinalIgnoreCase))
        {
            if (string.Equals(value, "fluidcontainer", StringComparison.OrdinalIgnoreCase))
            {
                _fluids.Add(id);
            }
            else if (string.Equals(value, "splash", StringComparison.OrdinalIgnoreCase))
            {
                _splashes.Add(id);
            }
        }
    }

    private static IEnumerable<ushort> ItemIds(XElement item)
    {
        var single = (string?)item.Attribute("id");
        if (single != null && ushort.TryParse(single, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id))
        {
            yield return id;
            yield break;
        }

        var from = (string?)item.Attribute("fromid");
        var to = (string?)item.Attribute("toid");
        if (from == null || to == null
            || !ushort.TryParse(from, NumberStyles.Integer, CultureInfo.InvariantCulture, out var first)
            || !ushort.TryParse(to, NumberStyles.Integer, CultureInfo.InvariantCulture, out var last))
        {
            yield break;
        }

        for (var value = first; value <= last; value++)
        {
            yield return value;
        }
    }

    private HashSet<ushort> Resolve(string kind)
    {
        switch (kind.ToLowerInvariant())
        {
            case "stackable":
                return _stackable;
            case "fluid":
                return _fluids;
            case "splash":
                return _splashes;
            default:
                throw new ArgumentException($"Unknown item kind '{kind}'.", nameof(kind));
        }
    }

    private static void AddToken(ISet<ushort> target, string token)
    {
        var dash = token.IndexOf('-');
        if (dash > 0)
        {
            if (ushort.TryParse(token.Substring(0, dash), out var first)
                && ushort.TryParse(token.Substring(dash + 1), out var last))
            {
                for (var value = first; value <= last; value++)
                {
                    target.Add(value);
                }
            }

            return;
        }

        if (ushort.TryParse(token, out var single))
        {
            target.Add(single);
        }
    }
}
