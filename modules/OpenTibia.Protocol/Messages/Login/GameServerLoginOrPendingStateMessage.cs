// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Login;

/// <summary>
/// The game-server's successful login (or queue-pending) confirmation, sent as the character enters the world.
/// </summary>
public sealed class GameServerLoginOrPendingStateMessage : IProtocolMessage
{
    /// <summary>
    /// Marker byte preceding an optional 20-byte violation reason flag block.
    /// </summary>
    public const byte ViolationReasonMarker = 0x0B;

    /// <summary>
    /// The fixed length of the violation reason flag block.
    /// </summary>
    public const int ViolationReasonFlagCount = 20;

    public GameServerLoginOrPendingStateMessage(
        uint playerId,
        ushort serverBeat,
        bool isBugReportingAllowed,
        IReadOnlyList<byte>? violationReasonFlags)
    {
        if (violationReasonFlags != null && violationReasonFlags.Count != ViolationReasonFlagCount)
        {
            throw new ArgumentException($"Violation reason flags must contain exactly {ViolationReasonFlagCount} byte(s).", nameof(violationReasonFlags));
        }

        PlayerId = playerId;
        ServerBeat = serverBeat;
        IsBugReportingAllowed = isBugReportingAllowed;
        ViolationReasonFlags = violationReasonFlags;
    }

    public byte Opcode => (byte)GameServerOpcode.LoginOrPendingState;

    public uint PlayerId { get; }

    /// <summary>
    /// Client heartbeat interval in milliseconds.
    /// </summary>
    public ushort ServerBeat { get; }

    public bool IsBugReportingAllowed { get; }

    /// <summary>
    /// Optional 20-byte violation reason flag block; absent when the account has none.
    /// </summary>
    public IReadOnlyList<byte>? ViolationReasonFlags { get; }

    public void Write(PacketWriter writer)
    {
        MessageWriter.WriteOpcode(writer, Opcode);
        writer.WriteUInt32(PlayerId);
        writer.WriteUInt16(ServerBeat);
        writer.WriteByte((byte)(IsBugReportingAllowed ? 1 : 0));

        var violationReasonFlags = ViolationReasonFlags;
        if (violationReasonFlags != null)
        {
            writer.WriteByte(ViolationReasonMarker);
            for (var index = 0; index < violationReasonFlags.Count; index++)
            {
                writer.WriteByte(violationReasonFlags[index]);
            }
        }
    }

    public static GameServerLoginOrPendingStateMessage Read(GameServerOpcode opcode, PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var playerId = reader.ReadUInt32();
        var serverBeat = reader.ReadUInt16();
        var isBugReportingAllowed = reader.ReadByte() != 0;

        byte[]? violationReasonFlags = null;
        if (reader.Remaining > 0 && reader.PeekByte() == ViolationReasonMarker)
        {
            reader.Skip(1);
            violationReasonFlags = reader.ReadBytes(ViolationReasonFlagCount);
        }

        return new GameServerLoginOrPendingStateMessage(playerId, serverBeat, isBugReportingAllowed, violationReasonFlags);
    }
}
