// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace OpenTibia.LiveHarness;

/// <summary>
/// Renders decoded messages as one readable line each.
/// </summary>
public static class MessageRenderer
{
    private const int MaxSequenceItems = 6;

    public static string NameOf(byte opcode)
    {
        return Enum.IsDefined(typeof(GameServerOpcode), (GameServerOpcode)opcode)
            ? ((GameServerOpcode)opcode).ToString()
            : $"Unknown_0x{opcode:X2}";
    }

    public static string Describe(IProtocolMessage message)
    {
        var fields = message.GetType()
            .GetProperties()
            .Where(property => property.Name != nameof(IProtocolMessage.Opcode)
                               && property.GetIndexParameters().Length == 0)
            .Select(property => $"{property.Name}={Render(Safe(property, message))}")
            .ToList();

        var head = $"0x{message.Opcode:X2} {NameOf(message.Opcode)}";
        return fields.Count == 0 ? head : head + "  " + string.Join(" ", fields);
    }

    private static object? Safe(PropertyInfo property, IProtocolMessage message)
    {
        try
        {
            return property.GetValue(message, null);
        }
        catch (Exception exception)
        {
            return $"<threw {exception.GetType().Name}>";
        }
    }

    private static string Render(object? value)
    {
        if (value == null)
        {
            return "null";
        }

        if (value is string text)
        {
            return "\"" + text + "\"";
        }

        if (value is IEnumerable sequence)
        {
            var items = sequence.Cast<object?>().Take(MaxSequenceItems).Select(Render).ToList();
            var suffix = items.Count == MaxSequenceItems ? ",..." : string.Empty;
            return "[" + string.Join(",", items) + suffix + "]";
        }

        return Convert.ToString(value, CultureInfo.InvariantCulture) ?? "?";
    }

    public static string Hex(byte[] bytes)
    {
        var hex = new StringBuilder(bytes.Length * 3);
        foreach (var value in bytes)
        {
            hex.Append(value.ToString("X2")).Append(' ');
        }

        return hex.ToString().TrimEnd();
    }

    /// <summary>
    /// Groups a capture by opcode so a long run can be summarised.
    /// </summary>
    public static IEnumerable<string> Histogram(IEnumerable<IProtocolMessage> messages)
    {
        return messages
            .GroupBy(message => message.Opcode)
            .OrderByDescending(group => group.Count())
            .Select(group => $"  0x{group.Key:X2} {NameOf(group.Key),-28} {group.Count(),6}");
    }
}
