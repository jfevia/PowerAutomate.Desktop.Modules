// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Types;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Types;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Services;

/// <summary>
/// Converts decoded protocol messages into the variable shapes a desktop flow can consume.
/// </summary>
public static class ServerMessageProjector
{
    public const string OpcodeColumn = "Opcode";

    public const string NameColumn = "Name";

    public const string ReceivedAtColumn = "ReceivedAt";

    public const string PayloadColumn = "Payload";

    public static string ResolveName(byte opcode)
    {
        var value = (GameServerOpcode)opcode;
        return Enum.IsDefined(typeof(GameServerOpcode), value)
            ? value.ToString()
            : $"Unknown_0x{opcode:X2}";
    }

    public static CustomObject ToPayload(IProtocolMessage message)
    {
        if (message == null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        var payload = new CustomObject();

        foreach (var property in message.GetType().GetProperties())
        {
            if (property.Name == nameof(IProtocolMessage.Opcode) || property.GetIndexParameters().Length > 0)
            {
                continue;
            }

            payload.AddOrUpdateProperty(property.Name, Flatten(property.GetValue(message, null)));
        }

        return payload;
    }

    public static TibiaServerMessage ToServerMessage(IProtocolMessage message, DateTime receivedAt)
    {
        if (message == null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        return new TibiaServerMessage(message.Opcode, ResolveName(message.Opcode), receivedAt, ToPayload(message));
    }

    public static DataTable ToDataTable(IEnumerable<TibiaServerMessage> messages)
    {
        if (messages == null)
        {
            throw new ArgumentNullException(nameof(messages));
        }

        var table = new DataTable("ServerMessages");
        table.Columns.Add(OpcodeColumn, typeof(int));
        table.Columns.Add(NameColumn, typeof(string));
        table.Columns.Add(ReceivedAtColumn, typeof(DateTime));
        table.Columns.Add(PayloadColumn, typeof(CustomObject));

        foreach (var message in messages)
        {
            table.Rows.Add(message.Opcode, message.Name, message.ReceivedAt, message.Payload);
        }

        return table;
    }

    /// <summary>
    /// Reduces protocol values to something the variables pane can render.
    /// </summary>
    private static object Flatten(object? value)
    {
        if (value == null)
        {
            return string.Empty;
        }

        if (value is string || value.GetType().IsPrimitive)
        {
            return value;
        }

        if (value is Enum)
        {
            return value.ToString()!;
        }

        if (value is IEnumerable sequence)
        {
            var items = new List<object>();
            foreach (var item in sequence)
            {
                items.Add(Flatten(item));
            }

            return items;
        }

        return Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
    }
}
