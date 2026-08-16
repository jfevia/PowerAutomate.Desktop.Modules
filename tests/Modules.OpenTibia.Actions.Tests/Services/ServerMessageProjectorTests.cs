// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Types;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Services;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Types;
using PowerAutomate.Desktop.OpenTibia.Protocol.Geometry;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Movement;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Services;

/// <summary>
/// Exercises every value shape the projector has to flatten.
/// </summary>
internal sealed class ProbeMessage : IProtocolMessage
{
    public byte Opcode => (byte)GameServerOpcode.TextMessage;

    public string Text { get; set; } = "hello";

    public int Level { get; set; } = 7;

    public Direction Facing { get; set; } = Direction.NorthEast;

    public IReadOnlyList<int> Values { get; set; } = new[] { 1, 2 };

    public Position Where { get; set; } = new Position(1, 2, 3);

    public NullRenderer Silent { get; set; } = new NullRenderer();

    public string? Missing { get; set; }

    public string this[int index] => "indexer";
}

/// <summary>
/// Renders as null, which is the only way to reach the projector null-coalescing fallback.
/// </summary>
internal sealed class NullRenderer
{
    public override string ToString()
    {
        return null!;
    }
}

[TestFixture]
public class ServerMessageProjectorTests
{
    private static readonly DateTime Timestamp = new DateTime(2026, 8, 16, 1, 2, 3, DateTimeKind.Utc);

    [Test]
    public void ResolveName_WithKnownOpcode_ReturnsEnumName()
    {
        Assert.That(
            ServerMessageProjector.ResolveName((byte)GameServerOpcode.PlayerData),
            Is.EqualTo(nameof(GameServerOpcode.PlayerData)));
    }

    [Test]
    public void ResolveName_WithUnknownOpcode_ReturnsHexFallback()
    {
        Assert.That(ServerMessageProjector.ResolveName(0xFE), Is.EqualTo("Unknown_0xFE"));
    }

    [Test]
    public void ToPayload_FlattensEveryValueShape()
    {
        var payload = ServerMessageProjector.ToPayload(new ProbeMessage());

        Assert.Multiple(() =>
        {
            Assert.That(payload.GetProperty("Text"), Is.EqualTo("hello"));
            Assert.That(payload.GetProperty("Level"), Is.EqualTo(7));
            Assert.That(payload.GetProperty("Facing"), Is.EqualTo("NorthEast"));
            Assert.That(payload.GetProperty("Values"), Is.EqualTo(new List<object> { 1, 2 }));
            Assert.That(payload.GetProperty("Where"), Is.EqualTo("(1, 2, 3)"));
            Assert.That(payload.GetProperty("Silent"), Is.EqualTo(string.Empty));
            Assert.That(payload.GetProperty("Missing"), Is.EqualTo(string.Empty));
        });
    }

    [Test]
    public void ToPayload_OmitsOpcodeAndIndexers()
    {
        var payload = ServerMessageProjector.ToPayload(new ProbeMessage());

        Assert.Multiple(() =>
        {
            Assert.That(payload.HasProperty("Opcode"), Is.False);
            Assert.That(payload.HasProperty("Item"), Is.False);
        });
    }

    [Test]
    public void ToServerMessage_CarriesOpcodeNameAndTimestamp()
    {
        var message = ServerMessageProjector.ToServerMessage(new ProbeMessage(), Timestamp);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((int)GameServerOpcode.TextMessage));
            Assert.That(message.Name, Is.EqualTo(nameof(GameServerOpcode.TextMessage)));
            Assert.That(message.ReceivedAt, Is.EqualTo(Timestamp));
        });
    }

    [Test]
    public void ToDataTable_BuildsTheDocumentedColumns()
    {
        var messages = new[] { ServerMessageProjector.ToServerMessage(new ProbeMessage(), Timestamp) };

        var table = ServerMessageProjector.ToDataTable(messages);

        Assert.Multiple(() =>
        {
            Assert.That(table.Columns.Cast<DataColumn>().Select(column => column.ColumnName),
                Is.EqualTo(new[] { "Opcode", "Name", "ReceivedAt", "Payload" }));
            Assert.That(table.Rows, Has.Count.EqualTo(1));
            Assert.That(table.Rows[0][ServerMessageProjector.NameColumn],
                Is.EqualTo(nameof(GameServerOpcode.TextMessage)));
            Assert.That(table.Rows[0][ServerMessageProjector.PayloadColumn], Is.InstanceOf<CustomObject>());
        });
    }

    [Test]
    public void ToDataTable_WithNoMessages_ReturnsEmptyTable()
    {
        Assert.That(ServerMessageProjector.ToDataTable(Array.Empty<TibiaServerMessage>()).Rows, Is.Empty);
    }

    [Test]
    public void ToPayload_WithNullMessage_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => ServerMessageProjector.ToPayload(null!));
    }

    [Test]
    public void ToServerMessage_WithNullMessage_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => ServerMessageProjector.ToServerMessage(null!, Timestamp));
    }

    [Test]
    public void ToDataTable_WithNullSequence_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => ServerMessageProjector.ToDataTable(null!));
    }
}
