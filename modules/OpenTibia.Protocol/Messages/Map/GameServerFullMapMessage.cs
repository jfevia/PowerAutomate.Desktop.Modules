// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Geometry;
using PowerAutomate.Desktop.OpenTibia.Protocol.Items;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Map;

/// <summary>
/// Server's complete map view around the local player, sent on login and teleport (s2c 0x64).
/// </summary>
public sealed class GameServerFullMapMessage : IProtocolMessage
{
    private const int FloorWidth = 18;
    private const int FloorHeight = 14;

    public GameServerFullMapMessage(Position ownPosition, IReadOnlyList<IReadOnlyList<MapTile?>> floors)
    {
        OwnPosition = ownPosition;
        Floors = floors;
    }

    public byte Opcode => (byte)GameServerOpcode.FullMap;

    public Position OwnPosition { get; }

    public IReadOnlyList<IReadOnlyList<MapTile?>> Floors { get; }

    public static IProtocolMessage Read(GameServerOpcode opcode, PacketReader reader, IItemTypeProvider itemTypes)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        if (itemTypes == null)
        {
            throw new ArgumentNullException(nameof(itemTypes));
        }

        var ownPosition = PositionCodec.Read(reader);
        var floors = MapDescriptionCodec.Read(reader, itemTypes, ownPosition.Z, FloorWidth, FloorHeight);
        return new GameServerFullMapMessage(ownPosition, floors);
    }
}
